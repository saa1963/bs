using Microsoft.VisualStudio.TestTools.UnitTesting;
using HSignManaged;
using System.Text;
using System;

namespace TestProject
{
    [TestClass]
    public class Sign
    {
        [TestMethod]
        public void TestSign()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes("О сколько нам открытий чудных, готовит просвещенья дух!");
                byte[] signedMessage = HSign.Sign(buffer, "1a30ea2c9853f1195005a88f0aa32ce62c7de9c5");
                Assert.IsTrue(signedMessage.Length > 0);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
