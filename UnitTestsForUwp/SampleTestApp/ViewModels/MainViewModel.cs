using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Plugin.Media;
using SampleTestApp.CognitiveServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SampleTestApp.ViewModels
{
    [Bindable]
    public class MainViewModel : ViewModelBase
    {
        ICognitiveClient _cognitiveClient;
        public ICommand AnalyzeImageCommand { get; set; }
        private string _analysisResult;

        public string AnalysisResult
        {
            get { return _analysisResult; }
            set { _analysisResult = value; RaisePropertyChanged(); }
        }

        private BitmapImage _myImageSource;
        public BitmapImage MyImageSource
        {
            set
            {
                _myImageSource = value;
                RaisePropertyChanged();
            }
            get
            {
                if (_myImageSource == null)
                    return new BitmapImage(new Uri("ms-appx:///Assets/placeholder.jpg", UriKind.Absolute));
                else
                    return _myImageSource;
            }
        }


        public MainViewModel()
        {
            AnalyzeImageCommand = new RelayCommand(async () => await OnAddNewImage());
            _cognitiveClient = ServiceLocator.Current.GetInstance<ICognitiveClient>();
        }

        async Task OnAddNewImage()
        {
            var image = await CrossMedia.Current.PickPhotoAsync();
            if (image != null)
            {
                var stream = image.GetStream();
                MyImageSource = new BitmapImage(new Uri(image.Path, UriKind.Absolute));
                var analysisResult = await _cognitiveClient.GetImageDescription(stream);
                string tags = "";
                analysisResult.Description.Tags.ToList().ForEach(t => tags = tags + "\n" + t);
                AnalysisResult = tags;
            }
        }
    }
}
