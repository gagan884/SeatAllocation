
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public class DocumentStyleSheet
    {
        public static string DefaultStyle = @"<style>
        h3 { color:darkblue;
             padding:4px 2px;
        }

        h4 { color:darkslategray;
             padding:4px 2px;
        }

        table {
            border:1px solid lightgray;
            border-collapse:collapse;
            width:98%;
            margin:10px 0px;
        }

        table tr td{
            border:1px solid lightgray;
            border-collapse:collapse;
            padding:4px 2px;
        }

        table tr th{
            border:1px solid lightgray;
            border-collapse:collapse;
            padding:4px 2px;
            font-weight:600;
            background-color: lightblue;
    
        }

    </style>";
    }
}
