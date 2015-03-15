using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestWW3;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     Window that allows new polygons to be created.
    /// </summary>
    public partial class LevelDesigner : Window
    {
        public MainWindow _parentWindow;
        private bool _savingCoordinates = false;
        private int MAX_COUNTRIES = 5;
        private int numberOfCountries;
        private PointCollection pointCollection = new PointCollection();
        private readonly List<Polygon> countriesList = new List<Polygon>();

        public LevelDesigner(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            InitializeComponent();
        }

        private void CountryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (char.IsLetter((char) e.Key)) e.Handled = true;
        }

        private Polygon CreatePolygon(string Name, PointCollection pc)
        {
            var polygon = new Polygon();
            polygon.Name = Name;
            polygon.Points = pc;
            polygon.Stroke = new SolidColorBrush(Colors.Green);

            polygon.Fill = (VisualBrush)this.Resources["HatchBrush"];

            var xMin = pc.Min(p => p.X);
            var yMin = pc.Min(p => p.Y);
            
            var countrylabel = new TextBlock();
            countrylabel.Text = Name;
            Panel.SetZIndex(countrylabel, 10);
            Canvas.SetLeft(countrylabel, xMin);
            Canvas.SetTop(countrylabel, yMin);
            CreateCanvas.Children.Add(countrylabel);
            return polygon;
        }

        private void SaveCountryButton_Click(object sender, RoutedEventArgs e)
        {
            numberOfCountries++;
            var country = CreatePolygon(CountryTextBox.Text, pointCollection);
            CreateCanvas.Children.Add(country);
            countriesList.Add(country);
            pointCollection = null;
            pointCollection = new PointCollection();
            if (numberOfCountries.Equals(5))
            {
                MessageBox.Show("Limit reached!");
                SaveCountryButton.IsEnabled = false;
                CountryTextBox.IsEnabled = false;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            {
                //reads x-y coordinates from mouseclick on canvas
                var p = Mouse.GetPosition(CreateCanvas);
                pointCollection.Add(p);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCanvas.Children.Clear();
            countriesList.Clear();
            SaveCountryButton.IsEnabled = true;
            CountryTextBox.IsEnabled = true;
            numberOfCountries = 0;
        }

        private void DeleteLastButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var removeCountry = countriesList.Last();
                countriesList.Remove(removeCountry);
                CreateCanvas.Children.Remove(removeCountry);

                var removeLabel = from r in CreateCanvas.Children.OfType<TextBlock>()
                    where r.Text == removeCountry.Name
                    select r;
                var result = removeLabel.First();
                CreateCanvas.Children.Remove(result);
            }
            catch (Exception)
            {
            }
        }

        private void SaveWorldButton_Click(object sender, RoutedEventArgs e)
        {
            //cleanup used canvas and remove parent relationships
            var polygons = CreateCanvas.Children.OfType<Polygon>().ToList();
            foreach (var polygon in polygons)
            {
                CreateCanvas.Children.Remove(polygon);
            }
            

            //set current list of polygons to Environment
            Environment.World = countriesList;

            //force update of simulation world canvas
            _parentWindow.UpdateWorld();

            CreateCanvas = null;
            Close();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SaveCountryButton_Click(sender, e);

        }
    }
}