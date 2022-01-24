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
    public class NewSn
    {
        [TestMethod]
        public void TestPrepareObject()
        {
            // одежда
            var q1 = @"
                {
                  ""contactPerson"": ""Ivanov"",
                  ""releaseMethodType"": ""IMPORT"",
                  ""createMethodType"": ""SELF_MADE"",
                  ""products"": [
                    {
                      ""templateId"": 10,
                      ""gtin"": ""04607189361432"",
                      ""quantity"": 11,
                      ""serialNumberType"": ""SELF_MADE"",
                      ""cisType"": ""UNIT"",
                      ""serialNumbers"": [ ""211174317552"", ""211138127642"", ""211116799152"", ""211178131828"", ""211194245309"", ""211191549465"", ""211131717167"", ""211111906894"", ""211116829838"", ""211107890245"", ""211172897019"" ]
                    }
                  ]
                }";
            // обувь
            var q2 = @"
                {
                  ""contactPerson"": ""Ivanov"",
                  ""releaseMethodType"": ""IMPORT"",
                  ""createMethodType"": ""SELF_MADE"",
                  ""products"": [
                    {
                      ""templateId"": 1,
                      ""gtin"": ""04607189360350"",
                      ""quantity"": 7,
                      ""serialNumberType"": ""SELF_MADE"",
                      ""serialNumbers"": [ ""2110062190336"", ""2110594990764"", ""2110646449222"", ""2110688209997"", ""2110447503042"", ""2110138639741"", ""2110186535990"" ]
                    }
                  ]
                }";
            // шины
            var q3 = @"{
                  ""contactPerson"": ""Ivanov"",
                  ""releaseMethodType"": ""IMPORT"",
                  ""createMethodType"": ""SELF_MADE"",
                  ""products"": [
                    {
                      ""templateId"": 7,
                      ""gtin"": ""04607189361241"",
                      ""quantity"": 6,
                      ""serialNumberType"": ""SELF_MADE"",
                      ""serialNumbers"": [ ""2110752557289"", ""2110046899636"", ""2110966817799"", ""2110938873453"", ""2110697924448"", ""2110746300520"" ]
                    }
                  ]
                }";

            try
            {
                var o1 = new InputOrder3[3];
                o1[0] = new InputOrder3();
                o1[0].tg = 10;
                o1[0].sn = new List<string>() {"211174317552", "211138127642", "211116799152", "211178131828", "211194245309",
                    "211191549465", "211131717167", "211111906894", "211116829838", "211107890245", "211172897019"};
                o1[0].ordered = 11;
                o1[0].article = "5823351-54";
                o1[0].gtin = "04607189361432";

                o1[1] = new InputOrder3();
                o1[1].tg = 1;
                o1[1].sn = new List<string>() {"2110062190336", "2110594990764", "2110646449222", "2110688209997", "2110447503042",
                    "2110138639741", "2110186535990"};
                o1[1].ordered = 7;
                o1[1].article = "5976594-42";
                o1[1].gtin = "04607189360350";

                o1[2] = new InputOrder3();
                o1[2].tg = 7;
                o1[2].sn = new List<string>() {"2110752557289", "2110046899636", "2110966817799", "2110938873453", "2110697924448", "2110746300520"};
                o1[2].ordered = 6;
                o1[2].article = "5321258-33";
                o1[2].gtin = "04607189361241";

                //var cnt = new NewSnController(TestConfiguration.GetConfiguration(), null);
                var cnt = new bs.Pages.New(TestConfiguration.GetConfiguration());
                var o2 = cnt.MakeOrders(o1).ToList();

                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.Converters.Add(new OrderProductSnJConverter());
                var s1 = JsonSerializer.Deserialize<OrderLpSn>(q1, options);
                var s2 = JsonSerializer.Deserialize<OrderShoesSn>(q2, options);
                var s3 = JsonSerializer.Deserialize<OrderTiresSn>(q3, options);

                Assert.IsNotNull(o2);
                Assert.AreEqual(o2.Count(), 3);
                Assert.IsNotNull(o2[0]);
                Assert.IsNotNull(o2[1]);
                Assert.IsNotNull(o2[2]);
                Assert.IsNotNull(s1);
                Assert.IsNotNull(s2);
                Assert.IsNotNull(s3);
                foreach (OrderSn o in o2)
                {
                    switch (o)
                    {
                        case OrderLpSn order:
                            Assert.AreEqual(s1, order);
                            break;
                        case OrderShoesSn order:
                            Assert.AreEqual(s2, order);
                            break;
                        case OrderTiresSn order:
                            Assert.AreEqual(s3, order);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}