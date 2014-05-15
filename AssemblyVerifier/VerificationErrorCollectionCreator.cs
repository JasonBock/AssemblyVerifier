using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AssemblyVerifier
{
	/// <summary>
	/// Contains a method to create a collection of error information.
	/// </summary>
	public static class VerificationErrorCollectionCreator
	{
		/// <summary>
		/// Creates a <see cref="ReadOnlyCollection&lt;VerificationError&gt;" /> based
		/// on the output of peverify.
		/// </summary>
		/// <param name="peVerifyOutput">The output from peverify.</param>
		/// <returns>A collection of errors.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "pe")]
		public static ReadOnlyCollection<VerificationError> Create(TextReader peVerifyOutput)
		{
			var errors = new List<VerificationError>();

			if (peVerifyOutput != null)
			{
				var peOutputLine = peVerifyOutput.ReadLine();
				while (peOutputLine != null)
				{
					peOutputLine = peOutputLine.Replace("\0", string.Empty);

					if (peOutputLine.Length > 0)
					{
						var error = VerificationError.Create(peOutputLine);

						if (error != null)
						{
							errors.Add(error);
						}
					}

					peOutputLine = peVerifyOutput.ReadLine();
				}
			}

			return errors.AsReadOnly();
		}
	}
}

