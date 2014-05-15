using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace AssemblyVerifier.Tests
{
	public static class VerificationExceptionTests
	{
		[Fact]
		public static void CreateExceptionWithVerificationErrors()
		{
			var errors = new List<VerificationError>();
			errors.Add(VerificationError.Create(
				@"[IL]: Error: [C:\Proxy.dll : AType::HookByRefValueTypeProxy][offset 0x00000025][found Int32] Expected an ObjRef on the stack."));
			var exception = new VerificationException(errors.AsReadOnly());
			Assert.Equal(1, exception.Errors.Count);
		}

		[Fact]
		public static void CreateExceptionWithNullCollection()
		{
			Assert.Throws<ArgumentNullException>(() => new VerificationException(null as ReadOnlyCollection<VerificationError>));
		}
	}
}