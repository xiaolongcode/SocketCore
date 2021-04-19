using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeiot.Model.Base
{
    public class Property : System.Attribute
    {
        public string Value { get; set; }

        public Property(string Value)
        {
            this.Value = Value;
        }
    }
}
