using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAM.Models;

namespace SAM.Extensions
{
    public static class AccountExtensions
    {
        public static bool HasActiveTimeout(this Account self)
        {
            if (self.Timeout == null || self.Timeout == new DateTime() || self.Timeout.Value.CompareTo(DateTime.Now) <= 0)
            {
                self.Timeout = null;
                self.TimeoutTimeLeft = null;

                return false;
            }

            return true;
        }
    }
}