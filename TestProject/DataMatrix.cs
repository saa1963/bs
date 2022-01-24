using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Resources;
using bs;
using System.IO;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class DataMatrix
    {
        [TestMethod]
        public void TestGetBmp()
        {
            try
            {
                string km = @"0104607189360114212110311187827\u0029910098\u002992zNjjkWX0vrN5yfvrEMoQg+HEyyhmrLk/R5/f5407SlmU6jd+6gu+OcZaSzUE+KrHjT/Ey2qtalLdRQYhCxT8DA==";
                var res = Const.GetImage(km);
                ResourceManager rm = new ResourceManager(typeof(DataMatrix));

                System.Reflection.Assembly assem =
                    System.Reflection.Assembly.GetExecutingAssembly();
                using (System.IO.Stream fs =
                             assem.GetManifestResourceStream("TestProject.binary.testbmp.bmp"))
                {
                    MemoryStream ms = new MemoryStream();
                    fs.CopyTo(ms);
                    var res1 =  ms.GetBuffer();
                    ms.Dispose();
                    Assert.IsTrue(res.SequenceEqual(res1));
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
