using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures {
  class TrackItem {
    public string symbol { get; set; }
    public string aux = "";
    public int stackNumber { get; set; }
    public bool accepted = false;

    public override string ToString() {
      return symbol + " " + aux + " " + stackNumber;
    }
  }
}