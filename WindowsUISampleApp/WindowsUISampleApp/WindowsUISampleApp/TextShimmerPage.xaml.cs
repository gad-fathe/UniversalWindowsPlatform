using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsUISampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextShimmerPage : Page
    {
        public TextShimmerPage()
        {
            this.InitializeComponent();
            Loaded += TextShimmer_Loaded;
        }

        private Compositor _compositor;
        private PointLight _pointLight;
        private void TextShimmer_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //get interop compositor
            _compositor = ElementCompositionPreview.GetElementVisual(SampleTextBlock).Compositor;

            //get interop visual for XAML TextBlock
            var text = ElementCompositionPreview.GetElementVisual(SampleTextBlock);

            _pointLight = _compositor.CreatePointLight();
            _pointLight.Color = Colors.AliceBlue;
            _pointLight.CoordinateSpace = text; //set up co-ordinate space for offset
            _pointLight.Targets.Add(text); //target XAML TextBlock

            //starts out to the left; vertically centered; light's z-offset is related to fontsize
            _pointLight.Offset = new Vector3(-(float)SampleTextBlock.ActualWidth, (float)SampleTextBlock.ActualHeight / 2, (float)SampleTextBlock.FontSize);

            //simple offset.X animation that runs forever
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1, 2 * (float)SampleTextBlock.ActualWidth);
            animation.Duration = TimeSpan.FromSeconds(2.3f);
            animation.IterationBehavior = AnimationIterationBehavior.Forever;

            _pointLight.StartAnimation("Offset.X", animation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ParalaxedListViewPage));
        }
    }
}
