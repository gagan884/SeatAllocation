using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public class CustomeResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }

        public CustomeResponse(string code, string message, object value)
        {
            Code = code;
            Message = message;
            Value = value;
        }
    }
}
