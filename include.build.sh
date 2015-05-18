
VERSION=0.0.0
LOCAL_REPO=~/workspace/local-nuget-repo
TMP_DIR=obj/tmp

function filterHtml(){
    in="$1"
    cat $in | sed -e 's/</\&#8249;/g' -e 's/>/\&#8250;/g'
}

function generateNuSpecFile(){
    projName="$1"

    in="$projName.nuspec.template"
    out="$projName.nuspec" 
    
    echo "filtering template $in to $out"
    mkdir -p $TMP_DIR

    cp -f "$in" "$out"
    filterHtml "../README.md" > "$TMP_DIR/README.md.clean"
    filterHtml "../RELEASENOTES.txt" > "$TMP_DIR/RELEASENOTES.txt.clean"

    sed -i -e "/TOKEN_MAIN_DESCRIPTON/{r $TMP_DIR/README.md.clean" -e 'd}' $out
    sed -i -e "/TOKEN_MAIN_RELEASENOTES/{r $TMP_DIR/RELEASENOTES.txt.clean" -e 'd}' $out
    
    rm "$TMP_DIR/README.md.clean"
    rm "$TMP_DIR/RELEASENOTES.txt"
    
    rm -fR $TMP_DIR
}

function removeNuSpecFile(){
    projName="$1"
    rm "$projName.nuspec"    
}

function nupack(){
    projName="$1"

    pushd $projName
    generateNuSpecFile projName
    nuget pack "$projName.nuspec"  -Symbols -Prop Configuration=Release -Version $VERSION 
    nuget pack "$projName.nuspec"  -Prop Configuration=Release -Version $VERSION
    removeNuSpecFile projName
    popd
}

function packAll(){
    nupack "TestFirst.Net"
    nupack "TestFirst.Net.Extensions"
    nupack "TestFirst.Net.Performance"
}

function removeOldNuPkgs(){
    echo "Removing old nuget packages"
    find . -name \*.nupkg | xargs -i rm {}
}

function copyNuPkgsToLocalRepo(){
    echo "copying packages into single directory $LOCAL_REPO"
    mkdir -p $LOCAL_REPO
    find . -name \*.nupkg | xargs -i cp {} --target $LOCAL_REPO

    echo "packages copied are:"
    ls $LOCAL_REPO
}

# ------------- build targets ---------------

function printHelp() {
    echo "targets:"
    echo "  clean : remve all built artifacts (dlls)"
    
    echo "  build : run the xbuild for all projects"
    
    echo "  pack : pack the built artifacts into nuget packages"
    
    echo "  release : clean, build, pack, tag git"
    
    echo "  build-release : clean, build, pack"
    

}

function cleanAll(){
    xbuild TestFirst.Net.sln /t:Clean 
    xbuild TestFirst.Net.sln /t:Clean  /p:Configuration=Release
}

#msbuild.exe TestFirst.Net.sln -t:Clean,Build -p:Configuration=Release
function buildAll(){
    xbuild TestFirst.Net.sln /t:Build /p:Configuration=Release /verbosity:diagnostic
}

function buildRelease(){
    read -p "Build as nuget version: " VERSION

    removeOldNuPkgs
    cleanAll
    buildAll
    packAll
    copyNuPkgsToLocalRepo

}

function release(){
    echo "-------------"
    while true; do
	    echo "this will build and package TestFirst.Net and then tag git"
	    echo "will place a copy of the built packages into $LOCAL_REPO for testing"
        read -p "Continue? yn :" yn
        case $yn in
            [Yy]* ) 
			    break;;
            * ) exit;;
        esac
    done

    buildRelease

    echo 
    echo "-------------"
    while true; do
        read -p "tag git with version v$VERSION? yn :" yn
        case $yn in
            [Yy]* ) 
			    git tag -a v$VERSION -m "Release version $VERSION"; 
			    echo "tagged with v$VERSION"
			    break;;
            [Nn]* )
			    echo "not tagging"
			    break;;
            * ) echo "Please answer y or n.";;
        esac
    done

}

function runTarget(){
    target="$1"
    echo "executing target '$target'"

    case $target in
        'clean' )
            cleanAll
            ;;
        'build' )
            buildAll
            ;;
        'pack' )
            packAll
            ;; 
        'build-release' )
            releaseBuild
            ;;
        'release' )
            release
            ;;
        'help' )
            printHelp
            ;;             
        * ) 
            echo "No build target '$target'. For help run target 'help'";
            exit -1
    esac
}


