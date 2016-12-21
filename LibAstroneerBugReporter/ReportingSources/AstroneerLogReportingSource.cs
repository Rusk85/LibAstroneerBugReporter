using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter.ReportingSources
{
    public class AstroneerLogReportingSource : IReportingSource
    {
        public FileInfo GenerateReport()
        {
            string logsDirectory = ReportingSourcesDirectoryHelper.AstroneerLogDirectory;
            if (Directory.Exists(logsDirectory))
            {
                string logFilter = "*.log";
                string[] logs = Directory.GetFiles(logsDirectory, logFilter, SearchOption.TopDirectoryOnly);

                if (logs.Any())
                {
                    List<FileInfo> logsAsFiles = logs.Select(l => new FileInfo(l)).ToList();
                    var latestLog = logsAsFiles.MaxBy(l => l.CreationTime);
                    return latestLog;
                }
            }
            return null;
        }
    }
}
