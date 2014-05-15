using System;
using System.Diagnostics;

namespace AssemblyVerifier.Tests
{
	internal static class AssemblyCreator
	{
		/// <summary>
		/// Creates an assembly based on a given IL file
		/// </summary>
		/// <param name="ilFile">The IL file.</param>
		/// <param name="assemblyKind">The kind of assembly (/dll or /exe).</param>
		internal static void Create(string ilFile, string assemblyKind)
		{
			var startInformation = new ProcessStartInfo("ilasm");
			startInformation.CreateNoWindow = true;
			startInformation.Arguments = ilFile + " " + assemblyKind;
			startInformation.RedirectStandardOutput = true;
			startInformation.UseShellExecute = false;

			Process.Start(startInformation).WaitForExit();
		}
	}
}
