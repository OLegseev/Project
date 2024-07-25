using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using HtmlAgilityPack;
using static Project.MainWindow;

namespace Project
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GetCurrentValue getCurrentValue = new GetCurrentValue();
        public sealed class GetColor
        {
            private static GetColor instance = null;
            private static readonly object padlock = new object();

            private List<Brush> brushes = new List<Brush>();
            private List<int> useValue = new List<int>();

            private GetColor()
            {
                brushes.Add(Brushes.Black);
                brushes.Add(Brushes.White);
                brushes.Add(Brushes.Brown);
                brushes.Add(Brushes.Blue);
                brushes.Add(Brushes.LightBlue);
                brushes.Add(Brushes.Red);
                brushes.Add(Brushes.Yellow);
                brushes.Add(Brushes.Green);
                brushes.Add(Brushes.Gray);
                brushes.Add(Brushes.Pink);

                for (int i = 0; i < 10; i++)
                {
                    useValue.Add(i);
                }
            }

            public static GetColor Instance
            {
                get
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GetColor();
                        }
                        return instance;
                    }
                }
            }

            public Brush FullColor()
            {
                Random random = new Random();
                int randValue = random.Next(0, useValue.Count);
                Brush retColor = brushes[randValue];
                useValue.RemoveAt(randValue);
                brushes.RemoveAt(randValue);
                return retColor;
            }
            public Brush ChangeColor(Brush background)
            {
                Brush retColor = brushes[0];
                useValue.RemoveAt(0);
                brushes.RemoveAt(0);
                useValue.Add(0);
                brushes.Add(background);
                return retColor;
            }
        }
        public class GetCurrentValue
        {
            public string temperature = "";
           

            public GetCurrentValue()
            {
                GetTemperature();
           
            }
            public async void GetTemperature()
            {
                string url = @"https://world-weather.ru/pogoda/russia/moscow/24hours/";

                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync(url);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    // Пример извлечения данных: поиск всех заголовков h1
                    var headings = doc.DocumentNode.SelectNodes($"//*[@id='weather-now-number']");
                    foreach (var heading in headings)
                    {
                       temperature = heading.InnerText;
                    }
                }

            }
        }
    
        public  MainWindow()
        {
            
            InitializeComponent();
            var labels = GridLabels.Children;
           GetColor getColor = GetColor.Instance;
            foreach(var item in labels)
            {
                if (item is Label label)
                {
                    label.Background = getColor.FullColor();
                    label.Foreground = label.Background;
                }
            }

            DispatcherTimer timer;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            // Обновление каждую секунду
            timer.Tick += GetData;
            timer.Start();
        }
  

       
        public  void GetData(object sender, EventArgs e)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            timeTB.Text = currentTime;
            tempTB.Text = getCurrentValue.temperature;
        }
        private void Bdr1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GetColor getColor = GetColor.Instance;
            Label selectedBorder = (Label)sender;
            selectedBorder.Background = getColor.ChangeColor(selectedBorder.Background);
            selectedBorder.Foreground = selectedBorder.Background;
            inc.Text = Convert.ToString(Convert.ToInt32(inc.Text) + 1);
        }

   
    }
}
