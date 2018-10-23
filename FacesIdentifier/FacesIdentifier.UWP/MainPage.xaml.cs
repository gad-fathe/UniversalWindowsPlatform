using FacesIdentifier.UWP.Services;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FacesIdentifier.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly FaceRecognitionService _faceRecognitionService;
        private MediaCapture _mediaCapture;
        private byte[] _takenImage;

        public MainPage()
        {
            this.InitializeComponent();
            _faceRecognitionService = new FaceRecognitionService();
            _faceRecognitionService.TrainingStatusChanged += (s, e) =>
            {
                InfoTextBlock.Text = e;
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            previewElement.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
        }

        private async void TakePictureButton_Click(object sender, RoutedEventArgs e)
        {
            await CapturePhoto();
        }

        private async void SavePictureButton_Click(object sender, RoutedEventArgs e)
        {
            await SavePhotoAndTrain();
        }

        private async Task SavePhotoAndTrain()
        {
            if (string.IsNullOrEmpty(GroupIdTextBox.Text) || string.IsNullOrEmpty(GroupNameTextBox.Text) || string.IsNullOrEmpty(PersonNameTextBox.Text))
            {
                await ShowDialog("Group name, group ID and person name cannot be empty");
                return;
            }

            try
            {
                Stream stream = await GetImageStream();

                var groupID = GroupIdTextBox.Text;
                var groupName = GroupNameTextBox.Text;
                var personName = PersonNameTextBox.Text;

                await _faceRecognitionService.CreatePersonGroup(groupID, groupName);
                await _faceRecognitionService.AddNewPersonToGroup(groupID, personName);
                Tuple<string, CreatePersonResult> definePersonGroupResult = await _faceRecognitionService.AddNewPersonToGroup(groupID, personName);
                var registerPersonResult = await _faceRecognitionService.RegisterPerson(definePersonGroupResult.Item1, definePersonGroupResult.Item2, stream);
                await _faceRecognitionService.TrainPersonGroup(definePersonGroupResult.Item1);
                var trainingStatus = await _faceRecognitionService.VerifyTrainingStatus(definePersonGroupResult.Item1);

                stream = await GetImageStream();
                InfoTextBlock.Text = await _faceRecognitionService.VerifyFaceAgainstTraindedGroup(definePersonGroupResult.Item1, stream);
            }

            catch (FaceAPIException ex)
            {
                await ShowDialog("Unfortunately error occured: " + ex.Message);
            }

        }

        private async Task CapturePhoto()
        {
            using (var captureStream = new InMemoryRandomAccessStream())
            {
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                await captureStream.FlushAsync();
                captureStream.Seek(0);

                var readStream = captureStream.AsStreamForRead();
                _takenImage = new byte[readStream.Length];
                await readStream.ReadAsync(_takenImage, 0, _takenImage.Length);

                var image = new BitmapImage();
                captureStream.Seek(0);
                await image.SetSourceAsync(captureStream);

                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    VideoCaptureImage.Source = image;
                });
            }
        }

        private async Task<Stream> GetImageStream()
        {
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));

            writer.WriteBytes(_takenImage);
            await writer.StoreAsync();
            return stream.AsStream();
        }

        private async Task ShowDialog(string text)
        {
            var messageDialog = new MessageDialog(text);

            messageDialog.Commands.Add(new UICommand("OK"));

            messageDialog.DefaultCommandIndex = 0;

            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }
    }
}
