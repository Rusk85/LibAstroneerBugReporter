using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAstroneerBugReporter
{
    static class ReportingSourcesDirectoryHelper
    {
        private const string homeDriveEnvVarName = "HOMEDRIVE";
        private const string homePathEnvVarName = "HOMEPATH";
        private const string astroneerRelativeAppDataPath = @"AppData\Local\Astro\Saved";
        private const string astroneerRelativeLogFilePath = "Logs";
        private const string astroneerRelativeSaveGamesPath = "SaveGames";

        private static string _userHomeDir = null;
        private static string _astroneerLogs = null;
        private static string _astroneerSaves = null;

        static ReportingSourcesDirectoryHelper()
        {
            if(_userHomeDir == null)
            {
                composeAstroneerPaths();
            }
        }

        private static void composeAstroneerPaths()
        {
            string homeDrive = Environment.GetEnvironmentVariable(homeDriveEnvVarName);
            string homePath = Environment.GetEnvironmentVariable(homePathEnvVarName);
            _userHomeDir = String.Format("{0}{1}", homeDrive, homePath);
            string astroneerAppDataDir = Path.Combine(_userHomeDir, astroneerRelativeAppDataPath);
            _astroneerLogs = Path.Combine(astroneerAppDataDir, astroneerRelativeLogFilePath);
            _astroneerSaves = Path.Combine(astroneerAppDataDir, astroneerRelativeSaveGamesPath);
        }

        public static string AstroneerLogDirectory
        {
            get
            {
                return _astroneerLogs;
            }
        }

        public static string AstroneerSaveGamesDirectory
        {
            get
            {
                return _astroneerSaves;
            }
        }


    }
}
