#!/bin/sh

local_nuget_pkg_dir=~/workspace/local-nuget-repo

echo "-------------"
while true; do
	echo "this will build and package TestFirst.Net and then tag git"
	echo "will place a copy of the built packages into $local_nuget_pkg_dir for testing"
    read -p "Continue? yn :" yn
    case $yn in
        [Yy]* ) 
			break;;
        * ) exit;;
    esac
done

read -p "Release as nuget version: " version

echo "Removing old nuget packages"
find . -name \*.nupkg | xargs -i rm {}

msbuild.exe TestFirst.Net.sln -t:Clean,Build -p:Configuration=Release

cd TestFirst.Net
nuget pack TestFirst.Net.csproj  -Symbols -Prop Configuration=Release -Version $version 
nuget pack TestFirst.Net.csproj  -IncludeReferencedProjects -Prop Configuration=Release -Version $version
cd ..

cd TestFirst.Net.Extensions
nuget pack TestFirst.Net.Extensions.csproj  -Symbols -Prop Configuration=Release -Version $version
nuget pack TestFirst.Net.Extensions.csproj  -IncludeReferencedProjects -Prop Configuration=Release -Version $version
cd ..

cd TestFirst.Net.Performance
nuget pack TestFirst.Net.Performance.csproj  -Symbols -Prop Configuration=Release -Version $version
nuget pack TestFirst.Net.Performance.csproj  -IncludeReferencedProjects -Prop Configuration=Release -Version $version
cd ..

echo "copying packages into single directory"
mkdir -p $local_nuget_pkg_dir
find . -name \*.nupkg | xargs -i cp {} --target $local_nuget_pkg_dir

echo 
echo "-------------"
while true; do
    read -p "tag git with version v$version? yn :" yn
    case $yn in
        [Yy]* ) 
			git tag -a v$version -m "Release version $version"; 
			echo "tagged with v$version"
			break;;
        [Nn]* )
			echo "not tagging"
			break;;
        * ) echo "Please answer y or n.";;
    esac
done

