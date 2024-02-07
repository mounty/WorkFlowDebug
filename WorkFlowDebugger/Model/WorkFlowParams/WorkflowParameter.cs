using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avihs.WorkFlow
{
    public class WorkflowParameter
    {
        public bool persist { get; set; }
        public string name { get; set; } = string.Empty;
        public string value { get; set; } = string.Empty;
    }

    public class WorkflowLaunchModel
    {
        public string[] ProcessId { get; set; }
        public string schemeCode { get; set; } = string.Empty;
        public List<WorkflowParameter> parameters { get; set; } = new List<WorkflowParameter>();
    }
}
