using System;

namespace ConsoleApplication1
{
    public class MyDTO
    {
        public string Name { get; set; }
    }

    public class MyModel
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
