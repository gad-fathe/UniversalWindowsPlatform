using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsUISampleApp.DataSource
{
    public class Thumbnail
    {
        public Thumbnail()
        {

        }

        public Thumbnail(string name, string url, string description)
        {
            Name = name;
            ImageUrl = url;
            Description = description;
        }

        public string Name
        {
            get; set;
        }

        public string ImageUrl
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }

    public class LocalDataSource
    {
        public LocalDataSource()
        {
            Items = new ObservableCollection<Thumbnail>();
            Items.Add(new Thumbnail("Sample 1", PREFIX_URL + "sample1.png", "24mm f/4.0 1/2500 ISO 200"));
            Items.Add(new Thumbnail("Sample 2", PREFIX_URL + "sample2.png", "24mm f/8.0 1/2000 ISO 100"));
            Items.Add(new Thumbnail("Sample 3", PREFIX_URL + "sample3.png", "24mm f/8.0 1/640 ISO 100"));
            Items.Add(new Thumbnail("Sample 4", PREFIX_URL + "sample4.png", "70mm f/2.8 1/8000 ISO 400"));
            Items.Add(new Thumbnail("Sample 5", PREFIX_URL + "sample5.png", "70mm f/2.8 1/5000 ISO 400"));
        }

        public ObservableCollection<Thumbnail> Items
        {
            get; set;
        }

        private static readonly string PREFIX_URL = "ms-appx:///Assets/";
    }
}
