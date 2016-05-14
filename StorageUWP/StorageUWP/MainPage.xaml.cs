using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace StorageUWP
{
    public sealed partial class MainPage : Page
    {
        ObservableCollection<string> _notes;
        public MainPage()
        {
            this.InitializeComponent();
            _notes = new ObservableCollection<string>();
            NotesListView.ItemsSource = _notes;
            retrieveNotes();
        }

        private void saveThemeInSettings()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["AppTheme"] = RequestedTheme.ToString();
        }

        private void setAppThemeTheme(ElementTheme requestedTheme)
        {
            RequestedTheme = requestedTheme;
            saveThemeInSettings();
        }

        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            setAppThemeTheme(ElementTheme.Light);
        }

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            setAppThemeTheme(ElementTheme.Dark);
        }

        private async void saveNotes()
        {
            /* Firstly we will use StorageFolder class from the Windows.Storage namespace
               to get path to the LocalFolder for our application: */
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            /* Then we need to have reference to the file where we can store notes:
               Note that if file exists we do not want to create another one: */
            StorageFile notesFile = await storageFolder.CreateFileAsync("notes.txt", CreationCollisionOption.OpenIfExists);

            // Now we want to serialize list with the notes to save it in the JSON format ine the file:
            var serializedNotesList = JsonConvert.SerializeObject(_notes);

            // Last step is to write serialized list with notes to the text file:
            await FileIO.WriteTextAsync(notesFile, serializedNotesList);
        }

        private async void retrieveNotes()
        {
            /* Firstly we will use StorageFolder class from the Windows.Storage namespace
               to get path to the LocalFolder for our application: */
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            /* Then we need to have reference to the file where we can store notes:
               Note that if file exists we do not want to create another one: */
            StorageFile notesFile = await storageFolder.CreateFileAsync("notes.txt", CreationCollisionOption.OpenIfExists);

            // Read serialized notes list from the file:
            string serializedNotesList = await FileIO.ReadTextAsync(notesFile);

            // Deserialize JSON list to the ObservableCollection:
            if(serializedNotesList!=null)
            {
                _notes = JsonConvert.DeserializeObject<ObservableCollection<string>>(serializedNotesList);
                NotesListView.ItemsSource = _notes;
            } 
        }

        private void addNoteToList()
        {
            _notes.Add(NoteTextBox.Text);
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            addNoteToList();
            saveNotes();
        }
    }
}
