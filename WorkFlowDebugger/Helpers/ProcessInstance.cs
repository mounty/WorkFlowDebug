using Newtonsoft.Json.Linq;
using OptimaJet.Workflow.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avihs.WorkFlow
{
    public class ProcessInstance
    {
        public Guid ProcessId = Guid.NewGuid();

        private Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        
        public T GetParameter<T>(string key) 
        {
            try
            {
                keyValuePairs.TryGetValue(key, out object val);
                return (T)(val);
            }
            catch(Exception ex)
            {
                //throw;
            }

            return default(T);
        }

        public void SetParameter<T>(string key, T value)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
        }

        public void SetParameter<T>(string key, T value, ParameterPurpose parameterPurpose = ParameterPurpose.Persistence)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
        }


        public void SetParameter(string key, object value, ParameterPurpose parameterPurpose = ParameterPurpose.Persistence)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
        }
    }
}
