public static MyModel Map(MyDTO dto)
{
    return new MyModel
    {
        Named = new ConsoleApplication1.MySubModel
        {
            Namedd = new ConsoleApplication1.MySubModel2
            {
                Name = dto.Named.Namedd.Name
            }
        }
    };
}