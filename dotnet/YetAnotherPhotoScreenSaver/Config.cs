using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;

namespace Org.Kuhn.Yapss {
    public class Config {
        public Config() {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(KEY);
            if (reg == null)
                return;
            xCount = (int)reg.GetValue("xCount", xCount);
            longInterval = (int)reg.GetValue("longInterval", longInterval);
            shortInterval = (int)reg.GetValue("shortInterval", shortInterval);
            isEnabledFileImageSource = (int)reg.GetValue("isEnabledFileImageSource", isEnabledFileImageSource) == 1;
            fileImageSourcePath = (string)reg.GetValue("fileImageSourcePath", fileImageSourcePath);
            isEnabledFlickrImageSource = (int)reg.GetValue("isEnabledFlickrImageSource", isEnabledFlickrImageSource) == 1;
            flickrImageSourceTags = (string)reg.GetValue("flickrImageSourceTags", flickrImageSourceTags);
            isFlickrImageSourceTagAndLogic = (int)reg.GetValue("isFlickrImageSourceTagAndLogic", isFlickrImageSourceTagAndLogic) == 1;
            flickrImageSourceUserName = (string)reg.GetValue("flickrImageSourceUserName", flickrImageSourceUserName);
            flickrImageSourceText = (string)reg.GetValue("flickrImageSourceText", flickrImageSourceText);
            theme = (Theme)Enum.Parse(typeof(Theme), (string)reg.GetValue("theme", Enum.GetName(typeof(Theme), theme)));
            isLoggingEnabled = (int)reg.GetValue("isLoggingEnabled", isLoggingEnabled) == 1;
            reg.Close();
        }
        public void Save() {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(KEY);
            reg.SetValue("xCount", xCount, RegistryValueKind.DWord);
            reg.SetValue("longInterval", longInterval,RegistryValueKind.DWord);
            reg.SetValue("shortInterval", shortInterval, RegistryValueKind.DWord);
            reg.SetValue("isEnabledFileImageSource", isEnabledFileImageSource, RegistryValueKind.DWord);
            reg.SetValue("fileImageSourcePath", fileImageSourcePath, RegistryValueKind.String);
            reg.SetValue("isEnabledFlickrImageSource", isEnabledFlickrImageSource, RegistryValueKind.DWord);
            reg.SetValue("flickrImageSourceTags", flickrImageSourceTags, RegistryValueKind.String);
            reg.SetValue("isFlickrImageSourceTagAndLogic", isFlickrImageSourceTagAndLogic, RegistryValueKind.DWord);
            reg.SetValue("flickrImageSourceUserName", flickrImageSourceUserName, RegistryValueKind.String);
            reg.SetValue("flickrImageSourceText", flickrImageSourceText, RegistryValueKind.String);
            reg.SetValue("theme", Enum.GetName(typeof(Theme), theme), RegistryValueKind.String);
            reg.SetValue("isLoggingEnabled", isLoggingEnabled, RegistryValueKind.DWord);
            reg.Close();
        }
        public int XCount {
            get { return xCount; }
            set { xCount = value; }
        }
        public int LongInterval {
            get { return longInterval; }
            set { longInterval = value; }
        }
        public int ShortInterval {
            get { return shortInterval; }
            set { shortInterval = value; }
        }
        public bool IsEnabledFileImageSource {
            get { return isEnabledFileImageSource; }
            set { isEnabledFileImageSource = value; }
        }
        public string FileImageSourcePath {
            get { return fileImageSourcePath; }
            set { fileImageSourcePath = value; }
        }
        public bool IsEnabledFlickrImageSource {
            get { return isEnabledFlickrImageSource; }
            set { isEnabledFlickrImageSource = value; }
        }
        public string FlickrImageSourceTags {
            get { return flickrImageSourceTags; }
            set { flickrImageSourceTags = value; }
        }
        public bool IsFlickrImageSourceTagAndLogic {
            get { return isFlickrImageSourceTagAndLogic; }
            set { isFlickrImageSourceTagAndLogic = value; }
        }
        public string FlickrImageSourceUserName {
            get { return flickrImageSourceUserName; }
            set { flickrImageSourceUserName = value; }
        }
        public string FlickrImageSourceText {
            get { return flickrImageSourceText; }
            set { flickrImageSourceText = value; }
        }
        public Theme Theme {
            get { return theme; }
            set { theme = value; }
        }
        public bool IsLoggingEnabled {
            get { return isLoggingEnabled; }
            set { isLoggingEnabled = value; }
        }

        private int xCount = 9;
        private int longInterval = 5000;
        private int shortInterval = 1000;
        private bool isEnabledFileImageSource = false;
        private string fileImageSourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private bool isEnabledFlickrImageSource = true;
        private string flickrImageSourceTags = "";
        private bool isFlickrImageSourceTagAndLogic = false;
        private string flickrImageSourceUserName = "";
        private string flickrImageSourceText = "";
        private Theme theme = Theme.Dark;
        private bool isLoggingEnabled = false;

        private static readonly string KEY = "Software\\YetAnotherPhotoScreenSaver";  
    }

    public enum Theme {
        Light,
        Dark,
        Random
    } 
}
