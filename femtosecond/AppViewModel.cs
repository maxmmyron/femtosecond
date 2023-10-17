using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace femtosecond
{
    public class AppViewModel : BaseBind
    {
        public AppViewModel() { }

        private string editorContents;
        public string EditorContents
        {
            get { return editorContents; }
            set { SetProperty(ref editorContents, value); }
        }

        private string workspaceDirectory = null;
        public string WorkspaceDirectory
        {
            get { return workspaceDirectory; }
            set { SetProperty(ref workspaceDirectory, value); }
        }
    }
}
