using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Column : Attribute
    {
        public string Name { get; set; }

        public string Table { get; set; }

        public bool Insert { get; set; }

        public bool Update { get; set; }

        public Column(string name)
        {
            Name = name;
            Insert = true;
            Update = true;
        }

        public Column(string name, string table)
        {
            Name = name;
            Table = table;
            Insert = true;
            Update = true;
        }

        public Column(string name, string table, bool insert, bool update)
        {
            Name = name;
            Table = table;
            Insert = insert;
            Update = update;
        }
    }
}
