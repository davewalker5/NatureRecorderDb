using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Environment;

namespace NatureRecorder.Interpreter.Entities
{
    public class UserSettings
    {
        private const string DefaultSettingsFileName = "naturerecorder.settings";
        private const string DefaultPrompt = ">>";

        public string Location { get; set; }
        public string Prompt { get; set; }

        [JsonIgnore]
        public string SettingsFilePath { get; set; }

        public UserSettings()
        {
            SettingsFilePath = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), DefaultSettingsFileName);
            Location = "";
            Prompt = DefaultPrompt;
        }

        public UserSettings(string settingsFilePath)
        {
            SettingsFilePath = settingsFilePath;
            Location = "";
            Prompt = DefaultPrompt;
        }

        /// <summary>
        /// Write the settings to the user settings file
        /// </summary>
        public void Save()
        {
            string settingsFile = SettingsFilePath;
            using (StreamWriter writer = new StreamWriter(settingsFile))
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                string settings = JsonSerializer.Serialize<UserSettings>(this, options);
                writer.WriteLine(settings);
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
                Prompt = (!string.IsNullOrEmpty(settings.Prompt)) ? settings.Prompt : DefaultPrompt;
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
            writer.WriteLine($"Prompt   = {Prompt ?? "Empty"}");
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
