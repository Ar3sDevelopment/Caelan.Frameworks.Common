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
		[MapEquals]
		class TestA
		{
			public string A { get; set; }

			[MapField("B")]
			public string C { get; set; }
		}

		[MapEquals]
		class TestB
		{
			public string A { get; set; }
			[MapField("C")]
			public string B { get; set; }
		}

		class ABMapper : DefaultMapper<TestA, TestB>
		{
			public override void Map(TestA source, ref TestB destination)
			{
				base.Map(source, ref destination);
				destination.B += " mapper";
			}
		}

		[Test]
		public void TestNoBuilder()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var a = new TestA
			{
				A = "test",
				C = "test"
			};
			var b = new TestB
			{
				A = a.A,
				B = a.C + " no mapper"
			};

			var str = "A: " + b.A + " B: " + b.B;

			Assert.AreEqual (str, "A: test B: test no mapper");
			Console.WriteLine(str);

			stopWatch.Stop();
			Console.WriteLine("{0} ms", stopWatch.ElapsedMilliseconds);
		}

		[Test]
		public void TestBuilder()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var a = new TestA
			{
				A = "test",
				C = "test"
			};
			var b = Builder.Source<TestA>().Destination<TestB>().Build(a);

			var str = "A: " + b.A + " B: " + b.B;

			Assert.AreEqual (str, "A: test B: test mapper");
			Console.WriteLine(str);

			stopWatch.Stop();
			Console.WriteLine("{0} ms", stopWatch.ElapsedMilliseconds);
		}

		[Test]
		public void TestDefaultMapper()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var b = new TestB
			{
				A = "test",
				B = "test2"
			};
			var a = Builder.Source<TestB>().Destination<TestA>().Build(b);

			var str = "A: " + a.A + " C: " + a.C;

			Assert.AreEqual (str, "A: test C: test2");
			Console.WriteLine(str);

			stopWatch.Stop();
			Console.WriteLine("{0} ms", stopWatch.ElapsedMilliseconds);
		}
	}
}