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
        private bool canSave = true;
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

            polygon.Fill = Brushes.LightGreen;
            //determine left-top most coordinate of polygon
            var xMin = pc.Min(p => p.X);
            var yMin = pc.Min(p => p.Y);
            
            //create label for country name
            var countrylabel = new TextBlock();
            countrylabel.Text = Name;
            countrylabel.FontSize = 28;
            countrylabel.TextDecorations = TextDecorations.Underline;
            Panel.SetZIndex(countrylabel, 10);
            //educated guess for placement of label
            Canvas.SetLeft(countrylabel, xMin+50.0);
            Canvas.SetTop(countrylabel, yMin+50.0);
            CreateCanvas.Children.Add(countrylabel);
            return polygon;
        }

        private void SaveCountryButton_Click(object sender, RoutedEventArgs e)
        {
            if (canSave)
            {
                //persists drawn points to a 
                numberOfCountries++;
                var country = CreatePolygon(CountryTextBox.Text, pointCollection);
                CreateCanvas.Children.Add(country);
                countriesList.Add(country);
                pointCollection = null;
                pointCollection = new PointCollection();
                //limit of 5 countries for performance reasons
                if (numberOfCountries.Equals(5))
                {
                    MessageBox.Show("Limit reached!");
                    SaveCountryButton.IsEnabled = false;
                    CountryTextBox.IsEnabled = false;
                }
            }
            canSave = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canSave = true;
            //reads x-y coordinates from mouseclick on canvas
            var p = Mouse.GetPosition(CreateCanvas);
            pointCollection.Add(p);
            
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //clears all canvas children
            CreateCanvas.Children.Clear();
            countriesList.Clear();
            SaveCountryButton.IsEnabled = true;
            CountryTextBox.IsEnabled = true;
            numberOfCountries = 0;
        }

        private void DeleteLastButton_Click(object sender, RoutedEventArgs e)
        {
            //delete last added country and its label
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
                numberOfCountries--;
            }
            catch (Exception)
            {
                //TODO
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