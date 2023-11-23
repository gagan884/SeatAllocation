using AppFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public class ActionOutput
    {
        public ActionStatus resultType { get; set; }
        public string message { get; set; }
        public object data {get;set;}
        public DataType dataType {get;set; }

        public ActionOutput(ActionStatus resultType)
        {
            this.resultType = resultType;
            message = string.Empty;
            dataType = DataType.None;
        }

        public ActionOutput(ActionStatus resultType, string message)
        {
            this.resultType = resultType;
            this.message = message;
            dataType = DataType.None;
        }

        public ActionOutput(ActionStatus resultType, string message,object data,DataType dataType)
        {
            this.resultType = resultType;
            this.message = message;
            this.dataType = dataType;
            this.data = data;
        }
    }
}
