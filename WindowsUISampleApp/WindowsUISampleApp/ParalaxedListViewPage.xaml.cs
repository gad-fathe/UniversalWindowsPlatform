using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsUISampleApp.DataSource;
using WindowsUISampleApp.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsUISampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ParalaxedListViewPage : Page
    {
        private ExpressionAnimation _parallaxExpression;
        private CompositionPropertySet _scrollProperties;

        public ParalaxedListViewPage()
        {
            Model = new LocalDataSource();
            this.InitializeComponent();
        }

        public static string StaticSampleName { get { return "Parallaxing ListView Items"; } }     

        public LocalDataSource Model { set; get; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Get scrollviewer
            ScrollViewer myScrollViewer = ThumbnailList.GetFirstDescendantOfType<ScrollViewer>();
            _scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(myScrollViewer);

            // Setup the expression
            _parallaxExpression = compositor.CreateExpressionAnimation();
            _parallaxExpression.SetScalarParameter("StartOffset", 0.0f);
            _parallaxExpression.SetScalarParameter("ParallaxValue", 0.5f);
            _parallaxExpression.SetScalarParameter("ItemHeight", 0.0f);
            _parallaxExpression.SetReferenceParameter("ScrollManipulation", _scrollProperties);
            _parallaxExpression.Expression = "(ScrollManipulation.Translation.Y + StartOffset - (0.5 * ItemHeight)) * ParallaxValue - (ScrollManipulation.Translation.Y + StartOffset - (0.5 * ItemHeight))";

            ThumbnailList.ItemsSource = Model.Items;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_parallaxExpression != null)
            {
                _parallaxExpression.Dispose();
                _parallaxExpression = null;
            }

            if (_scrollProperties != null)
            {
                _scrollProperties.Dispose();
                _scrollProperties = null;
            }
        }

        private void ThumbanilList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            Thumbnail thumbnail = args.Item as Thumbnail;
            Image image = args.ItemContainer.ContentTemplateRoot.GetFirstDescendantOfType<Image>();

            Visual visual = ElementCompositionPreview.GetElementVisual(image);
            visual.Size = new Vector2(960f, 960f);

            if (_parallaxExpression != null)
            {
                _parallaxExpression.SetScalarParameter("StartOffset", (float)args.ItemIndex * visual.Size.Y / 4.0f);
                visual.StartAnimation("Offset.Y", _parallaxExpression);
            }
        }
    }
}
