using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Analyzer1.Test
{
    [TestClass]
    public class MySyntaxRewriterTest
    {
        private const string testFilesPath = @".\TestFiles\MySyntaxRewriter\";
        
        [TestMethod]
        public void MappingBasicProperties()
        {
            var start = GetFileAsString("BasicPropertiesStart.txt");
            var fix = GetFileAsString("BasicPropertiesFix.txt");


        }

        private string GetFileAsString(string filename)
        {
            return File.ReadAllText(testFilesPath + filename);
        }
    }
}
