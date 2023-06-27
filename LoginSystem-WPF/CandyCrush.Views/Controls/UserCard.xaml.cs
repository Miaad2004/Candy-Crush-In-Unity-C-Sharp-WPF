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
    /// Interaction logic for UserCard.xaml
    /// </summary>
    public partial class UserCard : UserControl
    {
        public event EventHandler<UserEventArgs> AddFriendButtonClicked;

        public UserCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(UserCard));
        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public static readonly DependencyProperty LastSeenProperty = DependencyProperty.Register("LastSeen", typeof(string), typeof(UserCard));
        public string LastSeen
        {
            get { return (string)GetValue(LastSeenProperty); }
            set { SetValue(LastSeenProperty, value); }
        }

        public static readonly DependencyProperty ProfileImagePathProerty = DependencyProperty.Register("ProfileImagePath", typeof(string), typeof(UserCard));

        public string ProfileImagePath
        {
            get { return (string)GetValue(ProfileImagePathProerty); }
            set { SetValue(ProfileImagePathProerty, value); }
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            AddFriendButtonClicked?.Invoke(this, new UserEventArgs(Username));
        }
    }
}
