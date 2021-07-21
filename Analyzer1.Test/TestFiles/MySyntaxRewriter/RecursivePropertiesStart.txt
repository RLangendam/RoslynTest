using System;

namespace ConsoleApplication1
{
    public class MySubDTO2
    {
        public string Name { get; set; }
    }

    public class MySubDTO
    {
        public MySubDTO2 Namedd { get; set; }
    }

    public class MyDTO
    {
        public MySubDTO Named { get; set; }
    }

    public class MySubModel2
    {
        public string Name { get; set; }
    }

    public class MySubModel
    {
        public MySubModel2 Namedd { get; set; }
    }

    public class MyModel
    {
        public MySubModel Named { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}