using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        SimulationEngine engine;
        public MainWindow()
        {
            InitializeComponent();
            SimulationEngine engine = new SimulationEngine();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"ocean.jpg", UriKind.Relative));
            WorldCanvas.Background = ib;
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
            foreach (var country in Environment.world)
            {
                var xMin = country.Points.Min(p => p.X);
                var yMin = country.Points.Min(p => p.Y);
                var countrylabel = new TextBlock();
                countrylabel.Text = country.Name;
                Panel.SetZIndex(countrylabel, 10);
                Canvas.SetLeft(countrylabel, xMin);
                Canvas.SetTop(countrylabel, yMin);
                WorldCanvas.Children.Add(countrylabel);
                WorldCanvas.Children.Add(country);
            }

            

            




        }




    }
}
