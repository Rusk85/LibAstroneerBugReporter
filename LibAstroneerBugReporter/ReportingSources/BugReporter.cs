using System.Windows.Forms;
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

        public void Run(bool uploadToGDrive = false, bool shortenUrl = false, 
            bool copyUrlResultToClipboard = false)
        {
            Action newLine = () => Console.WriteLine(String.Empty);
            string successMsg = "!+~+~+~+~+~+~+~+~+~+~+~+~ S U C C E S S +~+~+~+~+~+~+~+~+~+~+~+~!";
            newLine();
            Action success = () => Console.WriteLine(successMsg);
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
                success();
                Console.WriteLine("!+~+~+~+~+~+~+~+~+~+~+~+~ Bug Report Generation concluded +~+~+~+~+~+~+~+~+~+~+~+~!");
                newLine();
                if (copyUrlResultToClipboard)
                {
                    Clipboard.SetText(downloadLink);
                    Console.WriteLine("Link '{0}' containing Bug Report copied to clipboard.", downloadLink);
                    newLine();
                } else
                {
                    Console.WriteLine("!+~+~+~+~+~+~+~+~+~+~+~+~ Copy and Add below link to your bug report +~+~+~+~+~+~+~+~+~+~+~+~!");
                    Console.WriteLine(String.Format("Link to archive containing reports: {0}", downloadLink));
                    Console.WriteLine("!+~+~+~+~+~+~+~+~+~+~+~+~ Copy and Add above link to your bug report +~+~+~+~+~+~+~+~+~+~+~+~!");
                    newLine();
                }
            } else
            {
                success();
            } 
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }



    }
}
