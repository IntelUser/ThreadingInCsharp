using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestWW3;

namespace ServersVSHackers_V1
{
    /// <summary>
    /// Window that allows new polygons to be created.
    /// </summary>
    public partial class LevelDesigner : Window
    {
        PointCollection pointCollection = new PointCollection();
        List<Polygon> countriesList = new List<Polygon>();
        private bool _savingCoordinates = false;
        public LevelDesigner()
        {
            InitializeComponent();

        }

        private void NewCountryButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void CountryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (char.IsLetter((char)e.Key)) e.Handled = true;
        }
        private Polygon CreatePolygon(string Name, PointCollection pc)
        {
            Polygon polygon = new Polygon();
            polygon.Name = Name;
            polygon.Points = pc;
            polygon.Stroke = new SolidColorBrush(Colors.Green);
            polygon.Fill = new SolidColorBrush(Colors.Yellow);
            var xMin = pc.Min(p => p.X);
            var yMin = pc.Min(p => p.Y);


            TextBlock countrylabel = new TextBlock();
            countrylabel.Text = Name;
            Canvas.SetLeft(countrylabel, xMin);
            Canvas.SetTop(countrylabel, yMin);
            CreateCanvas.Children.Add(countrylabel);
            return polygon;            
        }


        private void SaveCountryButton_Click(object sender, RoutedEventArgs e)
        {

            //CreateCanvas.Children.Add(CreatePolygon(CountryTextBox.Text, pointCollection));
            Gridx.Children.Add(CreatePolygon(CountryTextBox.Text, pointCollection));
            pointCollection = null;
            pointCollection = new PointCollection();

        }

        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            {
                //reads x-y coordinates from mouseclick on canvas
                Point p = Mouse.GetPosition(CreateCanvas);
                pointCollection.Add(p);
            }
        }



    }
}
