using System;
using System.Windows.Forms;
using System.Collections.Generic;
using DataStructures;

namespace Clases
{

  /// <summary>Class that prints and handles the main menu</summary>
  class MainMenu
  {

    #region Principal Methods

    /// <summary>
    /// Main method of the menu, runs all the code and 
    /// the connections with other services.
    /// </summary>
    public void Run(){
    string WorkingFile = "";
      while (true){
        PrintMenu(WorkingFile);

        if(ParseNumber(Console.ReadLine(), out int response))
          MenuService(response, ref WorkingFile);
        else
          continue;
      }
    }



    /// <summary>Runs the service selected by the user</summary>
    /// <param name="response">Service selected by the user</param>
    /// <param name="FilePath">Selected file that services will work with</param>
    private void MenuService(int response, ref string FilePath){
      switch (response){
        //Choose File
        case 1:
          FilePath = ChooseFile();
          break;
        
        //Process File
        case 2:
          ProcessFile(FilePath);
          break;
        
        //Exit
        case 3:
          TerminateProcess();
          break;
        
        //Not Found
        default:
          WriteAndWait("No se encontro la opcion indicada, por favor intente con un numero valido");
          break;
      }
    }

    #endregion


    #region Services

    /// <summary>
    /// Opens a File Dialog for the user to choose the
    /// file that wants to process
    /// </summary>
    /// <returns>String with the path of the file</returns>
    private string ChooseFile(){
      OpenFileDialog OFD = new OpenFileDialog();
      OFD.Multiselect = false;
      OFD.Title = "Select file to process";
      OFD.ShowDialog();
      return OFD.FileName;
    }



    /// <summary>
    /// Calls the analizer, process the file and generates
    /// the output file
    /// </summary>
    private void ProcessFile(string FilePath){
      if(FilePath != null && FilePath != ""){
        bool success;
        AnalizeLexicon(FilePath, out success, out Queue<Token> tokensQueue);
        if(success) AnalizeSintax(FilePath, out success, ref tokensQueue);
        else Console.WriteLine("Se encontro uno o más errores mientras se analizaba el Lexico, finalizando programa");
        WriteAndWait("Archivo de salida: " + FilePath.Replace("frag", "out"));
      } else {
        WriteAndWait("Debe seleccionar un archivo primero!");
      }
    }
    

    
    /// <summary>Terminates the app process</summary>
    private void TerminateProcess(){
      WriteAndWait(
        "Gracias por usar nuestro sistema!\n" +
        "Cualquier bug que encuentre, por favor\n" +
        "reportarlo en el repositorio de Github.\n" +
        "Gracias!!! :)\n" +
        "https://github.com/WalterOsoy/MiniCSharp"
      );
      
      Console.Clear();
      Environment.Exit(0);
    }
    
    #endregion


    #region Helpfull methods 

    /// <summary>Prints the available options of the main menu</summary>
    /// <param name="FilePath">Selected file that services will work with</param>
    private void PrintMenu(string FilePath){
      Console.Clear();
      Console.WriteLine("Mini C#");
      Console.WriteLine("Archivo Actual =>  " + FilePath);
      Console.WriteLine("1 => Subir Archivo");
      Console.WriteLine("2 => Procesar Archivo");
      Console.WriteLine("3 => Salir del programa");
    }



    /// <summary>Tries to parse a string to number</summary>
    /// <param name="readedString">string given by the user</param>
    /// <param name="response">response in number type</param>
    /// <returns>True if string was succesfully parsed. False if not</returns>
    private bool ParseNumber(string readedString, out int response){
      if (int.TryParse(readedString, out response)) return true;
      
      WriteAndWait("Por favor ingresar un numero");
      return false;
    }



    /// <summary>Console.WriteLine() and ReadKey() in one method</summary>
    /// <param name="text">Text to log in the console</param>
    private void WriteAndWait(string text){
      Console.WriteLine(text);
      Console.ReadKey();
    }
    

    private void AnalizeLexicon(string FilePath, out bool succed, out Queue<Token> tokensQueue){
      Console.Clear();
      Console.WriteLine("Analizando Lexico...");
      succed = new LexicalAnalyzer(FilePath).Analize(out tokensQueue);
      Console.WriteLine("Archivo Analizdo");
    }

    private void AnalizeSintax(string FilePath, out bool succed, ref Queue<Token> tokensQueue){
      Console.Clear();
      Console.WriteLine("Analizando Sintaxis...");
      succed = new SintacticalAnalizer(ref tokensQueue).Analize();
      Console.WriteLine("Archivo Analizdo");
    }

    #endregion

  }
}