using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace LibAstroneerBugReporter
{
    public class ReportingSourceCollector : IReportingSourcesCollector
    {
        private IEnumerable<IReportingSource> _reportingSources = null;

        private List<FileInfo> _reportsAsFiles = null;

        private string _reportOutputPath = null;

        public ReportingSourceCollector()
        {
            Type tgtInterface = typeof(IReportingSource);
            Assembly bugReporterAssembly = Assembly.GetAssembly(typeof(IReportingSource));
            _reportingSources = null;

            var tmpList = bugReporterAssembly.GetTypes()
                .Where(t => tgtInterface.IsAssignableFrom(t)
                && !t.IsInterface)
                .ToList();

            _reportingSources = tmpList.Select(t => 
            (IReportingSource)Activator.CreateInstance(t, new object[] { }))
            .ToList();

            _reportsAsFiles = new List<FileInfo>();

            generateReportOutputPath();
        }

        private void generateReportOutputPath()
        {
            string filename = String.Format("Astroneer_Bug_Report_{0}.zip", DateTime.Now.ToString("dd-MM-yyy_HH-mm"));
            string directory = Environment.CurrentDirectory;
            _reportOutputPath = Path.Combine(directory, filename);
        }

        private void compressReportsAsZipArchive()
        {
            ZipArchive archive = null;
            using(var writer = new FileStream(_reportOutputPath, FileMode.CreateNew))
            {
                archive = new ZipArchive(writer, ZipArchiveMode.Create, false);
                foreach (FileInfo report in _reportsAsFiles)
                {
                    archive.CreateEntryFromFile(report.FullName, report.Name, CompressionLevel.Optimal);
                }
            }
        }

        public FileInfo CreateReportSummary()
        {
            foreach (IReportingSource report in _reportingSources)
            {
                _reportsAsFiles.Add(report.GenerateReport());
            }
            compressReportsAsZipArchive();
            return new FileInfo(_reportOutputPath);
        }
    }
}
