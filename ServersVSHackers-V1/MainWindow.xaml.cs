using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Nest;
using ServersVSHackers_V1.Database;
using ServersVSHackers_V1.Properties;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabaseController _dbController;
        private readonly DispatcherTimer _timerOne = new DispatcherTimer();
        private readonly DispatcherTimer _timerTwo = new DispatcherTimer();
        private readonly DispatcherTimer _timerThree = new DispatcherTimer();
        public SimulationEngine engine;
        private TimeSpan interval = new TimeSpan(0, 0, 0, 0, 10);
        private int threadCounter;

        public MainWindow()
        {
            InitializeComponent();
            //set background image
            var bgBrush = new ImageBrush();
            bgBrush.ImageSource = new BitmapImage(new Uri(@"ocean.jpg", UriKind.Relative));
            WorldCanvas.Background = bgBrush;

            
        }

        /// <summary>
        /// Adds log string to log RTB.
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log)
        {
            LogTextBox.Document.Blocks.Add(new Paragraph(new Run(log)));
            LogTextBox.ScrollToEnd();
        }

        /// <summary>
        /// Saves balance value to settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Settings.Default.balanceValue = EntitySlider.Value;
            Settings.Default.Save();
            Console.WriteLine(@"Saved balanceValue = {0}", Settings.Default.balanceValue);
        }

        /// <summary>
        /// Opens leveldesigner.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelDesignerButton_Click(object sender, RoutedEventArgs e)
        {
            new LevelDesigner(this).Show();
        }

        /// <summary>
        /// Empties canvas. Adds designed countries to Canvas, sets labels.
        /// </summary>
        public void UpdateWorld()
        {
            WorldCanvas.Children.Clear();
            foreach (Polygon country in Environment.World)
            {
                country.Effect =
                    new DropShadowEffect
                    {
                        Color = new Color {A = 255, R = 255, G = 255, B = 255},
                        Direction = 320,
                        ShadowDepth = 8,
                        Opacity = 0.7
                    };
                double xMin = country.Points.Min(p => p.X);
                double yMin = country.Points.Min(p => p.Y);
                var countrylabel = new TextBlock();
                countrylabel.Text = country.Name;
                countrylabel.Text = Name;
                countrylabel.FontSize = 28;
                countrylabel.TextDecorations = TextDecorations.Underline;
                Panel.SetZIndex(countrylabel, 10);
                Canvas.SetLeft(countrylabel, xMin + 50.0);
                Canvas.SetTop(countrylabel, yMin + 50.0);
                WorldCanvas.Children.Add(countrylabel);
                WorldCanvas.Children.Add(country);
            }
        }

        /// <summary>
        /// Lazy load engine, starts initialisation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (engine == null)
            {
                engine = new SimulationEngine(this);
            }
            engine.GenerateEntities();
        }

        /// <summary>
        /// Starts timer and 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttackButton_Click(object sender, RoutedEventArgs e)
        {
            _timerOne.Tick += TimerOneTick;
            _timerOne.Interval = interval;
            _timerOne.Start();
            threadCounter++;
            AttackButton.IsEnabled = false;
        }

        /// <summary>
        /// Triggered on _timerOne tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOneTick(object sender, EventArgs e)
        {
            engine.PerformAttack(Brushes.Orange);
        }

        /// <summary>
        /// Triggered on _timerTwo tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTwoTick(object sender, EventArgs e)
        {
            threadCounter++;
            Task.Factory.StartNew(AttackThree);
        }


        /// <summary>
        /// Triggered on _timerThree tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerThreeTick(object sender, EventArgs e)
        {
            threadCounter++;
            Task.Factory.StartNew(AttackTwo);
        }

        
        private void AttackTwo()
        {
            engine.PerformAttack(Brushes.IndianRed);
        }

        private void AttackThree()
        {
            engine.PerformAttack(Brushes.GreenYellow);
        }

        private void PerformAttack_Click(object sender, RoutedEventArgs e)
        {
            AddAttack();
        }

        /// <summary>
        /// Adds new attack Task and start timer if amount of Tasks less than 4
        /// </summary>
        private void AddAttack()
        {
            if (threadCounter.Equals(1))
            {
                _timerThree.Tick += TimerThreeTick;
                _timerThree.Interval = interval;
                _timerThree.Start();
            }
            else if (threadCounter.Equals(2))
            {
                _timerTwo.Tick += TimerTwoTick;
                _timerTwo.Interval = interval;
                _timerTwo.Start();
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

        //testcomment

        public void HackersWin(int c)
        {
            _timerOne.Stop();
            _timerThree.Stop();
            _timerTwo.Stop();
            MessageBox.Show("Hackers WIN!! They've stolen €" + c);
        }

        public void ServersWin(int c)
        {
            _timerOne.Stop();
            _timerThree.Stop();
            _timerTwo.Stop();
            MessageBox.Show("Servers WIN!! They didn't lose €" + c);
        }


        private void IntervalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            interval = TimeSpan.FromMilliseconds(Settings.Default.interval);
        }

        /// <summary>
        /// Opens statistics window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new Statistics(engine._attacks).Show();
        }
    }
}