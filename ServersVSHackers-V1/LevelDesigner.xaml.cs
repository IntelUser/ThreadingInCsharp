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
    ///     <author>Reinier Weerts</author>
    ///     <author>Johannes Elzinga</author>
    ///     <date>02-2015</date>
    ///     Window that allows new polygons to be created.
    ///     
    /// </summary>

    public partial class LevelDesigner
    {
        private readonly List<Polygon> _countriesList = new List<Polygon>();
        private int MAX_COUNTRIES = 5;
        public MainWindow ParentWindow;
        private bool _savingCoordinates = false;
        private bool _canSave = true;
        private int _numberOfCountries;
        private PointCollection _pointCollection = new PointCollection();

        public LevelDesigner(MainWindow parentWindow)
        {
            ParentWindow = parentWindow;
            InitializeComponent();
        }

        /// <summary>
        /// Disallows numeric input in country name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CountryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (char.IsLetter((char) e.Key)) e.Handled = true;
        }

        /// <summary>
        /// Creates Polygon from points in pointcollection
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="pc"></param>
        /// <returns>Polygon</returns>
        private Polygon CreatePolygon(string Name, PointCollection pc)
        {
            var polygon = new Polygon
            {
                Name = Name,
                Points = pc,
                Stroke = new SolidColorBrush(Colors.Green),
                Fill = Brushes.LightGreen
            };

            //determine left-top most coordinate of polygon
            double xMin = pc.Min(p => p.X);
            double yMin = pc.Min(p => p.Y);

            //create label for country name
            var countrylabel = new TextBlock {Text = Name, FontSize = 28, TextDecorations = TextDecorations.Underline};
            Panel.SetZIndex(countrylabel, 10);

            //educated guess for placement of label
            Canvas.SetLeft(countrylabel, xMin + 50.0);
            Canvas.SetTop(countrylabel, yMin + 50.0);
            CreateCanvas.Children.Add(countrylabel);
            return polygon;
        }

        /// <summary>
        /// Saves designed Polygon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCountryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_canSave)
            {
                //persists drawn points to a 
                _numberOfCountries++;
                Polygon country = CreatePolygon(CountryTextBox.Text, _pointCollection);
                CreateCanvas.Children.Add(country);
                _countriesList.Add(country);
                _pointCollection = null;
                _pointCollection = new PointCollection();
                //limit of 5 countries for performance reasons
                if (_numberOfCountries.Equals(5))
                {
                    MessageBox.Show("Limit reached!");
                    SaveCountryButton.IsEnabled = false;
                    CountryTextBox.IsEnabled = false;
                }
            }
            _canSave = false;
        }

        /// <summary>
        /// reads x-y coordinates from mouseclick on canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _canSave = true;
            Point p = Mouse.GetPosition(CreateCanvas);
            _pointCollection.Add(p);
        }

        /// <summary>
        /// clears all canvas children and reenables controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCanvas.Children.Clear();
            _countriesList.Clear();
            SaveCountryButton.IsEnabled = true;
            CountryTextBox.IsEnabled = true;
            _numberOfCountries = 0;
        }

        /// <summary>
        /// Delete last added country and its label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLastButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Polygon removeCountry = _countriesList.Last();
                _countriesList.Remove(removeCountry);
                CreateCanvas.Children.Remove(removeCountry);

                IEnumerable<TextBlock> removeLabel = from r in CreateCanvas.Children.OfType<TextBlock>()
                    where r.Text == removeCountry.Name
                    select r;
                TextBlock result = removeLabel.First();
                CreateCanvas.Children.Remove(result);
                _numberOfCountries--;
            }
            catch (Exception)
            {
                //TODO
            }
        }

        /// <summary>
        /// Saves created countries and closes designer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveWorldButton_Click(object sender, RoutedEventArgs e)
        {
            //cleanup used canvas and remove parent relationships
            List<Polygon> polygons = CreateCanvas.Children.OfType<Polygon>().ToList();
            foreach (Polygon polygon in polygons)
            {
                CreateCanvas.Children.Remove(polygon);
            }

            //set current list of polygons to Environment
            Environment.World = _countriesList;

            //force update of simulation world canvas
            ParentWindow.UpdateWorld();

            CreateCanvas = null;
            Close();
        }

        /// <summary>
        /// Maps save functionality to rightmouseclick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SaveCountryButton_Click(sender, e);
        }
    }
}