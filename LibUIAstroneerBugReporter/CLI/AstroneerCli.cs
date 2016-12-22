using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUIAstroneerBugReporter.CLI
{
    public class AstroneerCli : IUiImplementation
    {
        private AstroneerCliOptions _cliOptions = null;

        public object Show(object businessLogic)
        {
            _cliOptions = new AstroneerCliOptions();

            if (businessLogic.GetType() == typeof(string[]))
            {
                string[] args = (string[])businessLogic;
                var result = Parser.Default.ParseArguments<AstroneerCliOptions>(args);
                return result;
            }
            throw new InvalidOperationException("Argument must be of type 'string[]'.");
        }
    }
}
