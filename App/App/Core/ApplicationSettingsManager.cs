using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace App.Core
{
    public class ApplicationSettingsManager
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(ApplicationSettings));

        public string SettingsFilePath { get; }
        public static ApplicationSettingsManager Instance { get; private set; } = new ApplicationSettingsManager();

        public ApplicationSettings Settings { get; private set; }

        public Exception? LastException { get; private set; }

        public event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        public ApplicationSettingsManager()
        {
            SettingsFilePath = Path.Combine(App.ApplicationDataStoragePath, "Settings.sxf");
            InitializeSettings();
        }

        public void InitializeSettings()
        {
            if (!File.Exists(SettingsFilePath))
                CreateSettings();

            if (!ValidateSettings())
                RecreateSettings();

            LoadSettings();
        }

        public void CreateSettings()
        {
            Settings = new ApplicationSettings();
            SerializeSettings();
        }

        public bool ValidateSettings()
        {
            try
            {
                using (FileStream settingsFileStream = File.OpenRead(SettingsFilePath))
                {
                    using (XmlReader reader = XmlReader.Create(settingsFileStream))
                        return serializer.CanDeserialize(reader);
                }
            }
            catch (Exception e)
            {
                LastException = e; return false;
            }
        }

        public void RecreateSettings()
        {
            File.Copy(SettingsFilePath, Path.ChangeExtension(SettingsFilePath, "sxf.bak"), true);
            File.Delete(SettingsFilePath);
            CreateSettings();
        }

        public void LoadSettings()
        {
            ApplicationSettings? settings = DeserializeSettings();
            Settings = settings;
            if (settings is null)
                Settings = new ApplicationSettings();
        }

        private ApplicationSettings? DeserializeSettings()
        {
            try
            {
                using (FileStream settingsFileStream = File.OpenRead(SettingsFilePath))
                {
                    return serializer.Deserialize(settingsFileStream) as ApplicationSettings;
                }
            }
            catch (Exception e)
            {
                LastException = e;
                return null;
            }
        }

        private void SerializeSettings()
        {
            try
            {
                using (FileStream settingsFileStream = File.Open(SettingsFilePath, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(settingsFileStream, Settings);
                }
                SettingsSaved?.Invoke(this, new SettingsSavedEventArgs(null));
            }
            catch (Exception e)
            {
                SettingsSaved?.Invoke(this, new SettingsSavedEventArgs(e));
            }
        }
    }

    public class SettingsSavedEventArgs : EventArgs
    {
        public Exception? LastException { get; }

        public SettingsSavedEventArgs(Exception? lastException)
        {
            LastException = lastException;
        }
    }
}
