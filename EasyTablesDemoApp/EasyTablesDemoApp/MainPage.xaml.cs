using EasyTablesDemoApp.AzureAccess;
using EasyTablesDemoApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EasyTablesDemoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AzureDataService _azureDataService;
        List<User> _usersList;
        public MainPage()
        {
            this.InitializeComponent();
            _azureDataService = new AzureDataService();
            _usersList = new List<User>();
            connectWithbackend();
        }

        private async void connectWithbackend()
        {
            await _azureDataService.Initialize();
        }
        //Fill the listview with users:
        private async void fillUsersList()
        {
            _usersList = await _azureDataService.GetUsers();
        }

        //Add new user (it is hardcoded now in the AzureDataService class):
        private async void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            await _azureDataService.AddUser();
            fillUsersList();
        }
    }
}
