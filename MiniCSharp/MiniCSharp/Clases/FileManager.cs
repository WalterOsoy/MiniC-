using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Clases
{
  class FileManager
  {
    private BinaryReader BinaryRead;
    private BinaryWriter BinaryWrite;

    public FileManager(string FilePath){
      BinaryRead = new BinaryReader(new FileStream(FilePath, FileMode.Open));
      BinaryWrite = new BinaryWriter(new FileStream(FilePath.Replace("txt", "out"), FileMode.Create));
    }



    public char ReadChar(){
      throw new NotImplementedException();
    }

    public void WriteLine(){
      throw new NotImplementedException();
    }
  }
}