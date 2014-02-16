#!/bin/sh

cd TestFirst.Net
nuget pack TestFirst.Net.csproj  -Symbols
nuget pack TestFirst.Net.csproj  -IncludeReferencedProjects
cd ..

cd TestFirst.Net.Extensions
nuget pack TestFirst.Net.Extensions.csproj  -Symbols
nuget pack TestFirst.Net.Extensions.csproj  -IncludeReferencedProjects
cd ..

cd TestFirst.Net.Performance
nuget pack TestFirst.Net.Performance.csproj  -Symbols
nuget pack TestFirst.Net.Performance.csproj  -IncludeReferencedProjects
cd ..
