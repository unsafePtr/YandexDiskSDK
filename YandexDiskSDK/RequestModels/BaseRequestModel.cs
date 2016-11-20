using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.RequestModels
{
    public abstract class BaseRequestModel
    {
        private Dictionary<string, string> parameters;
        
        protected BaseRequestModel()
        {
            this.parameters = new Dictionary<string, string>();
        }

        protected abstract void AddRequestParameters();
        protected abstract string PathSuffix { get; }
        
        public string RequestUrl(string baseUrl)
        {
            ThrowIfNullArgument(baseUrl);

            AddRequestParameters();

            string path = baseUrl + this.PathSuffix + "?";
            
            foreach (var parameter in this.parameters)
            {
                char lastPathSymbol = path.Last();
                if (lastPathSymbol == '&' || lastPathSymbol == '?')
                    path += parameter.Key + "=" + parameter.Value;
                else
                    path += "&" + parameter.Key + "=" + parameter.Value;
            }

            return path;
        }
        
        protected void AddParameter(string key, string value)
        {
            ThrowIfNullArgument(key);

            if (value != null)
                parameters[key] = value;
        }

        protected void AddParameter<T>(string key, T value)
        {
            ThrowIfNullArgument(key);

            if (value != null)
                parameters[key] = value.ToString();
        }

        protected void AddParameter<T>(string key, Func<T> valueCallback)
        {
            ThrowIfNullArgument(key);
            ThrowIfNullArgument(valueCallback);

            T value = valueCallback();
            if (value != null)
                parameters[key] = value.ToString();
        }

        protected void AddParameter(string key, Func<string> valueCallback)
        {
            ThrowIfNullArgument(key);
            ThrowIfNullArgument(valueCallback);

            string value = valueCallback();
            if (value != null)
                parameters[key] = value;
        }
        
        protected void AddParameter(KeyValuePair<string, string> parameter)
        {
            if (parameter.Key != null && parameter.Value != null)
                parameters[parameter.Key] = parameter.Value;
        }

        protected void ThrowIfNullArgument(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
        }
    }
}
