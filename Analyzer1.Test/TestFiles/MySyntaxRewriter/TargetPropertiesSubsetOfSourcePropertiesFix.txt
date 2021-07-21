public static MyModel Map(MyDTO dto)
{
    return new MyModel { Name = dto.Name };
}