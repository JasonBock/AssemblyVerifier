using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace AssemblyVerifier.Tests
{
	public static class DynamicVerificationTests
	{
		[Fact]
		public static void VerifyDynamicDllAssembly()
		{
			AssemblyBuilder goodAssembly = null;

			try
			{
				goodAssembly = DynamicVerificationTests.BuildAssembly(true);
				AssemblyVerification.Verify(goodAssembly);
			}
			finally
			{
				var assemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
				var assemblyFile = Path.Combine(Path.GetDirectoryName(assemblyUri.LocalPath),
					 goodAssembly.GetName().Name + ".dll");

				if (File.Exists(assemblyFile) == true)
				{
					File.Delete(assemblyFile);
				}
			}
		}

		[Fact]
		public static void VerifyDynamicExeAssembly()
		{
			AssemblyBuilder goodAssembly = null;

			try
			{
				goodAssembly = DynamicVerificationTests.BuildAssembly(false);
				AssemblyVerification.Verify(goodAssembly);
			}
			finally
			{
				var assemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
				var assemblyFile = Path.Combine(Path.GetDirectoryName(assemblyUri.LocalPath),
					 goodAssembly.GetName().Name + ".dll");

				if (File.Exists(assemblyFile) == true)
				{
					File.Delete(assemblyFile);
				}
			}
		}

		[Fact]
		public static void VerifyDynamicAssemblyWithVerificationErrors()
		{
			AssemblyBuilder badAssembly = null;

			try
			{
				Assert.Throws<VerificationException>(() =>
				{
					try
					{
						badAssembly = DynamicVerificationTests.BuildInvalidAssembly();
						AssemblyVerification.Verify(badAssembly);
					}
					catch (VerificationException verification)
					{
						Assert.Equal(2, verification.Errors.Count);

						foreach (var error in verification.Errors)
						{
							//this.TestContext.WriteLine("Type: {0}, location: {1}, description: {2}",
							//	 error.GetType().Name, error.Location, error.Description);
						}

						throw;
					}
				});
			}
			finally
			{
				var assemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
				var assemblyFile = Path.Combine(Path.GetDirectoryName(assemblyUri.LocalPath),
					 badAssembly.GetName().Name + ".dll");

				if (File.Exists(assemblyFile) == true)
				{
					File.Delete(assemblyFile);
				}
			}
		}

		private static AssemblyBuilder BuildAssembly(bool isDll)
		{
			AssemblyName name = new AssemblyName();
			name.Name = Guid.NewGuid().ToString("N");
			name.Version = new Version(1, 0, 0, 0);
			name.CodeBase = Directory.GetCurrentDirectory();
			var fileName = name.Name + (isDll ? ".dll" : ".exe");

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.RunAndSave);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			var typeBuilder = moduleBuilder.DefineType(
				 Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public,
				 typeof(object));

			var constructor = typeBuilder.DefineConstructor(
				 MethodAttributes.Public | MethodAttributes.SpecialName |
				 MethodAttributes.RTSpecialName,
				 CallingConventions.Standard, new Type[0]);

			var constructorMethod = typeof(object).GetConstructor(new Type[0]);
			var constructorGenerator = constructor.GetILGenerator();
			constructorGenerator.Emit(OpCodes.Ldarg_0);
			constructorGenerator.Emit(OpCodes.Call, constructorMethod);
			constructorGenerator.Emit(OpCodes.Ret);

			var methodBuilder = typeBuilder.DefineMethod(
				 Guid.NewGuid().ToString("N"), MethodAttributes.Public | MethodAttributes.HideBySig |
				 MethodAttributes.NewSlot | MethodAttributes.Virtual,
				 typeof(int), new Type[] { typeof(int) });

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ldarg_1);
			methodGenerator.Emit(OpCodes.Ret);

			typeBuilder.CreateType();

			if (!isDll)
			{
				DynamicVerificationTests.AddEntryPoint(assemblyBuilder, moduleBuilder);
			}

			assemblyBuilder.Save(fileName);
			return assemblyBuilder;
		}

		private static void AddEntryPoint(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
		{
			var typeBuilder = moduleBuilder.DefineType(
				 Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public,
				 typeof(object));

			var methodBuilder = typeBuilder.DefineMethod("Main",
				MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public,
				typeof(void), new Type[] { typeof(string[]) });

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.EmitWriteLine("go!");
			methodGenerator.Emit(OpCodes.Ret);

			typeBuilder.CreateType();

			assemblyBuilder.SetEntryPoint(methodBuilder);
		}

		private static AssemblyBuilder BuildInvalidAssembly()
		{
			var name = new AssemblyName();
			name.Name = Guid.NewGuid().ToString("N");
			name.Version = new Version(1, 0, 0, 0);
			var fileName = name.Name + ".dll";

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			var typeBuilder = moduleBuilder.DefineType(
				 Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public,
				 typeof(object));

			var constructor = typeBuilder.DefineConstructor(
				 MethodAttributes.Public | MethodAttributes.SpecialName |
				 MethodAttributes.RTSpecialName,
				 CallingConventions.Standard, new Type[0]);

			var constructorMethod = typeof(object).GetConstructor(new Type[0]);
			var constructorGenerator = constructor.GetILGenerator();
			constructorGenerator.Emit(OpCodes.Ldarg_0);
			constructorGenerator.Emit(OpCodes.Call, constructorMethod);
			constructorGenerator.Emit(OpCodes.Ret);

			var methodMDErrorBuilder = typeBuilder.DefineMethod(
				 Guid.NewGuid().ToString("N"), MethodAttributes.Public | MethodAttributes.HideBySig |
				 MethodAttributes.NewSlot,
				 typeof(int), new Type[] { typeof(int) });

			var methodMDErrorGenerator = methodMDErrorBuilder.GetILGenerator();
			methodMDErrorGenerator.Emit(OpCodes.Ldloc_0);
			methodMDErrorGenerator.Emit(OpCodes.Ret);

			var methodILErrorBuilder = typeBuilder.DefineMethod(
				 Guid.NewGuid().ToString("N"), MethodAttributes.Public | MethodAttributes.HideBySig |
				 MethodAttributes.NewSlot | MethodAttributes.Virtual,
				 typeof(int), new Type[] { typeof(int) });

			var methodILErrorGenerator = methodILErrorBuilder.GetILGenerator();
			// Note that we're returning the "this" value :(.
			methodILErrorGenerator.Emit(OpCodes.Ldloc_0);
			methodILErrorGenerator.Emit(OpCodes.Ret);

			typeBuilder.CreateType();
			assemblyBuilder.Save(fileName);
			return assemblyBuilder;
		}
	}
}
