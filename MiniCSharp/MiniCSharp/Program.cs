using System;
using Clases;

namespace MiniCSharp
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      new MainMenu().Run();
    }
  }
}