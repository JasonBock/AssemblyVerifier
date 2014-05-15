using System;
using Xunit;

namespace AssemblyVerifier.Tests
{
	public static class VerificationErrorTests
	{
		[Fact]
		public static void CompareErrors()
		{
			var error1 = VerificationError.Create(
				@"[IL]: Error: [C:\MyDll.dll : MyDll::MyBadCode][offset 0x00000025][found Int32] Expected an ObjRef on the stack.");
			var error2 = VerificationError.Create(
				@"[IL]: Error: [C:\MyDll.dll : MyDll::MyBadCode2][offset 0x00000025][found Int32] Expected an ObjRef on the stack.");
			var error3 = VerificationError.Create(
				@"[IL]: Error: [C:\MyDll.dll : MyDll::MyBadCode][offset 0x00000025][found Int32] Expected an ObjRef on the stack.");
			var error4 = VerificationError.Create(
				@"[IL]: Error: [C:\MyDll.dll : MyDll::MyBadCode][offset 0x00000025][found Int32] Expected an ObjRef on the stack, baby!");

			Assert.NotEqual(error1, error2);
			Assert.Equal(error1, error3);
			Assert.NotEqual(error1, error4);
			Assert.NotEqual(error2, error3);
			Assert.NotEqual(error2, error4);
			Assert.NotEqual(error3, error4);

			Assert.NotEqual(error1.GetHashCode(), error2.GetHashCode());
			Assert.Equal(error1.GetHashCode(), error3.GetHashCode());
			Assert.NotEqual(error1.GetHashCode(), error4.GetHashCode());
			Assert.NotEqual(error2.GetHashCode(), error3.GetHashCode());
			Assert.NotEqual(error2.GetHashCode(), error4.GetHashCode());
			Assert.NotEqual(error3.GetHashCode(), error4.GetHashCode());
		}

		[Fact]
		public static void CreateILError()
		{
			var error = VerificationError.Create(
				@"[IL]: Error: [C:\JasonBock\Personal\.NET Projects\AssemblyVerifier\TestResults\JasonB_JASONB-PC 2007-12-14 15_16_31\Out\DynamicProxies.Tests.Proxy.dll : DynamicProxies.Tests.Types.ByRefValueTypeProxy::HookByRefValueTypeProxy][offset 0x00000025][found Int32] Expected an ObjRef on the stack.");
			Assert.Equal("[offset 0x00000025][found Int32] Expected an ObjRef on the stack.", error.Description);
			Assert.Equal("DynamicProxies.Tests.Types.ByRefValueTypeProxy::HookByRefValueTypeProxy", error.Location);
		}

		[Fact]
		public static void CreateMDError()
		{
			var error = VerificationError.Create(
				@"[MD]: Error: Method marked Final or NewSlot or CheckAccessOnOverride but not Virtual. [token:0x06000002]");
			Assert.Equal("Method marked Final or NewSlot or CheckAccessOnOverride but not Virtual. [token:0x06000002]", error.Description);
			Assert.Equal("token:0x06000002", error.Location);
		}

		[Fact]
		public static void CreateErrorWithEmptyString()
		{
			Assert.Throws<ArgumentException>(() => VerificationError.Create(string.Empty));
		}

		[Fact]
		public static void CreateErrorWithNullString()
		{
			Assert.Throws<ArgumentNullException>(() => VerificationError.Create(null));
		}
	}
}
