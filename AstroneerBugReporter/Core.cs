using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibAstroneerBugReporter;
using LibAstroneerBugReporter.ReportingSources;
using System.IO;

namespace AstroneerBugReporter
{
    class Core
    {
        public static void Main(string[] args)
        {
            generateBugReport(args);
        }

        private static void generateBugReport(string[] args)
        {
            bool uploadToGoogleDrive = args.Length == 1
                && (args[0].ToLower() == "/u"
                    || args[0].ToLower() == "/upload");
            BugReporter reporter = new BugReporter(new ReportingSourceCollector());
            reporter.Run(uploadToGoogleDrive);
        }
    }
}
