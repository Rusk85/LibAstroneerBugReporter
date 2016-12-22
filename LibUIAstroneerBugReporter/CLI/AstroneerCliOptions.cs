using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace LibUIAstroneerBugReporter.CLI
{
    public class AstroneerCliOptions
    {

        [Option('o', "omit", SetName = "NoUpload", Required = false, Default = false)]
        public bool SkipUploadingReport { get; set; }

        [Option('u', "upload", SetName ="Upload", HelpText = "Upload bug report result to Google Drive.")]
        public bool UploadToGoogleDrive { get; set; }

        [Option('s', "shorten", SetName ="Upload", HelpText = "Shorten download url.")]
        public bool ShortenDownloadLinkUrl { get; set; }

        [Option('c', "copy", SetName = "Upload", HelpText = "Copies either url result to your clipboard.")]
        public bool CopyLinkToClipboard { get; set; }

        //[('h', "help")]
        //public string ShowUsage()
        //{
        //    var help = new HelpText("Astroneer Bug Reporter", new CopyrightInfo("Sven Muncic", 2016), this);
        //    return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        //}
    }

    [Verb("upload", HelpText = "Uploads bug report to Google Drive.")]
    public class AstroneerUploadOptions
    {
        [Option('s', "shorten", HelpText = "Shortens the Url that points to the uploaded bug report.", SetName = "Upload")]
        public bool ShortenDownloadLinkUrl { get; set; }
    }
}
