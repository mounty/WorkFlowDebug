using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Avihs.WorkFlow.Schemes
{
    public class Sample : BaseProcess
    {
        public Sample() :
         base()
        {
            processInstance.SetParameter<string>("Sampe", "Sample");
        }
        public async Task Start(string parameter)
        {
            string str = "Hello";
        }

        public async Task Execute(string parameter)
        {
            string str = "execute";
        }

        public async Task Final(string parameter)
        {
            string str = "Final";
        }

        public async void ExecuteWorkFlow()
        {            
            // set the ProcessId in the inputParams.json and execute below 
            //await ExecuteLogic(nameof(Sample), this);
        }
    }
}
