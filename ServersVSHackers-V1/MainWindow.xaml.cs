using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServersVSHackers_V1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly System.Windows.Threading.DispatcherTimer _threadOne = new System.Windows.Threading.DispatcherTimer();
        readonly System.Windows.Threading.DispatcherTimer _threadTwo = new System.Windows.Threading.DispatcherTimer();
        readonly System.Windows.Threading.DispatcherTimer _threadThree = new System.Windows.Threading.DispatcherTimer();
        private int threadCounter = 0;
        public SimulationEngine engine;
        public MainWindow()
        {
            InitializeComponent();                        
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"ocean.jpg", UriKind.Relative));
            WorldCanvas.Background = ib;
        
        }

        public void Log(string log)
        {                      
            LogTextBox.Document.Blocks.Add(new Paragraph(new Run(log)));
            LogTextBox.ScrollToEnd();
        }

        private void EntitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Properties.Settings.Default.balanceValue = e.NewValue;
            //Properties.Settings.Default.Save();
        }

        private void MySlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Properties.Settings.Default.balanceValue = EntitySlider.Value;
            Properties.Settings.Default.Save();
            Console.WriteLine(@"Saved balanceValue = {0}", Properties.Settings.Default.balanceValue);
        }

        private void LevelDesignerButton_Click(object sender, RoutedEventArgs e)
        {
           new LevelDesigner(this).Show();
        }

        public void UpdateWorld()
        {
            WorldCanvas.Children.Clear();
            foreach (var country in Environment.World)
            {
                country.Effect =
                    new DropShadowEffect
                    {
                        Color = new Color {A = 255, R = 255, G = 255, B = 255},
                        Direction = 320,
                        ShadowDepth = 8,
                        Opacity = 0.7
                    };
                var xMin = country.Points.Min(p => p.X);
                var yMin = country.Points.Min(p => p.Y);
                var countrylabel = new TextBlock();
                countrylabel.Text = country.Name;
                countrylabel.Text = Name;
                countrylabel.FontSize = 28;
                countrylabel.TextDecorations = TextDecorations.Underline;
                Panel.SetZIndex(countrylabel, 10);
                Canvas.SetLeft(countrylabel, xMin+50.0);
                Canvas.SetTop(countrylabel, yMin+50.0);
                WorldCanvas.Children.Add(countrylabel);
                WorldCanvas.Children.Add(country);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (engine == null)
            {
                engine = new SimulationEngine(this);
            }
            engine.GenerateEntities();
        }

        private void AttackButton_Click(object sender, RoutedEventArgs e)
        {            
            _threadOne.Tick += ThreadOneTick;
            _threadOne.Interval = new TimeSpan(0, 0, 0, 0, 300);
            _threadOne.Start();
            threadCounter++;
            AttackButton.IsEnabled = false;
        }


        private void ThreadOneTick(object sender, EventArgs e)
        {
             engine.PerformAttack(Brushes.Orange);
        }

        private void ThreadTwoTick(object sender, EventArgs e)
        {
            threadCounter++;
            Task.Factory.StartNew(AttackTwo);            
        }

        private void ThreadThreeTick(object sender, EventArgs e)
        {
            threadCounter++;
            Task.Factory.StartNew(AttackThree);
        }
        private void AttackTwo()
        {
            engine.PerformAttack(Brushes.IndianRed);
        }

        private void AttackThree()
        {
            engine.PerformAttack(Brushes.GreenYellow);          
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddAttack();
        }

        private void AddAttack()
        {
            if (threadCounter.Equals(1))
            {
                _threadTwo.Tick += ThreadTwoTick;
                _threadTwo.Interval = new TimeSpan(0, 0, 0, 0, 200);
                _threadTwo.Start();
            }
            else if (threadCounter.Equals(2))
            {
                _threadThree.Tick += ThreadThreeTick;
                _threadThree.Interval = new TimeSpan(0, 0, 0, 0, 400);
                _threadThree.Start();
            }
            else
            {
                MoreButton.IsEnabled = false;
                for (int i = 0; i < 50; i++)
                {
                    Log("ITS ENOUGH!!!");
                }
            }



        }
        public void HackersWin(int c)
        {
            _threadOne.Stop();
            _threadTwo.Stop();
            _threadThree.Stop();
            MessageBox.Show("Hackers WIN!! They've stolen €" + c.ToString());
        }

        public void ServersWin(int c)
        {
            _threadOne.Stop();
            _threadTwo.Stop();
            _threadThree.Stop();
            MessageBox.Show("Servers WIN!! They didn't lose €" + c.ToString());

        }
    }
}
