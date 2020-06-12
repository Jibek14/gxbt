using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Cryptocurrency.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;
using NLog;
using System.Windows.Threading;

namespace Cryptocurrency
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CourseContext db;
        Courses check = new Courses();
        string[] names = { "BNBUSDT", "BTCUSDT", "ETHUSDT", "XRPUSDT", "BCHUSDT", "LTCUSDT" };
        double[] percents = new double[6];
        double[] lastPrice = new double[6];
        string info;
        Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();
            logger.Info("Запуск программы");;
            db = new CourseContext();
            //устанавливаем привязку к кэшу
            Closing += MainWindow_Closing;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
            Thread thread = new Thread(Count);
            thread.Start();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
       void Count()
        {
            for (int i = 0; i < names.Length; i++)
            {
                info = check.GetAllData(names[i]);
                percents[i] = check.GetChangePricePercent(info);
                lastPrice[i] = check.GetLastPrice(info);
            };
        } 
      public void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Count();
            bnbPercent.Text =percents[0].ToString();
            btcPercent.Text = percents[1].ToString();
            ethPercent.Text = percents[2].ToString();
            xrpPercent.Text = percents[3].ToString();
            bchPercent.Text = percents[4].ToString();
            ltcPercent.Text = percents[5].ToString();
            bnbLastPriceValue.Text = lastPrice[0].ToString();
            btcLastPriceValue.Text = lastPrice[1].ToString();
            ethLastPriceValue.Text = lastPrice[2].ToString();
            xrpLastPriceValue.Text = lastPrice[3].ToString();
            bchLastPriceValue.Text = lastPrice[4].ToString();
            ltcLastPriceValue.Text = lastPrice[5].ToString();
            // write to db
            check.WriteToDB(names, lastPrice, percents);
            db.Course.Load();//загружаем данные
            CourseGrid.ItemsSource = db.Course.Local.ToBindingList().OrderByDescending(x => x.Id);
           if (percents[0] < 0)
                bnbPercent.Background = Brushes.Red;
            else
                bnbPercent.Background = Brushes.Green;
            if (percents[1] < 0)
                btcPercent.Background = Brushes.Red;
            else
                btcPercent.Background = Brushes.Green;
            if (percents[2] < 0)
                ethPercent.Background = Brushes.Red;
            else
                ethPercent.Background = Brushes.Green;
            if (percents[3] < 0)
                xrpPercent.Background = Brushes.Red;
            else
                xrpPercent.Background = Brushes.Green;
            if (percents[4] < 0)
                bchPercent.Background = Brushes.Red;
            else
                bchPercent.Background = Brushes.Green;
            if (percents[5]< 0)
                ltcPercent.Background = Brushes.Red;
            else
                ltcPercent.Background = Brushes.Green;
        }
        //private void UpdateButton_Click(object sender, RoutedEventArgs e)
        //{
        //    db.SaveChanges();
        //}
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CourseGrid.SelectedItems.Count > 0)
                {
                    for (int i = 0; i < CourseGrid.SelectedItems.Count; i++)
                    {
                        Course cash = CourseGrid.SelectedItems[i] as Course;
                        if (cash != null)
                        {
                            db.Course.Remove(cash);
                            logger.Info("удалены данные под номером(id): " + cash.Id);
                        }
                    }
                }
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message,"ошибка при удалении данных");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                string dataSheet = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var excelApp = new Excel.Application
                {
                    Visible = true
                };
                Excel.Workbooks Data = excelApp.Workbooks;
                Excel.Workbook mySheet = excelApp.Workbooks.Open(System.IO.Path.Combine(dataSheet, "CryptoData.xlsx"));
                Excel.Workbook sheet = Data.Open(dataSheet);
               logger.Info("открыт excel файл");
            }
            catch(Exception ex)
            { 
                logger.Error(ex.Message, "файл excel открылся не корректно");
                result.Text = "Данные получены";
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
           Close();
        }
    }
}
