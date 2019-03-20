using System.Collections.Generic;

namespace RiverFlowApi
{
    public class AppSettings
    {
        public AppSettings()
        {
            this.CloudFoundryOverrides = new Dictionary<string, string>();
        }

        public Dictionary<string, string> CloudFoundryOverrides
        {
            get;
            set;
        }
    }
}