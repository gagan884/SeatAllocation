using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public class DataType : IEquatable<DataType>
    {
        public int code { get; private set; }
        public string name { get; private set; }

        public static DataType None => new DataType(1, "None");
        public static DataType DataTable => new DataType(2, "DataTable");
        public static DataType DataSet => new DataType(3, "DataSet");
        public static DataType ListOfString => new DataType(4, "List Of String");
        public static DataType KeyValuePair => new DataType(5, "Key Value Pair");
        public static DataType StringValue => new DataType(6, "String Value");
        public static DataType IntValue => new DataType(7, "Int Value");
        public static DataType ByteArray => new DataType(8, "ByteArray");
        public static DataType HTML => new DataType(9, "HTML");

        private DataType(int code, string name)
        {
            this.code = code;
            this.name = name;
        }

        public bool Equals(DataType other)
        {
            return Equals(other as DataType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DataType);
        }

        public override string ToString()
        {
            return name.ToString();
        }

        public static bool operator ==(DataType left, DataType right)
        {
            if ((object)left == null)
            {
                return ((object)right == null);
            }
            else if ((object)right == null)
            {
                return ((object)left == null);
            }

            return left.code==right.code;
        }



        public static bool operator !=(DataType left, DataType right)
        {
            return !(left.code == right.code);
        }

        public static DataType GetObjectDataType(int code)
        {
            switch (code)
            {
                case 1:
                    return None;
                case 2:
                    return DataTable;
                case 3:
                    return DataSet;
                case 4:
                    return ListOfString;
                case 5:
                    return KeyValuePair;
                case 6:
                    return StringValue;
                case 7:
                    return IntValue;
                case 8:
                    return ByteArray;
                case 9:
                    return HTML;
                default:
                    throw new InvalidCastException();
            }
        }


    }
}
