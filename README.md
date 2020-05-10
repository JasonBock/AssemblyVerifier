AssemblyVerifier
================

This is a simple wrapper around peverify. The nice thing about this assembly is that you can verify dynamic assemblies (generated via Reflection.Emit) directly in code so you'll know right away if something's wrong with your emitted code.

Note that no more work will go into this project. [ILVerify](https://github.com/dotnet/corert/tree/master/src/ILVerify/) is (potentially) the .NET Core/.NET 5 library version that may be the solution to this.
