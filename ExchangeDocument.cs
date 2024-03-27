using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PC
{
    public class ExchangeDocument
    {
        public int? r030 { get; set; }
        public string? txt { get; set; } = null!;
        public double? rate { get; set; }
        public string? cc { get; set; } = null!;
        public DateTime? exchangedate { get; set; }

        public override string ToString()
        {
            return $"{r030} {txt} {rate} {cc} {exchangedate?.ToShortDateString()}\n\n";
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange");
                List<ExchangeDocument> exchanges = [];
                foreach (XmlNode i in xml!.ChildNodes[1])
                {
                    if (Convert.ToDouble(i.ChildNodes[2]!.FirstChild!.Value.Replace('.', ',')) > 20)
                    {
                        ExchangeDocument doc = new ExchangeDocument();
                        doc.r030 = Convert.ToInt32(i.ChildNodes[0]!.FirstChild!.Value);
                        doc.txt = i.ChildNodes[1]!.FirstChild?.Value;
                        doc.rate = Convert.ToDouble
                            (i.ChildNodes[2]!
                            .FirstChild?.Value
                            .Replace('.', ','));
                        doc.cc = i.ChildNodes[3]!.FirstChild?.Value;
                        doc.exchangedate = Convert.ToDateTime(i.ChildNodes[4]!.FirstChild?.Value);
                        exchanges.Add(doc);
                        //Console.WriteLine($"{i.ChildNodes[0]!.FirstChild!.Value} - {i.ChildNodes[2]!.FirstChild!.Value}");
                    }
                }
                exchanges.ForEach(x => Console.Write(x.ToString()));
               
                using (XmlTextWriter write = new XmlTextWriter("NewExchange.xml", Encoding.Unicode))
                {
                    write.WriteStartDocument();
                    write.WriteStartElement("exchange");
                    foreach (ExchangeDocument doc in exchanges)
                    {
                        write.WriteStartElement("currency");
                        write.WriteElementString("r030", Convert.ToString(doc.r030));
                        write.WriteElementString("txt", doc.txt);
                        write.WriteElementString("rate", Convert.ToString(doc.rate));
                        write.WriteElementString("cc", doc.cc);
                        write.WriteElementString("exchangedate",
                                                 Convert.ToString(doc.exchangedate.Value
                                                 .ToShortDateString()));
                        write.WriteEndElement();
                    }
                    write.WriteEndElement();
                    write.Close();
                    Console.WriteLine("File save!");
                }

            }
            catch (Exception? error) { Console.WriteLine(error.Message); }
        }
    }
}
