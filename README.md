# cc-dotnet-sdk
 
 The dotnet sdk for developing plugins for [cloud computes](https://github.com/USACE/cloudcompute)



Steps for a new release

 - Update PackageVersion in  [Usace.CC.Plugin](Usace.CC.Plugin/Usace.CC.Plugin.csproj)
 - Create a new release with tag such as 'v1.0.5' , that matches the PackageVersion above.
 - the workflow [publish.yml](.github/workflows/publish.yml) saves a nuget package to  https://nuget.pkg.github.com/USACE/
 
 
warning: Don't re-use release tags
