using System;
using System.Windows.Forms;

namespace MiniCSharp
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      string FilePath = "";
      while (true)
      {
        PrintMenu(FilePath);
        if(ParseNumber(Console.ReadLine(), out int response))
          Menu(response, ref FilePath);
        else
          continue;
      }
    }


    static void PrintMenu(string FilePath){
      Console.Clear();
      Console.WriteLine("Mini C#");
      Console.WriteLine("Archivo Actual =>  " + FilePath);
      Console.WriteLine("1 => Subir Archivo");
      Console.WriteLine("2 => Procesar Archivo");
      Console.WriteLine("3 => Salir del programa");
    }


    static bool ParseNumber(string readedString, out int response){
      if (int.TryParse(readedString, out response)) return true;
      
      WriteAndWait("Por favor ingresar un numero");
      return false;
    }

    static void Menu(int response, ref string FilePath){
      switch (response)
        {
          case 1:
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = false;
            OFD.Title = "Select file to process";
            OFD.Filter = "txt files (*.txt)|*.txt";
            OFD.ShowDialog();
            FilePath = OFD.FileName;
            break;
          
          case 2:
            WriteAndWait("Procesando archivo");
            break;
          case 3:
            WriteAndWait("Gracias por usar nuestro sistema!\nCualquier bug que encuentre, por favor\nreportarlo en el repositorio de Github.\nGracias!!! :)");
            Environment.Exit(0);
            break;
          
          default:
            WriteAndWait("No se encontro la opcion indicada, por favor intente con un numero valido");
            break;
        }
    }

    static void WriteAndWait(string text){
      Console.WriteLine(text);
      Console.ReadKey();
    }

  }
}