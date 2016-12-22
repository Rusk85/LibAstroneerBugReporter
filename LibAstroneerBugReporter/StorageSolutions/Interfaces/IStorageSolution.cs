using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter
{
    public interface IStorageSolution
    {
        string Store(object resourceHandle);
    }
}
