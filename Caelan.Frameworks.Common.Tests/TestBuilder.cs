using System;
using Caelan.Frameworks.Common.Classes;
using Caelan.Frameworks.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caelan.Frameworks.Common.Tests
{
	class TestA
	{
		public TestA()
		{
			A = "test";
		}

		public string A { get; set; }
	}

	class TestB
	{
		public TestB()
		{
			A = "test2";
		}

		public string A { get; set; }
	}

	class ABMapper : DefaultMapper<TestA, TestB>
	{
		public override void Map(TestA source, ref TestB destination, MapType mapType)
		{
			switch (mapType)
			{
				case MapType.NewObject:
				case MapType.EditObject:
					destination.A = source.A;
					break;
			}
		}
	}

	[TestClass]
	public class TestBuilder
	{
		[TestMethod]
		public void TestAtoB()
		{
			var a = new TestA();
			var b = Builder<TestA, TestB>.Create().Build(a);

			Assert.AreEqual(b.A, "test");

			Console.WriteLine(b.A);
		}
	}
}
