using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avihs.WorkFlow
{

    public class RootMap
    {
        public List<Activity> Activities { get; set;}
        public List<Transitions> Transitions { get; set; }
    }

    public class Activity
    {
        public string Name { get; set; }
        public string IsInitial { get; set; }
        public string IsFinal { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
    }
    public class Transitions
    {
        public bool processFurther { get; set; } = true;
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public List<Condition> Condition { get; set; } = new List<Condition>();

        public string Operand { get; set; } = "";
    }

    public class Node
    {
        public Node parentNode { get; set; }
        public Activity activity { get; set; }
        public Transitions transitions { get; set; }
    }

    public class Condition
    {
        public string Expression { get; set; }
        public dynamic Value { get; set; }
        public string Assignment { get; set; }
        public string ConditionType { get; set; }
        public string Operand { get; set; }
    }
}
