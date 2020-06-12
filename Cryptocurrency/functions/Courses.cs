using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Cryptocurrency.Models;
using System.Threading;
using System.Windows;
using NLog;
using NLog.Fluent;
using Newtonsoft.Json;

namespace Cryptocurrency
{
    public class Courses
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public string  GetAllData(string name)
        {
            try
            {
                string dataResponse;
                string dataFullInfo = "https://api.binance.com/api/v3/ticker/24hr?symbol=";//BNBUSDT
                HttpWebRequest bnbWebRequest = (HttpWebRequest)WebRequest.Create(dataFullInfo + name);
                HttpWebResponse bnbWebResponse = (HttpWebResponse)bnbWebRequest.GetResponse();
              using (StreamReader streamReader = new StreamReader(bnbWebResponse.GetResponseStream()))
                {
                    dataResponse = streamReader.ReadToEnd();
                }
                string text = dataResponse;
                Thread.Sleep(400);
                logger.Info("данные успешно получены с сервера");
                return text;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message, "не был подключен к сети интернет");
                MessageBox.Show("подключитесь к интернету!");
                Environment.Exit(0);
                return null;
            }
        }
        public class Rootobject
        {
            public string Symbol { get; set; }
            public string PriceChange { get; set; }
            public double PriceChangePercent { get; set; }
            public string WeightedAvgPrice { get; set; }
            public string PrevClosePrice { get; set; }
            public double LastPrice { get; set; }
            public string LastQty { get; set; }
            public string BidPrice { get; set; }
            public string BidQty { get; set; }
            public string AskPrice { get; set; }
            public string AskQty { get; set; }
            public string OpenPrice { get; set; }
            public string HighPrice { get; set; }
            public string LowPrice { get; set; }
            public string Volume { get; set; }
            public string QuoteVolume { get; set; }
            public long OpenTime { get; set; }
            public long CloseTime { get; set; }
            public int FirstId { get; set; }
            public int LastId { get; set; }
            public int Count { get; set; }
        }

        public double GetChangePricePercent(string data)
        {
            try
            {
                Rootobject mainObj = JsonConvert.DeserializeObject<Rootobject>(data);
                double result = mainObj.PriceChangePercent;
                return result;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                return 0;
            }
        }
        public double GetLastPrice(string data)
        {
            try
            {
                Rootobject mainObj = JsonConvert.DeserializeObject<Rootobject>(data);
                double result = mainObj.LastPrice;
                return result;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                return 0;
            }
        }
        public void WriteToDB(string[] names, double[] lastPrice, double[] changePercent)
        {
            var list = new List<Course>();
            int index = 0;
            try
            {
                foreach (var n in names)
                {
                    var item = new Course();
                    item.Title = n;
                    item.requestTime = DateTime.Now;
                    item.LastPrice = lastPrice[index];
                    item.ChangePercent =Convert.ToDouble(changePercent[index]);
                    list.Add(item);
                    index++;
                }
                CourseContext context = new CourseContext();
                context.Course.AddRange(list);
                context.SaveChanges();
                logger.Info("данные сохранены в базе");
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message, "ошибка при записи данных в базу");
            }
        }
    }
}