using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Avihs.WorkFlow
{
    public class ExecuteWorkflow
    {
        public static async Task<WorkFlowInstanceParameters> GetProcessParameters(string ProcessId)
        {

            string json = "{}";

            string result = string.Empty;

            try
            {
                //Set the workflow url here
                var launchRequest = CreateWorkflowCall($"https://workflow.com/workflowapi/getinstanceinfo/{ProcessId}");
                launchRequest.ContentType = "application/json";
                launchRequest.Method = "POST";
             
                using (var streamWriter = new StreamWriter(launchRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = await launchRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();

                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return  string.IsNullOrEmpty(result) ? null: JsonConvert.DeserializeObject<WorkFlowInstanceParameters>(result);

            HttpWebRequest CreateWorkflowCall(string address)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(address);
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";
                return webRequest;
            }           
        }
    }
}
