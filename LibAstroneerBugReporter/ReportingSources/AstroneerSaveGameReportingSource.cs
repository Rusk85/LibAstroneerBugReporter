using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter.ReportingSources
{
    public class AstroneerSaveGameReportingSource : IReportingSource
    {
        public FileInfo GenerateReport()
        {
            string saveGameDir = ReportingSourcesDirectoryHelper.AstroneerSaveGamesDirectory;
            if (Directory.Exists(saveGameDir))
            {
                string saveGameFilter = "*.sav";
                string[] saves = Directory.GetFiles(saveGameDir, saveGameFilter, SearchOption.TopDirectoryOnly);

                if (saves.Any())
                {
                    List<FileInfo> savesAsFiles = saves.Select(l => new FileInfo(l)).ToList();
                    var latestSave = savesAsFiles.MaxBy(l => l.CreationTime);
                    return latestSave;
                }
            }
            return null;
        }
    }
}
