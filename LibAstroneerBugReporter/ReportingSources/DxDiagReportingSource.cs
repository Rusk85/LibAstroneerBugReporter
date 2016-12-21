using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter.ReportingSources
{
    public class DxDiagReportingSource : IReportingSource
    {

        public FileInfo GenerateReport()
        {
            string pathToOutput = Path.Combine(Environment.CurrentDirectory, 
                String.Format("DxDiag_Report_{0}.txt", DateTime.Now.ToString("dd-MM-yyy")));
            string dontSkip = @"/dontskip";
            string disableWHQL = @"/whql:off";
            string outputCmd = String.Format(@"/t {0}", pathToOutput);
            string dxdiag = @"dxdiag";
            string composedCmd = String.Format("{0} {1} {2} {3}", dxdiag, dontSkip, disableWHQL, outputCmd);
            string shell = Path.Combine(Environment.SystemDirectory, "cmd.exe");

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.WorkingDirectory = @"C:\";
                    process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                    process.StartInfo.RedirectStandardInput = true;

                    process.OutputDataReceived += ProcessOutputDataHandler;
                    process.ErrorDataReceived += ProcessErrorDataHandler;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.StandardInput.WriteLine(composedCmd);
                    process.StandardInput.WriteLine("exit");

                    process.WaitForExit();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return new FileInfo(pathToOutput);
        }

        public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
