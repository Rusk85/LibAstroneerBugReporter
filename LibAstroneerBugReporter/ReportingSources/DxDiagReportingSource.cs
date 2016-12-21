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
        public FileInfo GenerateReport2()
        {
            string pathToOutput = @"C:\dxFromCode.txt";
            string dontSkip = @"/dontskip";
            string disableWHQL = @"/whql:off";
            string outputCmd = String.Format(@"/t {0}", pathToOutput);
            string composedCmd = String.Format("{0} {1} {2}", dontSkip, disableWHQL, outputCmd);
            //string dxdiag = @"C:\Windows\System32\dxdiag.exe";
            string dxdiag = @"C:\Windows\SysWOW64\dxdiag.exe";
            // /dontskip /whql:off /t C:\dxdiag_outputTest1.txt
            //Create process
            //System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            ProcessStartInfo psi = new ProcessStartInfo(dxdiag);
            psi.Arguments = composedCmd;

            using(var proc = Process.Start(psi))
            {
                proc.WaitForExit();
                if(proc.ExitCode != 0)
                {
                    throw new Exception("DxDiag failed with error code " + proc.ExitCode);
                }
            }

            //pProcess.StartInfo.FileName = "dxdiag";
            //pProcess.StartInfo.Arguments = composedCmd;
            //pProcess.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            //pProcess.StartInfo.RedirectStandardOutput = false;
            //Start the process
            //pProcess.Start();
            //Wait for process to finish
            //pProcess.WaitForExit();

            return new FileInfo(pathToOutput);
        }

        public FileInfo GenerateReport()
        {
            string pathToOutput = Path.Combine(Environment.CurrentDirectory, 
                String.Format("DxDiag_Report_{0}.txt", DateTime.Now.ToString("dd-MM-yyy")));
            string dontSkip = @"/dontskip";
            string disableWHQL = @"/whql:off";
            string outputCmd = String.Format(@"/t {0}", pathToOutput);
            //string dxdiag = @"C:\Windows\SysWOW64\dxdiag.exe";
            //string dxdiag = @"C:\Windows\System32\dxdiag.exe";
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

                    // Redirects the standard input so that commands can be sent to the shell.
                    process.StartInfo.RedirectStandardInput = true;
                    // Runs the specified command and exits the shell immediately.
                    //process.StartInfo.Arguments = @"/c ""dir""";

                    process.OutputDataReceived += ProcessOutputDataHandler;
                    process.ErrorDataReceived += ProcessErrorDataHandler;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Send a directory command and an exit command to the shell
                    process.StandardInput.WriteLine(composedCmd);
                    //Thread.Sleep(15000);
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
