using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<Analyzer1.Analyzer1Analyzer, Analyzer1.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public async Task TestMethod3()
        {
            var test = @"
    public class MyDTO
{
    public string Name;
}

public class MyModel
{
    public string Name;
}

public class MyMapper 
{
    public MyDTO Map(MyModel myModel)
    {
        // magic happens...
    }
}";

            var fixtest = @"
    public class MyDTO
{
    public string Name;
}

public class MyModel
{
    public string Name;
}

public class MyMapper 
{
    public MyDTO Map(MyModel myModel)
    {
        return new MyDTO{Name = myModel.Name};
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

    }
}
