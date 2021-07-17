using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<
    Analyzer1.Analyzer1Analyzer,
    Analyzer1.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class Analyzer1UnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var test =
@"using System;
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
}";

            var fixtest =
@"using System;
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
            // Found
            return new MyModel();
        }
        public static MyModel Poep(MyDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(22, 31, 22, 34).WithArguments("Map");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

    }
}
