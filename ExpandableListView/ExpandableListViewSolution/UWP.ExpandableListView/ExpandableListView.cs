using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.ExpandableListView
{
    public class ExpandableListView : StackPanel
    {
        private Dictionary<string, List<SampleListViewItem>> _groupedItemsDictionary;
        public Dictionary<string, List<SampleListViewItem>> GroupedItemsDictionary
        {
            get
            {
                return _groupedItemsDictionary;
            }

            set
            {
                _groupedItemsDictionary = value;
            }
        }

        public ExpandableListView(Dictionary<string, List<SampleListViewItem>> groupedItemsDictionary)
        {
            GroupedItemsDictionary = groupedItemsDictionary;

           foreach(string groupName in GroupedItemsDictionary.Keys)
            {
                this.Children.Add(GenerateGroups(groupName, GroupedItemsDictionary[groupName]));
            }
        }

        private Grid GenerateGroups(string header, List<SampleListViewItem> groupItems)
        {
            Grid groupGrid = new Grid();
            var thickness = new Thickness();
            thickness.Top = 40;
            ColumnDefinition mainGridColumnDefinition1 = new ColumnDefinition();
            mainGridColumnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition mainGridColumnDefinition2 = new ColumnDefinition();
            mainGridColumnDefinition2.Width = new GridLength(2, GridUnitType.Star);
            groupGrid.ColumnDefinitions.Add(mainGridColumnDefinition1);
            groupGrid.ColumnDefinitions.Add(mainGridColumnDefinition2);
            groupGrid.Margin = thickness;

            RowDefinition mainGridRowDefinition1 = new RowDefinition();
            mainGridRowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition mainGridRowDefinition2 = new RowDefinition();
            mainGridRowDefinition2.Height = new GridLength(7, GridUnitType.Star);
            RowDefinition mainGridRowDefinition3 = new RowDefinition();
            mainGridRowDefinition3.Height = new GridLength(1, GridUnitType.Star);
            groupGrid.RowDefinitions.Add(mainGridRowDefinition1);
            groupGrid.RowDefinitions.Add(mainGridRowDefinition2);
            groupGrid.RowDefinitions.Add(mainGridRowDefinition3);

            StackPanel headerStackPanel = new StackPanel();
            headerStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            headerStackPanel.Orientation = Orientation.Horizontal;
            Grid.SetRow(headerStackPanel, 0);
            Grid.SetColumn(headerStackPanel, 0);
            groupGrid.Children.Add(headerStackPanel);


            Image expandGroupImage = new Image();
            expandGroupImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandArrow.png", UriKind.Absolute));
            expandGroupImage.Stretch = Stretch.Uniform;
            expandGroupImage.Width = 30;
            expandGroupImage.HorizontalAlignment = HorizontalAlignment.Left;
            expandGroupImage.Tag = new ImageState();
            headerStackPanel.Children.Add(expandGroupImage);

            thickness = new Thickness();
            thickness.Left = 46;
            StackPanel groupSubItemsPanel = new StackPanel();
            groupSubItemsPanel.Margin = thickness;
            groupSubItemsPanel.Visibility = Visibility.Collapsed;
            groupSubItemsPanel.VerticalAlignment = VerticalAlignment.Stretch;
            groupSubItemsPanel.Orientation = Orientation.Vertical;

            Grid.SetRow(groupSubItemsPanel, 1);
            Grid.SetColumnSpan(groupSubItemsPanel, 1);
            groupGrid.Children.Add(groupSubItemsPanel);

            thickness = new Thickness();
            thickness.Left = 6;
            TextBlock groupHeaderTextBlock = new TextBlock();
            groupHeaderTextBlock.Margin = thickness;
            groupHeaderTextBlock.Text = header;
            groupHeaderTextBlock.FontSize = 20;
            headerStackPanel.Children.Add(groupHeaderTextBlock);

            headerStackPanel.Tapped += (s, e) =>
            {
                changeGroupState(expandGroupImage, groupSubItemsPanel);
                e.Handled = true;
            };
            generateSubItems(groupSubItemsPanel, groupItems);

            return groupGrid;
        }

        private void generateSubItems(StackPanel groupSubItemsPanel, List<SampleListViewItem> groupItems)
        {
            foreach(SampleListViewItem groupItem in groupItems)
            {
                var thickness = new Thickness();
                thickness.Top = 20;
                StackPanel itemPanel = new StackPanel();
                itemPanel.Orientation = Orientation.Horizontal;
                itemPanel.Margin = thickness;

                Image groupItemTextImage = new Image();
                groupItemTextImage.Source = new BitmapImage(new Uri(groupItem.ImageSource, UriKind.Absolute));
                groupItemTextImage.Stretch = Stretch.Uniform;
                groupItemTextImage.Width = 30;
                itemPanel.Children.Add(groupItemTextImage);

                thickness = new Thickness();
                thickness.Left = 8;
                TextBlock groupItemTextBlock = new TextBlock();
                groupItemTextBlock.Margin = thickness;
                groupItemTextBlock.VerticalAlignment = VerticalAlignment.Center;
                groupItemTextBlock.Text = groupItem.Name;
                groupItemTextBlock.FontSize = 14;

                itemPanel.Children.Add(groupItemTextBlock);

                groupSubItemsPanel.Children.Add(itemPanel);
            }
        }

        private void changeGroupState(Image expandGroupImage, StackPanel groupSubItemsPanel)
        {
            var imageState = (ImageState)expandGroupImage.Tag;
            if (imageState.Expanded == true)
            {
                groupSubItemsPanel.Visibility = Visibility.Collapsed;
                expandGroupImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandArrow.png", UriKind.Absolute));
                imageState.Expanded = false;
            }
            else
            {
                groupSubItemsPanel.Visibility = Visibility.Visible;
                expandGroupImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseArrow.png", UriKind.Absolute));
                imageState.Expanded = true;
            }
        }

    }
}
