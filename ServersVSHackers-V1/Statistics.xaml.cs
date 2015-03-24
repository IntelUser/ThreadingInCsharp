using System;
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

namespace ServersVSHackers_V1
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {

        private IEnumerable<Attack> _attacks;
        private int _countAttacks;
        private double _averageCashStolen, _averageAttacksPerServer;
        private List<Top10> top10 = new List<Top10>(); 



        public Statistics(IEnumerable<Attack> attacks)
        {
            InitializeComponent();
            _attacks = attacks;
            
            

        }

        /// <summary>
        /// Executes PLINQ queries on Attack collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            //calculations
            _countAttacks = _attacks.Count();

            var averageCashQuery = from attack in _attacks.AsParallel()
                select attack.CashStolen;


            _averageCashStolen = Math.Round(((double)averageCashQuery.Sum() / _countAttacks),2);


            CountAttacksBlock.Text = _countAttacks.ToString();

            AverageCashBlock.Text = "€ " + _averageCashStolen.ToString();

            //update ui
            //top10


            
            var result = _attacks.OrderBy(x => x.CashStolen).Reverse().Take(10);
            foreach( var i in result)
            {

                top10.Add(new Top10() {HackerId=i.Hacker.Id.ToString(),Cash=i.CashStolen.ToString()});
            }
            Top10DataGrid.DataContext = top10;
            Top10DataGrid.ItemsSource = top10;
            Top10DataGrid.MinColumnWidth = 150;


             var query = _attacks.GroupBy(l => l.TimeStamp, l => l.Server)
        .Select(g => new
        {
            Server = g.Key,
            Count = g.Distinct().Count()
        });

            var asdi = new Top10();


        }

        struct Top10
        {
            public string HackerId;
            public string Cash;
        }
    }
}
