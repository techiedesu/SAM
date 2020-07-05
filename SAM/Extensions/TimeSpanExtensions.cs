using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string FormatTimespanString(this TimeSpan self)
        {
            var years = self.Days / 365;
            var days = self.Days;

            if (years > 0)
                days = self.Days / (years * 365);

            return years.ToString("D2") + ":" + days.ToString("D2") + ":" + self.ToString(@"hh\:mm\:ss");
        }
    }
}