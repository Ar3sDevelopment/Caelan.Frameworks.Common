using System;
using System.Linq;

namespace Caelan.Frameworks.Common.Classes
{
    public class DateTimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateTimeRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }

            Start = start;
            End = end;
        }

        public bool IsInRange(DateTime date) => Start <= date && End >= date;
        public bool AreInRange(params DateTime[] dates) => dates.All(IsInRange);
        public bool IsInRange(DateTimeRange range) => AreInRange(range.Start, range.End);
        public TimeSpan ToTimeSpan() => End - Start;
    }
}