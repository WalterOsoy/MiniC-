﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    class Token
    {
        public string Value { get; set; }
        public string type { get; set; }
        public string line { get; set; }
        public string column { get; set;} 

        public override string ToString() {
          return Value;
        }
    }
}
