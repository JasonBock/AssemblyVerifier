using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AssemblyVerifier.Tests
{
	public abstract class ExceptionTests<T, TInner> : TestBase
		where T : Exception, new()
		where TInner : Exception, new()
	{
		private string message;

		protected ExceptionTests(string message)
			: base()
		{
			this.message = message;
		}

		protected void CreateExceptionTest()
		{
			T exception = new T();
			Assert.IsNotNull(exception.Message);
			Assert.IsNull(exception.InnerException);
		}

		protected void CreateExceptionWithMessageTest()
		{
			T exception = Activator.CreateInstance(typeof(T), this.message) as T;
			Assert.AreEqual(this.message, exception.Message);
			Assert.IsNull(exception.InnerException);
		}

		protected void CreateExceptionWithMessageAndInnerExceptionTest()
		{
			TInner innerException = new TInner();

			T exception = Activator.CreateInstance(typeof(T), this.message, innerException) as T;
			Assert.AreEqual(this.message, exception.Message);
			Assert.AreEqual(innerException, exception.InnerException);
		}

		protected void RoundtripExceptionTest()
		{
			T exception = Activator.CreateInstance(typeof(T), this.message) as T;
			T newException = null;

			IFormatter formatter = new BinaryFormatter();

			using(Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, exception);
				stream.Position = 0;
				newException = formatter.Deserialize(stream) as T;
			}

			Assert.IsNotNull(newException);
			Assert.AreEqual(this.message, newException.Message);
		}
	}
}
