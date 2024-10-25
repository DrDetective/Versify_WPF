using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Versify
{
    /// <summary>
    /// Interakční logika pro FullLyrics.xaml
    /// </summary>
    public partial class FullLyrics : Window
    {
        public FullLyrics()
        {
            InitializeComponent();
            MainWindow nevim = new MainWindow();
            LyricsTBox.Text = 
            Debug.Write(LyricsTBox);
        }
    }
}
