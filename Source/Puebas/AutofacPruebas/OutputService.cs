using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacPruebas
{
    public interface IOutput
    {
        void Write(string content);
    }
     
    public class OutputService : IOutput
    {
        public void Write(string content)
        {
            Console.WriteLine(content);
        }
    }
}
