using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace AssemblyVerifier.Tests
{
	public static class VerificationTests
	{
		[Fact]
		public static void VerifyAssemblyViaFileInfo()
		{
			AssemblyCreator.Create("GoodAssembly.il", "/dll");
			AssemblyVerification.Verify(new FileInfo("GoodAssembly.dll"));
		}

		[Fact]
		public static void VerifyAssemblyWithVerificationErrorsViaFileInfo()
		{
			AssemblyCreator.Create("BadAssembly.il", "/dll");

			Assert.Throws<VerificationException>(() =>
			{
				try
				{
					AssemblyVerification.Verify(new FileInfo("BadAssembly.dll"));
				}
				catch (VerificationException ex)
				{
					Assert.Equal(1, ex.Errors.Count);

					foreach (var error in ex.Errors)
					{
						//this.TestContext.WriteLine("Type: {0}, location: {1}, description: {2}",
						//	 error.GetType().Name, error.Location, error.Description);
					}

					throw;
				}
			});
		}

		[Fact]
		public static void VerifyAssemblyViaNullFileInfo()
		{
			Assert.Throws<ArgumentNullException>(() => AssemblyVerification.Verify(null as FileInfo));
		}

		[Fact]
		public static void VerifyAssemblyViaFileDoesNotExist()
		{
			Assert.Throws<FileNotFoundException>(() => AssemblyVerification.Verify(
				new FileInfo(Guid.NewGuid().ToString() + ".dll")));
		}


		[Fact]
		public static void VerifyAssemblyViaAssemblyReference()
		{
			AssemblyCreator.Create("GoodAssembly.il", "/dll");

			var name = new AssemblyName(
				"GoodAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
			AssemblyVerification.Verify(Assembly.Load(name));
		}

		[Fact]
		public static void VerifyAssemblyWithVerificationErrorsViaAssemblyReference()
		{
			AssemblyCreator.Create("BadAssembly.il", "/dll");

			Assert.Throws<VerificationException>(() =>
			{
				try
				{
					var name = new AssemblyName(
						"BadAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
					AssemblyVerification.Verify(Assembly.Load(name));
				}
				catch (VerificationException ex)
				{
					Assert.Equal(1, ex.Errors.Count);

					foreach (var error in ex.Errors)
					{
						//this.TestContext.WriteLine("Type: {0}, location: {1}, description: {2}",
						//	 error.GetType().Name, error.Location, error.Description);
					}

					throw;
				}
			});
		}

		[Fact]
		public static void VerifyAssemblyViaNullAssemblyReference()
		{
			Assert.Throws<ArgumentNullException>(() => AssemblyVerification.Verify(null as Assembly));
		}
	}
}
