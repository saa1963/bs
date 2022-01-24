using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Resources;
using bs;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using bs.Data;

namespace TestProject
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        public void TestPrepareObject()
        {
            // одежда
            var q1 = @"
                {
                  ""trade_participant_inn"": ""5047004970"",
                  ""declaration_number"": ""10013160/091121/0699779"",
                  ""declaration_date"": ""2021-11-09"",
                  ""products_list"": [
                    {""cis"": ""0104607189361432215211107890245""},
                    {""cis"": ""0104607189361432215211111906894""},
                    {""cis"": ""0104607189361432215211116799152""},
                    {""cis"": ""0104607189361432215211116829838""},
                    {""cis"": ""0104607189361432215211131717167""},
                    {""cis"": ""0104607189361432215211138127642""},
                    {""cis"": ""0104607189361432215211172897019""},
                    {""cis"": ""0104607189361432215211174317552""},
                    {""cis"": ""0104607189361432215211178131828""},
                    {""cis"": ""0104607189361432215211191549465""},
                    {""cis"": ""0104607189361432215211194245309""}
                  ]
                }";
            // обувь
            var q2 = @"
                {
                  ""trade_participant_inn"": ""5047004970"",
                  ""declaration_number"": ""10013160/091121/0699779"",
                  ""declaration_date"": ""2021-11-09"",
                  ""products_list"": [
                    {""cis"": ""0104607189360350212110062190336""},
                    {""cis"": ""0104607189360350212110138639741""},
                    {""cis"": ""0104607189360350212110186535990""},
                    {""cis"": ""0104607189360350212110447503042""},
                    {""cis"": ""0104607189360350212110594990764""},
                    {""cis"": ""0104607189360350212110646449222""},
                    {""cis"": ""0104607189360350212110688209997""}
                  ]
                }";
            // шины
            var q3 = @"
                {
                  ""trade_participant_inn"": ""5047004970"",
                  ""declaration_number"": ""10009100/021121/0172303"",
                  ""declaration_date"": ""2021-11-02"",
                  ""products_list"": [
                    {""cis"": ""0104607189361241212110046899636""},
                    {""cis"": ""0104607189361241212110697924448""},
                    {""cis"": ""0104607189361241212110746300520""},
                    {""cis"": ""0104607189361241212110752557289""},
                    {""cis"": ""0104607189361241212110938873453""},
                    {""cis"": ""0104607189361241212110966817799""}
                  ]
                }";

            try
            {
                DateTime dt1 = new DateTime(2021, 11, 9), dt2 = new DateTime(2021, 11, 2);
                string gtd1 = "10013160/091121/0699779", gtd2 = "10009100/021121/0172303";
                string tg1 = "ОД", tg2 = "ОБ", tg3 = "ШИ";
                var lp = new List<Product>();
                lp.Add(new Product { cis = "0104607189361432215211107890245", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211111906894", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211116799152", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211116829838", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211131717167", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211138127642", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211172897019", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211174317552", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211178131828", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211191549465", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });
                lp.Add(new Product { cis = "0104607189361432215211194245309", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg1 });

                lp.Add(new Product { cis = "0104607189360350212110062190336", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110138639741", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110186535990", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110447503042", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110594990764", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110646449222", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });
                lp.Add(new Product { cis = "0104607189360350212110688209997", packType = "UNIT", GTDDate = dt1, GTDNum = gtd1, Tg = tg2 });

                lp.Add(new Product { cis = "0104607189361241212110046899636", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });
                lp.Add(new Product { cis = "0104607189361241212110697924448", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });
                lp.Add(new Product { cis = "0104607189361241212110746300520", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });
                lp.Add(new Product { cis = "0104607189361241212110752557289", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });
                lp.Add(new Product { cis = "0104607189361241212110938873453", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });
                lp.Add(new Product { cis = "0104607189361241212110966817799", packType = "UNIT", GTDDate = dt2, GTDNum = gtd2, Tg = tg3 });

                //var cnt = new PutController(TestConfiguration.GetConfiguration(), null, "c:\\");
                var cnt = bs.Pages
                var o2 = cnt.GetLpf(lp);

                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                var s1 = JsonSerializer.Deserialize<LP_FTS_INTRODUCE>(q1, options);
                var s2 = JsonSerializer.Deserialize<LP_FTS_INTRODUCE>(q2, options);
                var s3 = JsonSerializer.Deserialize<LP_FTS_INTRODUCE>(q3, options);

                Assert.IsNotNull(o2);
                Assert.AreEqual(o2.Count(), 3);
                Assert.IsNotNull(o2[0]);
                Assert.IsNotNull(o2[1]);
                Assert.IsNotNull(o2[2]);
                Assert.IsInstanceOfType(s1, typeof(LP_FTS_INTRODUCE));
                Assert.IsInstanceOfType(s2, typeof(LP_FTS_INTRODUCE));
                Assert.IsInstanceOfType(s3, typeof(LP_FTS_INTRODUCE));
                foreach (LP_FTS_INTRODUCE o in o2)
                {
                    Assert.IsInstanceOfType(o, typeof(LP_FTS_INTRODUCE));
                    if (o.products_list.FirstOrDefault(s => s.cis == "0104607189361432215211107890245") != null)
                    {
                        Assert.AreEqual(o, s1);
                    } else if (o.products_list.FirstOrDefault(s => s.cis == "0104607189360350212110062190336") != null)
                    {
                        Assert.AreEqual(o, s2);
                    }
                    else if (o.products_list.FirstOrDefault(s => s.cis == "0104607189361241212110046899636") != null)
                    {
                        Assert.AreEqual(o, s3);
                    }
                    else
                        Assert.Fail("Сработал <else>");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}