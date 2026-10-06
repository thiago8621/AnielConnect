using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.EmailService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void SetServiceName()
        {
            if (Context.Parameters.ContainsKey("ServiceName"))
            {
                serviceInstaller1.ServiceName = Context.Parameters["ServiceName"];
            }
            if (Context.Parameters.ContainsKey("DisplayName"))
            {
                serviceInstaller1.DisplayName = Context.Parameters["DisplayName"];
            }
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            SetServiceName();
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            SetServiceName();
            base.OnBeforeUninstall(savedState);
        }

    }
}
