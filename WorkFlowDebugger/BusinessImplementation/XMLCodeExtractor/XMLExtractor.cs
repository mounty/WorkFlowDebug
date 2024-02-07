using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Threading;

namespace Avihs.WorkFlow.XMLCodeExtractor
{
    internal class XMLExtractor
    {
        public static void Extract()
        {
            try
            {
                // Load the XML file
                XmlDocument doc = new XmlDocument();

                doc.Load("WorkFlow.xml");

                XmlElement root = doc?.DocumentElement;

                string classname = XElement.Parse(root.OuterXml).Attribute("Name").Value;

                List<string> methods = new List<string>();

                Dictionary<string, string> paramters = new Dictionary<string, string>();

                StringBuilder stringBuilder = new StringBuilder();

                List<string> addedMethods = new List<string>();

                List<Activity> activities = new List<Activity>();

                List<Transitions> transitions = new List<Transitions>();

                List<string> codeActions = new List<string>();

                List<Node> nodes = new List<Node>();

                List<string> ProcessedTransactions = new List<string>();



                if (root != null)
                {
                    foreach (XmlNode child in root.ChildNodes)
                    {
                        if (child.Name == "CodeActions")
                        {
                            foreach (XmlNode codeNode in child.ChildNodes)
                            {

                                XElement element = XElement.Parse(codeNode.OuterXml);
                                string nameRefValue = element.Attribute("Name")?.Value ?? string.Empty;
                                if (!string.IsNullOrEmpty(nameRefValue))
                                {
                                    if (methods.Contains(nameRefValue))
                                    {
                                        foreach (XmlNode valueNode in codeNode.ChildNodes)
                                        {
                                            if (valueNode.Name=="ActionCode")
                                            {
                                                if (valueNode.InnerText !="//TODO: Write your code")
                                                {
                                                    AppendCode(nameRefValue, valueNode.InnerText);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        if (child.Name == "Activities")
                        {
                            foreach (XmlNode codeNode in child.ChildNodes)
                            {
                                Activity activity = new Activity();

                                if (codeNode.Name == "Activity")
                                {
                                    XElement element = XElement.Parse(codeNode.OuterXml);
                                    activity.Name = element.Attribute("Name")?.Value ?? string.Empty;
                                    activity.IsInitial = element.Attribute("IsInitial")?.Value ?? string.Empty;
                                    activity.IsFinal = element.Attribute("IsFinal")?.Value ?? string.Empty;
                                }

                                foreach (XmlNode valueNode in codeNode.ChildNodes)
                                {
                                    if (valueNode.Name=="Implementation")
                                    {
                                        foreach (XmlNode actionRef in valueNode.ChildNodes)
                                        {
                                            XElement element = XElement.Parse(actionRef.OuterXml);
                                            string val = element.Attribute("NameRef")?.Value ?? string.Empty;
                                            methods.Add(val);
                                            activity.Actions.Add(val);
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(activity.Name))
                                {
                                    activities.Add(activity);
                                }
                            }
                        }
                        if (child.Name == "Transitions")
                        {
                            foreach (XmlNode Transitions in child.ChildNodes)
                            {
                                XElement element = XElement.Parse(Transitions.OuterXml);

                                Transitions transitions1 = new Transitions();

                                transitions1.Name =element.Attribute("Name")?.Value ?? string.Empty;

                                transitions1.From =element.Attribute("From")?.Value ?? string.Empty;
                                transitions1.To =element.Attribute("To")?.Value ?? string.Empty;

                                foreach (XmlNode Transition in Transitions.ChildNodes)
                                {

                                    foreach (XmlNode condition in Transition.ChildNodes)
                                    {
                                        if (condition.Name =="Condition")
                                        {
                                            XElement codelement = XElement.Parse(condition.OuterXml);

                                            string mainOperand = element.Attribute("ConditionsConcatenationType")?.Value ?? string.Empty;

                                            if (mainOperand == "And")
                                            {
                                                transitions1.Operand = "&&";
                                            }
                                            else if (mainOperand == "Or")
                                            {
                                                transitions1.Operand = "||";
                                            }

                                            string totcondition = condition.InnerText;

                                            string[] conditionalOpr = new string[] { "&&", "||" };

                                            string copr = "";

                                            foreach (string opr in conditionalOpr)
                                            {
                                                if (totcondition.Contains(opr))
                                                {
                                                    copr = opr;
                                                    transitions1.Operand = copr;
                                                    break;
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(totcondition))
                                            {

                                                string[] oprSPlitValue = totcondition.Split(new[] { copr }, StringSplitOptions.None);


                                                if (oprSPlitValue.Count()>0)
                                                {
                                                    foreach (string cond in oprSPlitValue)
                                                    {
                                                        if (!string.IsNullOrEmpty(cond))
                                                        {
                                                            string[] assignments = new string[] { "==", ">=", "<=", "!=", ">", "<" };

                                                            string condassig = "";
                                                            foreach (string assignment in assignments)
                                                            {
                                                                if (cond.Contains(assignment))
                                                                {
                                                                    condassig = assignment;
                                                                }
                                                            }


                                                            string[] wfConditions = cond.Split(new[] { condassig }, StringSplitOptions.None);


                                                            if (wfConditions!= null && wfConditions.Count()>0)
                                                            {
                                                                Condition condition1 = new Condition();
                                                                condition1.Expression = wfConditions[0].Replace("@", "");
                                                                condition1.Value = wfConditions[1];

                                                                if (condition1.Value.ToLower().Contains("createtasks"))
                                                                {

                                                                }

                                                                if (int.TryParse(wfConditions[1], out int val))
                                                                {
                                                                    condition1.ConditionType ="int";
                                                                }
                                                                else if (Boolean.TryParse(wfConditions[1], out bool val2))
                                                                {
                                                                    condition1.ConditionType ="bool";
                                                                }
                                                                else
                                                                {
                                                                    condition1.ConditionType ="string";
                                                                }

                                                                condition1.Assignment =condassig;

                                                                condition1.Operand = copr;

                                                                transitions1.Condition.Add(condition1);

                                                            }

                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                transitions.Add(transitions1);
                            }
                        }
                        if (child.Name == "Parameters")
                        {
                            foreach (XmlNode codeNode in child.ChildNodes)
                            {
                                XElement element = XElement.Parse(codeNode.OuterXml);
                                string nameRefValue = element.Attribute("Name")?.Value ?? string.Empty;
                                string value = element.Attribute("InitialValue")?.Value ?? string.Empty;
                                paramters.Add(nameRefValue, value);
                            }
                        }
                    }

                    CreateClass(classname);

                    CompleteClass();

                    CreateFile(classname);
                }

                Console.WriteLine("Extraction Done");

                void CreateClass(string clsname)
                {
                    stringBuilder.AppendLine("using Newtonsoft.Json;\r\nusing System.Globalization;\r\nusing Newtonsoft.Json.Linq;\r\nusing OptimaJet.Workflow.Core.Model;\r\nusing OptimaJet.Workflow.Core.Runtime;\r\nusing System;\r\nusing System.Collections.Generic;\r\nusing System.Dynamic;\r\nusing System.IO;\r\nusing System.Linq;\r\nusing System.Net;\r\nusing System.Net.Http;\r\nusing System.Net.Http.Headers;\r\nusing System.Net.NetworkInformation;\r\nusing System.Security.Policy;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\nusing System.Xml.Linq;\r\nusing System.Text.RegularExpressions;\r\nusing System.Security.Cryptography;\r\n");

                    stringBuilder.AppendLine("namespace Avihs.WorkFlow.Schemes\r\n{");

                    stringBuilder.AppendLine($"public class {clsname} : BaseProcess {{");

                    stringBuilder.AppendLine($"public {clsname}() :\r\n base(){{");

                    foreach (var item in paramters)
                    {
                        stringBuilder.AppendLine($"processInstance.SetParameter<string>(\"{item.Key}\", \"{item.Value.Replace("\"", "")}\");");
                    }

                    stringBuilder.AppendLine("}");

                    foreach (string str in codeActions)
                    {
                        stringBuilder.AppendLine(str);
                    }

                }

                void AppendCode(string methodName, string code)
                {
                    if (!addedMethods.Contains(methodName))
                    {
                        StringBuilder stringBuilder1 = new StringBuilder();
                        stringBuilder1.AppendLine($"public async Task {methodName}(string parameter){{");
                        stringBuilder1.AppendLine(code);
                        stringBuilder1.AppendLine("}");
                        codeActions.Add(stringBuilder1.ToString());
                        addedMethods.Add(methodName);
                    }
                }

                void Build()
                {
                    Node node = new Node();

                    Activity initialActivity = activities.Where(x => x.IsInitial=="True").First();

                    node.parentNode = null;

                    node.activity = initialActivity;

                    Transitions fromTransaction = transitions.Where(x => x.From == initialActivity.Name).FirstOrDefault();

                    node.transitions = fromTransaction;

                    BuildTree(fromTransaction, node);
                }

                void BuildTree(Transitions _fromTransaction, Node rootNode)
                {
                    if (_fromTransaction == null || _fromTransaction.processFurther == false)
                    {
                        nodes.Add(rootNode);
                        return;
                    }

                    List<Transitions> tans = transitions.Where(x => x.From == _fromTransaction.From)?.ToList();

                    if (tans!= null)
                    {
                        foreach (var fromTransaction in tans)
                        {
                            Node current = rootNode;

                            bool ProcessFurther = true;

                            while (current != null)
                            {
                                if (current.transitions.From == fromTransaction.To)
                                {
                                    ProcessFurther= false;
                                    break;
                                }

                                current = current.parentNode;
                            }


                            Node node = new Node();
                            node.parentNode=rootNode;
                            node.transitions=fromTransaction;
                            Activity initialActivity = activities.Where(x => x.Name== fromTransaction.To).First();
                            node.activity = initialActivity;
                            Transitions scr = transitions.Where(x => x.From == fromTransaction.To).FirstOrDefault();

                            if (scr != null)
                            {
                                scr.processFurther = ProcessFurther;
                            }

                            BuildTree(scr, node);
                        }
                    }
                }

                void AppendLogic()
                {
                    foreach (Node node in nodes)
                    {
                        Node currentNode = node;

                        string ChildContent = string.Empty;

                        while (currentNode != null)
                        {
                            if (currentNode!= null)
                            {
                                StringBuilder str = new StringBuilder();

                                bool hasCondition = false;

                                if (currentNode.transitions.Condition.Count>0)
                                {
                                    List<string> ifcond = new List<string>();
                                    foreach (var condtition in currentNode.transitions.Condition)
                                    {
                                        string variableName = GetRandom();

                                        str.AppendLine($"var {variableName} = processInstance.GetParameter<{condtition.ConditionType}>(\"{condtition.Expression}\");");
                                        ifcond.Add($"{variableName}{condtition.Assignment}{condtition.Value}");
                                    }


                                    str.AppendLine($"if({string.Join($" {currentNode.transitions.Operand.ToString()} ", ifcond.ToArray())})");
                                    hasCondition= true;
                                }

                                if (hasCondition)
                                    str.AppendLine($"{{");

                                foreach (string actions in currentNode.activity.Actions)
                                {
                                    str.AppendLine($"await {actions}();");
                                }

                                str.AppendLine(ChildContent);

                                if (hasCondition)
                                    str.AppendLine($"}}");

                                if (!string.IsNullOrEmpty(str.ToString()))
                                {
                                    ChildContent = str.ToString();
                                }

                                currentNode = currentNode.parentNode;

                                if (currentNode== null)
                                {
                                    stringBuilder.AppendLine(ChildContent);
                                }
                            }
                        }
                    }
                }

                void AddOtherMethods()
                {
                    stringBuilder.AppendLine("public async void ExecuteWorkFlow()");

                    stringBuilder.AppendLine("{");

                    stringBuilder.AppendLine($"await ExecuteLogic(nameof({classname}), this);");

                    stringBuilder.AppendLine("}");
                }

                void CompleteClass()
                {
                    AddOtherMethods();

                    stringBuilder.AppendLine("}");
                    stringBuilder.AppendLine("}");
                }

                void CreateFile(string filename)
                {
                    DirectoryInfo current = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

                    DirectoryInfo parent = current.Parent.Parent.Parent;

                    string rootPath = $"{parent.FullName}\\Schemes\\{filename}";

                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }

                    File.Copy("WorkFlow.xml", $"{rootPath}\\{filename}.xml", true);


                    if (!File.Exists($"{rootPath}\\inputParams.json"))
                    {
                        WorkflowLaunchModel workflowLaunchModel = new WorkflowLaunchModel();

                        workflowLaunchModel.schemeCode =filename;

                        File.WriteAllText($"{rootPath}\\inputParams.json", Newtonsoft.Json.JsonConvert.SerializeObject(workflowLaunchModel));
                    }


                    if (parent != null)
                    {
                        string path = $"{rootPath}\\{filename}.cs";

                        File.WriteAllText(path, stringBuilder.ToString());
                    }

                    string jsonPath = $"{rootPath}\\{filename}.json";

                    RootMap rootMap = new RootMap();
                    rootMap.Activities= activities;
                    rootMap.Transitions = transitions;
                    File.WriteAllText(jsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(rootMap));

                    string[] strings = new string[] {
                        $"Schemes\\{filename}\\{filename}.json",
                        $"Schemes\\{filename}\\{filename}.xml",
                        $"Schemes\\{filename}\\inputParams.json"};

                    AddFileToCsproj($"{parent.FullName}\\WorkFlowDebugger.csproj", strings);
                }

                void AddFileToCsproj(string csprojFilePath, string[] filePathToAdd)
                {
                    try
                    {
                        XDocument doc = XDocument.Load(csprojFilePath);

                        XElement itemGroup = doc.Root
                            .Elements("ItemGroup")
                            .FirstOrDefault(); // Find the first ItemGroup element

                        if (itemGroup == null)
                        {
                            itemGroup = new XElement("ItemGroup");
                            doc.Root.Add(itemGroup); // If there's no ItemGroup, create one
                        }

                        foreach (string filePath in filePathToAdd)
                        {
                            XElement contentElement = new XElement("Content", new XAttribute("Include", filePath), new XAttribute("CopyToOutputDirectory", "Always"));
                            itemGroup.Add(contentElement);
                        }

                        doc.Save(csprojFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message+ ex.StackTrace);
            }
        }

        static string GetRandom()
        {
            Random random = new Random();

            Thread.Sleep(10);

            const string alphabet = "abcdefghijklmnopqrstuvwxyz";

            char[] randomChars = new char[5];

            for (int i = 0; i < 5; i++)
            {
                int randomIndex = random.Next(alphabet.Length);
                randomChars[i] = alphabet[randomIndex];
            }

            return new string(randomChars);
        }
    }
}
