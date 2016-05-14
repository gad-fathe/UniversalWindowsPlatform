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
using Windows.UI.Xaml.Navigation;


namespace UWP.ExpandableListView
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ShowExpandableListView();
        }

        private void ShowExpandableListView()
        {
            Dictionary<string, List<SampleListViewItem>> groupedItemsDictionary = new Dictionary<string, List<SampleListViewItem>>();
            groupedItemsDictionary.Add("Sample header 1", new List<SampleListViewItem>() { new SampleListViewItem() {Name = "Sample sub item 1", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 2", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 3", ImageSource = "ms-appx:///Assets/Windows10Logo.png" } });
            groupedItemsDictionary.Add("Sample header 2", new List<SampleListViewItem>() { new SampleListViewItem() { Name = "Sample sub item 1", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 2", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 3", ImageSource = "ms-appx:///Assets/Windows10Logo.png" } });
            groupedItemsDictionary.Add("Sample header 3", new List<SampleListViewItem>() { new SampleListViewItem() { Name = "Sample sub item 1", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 2", ImageSource = "ms-appx:///Assets/Windows10Logo.png" }, new SampleListViewItem() { Name = "Sample sub item 3", ImageSource = "ms-appx:///Assets/Windows10Logo.png" } });
            ExpandableListView expandableListView = new ExpandableListView(groupedItemsDictionary);
            var thickness = new Thickness();
            thickness.Left = 20;
            thickness.Right = 20;
            expandableListView.Margin = thickness;
            Grid.SetRow(expandableListView, 1);
            MainGrid.Children.Add(expandableListView);
 
        }
    }
}
