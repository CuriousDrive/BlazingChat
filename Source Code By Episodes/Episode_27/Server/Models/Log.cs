using System;
using System.Collections.Generic;

#nullable disable

namespace BlazingChat.Server.Models
{
    public partial class Log
    {
        public long LogId { get; set; }
        public string LogLevel { get; set; }
        public string EventName { get; set; }
        public string Source { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string CreatedDate { get; set; }
    }
}
