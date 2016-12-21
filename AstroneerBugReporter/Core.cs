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
            generateBugReport();
        }

        private static void generateBugReport()
        {
            BugReporter reporter = new BugReporter(new ReportingSourceCollector());
            reporter.Run();
        }
    }
}
