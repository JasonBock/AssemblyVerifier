using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;

namespace AssemblyVerifier
{
	/// <summary>
	/// Provides methods to check the validity of an assembly.
	/// </summary>
	[SecurityCritical]
	public static class AssemblyVerification
	{
		/// <summary>
		/// Verifies an assembly based on its location on disk.
		/// </summary>
		/// <param name="assemblyLocation">The assembly's location on disk.</param>
		/// <exception cref="ArgumentNullException">Thrown if <c>assemblyLocation</c> is <c>null</c>.</exception>
		/// <exception cref="FileNotFoundException">Thrown if the given file does not exist.</exception>
		/// <exception cref="VerificationException">Thrown if the assembly has verification errors.</exception>
		public static void Verify(FileSystemInfo assemblyLocation)
		{
			if (assemblyLocation == null)
			{
				throw new ArgumentNullException("assemblyLocation");
			}

			if (!assemblyLocation.Exists)
			{
				throw new FileNotFoundException(
					"The assembly could not be found.", assemblyLocation.FullName);
			}

			AssemblyVerification.Verify(assemblyLocation.FullName);
		}

		/// <summary>
		/// Verifies an assembly based on an <see cref="Assembly" /> instance.
		/// </summary>
		/// <param name="assembly">The <see cref="Assembly" /> instance.</param>
		/// <exception cref="ArgumentNullException">Thrown if <c>assembly</c> is <c>null</c>.</exception>
		/// <exception cref="FileNotFoundException">Thrown if the given file does not exist.</exception>
		/// <exception cref="VerificationException">Thrown if the assembly has verification errors.</exception>
		public static void Verify(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			AssemblyVerification.Verify(assembly.Location);
		}

		/// <summary>
		/// Verifies an assembly based on an <see cref="AssemblyBuilder" /> instance.
		/// </summary>
		/// <param name="assemblyBuilder">The <see cref="AssemblyBuilder" /> instance.</param>
		/// <exception cref="ArgumentNullException">Thrown if <c>assemblyBuilder</c> is <c>null</c>.</exception>
		/// <exception cref="FileNotFoundException">Thrown if the given file does not exist.</exception>
		/// <exception cref="VerificationException">Thrown if the assembly has verification errors.</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void Verify(AssemblyBuilder assemblyBuilder)
		{
			if (assemblyBuilder == null)
			{
				throw new ArgumentNullException("assemblyBuilder");
			}

			var assemblyName = assemblyBuilder.GetName().Name;

			if (assemblyBuilder.EntryPoint != null)
			{
				assemblyName += ".exe";
			}
			else
			{
				assemblyName += ".dll";
			}

			AssemblyVerification.Verify(Path.Combine(Directory.GetCurrentDirectory(), assemblyName));
		}

		private static void Verify(string assemblyFileLocation)
		{
			var startInformation = new ProcessStartInfo("peverify");
			startInformation.CreateNoWindow = true;
			startInformation.Arguments = "\"" + assemblyFileLocation + "\" /MD /IL /UNIQUE";
			startInformation.RedirectStandardOutput = true;
			startInformation.UseShellExecute = false;

			var peVerify = Process.Start(startInformation);
			peVerify.WaitForExit();

			var errors = VerificationErrorCollectionCreator.Create(peVerify.StandardOutput);

			if (errors.Count > 0)
			{
				throw new VerificationException(errors);
			}
		}
	}
}
