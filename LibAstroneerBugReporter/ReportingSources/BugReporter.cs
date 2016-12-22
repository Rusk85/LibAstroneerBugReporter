using LibAstroneerBugReporter.UrlShortener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter
{
    public class BugReporter
    {
        IReportingSourcesCollector _reporter = null;

        public BugReporter(IReportingSourcesCollector reporter)
        {
            _reporter = reporter;
        }

        public void Run(bool uploadToGDrive = false, bool shortenUrl = false)
        {
            var result = _reporter.CreateReportSummary();
            if (uploadToGDrive)
            {
                IStorageSolution gDrive = new GoogleDriveStorageSolution();
                var downloadLink = gDrive.Store(result);
                IUrlShortener shortUrl = new GoogleUrlShortener(true);
                if (shortenUrl)
                {
                    downloadLink = shortUrl.Shorten(downloadLink);
                }
                Console.WriteLine("<--- Copy and Add this link to your bug report --->");
                Console.WriteLine(String.Format("Link to archive containing reports: {0}", downloadLink));
                Console.WriteLine("<--- Copy and Add this link to your bug report --->");
            } 
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }



    }
}
