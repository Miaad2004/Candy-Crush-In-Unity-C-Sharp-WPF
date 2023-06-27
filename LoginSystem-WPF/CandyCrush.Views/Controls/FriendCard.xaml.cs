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

namespace CandyCrush.UI.Controls
{
    /// <summary>
    /// Interaction logic for FriendCard.xaml
    /// </summary>
    public partial class FriendCard : UserControl
    {
        public event EventHandler<UserEventArgs> UnfriendButtonClicked;
        public event EventHandler<UserEventArgs> CreateContestButtonClicked;

        public FriendCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(FriendCard));
        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public static readonly DependencyProperty LastSeenProperty = DependencyProperty.Register("LastSeen", typeof(string), typeof(FriendCard));
        public string LastSeen
        {
            get { return (string)GetValue(LastSeenProperty); }
            set { SetValue(LastSeenProperty, value); }
        }

        public static readonly DependencyProperty ProfileImagePathProerty = DependencyProperty.Register("ProfileImagePath", typeof(string), typeof(FriendCard));

        public string ProfileImagePath
        {
            get { return (string)GetValue(ProfileImagePathProerty); }
            set { SetValue(ProfileImagePathProerty, value); }
        }

        private void UnfriendButton_Click(object sender, RoutedEventArgs e)
        {
            UnfriendButtonClicked?.Invoke(this, new UserEventArgs(Username));
        }

        private void CreateContestButton_Click(object sender, RoutedEventArgs e)
        {
            CreateContestButtonClicked?.Invoke(this, new UserEventArgs(Username));
        }
    }
}
