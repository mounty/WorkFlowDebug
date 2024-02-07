using Newtonsoft.Json;
using Avihs.WorkFlow.XMLCodeExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Avihs.WorkFlow
{
    internal class StartLogic
    {
        public Dictionary<int, string> classes = new Dictionary<int, string>();

        public void WorkFlow()
        {
            List<int> availabel = new List<int>() { 1, 2 };

            Console.WriteLine("Available Codes");
            Console.WriteLine("1: Generate xml");
            Console.WriteLine("2: Execute new workflow scheme");
            
            bool NextExecution = true;

            Console.Write("Enter Code:");

            while (NextExecution)
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out int code))
                {
                    if (availabel.Contains(code))
                    {
                        switch (code)
                        {
                            case 1:
                                XMLExtractor.Extract();
                                NextExecution =false;
                                break;
                            case 2:
                                WorkFlowLogic();
                                NextExecution =false;
                                break;                           
                        }
                    }
                    else
                    {
                        Console.WriteLine("Code not found in execution list");
                    }
                }
                else
                {
                    Console.WriteLine("Code not found in execution list");
                }
            }


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public void WorkFlowLogic()
        {
            ListAvailableSchemes();

            bool validCode = false;

            while (!validCode)
            {
                string code = Console.ReadLine();

                if (int.TryParse(code, out int exexecutionCode))
                {
                    if (classes.TryGetValue(exexecutionCode, out string className))
                    {
                        LaunchScheme(className);
                        validCode = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter valid scheme code from list");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter valid Code");
                }
            }
        }

      
        public void ListAvailableSchemes()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Get all types in the assembly
            Type[] types = currentAssembly.GetTypes();

            // Filter types by namespace
            var namespaceTypes = types
                .Where(type => type.Namespace == "Avihs.WorkFlow.Schemes" && type.IsClass && type.IsVisible)
                .ToList();

            // Display the names of the classes in the namespace

            Console.WriteLine($"List of Available Schemes");

            if (namespaceTypes.Any())
            {
                int count = 0;
                foreach (var type in namespaceTypes)
                {
                    string[] codes = type.FullName.Split('.');
                    Console.WriteLine($"{count}:{codes[codes.Length-1]}");
                    classes.Add(count, type.FullName);
                    count++;
                }
                Console.Write($"Enter Scheme Code");
            }
            else
            {
                Console.WriteLine($"No classes found in namespace 'Avihs.WorkFlow.Schemes'.");
            }
        }

        private bool LaunchScheme(string classname)
        {
            return Execute(classname);
        }

        private bool Execute(string SchemeName)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type type = currentAssembly.GetType(SchemeName);
            object instance = Activator.CreateInstance(type, null);
            MethodInfo method = type.GetMethod("ExecuteWorkFlow");
            if (method != null)
            {
                object value = method.Invoke(instance, null);
            }
            return true;
        }      
    }
}