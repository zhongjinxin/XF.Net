using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XF.Core.Const
{
    public class Secret
    {
        public string User { get; set; }

        public string DB { get; set; }

        public string Redis { get; set; }

        public string JWT { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string ExportFile = "C5ABA9E202D94C13A3CB66002BF77FAF";
    }
}
