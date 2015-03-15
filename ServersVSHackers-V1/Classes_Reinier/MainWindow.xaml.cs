using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestWW3.Properties;
using System.Drawing;

namespace TestWW3
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ValidPoint> points = new List<ValidPoint>();
        public Polygon ACountry = new Polygon();

        public MainWindow()
        {
            InitializeComponent();



            ConcurrentBag<IEntity> cbEntities = new ConcurrentBag<IEntity>();
            //one MILLION entities
            int _numberOfEntities = 5000;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers
            //value of Slider
            int _balance = 6;


            //hacker code
            int _numberOfHackers = (_numberOfEntities*_balance)/10;
            int _numberOfServers = (_numberOfEntities*(10 - _balance))/10;
            // balance = 1 (limit) is 10% hackers, 90% servers
            // balance = 9 (limit) is 90% hackers, 10% servers

            //generation code
            Stopwatch t = new Stopwatch();
            t.Start();

            Parallel.For(0, _numberOfServers, i => cbEntities.Add(new Server()));


            Parallel.For(0, _numberOfHackers, i => cbEntities.Add(new Hacker()));

            t.Stop();

            #region ACountry Definition
            Polygon ACountry = new Polygon();
            PointCollection pc = new PointCollection();
            pc.Add(new Point(0, 0));
            pc.Add(new Point(0, 200));
            pc.Add(new Point(122, 200));
            pc.Add(new Point(153, 0));
            ACountry.Points = pc;
            ACountry.Stroke = new SolidColorBrush(Colors.Green);
            ACountry.Fill = new SolidColorBrush(Colors.Yellow);
            Canvasx.Children.Add(ACountry);
            #endregion
           
            #region TestLine

            List<Line> lll = new List<Line>();
            Random X = new Random(1000);
            for (int i = 0; i < 25; i++)
            {
                
                Line l = new Line();
                l.X1 = l.Y1 = X.Next(440);
                l.X2 = l.Y2 = X.Next(440); 
                l.Stroke = Brushes.Orange;
                l.StrokeThickness = 6;
                lll.Add(l);
            }
            
            DoubleAnimation lineFade = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000));
          

            #endregion

            Point[] ps = pc.ToArray();

            for (double y = 0; y < Height; y++)
            {
                for (double x = 0; x < Width; x++)
                {
                    if (IsPointInPolygon(ps, new Point(x, y)))
                    {
                        points.Add(new ValidPoint() {x = x, y = y});
                    }

                }
            }

            Console.WriteLine("Number of Entities: {0}, ms elapsed = {1}", cbEntities.Count, t.ElapsedMilliseconds);
            Console.WriteLine("Amount of points: " + points.Count());
            this.Show();
            for (int i = 0; i < points.Count-10; i += 5)
            {
                points.RemoveAt(i);
            }

            DoubleAnimation da = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(1000));
            ACountry.BeginAnimation(OpacityProperty, da);
            Thread test = new Thread(Start);
            //test.Start();

         
            


            
        }
        
        private void Start()
        {
            int counter = 0;

            DispatcherOperation dispatcherOperation = Dispatcher.BeginInvoke(new Action(() =>
            {

                foreach (var i in points)
                {
                    Stopwatch tt = new Stopwatch();
                    //tt.Start();
                    System.Windows.Shapes.Rectangle rect;
                    rect = new System.Windows.Shapes.Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Red);
                    rect.Fill = new SolidColorBrush(Colors.Red);
                    rect.Width = 1;
                    rect.Height = 1;
                    Canvas.SetLeft(rect, i.x);
                    Canvas.SetTop(rect, i.y);
                    Canvasx.Children.Add(rect);
                    //Canvasx.Refresh();
                    //tt.Stop();
                    //MessageBox.Show(tt.ElapsedTicks.ToString());
                }

            }));
        }


        private bool IsPointInPolygon(Point[] polygon, Point point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
        public struct ValidPoint
        {
            public double x;
            public double y;
        }
    }
}