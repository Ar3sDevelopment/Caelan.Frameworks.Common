using NUnit.Framework;
using System;
using Autofac;
using Caelan.Frameworks.Common.Interfaces;
using Caelan.Frameworks.Common.Classes;
using System.Reflection;

namespace Caelan.Frameworks.Common.NUnit
{
	[TestFixture ()]
	public class AutoFacTest
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


		[Test ()]
		public void TestIoC()
		{
			var builder = new BuilderContainer();
			var container = builder.Build();

			using (var scope = container.BeginLifetimeScope())
			{
				var aBuilder = container.Resolve<Builder<TestA, TestB>>();
				var a = new TestA();
				var b = aBuilder.Build(a);

				Console.WriteLine(b.A);
			}
		}
	}
}

