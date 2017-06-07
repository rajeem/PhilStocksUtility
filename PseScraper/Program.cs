using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace PseScraper
{
    class StockData
    {
        public string s { get; set; }
        public int[] t { get; set; }
        public double[] h { get; set; }
        public double[] l { get; set; }
        public double[] o { get; set; }
        public double[] c { get; set; }
        public double[] v { get; set; }
    }

    class StockYearMonthData
    {
        public double Total { get; set; }
        public int N { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public double YearAve { get; set; }
    }

    class Program
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-fa")
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(@"application/json"));
                    client.BaseAddress = new Uri("http://pse.tools");
                    var content = client
                        .GetAsync("api/chart/history?symbol=ICT&resolution=D&from=1175905396&to=1490187599")
                        .Result.Content;
                    var stringResult = content.ReadAsStringAsync().Result;
                    var stockData = JsonConvert.DeserializeObject<StockData>(stringResult);
                    var list = new List<StockYearMonthData>();
                    for (int i = 0; i < stockData.t.Length; i++)
                    {
                        var dt = UnixTimeStampToDateTime(stockData.t[i]);
                        var sd = list.SingleOrDefault(x => x.Month == dt.Month && x.Year == dt.Year);
                        if (sd == null)
                        {
                            sd = new StockYearMonthData { Month = dt.Month, Year = dt.Year };
                            list.Add(sd);
                        }
                        sd.N++;
                        sd.Total += stockData.c[i];
                    }
                    var grp = list.GroupBy(x => x.Year);
                    foreach (var g in grp)
                    {
                        double total = 0;
                        int n = 0;
                        foreach (var item in g)
                        {
                            total += item.Total;
                            n += item.N;
                        }
                        var yearAve = total / n;
                        foreach (var item in g)
                        {
                            item.YearAve = yearAve;
                        }
                    }
                    var s = "";
                    foreach (var item in list)
                    {
                        s += item.Year.ToString() + "," + item.Month + "," + item.N.ToString() + "," + item.Total.ToString() + "," + item.YearAve.ToString() + Environment.NewLine;
                    }

                    File.WriteAllText(@"c:\test.csv", s);
                }
            }
            else
            {
                var list = new string[] { "ABS", "AR", "CNPF", "CNPF", "CROWN", "DIZ", "DMPL", "ELI", "FOOD", "GERI", "HOUSE", "LC", "LR", "MA", "MAXS", "MFC", "PAX", "SFI", "SMC", "UNI", "NIKL", "UBP", "EDC", "RCB", "PNB", "MWC", "DMC", "AT", "SECB", "PIP", "MPI", "SCC", "AEV", "RRHI", "JFC", "SMPH", "PX", "2GO", "AGI", "ANI", "AUB", "BDO", "DNL", "EEI", "EMP", "FLI", "FPH", "HLCM", "ICT", "JGS", "LPZ", "MAB", "MEG", "MER", "MWIDE", "NI", "PCOR", "PSB", "PSE", "RFM", "ROX", "SLF", "SM", "TEL", "VLL", "AAA", "AB", "ABA", "ABG", "ABSP", "AC", "ACE", "ACPA", "ACPB2", "ACR", "AGF", "ALCO", "ALCPB", "ALHI", "ALI", "ANS", "AP", "APC", "APL", "APO", "APX", "ARA", "ATI", "ATN", "ATNB", "BC", "BCB", "BCOR", "BCP", "BEL", "BH", "BHI", "BKR", "BLFI", "BLOOM", "BMM", "BPI", "BRN", "BSC", "CA", "CAB", "CAL", "CAT", "CDC", "CEB", "CEI", "CEU", "CHI", "CHIB", "CHP", "CIC", "CIP", "COAL", "COL", "COSCO", "CPG", "CPM", "CPV", "CPVB", "CSB", "CYBR", "DAVIN", "DD", "DDPR", "DFNN", "DNA", "DWC", "ECP", "EG", "EURO", "EVER", "EW", "FAF", "FDC", "FEU", "FFI", "FGEN", "FGENF", "FGENG", "FJP", "FJPB", "FMETF", "FPHP", "FPI", "GEO", "GLO", "GLOPA", "GLOPP", "GMA7", "GMAP", "GPH", "GREEN", "GSMI", "GTCAP", "GTPPA", "GTPPB", "H2O", "HI", "HVN", "I", "IDC", "IMI", "IMI", "IMP", "ION", "IPM", "IPO", "IRC", "IS", "ISM", "JAS", "JOH", "KEP", "KPH", "KPHB", "LAND", "LBC", "LCB", "LFM", "LIB", "LIHC", "LMG", "LOTO", "LRP", "LRW", "LSC", "LTG", "MAC", "MACAY", "MARC", "MB", "MBC", "MBT", "MCP", "MED", "MFIN", "MG", "MHC", "MJC", "MJIC", "MRC", "MRSGI", "MVC", "MWP", "NOW", "NRCP", "OM", "OPM", "OPMB", "ORE", "OV", "PA", "PAL", "PBB", "PBC", "PERC", "PF", "PFP", "PFP2", "PGOLD", "PHA", "PHEN", "PHES", "PHN", "PIZZA", "PLC", "PMPC", "PNX", "PNX3A", "PNX3B", "POPI", "PORT", "PPC", "PRC", "PRF2A", "PRF2B", "PRIM", "PRMX", "PSPC", "PTC", "PXP", "RCI", "REG", "RLC", "RLT", "ROCK", "RWM", "SBS", "SEVN", "SFIP", "SGI", "SGP", "SHLPH", "SHNG", "SLI", "SMC2A", "SMC2B", "SMC2C", "SMC2D", "SMC2E", "SMC2F", "SMC2G", "SMC2H", "SMC2I", "SOC", "SPC", "SPM", "SRDC", "SSI", "STI", "STR", "SUN", "T", "TAPET", "TBGI", "TECH", "TFC", "TFHI", "TUGS", "UPM", "URC", "V", "VITA", "VMC", "VUL", "VVT", "WEB", "WIN", "WPI", "X", "ZHI" };

                var s = "";
                foreach (var stock in list)
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://www.bloomberg.com");
                    var htmlStr = client.GetAsync("quote/" + stock + ":PM").Result.Content.ReadAsStringAsync().Result;
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlStr);
                    try
                    {
                        var div = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id='content']/div/div/div[8]/div/div/div[13]/div[2]").InnerText.Trim();
                        var pe = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id='content']/div/div/div[8]/div/div/div[8]/div[2]").InnerText.Trim();
                        s += stock + "," + div + "," + pe + "\n";
                        Console.WriteLine(stock);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                File.WriteAllText(@"c:\test.csv", s);
            }
        }
    }
}
