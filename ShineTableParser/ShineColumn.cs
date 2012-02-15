﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShineTableParser
{
    class ShineColumn : DataColumn
    {
        public string TypeName { get; private set; }

        public ShineColumn(string name, string type, ref int unkColumns)
        {
            if (name.Length < 2)
            {
                name = "Unk (" + unkColumns++ + ")";
            }

            this.Caption = name;
            this.ColumnName = name;
            
            this.TypeName = type;

            if (type.StartsWith("string["))
            {
                int from = type.IndexOf('['), till = type.IndexOf(']');
                string lenStr = type.Substring(from + 1, (till - from) - 1);
                int len = int.Parse(lenStr);
                this.MaxLength = len;
            }

        }


        public Type GetColumnType()
        {
            var typeName = TypeName.ToLower();
            switch (typeName)
            {
                case "byte": return typeof(Byte);

                case "word": return typeof(Int16);

                case "<integer>":
                case "dwrd":
                case "dword": return typeof(Int32);

                case "qword": return typeof(Int64);

                case "index": return typeof(String);

                case "<string>":
                case "string": return typeof(String);
            }
            /*
            if (typeName.StartsWith("string["))
            {
                // char array
                return typeof(char[]);
            }
            */
            return typeof(string); // Just to be sure ?!?! D:
        }
    }
}
