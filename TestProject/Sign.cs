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
                byte[] buffer = Encoding.UTF8.GetBytes("� ������� ��� �������� ������, ������� ����������� ���!");
                byte[] signedMessage = HSign.Sign(buffer, "��� \"���������\"", "���������");
                Assert.IsTrue(signedMessage.Length > 0);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
