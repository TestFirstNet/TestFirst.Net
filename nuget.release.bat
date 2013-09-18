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
