using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
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
        public async Task NoDiagnosticsWithoutCode()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MappingFields()
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
        public string Name { get; set; }
    }

    public class MyModel
    {
        public string Name { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            throw new NotImplementedException();
        }

        public static MyModel Poep(MyDTO left, MyDTO right)
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
        public string Name { get; set; }
    }

    public class MyModel
    {
        public string Name { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            return new MyModel { Name = dto.Name };
        }
        public static MyModel Poep(MyDTO left, MyDTO right)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(22, 31, 22, 34).WithArguments("Map");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task MappingRecursive()
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
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(42, 31, 42, 34).WithArguments("Map");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task TargetPropertiesSubsetOfSourceProperties()
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
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class MyModel
    {
        public string Name { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
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
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class MyModel
    {
        public string Name { get; set; }
    }

    public static class Mapper
    {
        public static MyModel Map(MyDTO dto)
        {
            return new MyModel { Name = dto.Name };
        }
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(23, 31, 23, 34).WithArguments("Map");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task SourcePropertiesSubsetOfTargetProperties()
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
            return new MyModel
            {
                Name = dto.Name,
                // Number = ...,
            };
        }
    }
}";

            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(23, 31, 23, 34).WithArguments("Map");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

//        [TestMethod]
//        public async Task SourcePropertiesSubsetOfTargetProperties2()
//        {
//            var test =
//@"using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace ConsoleApplication1
//{
//    public class MyDTO
//    {
//        public string Name { get; set; }
//    }

//    public class MyModel
//    {
//        public int Number { get; set; }
//        public string Name { get; set; }
//    }

//    public static class Mapper
//    {
//        public static MyModel Map(MyDTO dto)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}";

//            var fixtest =
//@"using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace ConsoleApplication1
//{
//    public class MyDTO
//    {
//        public string Name { get; set; }
//    }

//    public class MyModel
//    {
//        public int Number { get; set; }
//        public string Name { get; set; }
//    }

//    public static class Mapper
//    {
//        public static MyModel Map(MyDTO dto)
//        {
//            return new MyModel
//            {
//                // Number = ...,
//                Name = dto.Name,
//            };
//        }
//    }
//}";

//            var expected = VerifyCS.Diagnostic("Analyzer1").WithSpan(23, 31, 23, 34).WithArguments("Map");
//            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
//        }

        [TestMethod]
        public void TestCommentString()
        {
            var value = string.Join($",{Environment.NewLine}", new[] { "y = z", "// x = ..." });

            Console.WriteLine(value);
        }
    }
}
