using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ServersVSHackers_V1
{
    /// <summary>
    ///     Window that allows new polygons to be created.
    /// </summary>
    public partial class LevelDesigner : Window
    {
        private bool _savingCoordinates = false;
        private int MAX_COUNTRIES = 5;
        private int numberOfCountries;
        private PointCollection pointCollection = new PointCollection();
        private readonly List<Polygon> countriesList = new List<Polygon>();

        public LevelDesigner()
        {
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
            polygon.Fill = new SolidColorBrush(Colors.Yellow);
            var xMin = pc.Min(p => p.X);
            var yMin = pc.Min(p => p.Y);
            MessageBox.Show(String.Format("x: {0}, y= {1}", xMin, yMin));

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
            //CreateCanvas.Children.Add(CreatePolygon(CountryTextBox.Text, pointCollection));
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
            Environment.world = countriesList;
            this.Close();
        }
    }
}