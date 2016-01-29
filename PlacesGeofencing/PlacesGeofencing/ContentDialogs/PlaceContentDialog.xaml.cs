using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlacesGeofencing.ContentDialogs
{
    public sealed partial class PlaceContentDialog : ContentDialog
    {
        private string placeName;
        public string PlaceName
        {
            get
            {
                return placeName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                else
                {
                    placeName = value;
                    PlaceNameTextBlock.Text = placeName;
                }
            }
        }

        private string placeDescription;
        public string PlaceDescription
        {
            get
            {
                return placeDescription;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                else
                {
                    placeDescription = value;
                    PlaceDescriptionTextBlock.Text = placeDescription;
                }
            }
        }

        public PlaceContentDialog()
        {
            InitializeComponent();
        }

        public PlaceContentDialog(string personName, string aboutPerson)
        {
            InitializeComponent();
            setContentDialogData(personName, aboutPerson);
        }

        private void setContentDialogData(string personName, string aboutPerson)
        {
            PlaceName = personName;
            PlaceDescription = aboutPerson;
        }
    }
}
