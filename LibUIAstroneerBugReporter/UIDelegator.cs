using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUIAstroneerBugReporter
{
    public interface IUiImplementation
    {
        object Show(object businessLogic);
    }

    public class UIDelegator
    {
        private IUiImplementation _ui = null;

        public UIDelegator(IUiImplementation ui)
        {
            _ui = ui;
        }

        public object ShowUI(object businessLogic)
        {
            return _ui.Show(businessLogic);
        }

    }
}
