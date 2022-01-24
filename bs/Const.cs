using DataMatrixCore.net;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO.Compression;

namespace bs
{
    internal class Const
    {
        public static Dictionary<string, string> dictTg = new Dictionary<string, string> { { "ШИ", "tires" }, { "ОБ", "shoes" }, { "ОД", "lp" } };
        public static string CreatePdf1(string directory, List<PdfInput> lists, string contentRootPath)
        {
            PdfFont russian = PdfFontFactory.CreateFont(
                Path.Combine(contentRootPath, "micross.ttf"), "CP1251", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            string fname = Path.Combine(directory, "__temp.pdf");
            PdfWriter writer = new PdfWriter(fname);
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(new iText.Kernel.Geom.PageSize(Fmm(100f), Fmm(75f)));

            Document doc = new Document(pdf);
            doc.SetMargins(0, 0, 0, 0);
            doc.SetFont(russian);

            foreach (var o in lists)
            {
                Table t1 = new Table(new float[] { Fmm(40f), Fmm(58f) });
                //Table t1 = new Table(2);
                t1.UseAllAvailableWidth();
                t1.SetMargins(0, 0, 0, 0);
                t1.SetBorder(Border.NO_BORDER);

                Cell c1 = new Cell();
                c1.Add(new Paragraph("Артикул:"));
                c1.Add(new Paragraph(o.article));
                c1.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                c1.SetHeight(Fmm(73f));
                c1.SetMargins(0, 0, 0, 0);
                c1.SetBorder(Border.NO_BORDER);
                c1.SetPaddingLeft(Fmm(10f));
                t1.AddCell(c1);

                Cell c2 = new Cell();
                byte[] im = GetImage(o.km);
                var imData = iText.IO.Image.ImageDataFactory.CreateBmp(im, false);
                c2.Add(new iText.Layout.Element.Image(imData));
                c2.Add(new Paragraph(String.Concat(o.km.TakeWhile(s => s != '\u001D'))));
                c2.SetMargins(0, 0, 0, 0);
                c2.SetPaddingRight(Fmm(5));
                c2.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                c2.SetBorder(Border.NO_BORDER);
                c2.SetFontSize(9);
                t1.AddCell(c2);

                doc.Add(t1);
            }
            doc.Close();
            return fname;
        }
        private static float Fmm(float v)
        {
            return v * 72f / 25.4f;
        }
        internal static byte[] GetImage(string km)
        {
            DmtxImageEncoder encoder = new DmtxImageEncoder();
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions();
            options.ModuleSize = 4;
            options.MarginSize = 0;
            options.BackColor = System.Drawing.Color.White;
            options.ForeColor = System.Drawing.Color.Black;
            options.Scheme = DmtxScheme.DmtxSchemeAsciiGS1;
            var bm = encoder.EncodeImage(km, options);
            using (var st = new MemoryStream())
            {
                bm.Save(st, ImageFormat.Bmp);
                return st.ToArray();
            }
        }
        public static List<string> _SavePdfFromFile(string[] lst, string path, string connectionString, ILogger logger, 
            string contentRootPath,  out List<string> fnames)
        {
            fnames = new List<string>();
            var rt = new List<string>();
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                var lst1 = new List<PdfInput>();
                var cmd = new SqlCommand("sp_Mark_RUC_Query", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "select_get_pdf3");
                cmd.Parameters.AddWithValue("@km", "");
                foreach (var s in lst)
                {
                    cmd.Parameters["@km"].Value = s;
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            lst1.Add(new PdfInput { ordernum = dr.GetString(0), article = dr.GetString(1), km = dr.GetString(2) });
                        }
                    }
                }
                if (lst1.Count == 0)
                {
                    throw new SaException("Запрос не возвратил записей.");
                }
                foreach (var o in lst1.GroupBy(s => new { s.ordernum, s.article }))
                {
                    var fname = Const.CreatePdf1(path, o.ToList(), contentRootPath);
                    var fname1 = $"{o.Key.ordernum}_{o.Key.article}.pdf";
                    var fname2 = Path.Combine(path, fname1);
                    if (System.IO.File.Exists(fname2))
                        System.IO.File.Delete(fname2);
                    System.IO.File.Move(fname, Path.Combine(path, fname1));
                    var msg = $"Файл {Path.Combine(path, fname1)} сохранен.";
                    fnames.Add(Path.Combine(path, fname1));
                    logger.LogInformation(msg);
                    rt.Add(msg);
                    foreach (var km in o)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@type", "save_last_print");
                        cmd.Parameters.AddWithValue("@km", km.km);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return rt;
        }
        public static byte[] ZipFiles(List<string> fnames)
        {
            MemoryStream ms = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var fname in fnames)
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(Path.GetFileName(fname));
                    using (var entry = readmeEntry.Open())
                    {
                        entry.Write(System.IO.File.ReadAllBytes(fname));
                    }
                }
            }
            return ms.ToArray();
        }
    }
}
