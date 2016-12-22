using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibAstroneerBugReporter;
using LibAstroneerBugReporter.ReportingSources;
using System.IO;
using LibUIAstroneerBugReporter.CLI;
using LibUIAstroneerBugReporter;
using CommandLine;

namespace AstroneerBugReporter
{
    class Core
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //generateBugReport(args);
            runWithCmdLineInterface(args);
        }

        private static void generateBugReport(string[] args)
        {
            bool uploadToGoogleDrive = args.Length == 1
                && (args[0].ToLower() == "/u"
                    || args[0].ToLower() == "/upload");
            BugReporter reporter = new BugReporter(new ReportingSourceCollector());
            reporter.Run(uploadToGoogleDrive);
            Environment.Exit(0);
        }

        private static void runWithCmdLineInterface(string[] args)
        {
            UIDelegator ui = new UIDelegator(new AstroneerCli());
            var parserResult = (ParserResult<AstroneerCliOptions>)ui.ShowUI(args);
            AstroneerCliOptions cliOptions = null;
            var uploadOptions = parserResult.WithParsed<AstroneerCliOptions>(a => cliOptions = a);
            BugReporter reporter = new BugReporter(new ReportingSourceCollector());
            reporter.Run(cliOptions.UploadToGoogleDrive, cliOptions.ShortenDownloadLinkUrl, cliOptions.CopyLinkToClipboard);
        }

    }
}
