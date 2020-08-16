using System;
using System.Windows.Forms;

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
      OFD.Filter = "txt files (*.txt)|*.txt";
      OFD.ShowDialog();
      return OFD.FileName;
    }



    /// <summary>
    /// Calls the analizer, process the file and generates
    /// the output file
    /// </summary>
    private void ProcessFile(string FilePath){
      if(FilePath != null){
        WriteAndWait("Procesando archivo");
        new LexicalAnalyzer().Analize(FilePath);
        WriteAndWait("Archivo Procesado correctamente");
      } else {
        WriteAndWait("Debe seleccionar un archivo primero!");
      }
    }
    

    
    /// <summary>Terminates the app process</summary>
    private void TerminateProcess(){
      WriteAndWait("Gracias por usar nuestro sistema!\nCualquier bug que encuentre, por favor\nreportarlo en el repositorio de Github.\nGracias!!! :)");
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
    
    #endregion

  }
}