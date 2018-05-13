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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PoligHellman
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PoligHellmanMethod poligHellman = new PoligHellmanMethod(7531,6,8101);
            //PoligHellmanMethod poligHellman = new PoligHellmanMethod(11850, 5, 24697);
            var res = poligHellman.GetX();
            var t = (new lib()).ModPow(5, res, 24697);
        }
    }
}
