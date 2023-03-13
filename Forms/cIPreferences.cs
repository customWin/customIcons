using System;
using System.Configuration;
using System.Windows.Forms;

namespace customIcons.Forms
{
    public partial class cIPreferences : Form
    {
        public cIPreferences()
        {
            InitializeComponent();

            textBox1.Text = (ReadSetting("path") != "") ? ReadSetting("path") : $"{Environment.GetEnvironmentVariable("appdata")}\\customIcons\\Packs";
            radioButton1.Checked = Boolean.Parse((ReadSetting("restartExplorer") != "") ? ReadSetting("restartExplorer") : "true");
            radioButton2.Checked = !Boolean.Parse((ReadSetting("restartExplorer") != "") ? ReadSetting("restartExplorer") : "true");

            AddUpdateAppSettings("path", textBox1.Text);
            AddUpdateAppSettings("restartExplorer", $"{radioButton1.Checked}");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            AddUpdateAppSettings("path", textBox1.Text);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            AddUpdateAppSettings("restartExplorer", $"{radioButton1.Checked}");
        }

        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? "";
            }
            catch (ConfigurationErrorsException) { return ""; }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException) { }
        }
    }
}
