#!/bin/sh

echo "-------------"
while true; do
    read -p "this will build and package TestFirst.Net and then tag git. Continue? yn :" yn
    case $yn in
        [Yy]* ) 
			break;;
        * ) exit;;
    esac
done

read -p "Release as version: " version

echo "Removing old nuget packages"
find . -name \*.nupkg | xargs -i rm {}

msbuild.exe TestFirst.Net.sln -t:Clean,Build -p:Configuration=Release

cd TestFirst.Net
nuget pack TestFirst.Net.csproj  -Symbols -Prop Configuration=Release
nuget pack TestFirst.Net.csproj  -IncludeReferencedProjects -Prop Configuration=Release
cd ..

cd TestFirst.Net.Extensions
nuget pack TestFirst.Net.Extensions.csproj  -Symbols -Prop Configuration=Release
nuget pack TestFirst.Net.Extensions.csproj  -IncludeReferencedProjects -Prop Configuration=Release
cd ..

cd TestFirst.Net.Performance
nuget pack TestFirst.Net.Performance.csproj  -Symbols -Prop Configuration=Release
nuget pack TestFirst.Net.Performance.csproj  -IncludeReferencedProjects -Prop Configuration=Release
cd ..

echo "copying packages into single directory"
mkdir -p bin/packages
find . -name \*.nupkg | xargs -i cp {} --target bin/packages/

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

