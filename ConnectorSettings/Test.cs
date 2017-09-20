using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Integration.Configuration;
using inRiver.Integration.Interface;

namespace ConnectorSettings
{
    public class Test : IOutboundConnector
    {
        public void Start()
        {
            ConfigurationManager.Instance.SetConnectorSetting(this.Id, "c:\\temp\\Alexis.png", "c:\temp");
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void InitConfigurationSettings()
        {
            throw new NotImplementedException();
        }

        public bool IsStarted { get; }
        public string Id { get; set; }
    }
}
