using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter.UrlShortener
{
    public interface IUrlShortener
    {
        string Shorten(string longDownloadLink);
    }
}
