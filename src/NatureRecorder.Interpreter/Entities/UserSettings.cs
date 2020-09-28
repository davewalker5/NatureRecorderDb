using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Environment;

namespace NatureRecorder.Interpreter.Entities
{
    public class UserSettings
    {
        private const string DefaultSettingsFileName = "naturerecorder.settings";

        public string Location { get; set; }

        [JsonIgnore]
        public string SettingsFilePath { get; set; }

        public UserSettings()
        {
            SettingsFilePath = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), DefaultSettingsFileName);
            Location = "";
        }

        public UserSettings(string settingsFilePath)
        {
            SettingsFilePath = settingsFilePath;
            Location = "";
        }

        /// <summary>
        /// Write the settings to the user settings file
        /// </summary>
        public void Save()
        {
            string settingsFile = SettingsFilePath;
            using (StreamWriter writer = new StreamWriter(settingsFile))
            {
                JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
                writer.WriteLine(JsonSerializer.Serialize<UserSettings>(this, options));
            }
        }

        /// <summary>
        /// Load the settings from the settings file
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            string settingsFile = SettingsFilePath;
            if (File.Exists(settingsFile))
            {
                // Load an instance of user settings from the JSON settings file
                string json = File.ReadAllText(settingsFile);
                UserSettings settings = JsonSerializer.Deserialize<UserSettings>(json);

                // And copy the properties
                Location = settings.Location;
            }
        }

        /// <summary>
        /// List the current settings
        /// </summary>
        /// <param name="writer"></param>
        public void List(StreamWriter writer)
        {
            writer.WriteLine($"File     = {SettingsFilePath ?? "Empty"}");
            writer.WriteLine($"Location = {Location ?? "Empty"}");
            writer.Flush();
        }

        /// <summary>
        /// Clear the current settings
        /// </summary>
        public void Clear()
        {
            Location = "";
        }
    }
}
