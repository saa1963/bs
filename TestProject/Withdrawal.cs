using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Resources;
using bs;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestProject
{
    [TestClass]
    public class Withdrawal
    {
        [TestMethod]
        public void TestPrepareObject()
        {
            // одежда
            var q1 = @"
                {
                  ""inn"": ""5047004970"",
                  ""action_date"": ""%current date%"",
                  ""action"": ""ENTERPRISE_USE"",
                  ""document_type"": ""OTHER"",
                  ""document_number"": ""002060592"",
                  ""document_date"": ""2021-11-25T00:00:00.000Z"",
                  ""primary_document_custom_name"": ""Акт"",
                  ""products"": [
                    {""cis"": ""0104607189361432215211107890245""}
                  ]
                }".Replace("%current date%", DateTime.Now.ToString("yyyy-MM-dd"));


            try
            {
                var lp = new List<WithDrawalInput>();
                lp.Add(new WithDrawalInput { km = "0104607189361432215211107890245", article = "5822288-02", tg = "ОД", ddoc = new DateTime(2021, 11, 25), ndoc = "002060592" });

                var cnt = new WithdrawalController(TestConfiguration.GetConfiguration(), null, "c:\\");
                var o2 = cnt.GetObs(lp);

                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                var s1 = JsonSerializer.Deserialize<LK_RECEIPT>(q1, options);

                Assert.IsNotNull(o2);
                Assert.AreEqual(o2.Count(), 1);
                Assert.IsNotNull(o2[0]);
                Assert.IsInstanceOfType(s1, typeof(LK_RECEIPT));
                var (t1, t2, t3) = o2[0];
                Assert.IsNotNull(t1);
                Assert.AreEqual(t1, "ОД");
                Assert.IsNotNull(t2);
                Assert.AreEqual(t2.Count(), 1);
                Assert.IsTrue(lp.OrderBy(s => s.km).SequenceEqual(t2.OrderBy(s => s.km)));
                Assert.AreEqual(s1, t3);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}