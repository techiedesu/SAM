using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Extensions
{
    public static class CharExtensions
    {
        private static readonly char[] SpecialChars = { '{', '}', '(', ')', '[', ']', '+', '^', '%', '~' };

        public static bool IsSpecial(this char self)
        {
            return SpecialChars.Any(c => c.Equals(self));
        }
    }
}
