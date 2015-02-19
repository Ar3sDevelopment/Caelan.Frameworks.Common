using NUnit.Framework;
using System;
using System.Diagnostics;
using Caelan.Frameworks.Common.Classes;
using Caelan.Frameworks.Common.Attributes;

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

			[MapField("A")]
			public string A { get; set; }
		}

		class TestB
		{
			public string A { get; set; }
		}

		class ABMapper : DefaultMapper<TestA, TestB>
		{
			public override void Map(TestA source, ref TestB destination)
			{
				base.Map(source, ref destination);
				destination.A += " mapper";
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

