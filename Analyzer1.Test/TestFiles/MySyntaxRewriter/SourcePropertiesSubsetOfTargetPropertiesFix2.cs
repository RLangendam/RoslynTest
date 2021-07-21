public static MyModel Map(MyDTO dto)
{
    return new MyModel
    {
        // Number = ...,
        Name = dto.Name,
    };
}