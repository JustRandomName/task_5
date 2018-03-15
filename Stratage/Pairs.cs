using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratage {
   public class Pairs {

        static public object[] value { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public Pairs(string tp, string nm) {
            type = tp;
            name = nm;
        }
        enum ArgumentTypes
        {
            String,
            Int,
            Long,
            Boolean,
            Double,
            DateTyme
        }

    }
}
