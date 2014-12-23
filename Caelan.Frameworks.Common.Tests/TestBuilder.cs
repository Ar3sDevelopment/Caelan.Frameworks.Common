using System;
using Caelan.Frameworks.Common.Classes;
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
		public override void Map(TestA source, ref TestB destination)
		{
			destination.A = source.A + " mapper";
		}
	}

	[TestClass]
	public class TestBuilder
	{
		[TestMethod]
		public void TestAtoB()
		{
			var a = new TestA();
			var b = Builder.Source<TestA>().Destination<TestB>().Build(a);

			Console.WriteLine(b.A);
		}
	}
}
