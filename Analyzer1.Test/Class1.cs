using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    public class MyDTO
    {
        public string Name;
    }

    public class MyModel
    {
        public string Name;
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            throw new NotImplementedException();
        }

        public static MyModel Poep(MyDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}