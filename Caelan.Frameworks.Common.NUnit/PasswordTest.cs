using NUnit.Framework;
using System;
using System.Diagnostics;
using Caelan.Frameworks.Common.Helpers;
using System.Runtime.CompilerServices;

namespace Caelan.Frameworks.Common.NUnit
{
	[TestFixture]
	public class PasswordTest
	{
		public class PasswordManager : PasswordHelper
		{
			public PasswordManager() : base("salt", "default")
			{
			}
		}

		[Test]
		public void TestPassword()
		{
			var pwd = new PasswordManager();

			Console.WriteLine(pwd.EncryptPassword("password"));
		}
	}
}

