using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avihs.WorkFlow
{
    public class WorkFlowInstanceParameters
    {
        public Data data { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
        public string message { get; set; }

    }
    public class Data
    {
        public string Id { get; set; }
        public object StateName { get; set; }
        public string ActivityName { get; set; }
        public string SchemeId { get; set; }
        public string SchemeCode { get; set; }
        public object PreviousState { get; set; }
        public object PreviousStateForDirect { get; set; }
        public object PreviousStateForReverse { get; set; }
        public string PreviousActivity { get; set; }
        public object PreviousActivityForDirect { get; set; }
        public object PreviousActivityForReverse { get; set; }
        public object ParentProcessId { get; set; }
        public Guid RootProcessId { get; set; }
        public bool IsDeterminingParametersChanged { get; set; }
        public int InstanceStatus { get; set; }
        public bool IsSubProcess { get; set; }
        public object TenantId { get; set; }
        public Transition[] Transitions { get; set; }
        public object[] History { get; set; }
        public Dictionary<string, object> ProcessParameters { get; set; }
        public object[] SubProcessIds { get; set; }
    }


    public class Transition
    {
        public string ProcessId { get; set; }
        public object ActorIdentityId { get; set; }
        public object ExecutorIdentityId { get; set; }
        public string FromActivityName { get; set; }
        public object FromStateName { get; set; }
        public bool IsFinalised { get; set; }
        public string ToActivityName { get; set; }
        public object ToStateName { get; set; }
        public string TransitionClassifier { get; set; }
        public DateTime TransitionTime { get; set; }
        public object TriggerName { get; set; }
    }
}
