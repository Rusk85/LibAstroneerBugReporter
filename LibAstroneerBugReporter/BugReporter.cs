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

        public void Run()
        {
            _reporter.CreateReportSummary();
        }
    }
}
