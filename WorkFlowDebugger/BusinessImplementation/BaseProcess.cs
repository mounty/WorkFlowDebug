using Newtonsoft.Json;
using System.Reflection;

namespace Avihs.WorkFlow
{
    public class BaseProcess
    {
        internal ProcessInstance processInstance;
        public BaseProcess()
        {
            processInstance = new ProcessInstance();
            processInstance.SetParameter<string>("Parameter", "");
        }

        public async Task<string> SetParameters(WorkflowLaunchModel model, string processId)
        {
            if (model.ProcessId == null ||  model.ProcessId.Count() ==0)
            {
                foreach (WorkflowParameter parameter in model.parameters)
                {
                    processInstance.SetParameter<string>(parameter.name, parameter.value);
                }
            }
            else
            {
                await SetParametersByProcessId(processId);
            }

            string returnResult = string.Empty;

            if (model.ProcessId != null && model.ProcessId.Count() >0)
            {
                int index = Array.IndexOf(model.ProcessId, processId);

                if (model.ProcessId.Count() < index)
                {
                    returnResult = model.ProcessId[index];
                }
            }

            return returnResult;
        }

        private async Task<WorkflowLaunchModel> GetWorkflowLaunchModel(string srcPath)
        {
            string param = File.ReadAllText($"Schemes\\{srcPath}\\inputParams.json");

            WorkflowLaunchModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkflowLaunchModel>(param);

            return model;
        }


        public async Task SetParametersByProcessId(string ProcessId)
        {
            WorkFlowInstanceParameters param = await ExecuteWorkflow.GetProcessParameters(ProcessId);

            string[] ignoreKeys = new string[] { "accesstoken", "usercode" };
            if (param?.data?.ProcessParameters?.Count>0)
            {
                foreach (KeyValuePair<string, object> inputParam in param.data.ProcessParameters)
                {
                    if (!ignoreKeys.Contains(inputParam.Key.ToLower()))
                    {
                        processInstance.SetParameter<object>(inputParam.Key, inputParam.Value);
                    }
                    else
                    {
                        
                    }
                    
                }

                processInstance.SetParameter<Guid>("ProcessId", param.data.RootProcessId);

                Console.WriteLine($"ProcessId:{param.data.RootProcessId}");
            }
        }

        public async Task SetParameters(string fileName)
        {
            WorkflowLaunchModel workflowLaunchModel = await GetWorkflowLaunchModel(fileName);

            foreach (WorkflowParameter parameter in workflowLaunchModel.parameters)
            {
                processInstance.SetParameter<string>(parameter.name, parameter.value);
            }
        }


        public async Task ExecuteLogic(string fileName, object obj)
        {
            WorkflowLaunchModel workflowLaunchModel = await GetWorkflowLaunchModel(fileName);

            if ((workflowLaunchModel.parameters != null && workflowLaunchModel.parameters.Count>0) ||
                (workflowLaunchModel.ProcessId != null && workflowLaunchModel.ProcessId.Count()>0))
            {
                bool execute = true;

                string firstProcessId = workflowLaunchModel.ProcessId[0];

                while (execute)
                {
                    string nextProcessId = await SetParameters(workflowLaunchModel, firstProcessId);

                    if (string.IsNullOrEmpty(nextProcessId))
                    {
                        execute = false;
                    }

                    await PreExecution(fileName, obj);
                }

                Console.WriteLine("Completed");
            }
            else
            {
                Console.WriteLine($"input ProcessId and parameters are not available for scheme. Please check  Schemes->{fileName}->inputParams.json");
            }
        }
        public async Task PreExecution(string fileName, object obj)
        {
            string content = File.ReadAllText($"Schemes\\{fileName}\\{fileName}.json");

            RootMap map = JsonConvert.DeserializeObject<RootMap>(content);

            List<Transitions> transitions = map.Transitions;

            Activity fromActivity = map.Activities.Where(x => x.IsInitial=="True").First();

            foreach (string action in fromActivity.Actions)
            {
                await InvokeMethodByName(action, obj);
            }

            Transitions fromTransaction = transitions.Where(x => x.From == fromActivity.Name).FirstOrDefault();

            Activity toActivity = map.Activities.Where(x => x.Name == fromTransaction.To).First();

            await PerformLogic(fromTransaction, toActivity, map, obj);
        }

        public async Task PerformLogic(Transitions Transaction, Activity Activity, RootMap map, object obj)
        {

            string[] assignments = new string[] { "==", ">=", "<=", "!=", ">", "<" };

            List<Condition> conditions = Transaction.Condition;

            List<bool> ExecuteLogic = new List<bool>();

            bool ProceedFurther = false;

            if (conditions.Count>0)
            {
                foreach (Condition condition in conditions)
                {
                    var Value = processInstance.GetParameter<dynamic>(condition.Expression);
                    var compareValue = condition.Value.Replace("\"", "");
                    if (Value != null)
                    {
                        switch (condition.Assignment)
                        {
                            case "==":
                                ExecuteLogic.Add((Value == compareValue));
                                break;
                            case ">=":
                                ExecuteLogic.Add(Value >= compareValue);
                                break;
                            case "<=":
                                ExecuteLogic.Add(Value <= compareValue);
                                break;
                            case "!=":
                                ExecuteLogic.Add(Value != compareValue);
                                break;
                            case ">":
                                ExecuteLogic.Add(Value > compareValue);
                                break;
                            case "<":
                                ExecuteLogic.Add(Value < compareValue);
                                break;
                        }
                    }

                    if (condition.Operand == "&&" || Transaction.Operand == "&&")
                    {
                        int falseCount = ExecuteLogic.Where(x => x==false).Count();
                        int trueCount = ExecuteLogic.Where(x => x==true).Count();
                        if (falseCount >0 && trueCount >0)
                        {
                            ProceedFurther = false;
                            break;
                        }

                        if (falseCount >=0)
                        {
                            ProceedFurther = false;
                        }
                        else
                        {
                            ProceedFurther = true;
                        }
                    }
                    else if (condition.Operand == "||"  || Transaction.Operand == "||")
                    {
                        int trueCount = ExecuteLogic.Where(x => x==true).Count();

                        if (trueCount >0)
                        {
                            ProceedFurther = true;
                        }
                        else
                        {
                            ProceedFurther = false;
                        }
                    }
                }
            }
            else
            {
                ProceedFurther=true;
            }

            if (ProceedFurther)
            {
                foreach (string action in Activity.Actions)
                {
                    await InvokeMethodByName(action, obj);
                }

                List<Transitions> toTransaction = map.Transitions.Where(x => x.From == Transaction.To).ToList();

                foreach (Transitions transition in toTransaction)
                {
                    if (toTransaction != null)
                    {
                        Activity toActivity = map.Activities.Where(x => x.Name == transition.To).FirstOrDefault();

                        if (toTransaction != null && toActivity != null)
                        {
                            await PerformLogic(transition, toActivity, map, obj);
                        }
                    }
                }
            }
        }

        public async Task InvokeMethodByName(string methodName, object obj)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName);

            if (method != null)
            {
                object[] parameters = { "Hello, MethodInfo!" };

                object result = method.Invoke(obj, parameters);
            }
            else
            {
                Console.WriteLine($"Method {methodName} not found in {GetType().Name}");
            }
        }
    }
}
