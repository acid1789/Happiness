using System;
using System.Collections.Generic;
using System.Linq;

namespace Happiness
{
    class Settings
    {
        const string SettingsFile = "Happiness.settings";

        public float SoundVolume;
        public float MusicVolume;
        public bool ExpSlowdown;
        public bool ErrorDetector;
        public bool ErrorDetector2;
        public bool DisableTimer;

        public static Settings LoadSettings()
        {
            Settings s = new Settings();
            s.SoundVolume = SoundManager.Inst.SoundVolume;
            s.MusicVolume = SoundManager.Inst.MusicVolume;
            
            string settingsFile = GetSettingsFileName();
            if (FileManager.Instance.Exists(settingsFile))
            {
                string[] lines = FileManager.Instance.ReadAllLines(settingsFile);
                Dictionary<string, string> settings = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    if (line.Contains("="))
                    {
                        string[] pieces = line.Split('=');
                        if( pieces.Length == 2 )
                            settings[pieces[0]] = pieces[1];
                    }
                }

                if( settings.ContainsKey("SoundVolume") )
                    float.TryParse(settings["SoundVolume"], out s.SoundVolume);
                if( settings.ContainsKey("MusicVolume") )
                    float.TryParse(settings["MusicVolume"], out s.MusicVolume);
                if( settings.ContainsKey("ExpSlowdown") )
                    bool.TryParse(settings["ExpSlowdown"], out s.ExpSlowdown);
                if (settings.ContainsKey("ErrorDetector"))
                    bool.TryParse(settings["ErrorDetector"], out s.ErrorDetector);
                if (settings.ContainsKey("ErrorDetector2"))
                    bool.TryParse(settings["ErrorDetector2"], out s.ErrorDetector2);
                if (settings.ContainsKey("DisableTimer"))
                    bool.TryParse(settings["DisableTimer"], out s.DisableTimer);
            }
            return s;
        }

        public void Save()
        {
            List<string> settings = new List<string>();
            settings.Add("SoundVolume=" + SoundVolume);
            settings.Add("MusicVolume=" + MusicVolume);
            settings.Add("ExpSlowdown=" + ExpSlowdown);
            settings.Add("ErrorDetector=" + ErrorDetector);
            settings.Add("ErrorDetector2=" + ErrorDetector2);
            settings.Add("DisableTimer=" + DisableTimer);

            string settingsFile = GetSettingsFileName();
            FileManager.Instance.WriteAllLines(settingsFile, settings.ToArray());            
        }

        static string GetSettingsFileName()
        {            
            string localHappinessDir = FileManager.Instance.HappinessPath;
            FileManager.Instance.CreateDirectory(localHappinessDir);
            return localHappinessDir + SettingsFile;
        }
    }
}
