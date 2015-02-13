using NUnit.Framework;
using System;
using Caelan.Frameworks.Common.Classes;

namespace Caelan.Frameworks.Common.NUnit
{
	[TestFixture]
	public class Test
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

		[Test]
		public void TestBuilder ()
		{
			var a = new TestA();
			var b = Builder.Source<TestA>().Destination<TestB>().Build(a);

			Console.WriteLine(b.A);
		}
	}
}

