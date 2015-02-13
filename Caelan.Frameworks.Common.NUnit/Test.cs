using NUnit.Framework;
using System;
using System.Diagnostics;
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
		public void TestNoBuilder()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			var a = new TestA();
			var b = new TestB
			{
				A = a.A + " no mapper"
			};
			Console.WriteLine(b.A);
			stopWatch.Stop();
			Console.WriteLine("{0} ms", stopWatch.ElapsedMilliseconds);
		}

		[Test]
		public void TestBuilder()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			var a = new TestA();
			var b = Builder.Source<TestA>().Destination<TestB>().Build(a);

			Console.WriteLine(b.A);
			stopWatch.Stop();
			Console.WriteLine("{0} ms", stopWatch.ElapsedMilliseconds);
		}
	}
}

