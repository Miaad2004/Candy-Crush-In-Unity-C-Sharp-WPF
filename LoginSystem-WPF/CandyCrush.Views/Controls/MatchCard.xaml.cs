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
    /// Interaction logic for MatchCard.xaml
    /// </summary>
    public partial class MatchCard : UserControl
    {
        public event EventHandler<MatchEventArgs> ViewMatchResultButtonClicked;
        public event EventHandler<MatchEventArgs> PlayButtonClicked;

        public MatchCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MatchIdProerty = DependencyProperty.Register("MatchId", typeof(Guid), typeof(MatchCard));
        public Guid MatchId
        {
            get { return (Guid)GetValue(MatchIdProerty); }
            set { SetValue(MatchIdProerty, value); }
        }

        public static readonly DependencyProperty MatchTitleProperty = DependencyProperty.Register("MatchTitle", typeof(string), typeof(MatchCard));
        public string MatchTitle
        {
            get { return (string)GetValue(MatchTitleProperty); }
            set { SetValue(MatchTitleProperty, value); }
        }

        public static readonly DependencyProperty CreationDateProerty = DependencyProperty.Register("CreationDate", typeof(string), typeof(MatchCard));
        public string CreationDate
        {
            get { return (string)GetValue(CreationDateProerty); }
            set { SetValue(CreationDateProerty, value); }
        }

        public static readonly DependencyProperty ProfileImagePath1Proerty = DependencyProperty.Register("ProfileImagePath1", typeof(string), typeof(MatchCard));

        public string ProfileImagePath1
        {
            get { return (string)GetValue(ProfileImagePath1Proerty); }
            set { SetValue(ProfileImagePath1Proerty, value); }
        }

        public static readonly DependencyProperty ProfileImagePath2Proerty = DependencyProperty.Register("ProfileImagePath2", typeof(string), typeof(MatchCard));

        public string ProfileImagePath2
        {
            get { return (string)GetValue(ProfileImagePath2Proerty); }
            set { SetValue(ProfileImagePath2Proerty, value); }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonClicked?.Invoke(this, new MatchEventArgs(MatchId));
        }

        private void ViewMatchResultButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMatchResultButtonClicked?.Invoke(this, new MatchEventArgs(MatchId));
        }
    }
}
