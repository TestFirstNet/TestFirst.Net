######!/bin/python
#import os.spawn
import os
import os.path
#import os.system
import sys
import glob
import re
import errno
import subprocess
import shutil

# reliably get the solution dir
SOLUTION_DIR=os.path.dirname(os.path.realpath(__file__))
USER_HOME=os.path.expanduser("~")
VERBOSITY='normal'
#VERBOSITY=quiet,  minimal,  normal, detailed, diagnostic
CONFIG='Release'
#CONFIG=Debug

VERSION='0.0.0'
NUNIT_VERSION='2.6.4'
OPENCOVER_VERSION='4.6.166'
REPORTGEN_VERSION='2.2.0.0'

LOCAL_REPO=USER_HOME + '/workspace/local-nuget-repo'
TMP_DIR='obj/tmp'
PROJECTS=['TestFirst.Net','TestFirst.Net.Extensions','TestFirst.Net.Performance']
TEST_PROJECTS=['TestFirst.Net.Tests','TestFirst.Net.Extensions.Test','TestFirst.Net.Performance.Test']

NUGET_SRC='http://www.nuget.org/api/v2/'
NUGET_PKG_DIR=SOLUTION_DIR + '/packages'
NUGET_CONFIG=SOLUTION_DIR + '/.nuget/NuGet.Config'
SOLUTION='TestFirst.Net.sln'

# to find
NUGET_EXE=None
MSBUILD_EXE=None
XBUILD_EXE=None
MONO_EXE=None

# install if not found
OPENCOVER_EXE=None
REPORTGEN_EXE=None
NUNIT_CONSOLE_EXE=None

class BuildError(Exception):

    def __init__(self, msg):
        self.msg = msg
        
class cd:
    """Context manager for changing the current working directory"""
    def __init__(self, newPath):
        self.newPath = os.path.expanduser(newPath)

    def __enter__(self):
        self.savedPath = os.getcwd()
        os.chdir(self.newPath)
        
    def __exit__(self, etype, value, traceback):
        os.chdir(self.savedPath)

def log(msg):
    print("[BUILD] " + str(msg))

def error(msg):
    print("[BUILD] [ERROR!] " + str(msg))

def ensure_dir_exists(dir):
    try:
        os.makedirs(path)
    except OSError as exception:
        if exception.errno != errno.EEXIST:
            raise

def find_build_tools():
    global NUGET_EXE
    global MSBUILD_EXE
    global MONO_EXE
    global OPENCOVER_EXE
    global REPORTGEN_EXE
    global OPENCOVER_EXE
    global XBUILD_EXE
    global NUNIT_CONSOLE_EXE

    if can_invoke('nuget'):
        log('Using NuGet on the path')
        NUGET_EXE='NuGet'
    else:
        if os.path.isfile(SOLUTION_DIR + '/.nuget/NuGet.exe'):
            NUGET_EXE=SOLUTION_DIR + '/.nuget/NuGet.exe'
            log('Using nuget at ' + NUGET_EXE)
        elif os.path.isfile(USER_HOME + "/.nuget/NuGet.exe"):
            NUGET_EXE=USER_HOME + '/.nuget/NuGet.exe'
            log('Using nuget at ' + NUGET_EXE)
        else:
            raise BuildError('Could not find any installed nuget')
            
    if can_invoke('MsBuild'):
        log('Using MSBuild on the path')
        MSBUILD_EXE="MsBuild"
    else:
        #find latest MSBuild 
        if os.environ.get('SYSTEMROOT') != None:
            dotnet_dir=os.environ.get('SYSTEMROOT') + '/Microsoft.NET/Framework'

        msbuilds=[]
        def onfile(path,name):
            if name.endswith('MSBuild.exe'):
                msbuilds.append(path)
        find_files(dotnet_dir,onfile)

        msbuilds.sort(reverse=True) #get latest
        if len(msbuilds) > 0:
            MSBUILD_EXE=msbuilds[0]
        if can_invoke(MSBUILD_EXE):
            log('Using MSBuild at ' + MSBUILD_EXE)
        elif can_invoke('xbuild'):
            log('Using xbuild on the path')
            XBUILD_EXE=xbuild
            MSBUILD_EXE=None
        else:
            raise BuildError('Could not find MsBuild or xbuild. Make one available on the path. Also tried looking under $SYSTEMROOT\\Microsoft.NET\\Framework')
        
    if can_invoke('Mono'):
        log('Using Mono on the path')
        MONO_EXE='Mono'
    else:
        MONO_EXE=None
    
    OPENCOVER_EXE=nugetInstallIfNotExists('OpenCover',OPENCOVER_VERSION,'OpenCover.Console.exe')
    REPORTGEN_EXE=nugetInstallIfNotExists('ReportGenerator',REPORTGEN_VERSION,'ReportGenerator.exe')
    NUNIT_CONSOLE_EXE=nugetInstallIfNotExists('NUnit.Runners',NUNIT_VERSION,'nunit-console.exe')

def find_files(path,callback):
    for root, dirs, files in os.walk(path):
        for file in files:
            full=os.path.join(root, file)
            callback(full,file)

def nupack(projName):
    log('packing' + projName)

    requireVersion()

    with cd(projName):
        #main package
        log('building main nuget package')
        filterTemplate(projName + '.nuspec.template', projName + '.nuspec')

        invoke(NUGET_EXE,['pack', projName + '.nuspec', '-Prop', 'Configuration=' + CONFIG,'-Version',VERSION])
        #rm -f ${projName}.nuspec

        log('building symbols nuget package')
        #symbols package
        invoke(NUGET_EXE,['pack', projName + '.nuspec', '-Symnbols', '-Prop', 'Configuration=' + CONFIG,'-Version',VERSION])    
        #rm -f ${projName}.nuspec

        #todo: gitlink

def nugetInstallIfNotExists(pkg,version,exe_name):
    exe='{base}/packages/{pkg}.{ver}/tools/{name}'.format(base=SOLUTION_DIR,ver=version,pkg=pkg,name=exe_name).replace('/','\\')
    if os.path.isfile(exe):
        return exe;
    log("downloading " + pkg + "-" + version)
    invokeNuget(['install',pkg,'-Source',NUGET_SRC,'-Version',version])
    if not os.path.isfile(exe):
        raise BuildError("couldn't install nuget pkg " + pkg + ", version " + version + ". Looking for " + exe + ". Tried to install from " + NUGET_SRC)   
    return exe

def nuPush(proj):
    requireVersion()

    pkg=proj + VERSION + '.nupkg'
    log('pushing nuget pkg ' + proj + '/' + pkg)
    with cd(proj):    
        invokeNuget(['Push', pkg])    

def invokeNuget(args=None):
    nu_args=[]
    if args:
        nu_args+=args
    nu_args+=['-OutputDir',NUGET_PKG_DIR,'-NonInteractive']    
    if os.path.isfile(NUGET_CONFIG):
        nu_args+=['-ConfigFile', NUGET_CONFIG]
    log('running nuget:' + NUGET_EXE)
    invoke(NUGET_EXE, nu_args)


def task_clean_local_repo():
    def onfile(path,name):
        if name.endswith('.nupkg'):
            os.remove(path)
    find_files(LOCAL_REPO,onfile)

def removeOldNuPkgs():
    def onfile(path,name):
        if name.endswith('.nupkg'):
            os.remove(path)
    find_files('.',onfile)

def copyNuPkgsToLocalRepo():
    log('copying packages to local test repo ' + LOCAL_REPO)
    ensure_dir_exists(LOCAL_REPO)

    def onfile(path,name):
        if name.endswith('.nupkg'):
            shutil.copy(path,LOCAL_REPO)
    find_files('.',onfile)

    log('packages in local repo are:')
    def print_name(path,name):
        if name.endswith('.nupkg'):
            log(name)
    find_files(LOCAL_REPO,print_name)


def requireVersion():
   if VERSION == "0.0.0":
        output=invoke('git',['tag'])
        
        log('Tagged git versions:' + output)
        
        VERSION = input('[BUILD] Build as nuget version: ')

#filter a template (replace tokens)
def filterTemplate(fromPath,toPath):
    log("filtering template {} to {}".format(fromPath,toPath))

    #convert html entities into something the nuspec parser can handle
    def filterHtml(html):
        return html.Replace('<','&#8249;').Replace('>','&#8250;')

    readMeText=filterHtml(open("../README.md").read())
    releaseNotesText=filterHtml(open("../RELEASENOTES.md").read())

    text=filterHtml(open(fromPath).read())
    text=Replace('TOKEN_MAIN_DESCRIPTON',releaseNotesText).Replace('TOKEN_MAIN_RELEASENOTES',readMeText)

    ensure_dir_exists(os.path.dirname(to))

    f=open(toPath, 'w')
    f.write(text)
    f.flush()
    f.close()


def can_invoke(prog, args=None):
    if prog==None:
        return False;
    if not args:
        args=['-version']

    return shutil.which(prog)

def win_invoke(prog, args=None):
    if not args:
        args=[]

    if MONO_EXE:
        invoke(MONO_EXE,[prog] + args)
    else:
        invoke(prog,args)
        
def invoke(prog, args=None):
    if not args:
        args=[]
    
    args=[prog] + args
    
    cmd=' '.join(args)
    proc=subprocess.Popen(cmd,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    print("invoked: {}".format(proc.args))

    for line in proc.stdout: 
        print(line.decode(), end='')

# ------------- build targets ---------------

def task_pack_all(): 
    log('packing all')
    for proj in PROJECTS:
        nupack(proj)

def task_nuget_restore():
    invokeNuget(['restore',SOLUTION,'-PackagesDirectory'])

def tagGit():
    log('tagging git')
    requireVersion()

    while true:
        yn = input('tag git with version v{}? yn :'.format(VERSION)).lower()
        if yn == 'y':
            invoke('git',['tag','-a','v' + VERSION, '-m', 'Release version ' + VERSION])
            log('tagged with v' + VERSION)
            break
        elif yn == 'n':
            log('not tagging')
            break
        else:
            print('Please answer y or n.')

def task_clean():
    log('clean')

    for proj in (PROJECTS + TEST_PROJECTS):
        shutil.rmtree(proj + '/bin/',ignore_errors=True)
        shutil.rmtree(proj + '/obj/',ignore_errors=True)
        
    removeOldNuPkgs()
    #
    #if [ $MSBUILD_EXE ]; then
    #    #msbuild complains about this
    #    $MSBUILD_EXE $SOLUTION /t:Clean >/dev/null
    #    
    #else
    #    $XBUILD_EXE $SOLUTION /t:Clean  /verbosity:quiet /nologo
    #    $XBUILD_EXE $SOLUTION /t:Clean  /p:Configuration=Release  /verbosity:quiet /nologo
    #    $XBUILD_EXE $SOLUTION /t:Clean  /p:Configuration=Debug  /verbosity:quiet /nologo
    #    $XBUILD_EXE $SOLUTION /t:Clean  /p:Configuration=$CONFIG  /verbosity:quiet /nologo
    
    
    # remove generated nuspec files
    def onfile(path,name):
        if name.endswith('.nuspec'):
            os.remove(path)
    find_files('.',onfile)

def task_clean_all():
    task_clean_local_repo()
    task_clean()

def task_build_all():
    log('building all')
    if os.path.isfile(MSBUILD_EXE):
        win_invoke(MSBUILD_EXE,[SOLUTION,'-t:Clean,Build','-p:Configuration=' + CONFIG, '/verbosity:' + VERBOSITY])
    else:
        invoke(XBUILD_EXE,[SOLUTION,'/t:Build','/p:Configuration=' + CONFIG, '/verbosity:' + VERBOSITY])

def task_test_all():
    log('Running tests')

    #e.g. ./TestFirst.Net.Performance.Test/obj/Release/TestFirst.Net.Performance.Test.dll
    for proj in TEST_PROJECTS:
        log('Executing tests in ' + proj)
        with cd(proj + '/bin/' + CONFIG):
            # to also handle the TestFirst.Net.Tests project (note the 's' after 'Test')
            dll_name=proj if not proj.endswith('s') else proj[:-1]

            win_invoke(NUNIT_CONSOLE_EXE,['-nologo',dll_name + '.dll'])

        log('finished running test ' + proj)

def task_test_coverage():
    log('generating code coverage')
    
    for proj in TEST_PROJECTS:
        log('Executing tests in {} for coverage'.format(proj))
        with cd(proj + '/bin/' + CONFIG):
            # to also handle the TestFirst.Net.Tests project (note the 's' after 'Test')
            test_dll_name=proj if not proj.endswith('s') else proj[:-1]
            proj_dll_name=(test_dll_name if not test_dll_name.endswith('.Test') else test_dll_name[:-5])
            
            win_invoke(OPENCOVER_EXE,[
                '-log:All',
                '-target:"{}"'.format(NUNIT_CONSOLE_EXE),
                '-targetargs:"/nologo {}.dll /noshadow /trace=Error"'.format(test_dll_name),
                '-filter:"+[' + proj_dll_name + ']*"',
                '-excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute"',
                '-register:user',
                '-output:_CodeCoverageResult.xml'])

            win_invoke(REPORTGEN_EXE,[
                '-reports:_CodeCoverageResult.xml',
                '-targetdir:_CodeCoverageReport' ])
              
        log('finished running code coverage ' + proj) 

def task_release_build():
    requireVersion()
    removeOldNuPkgs()
    task_clean()
    task_build_all()
    task_test_all()
    task_pack_all()
    copyNuPkgsToLocalRepo()

def task_release_push():
    for proj in PROJECTS:
        nuPush(proj)

def task_release():
    log('This will build and package TestFirst.Net and then tag git')
    log('This will place a copy of the built packages into {} for testing'.format(LOCAL_REPO))

    task_release_build()
    tagGit()


def task_print_help():
    log('USAGE:')
    log('   build.py target [target..] [varname1=value1..]')
    log('   eg build.py clean restore build test')
    log('TARGETS:')
    log('  clean : remove all built artifacts (*.dll)')
    log('  clean-repo : remove all *.nupkg files from local repo ' + LOCAL_REPO)
    log('  clean-all : clean,clean-repo')
    log('  restore : restore nuget packages to dir ' + NUGET_PKG_DIR)
    log('  build : build the solution using msbuild/xbuild')
    log('  test : run the unit tests')
    log('  pack : pack the built artifacts into nuget packages')
    log('  release-build : clean, build, test, pack')
    log('  release-push : push all the nuget packages to the nuget repo')
    log('  release : release-build,git-tag')
    log('VARIABLES:')
    log('  config : the solution config to use. Debug|Release. Current ' + CONFIG)
    log('  version : version to release at. Format MAJOR.MINOR.BUILD. Current ' + VERSION)
    log('  verbosity : xbuild verbosity. quiet|minimal|normal|detailed|diagnostic. Current ' + VERBOSITY)
    log('  local_repo : path to local nuget test repo. Current ' + LOCAL_REPO)
    log('  nunit_version : Version of nunit to use for testing. Current ' + NUNIT_VERSION)
    log('  nuget_src : Nuget source to install nunit runner from. Current ' + NUGET_SRC)
    log('To set script variables pass in <name>=<value> as in "build.py release version=1.2.3" (name is case insensitive)')

# ----------------- start the actual build ---------------------------

log('-------- TestFirst.Net Build -----')

def run_tasks():
    has_target=False
    tasks = {
        'clean': task_clean,
        'clean-repo': task_clean_local_repo,
        'clean-all': task_clean_all,
        'restore' :task_nuget_restore,
        'build' : task_build_all,
        'test' : task_test_all,
        'test-coverage' : task_test_coverage,
        'pack' : task_pack_all,
        'release-push' : task_release_push,
        'release-build' : task_release_build,
        'release' : task_release,
        'help' : task_print_help
    }
    
    #extract variable assignment. Expect VAR=VAL on command line
    for arg in sys.argv[1:]:  
        if arg.find('=') != -1:
            pair=str.split(arg,'=',1)
            name=pair[0].upper()
            val=pair[1]
            log('set {} ==> {}'.format(name,val))
            globals()[name]=val

    for target in sys.argv[1:]:
        #skip variable assignment
        if target.find('=') != -1:
            continue
        
        log("executing target '{}'".format(target))
        target=target.lower()
        task = tasks.get(target, None)
        if not task:
            raise BuildError("No build target '{}'. For help run target 'help'".format(target))
        # Execute the task
        task()

        has_target=True
    
    if not has_target:
        raise BuildError("no build target provided. For help run target 'help'")
    

#ensure we run from a known location
with cd(SOLUTION_DIR):
    find_build_tools()
    run_tasks()
