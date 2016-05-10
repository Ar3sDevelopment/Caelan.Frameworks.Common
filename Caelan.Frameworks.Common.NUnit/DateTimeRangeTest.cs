using System;
using Caelan.Frameworks.Common.Classes;
using NUnit.Framework;

namespace Caelan.Frameworks.Common.NUnit
{
    [TestFixture]
    public class DateTimeRangeTest
    {
        [Test]
        public void TestRange()
        {
            var range = new DateTimeRange(DateTime.Today, DateTime.Now);

            Assert.IsTrue(range.IsInRange(DateTime.Now.AddHours(-1)));
        }
    }
}