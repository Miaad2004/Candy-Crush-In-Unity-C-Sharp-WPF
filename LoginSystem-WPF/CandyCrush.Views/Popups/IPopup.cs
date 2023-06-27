using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace CandyCrush.UI.Popups
{
    public interface IPopup
    {
        void Window_MouseDown(object sender, MouseButtonEventArgs e);
        void Minimize_Click(object sender, RoutedEventArgs e);
        void Close_Click(object sender, RoutedEventArgs e);
    }
}
