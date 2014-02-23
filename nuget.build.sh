#!/bin/sh

local_nuget_pkg_dir=~/workspace/local-nuget-repo

read -p "Build as nuget version: " version

echo "Removing old nuget packages"
find . -name \*.nupkg | xargs -i rm {}

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


echo ""
echo "copying packages into local nuget repo dir at $local_nuget_pkg_dir"
mkdir -p $local_nuget_pkg_dir
find . -name \*.nupkg | xargs -i cp {} --target $local_nuget_pkg_dir


