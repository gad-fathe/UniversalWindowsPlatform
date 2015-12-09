using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FirstWindows10App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        Dictionary<string, string> _brandsDictionary;
        string _playerName, _actualBrand, _answer;
        public GamePage()
        {
            this.InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            _brandsDictionary = new Dictionary<string, string>();
            _brandsDictionary.Add("Microsoft", "Assets/Images/microsoft-logo.png");
            _brandsDictionary.Add("Office", "Assets/Images/office-logo.png");
            _brandsDictionary.Add("Windows", "Assets/Images/windows-logo.png");
            _brandsDictionary.Add("Windows phone", "Assets/Images/windowsPhone-logo.png");

            logoImage.Source = new BitmapImage(new Uri("ms-appx:///" + _brandsDictionary["Microsoft"]));
            _actualBrand = _brandsDictionary.Keys.ElementAt(0);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _playerName = (string)e.Parameter;
        }

        private void answerButton_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswer();
        }

        private void CheckAnswer()
        {
            _answer = answerTextBox.Text;

            if ( _answer.Equals(_actualBrand, StringComparison.CurrentCultureIgnoreCase))
            {

                GameResultTextBlock.Text = _playerName + " good answer!";
                GameResultTextBlock.Visibility = Visibility.Visible;
                nextButton.Visibility = Visibility.Visible;
                _brandsDictionary.Remove(_actualBrand);

                if (_brandsDictionary.Count == 0)
                    Frame.Navigate(typeof(EndGamePage), _playerName);
            }

            else
            {

                GameResultTextBlock.Text = _playerName + " try again!";
                GameResultTextBlock.Visibility = Visibility.Visible;
                answerTextBox.Text = "";
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayNextGame();
        }

        private void PlayNextGame()
        {
            answerTextBox.Text = "";
            GameResultTextBlock.Visibility = Visibility.Collapsed;
            nextButton.Visibility = Visibility.Collapsed;

            _actualBrand = _brandsDictionary.Keys.ElementAt(0);
            logoImage.Source = new BitmapImage(new Uri("ms-appx:///" + _brandsDictionary[_actualBrand]));
        }
    }
}
