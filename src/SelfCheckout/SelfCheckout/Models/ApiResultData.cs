using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfCheckout.Models
{
    public class ApiResultData<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty("listOfProcessTracking")]
        public ProcessTracking ListOfProcessTracking { get; set; }

        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }

        [JsonProperty("Message")]
        public Message[] Message { get; set; }

        public string DefaultMessage
        {
            get
            {
                var message = "";
                foreach (var msg in Message)
                {
                    message += msg.MessageDesc + "\n";
                }
                return message;
            }
        }
    }

    public class Message
    {
        [JsonProperty("MessageType")]
        public string MessageType { get; set; }

        [JsonProperty("MessageCode")]
        public string MessageCode { get; set; }

        [JsonProperty("MessageDesc")]
        public string MessageDesc { get; set; }
    }

    public class ProcessTracking
    {
        public int ProcessSeq { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDT { get; set; }
        public string ProcessData { get; set; }
    }

    public class Tracking
    {
        [JsonProperty("MyException")]
        public object MyException { get; set; }

        [JsonProperty("Namespace")]
        public string Namespace { get; set; }

        [JsonProperty("ClassName")]
        public string ClassName { get; set; }

        [JsonProperty("MethodName")]
        public string MethodName { get; set; }

        [JsonProperty("AssemblyInfo")]
        public string AssemblyInfo { get; set; }
    }
}
