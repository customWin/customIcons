using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using customIcons.Types;
using Newtonsoft.Json;

namespace customIcons.Forms
{
    public partial class iconPackStore : Form
    {
        public iconPackStore()
        {
            InitializeComponent();
            loadIconStore();
        }

        public void loadIconStore()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://raw.githubusercontent.com/customIcon/cIPacks/main/cIPackStore.json");
                var list = JsonConvert.DeserializeObject<List<IconStorePack>>(json);

                foreach (IconStorePack pack in list)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = pack.name;
                    item.Tag = pack;

                    listView1.Items.Add(item);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pack = (IconStorePack)listView1.SelectedItems[0].Tag;
            label1.Text = pack.name;
            label2.Text = $@"by {pack.author}";
            if (pack.author != pack.icon_author) label3.Text = $@"Icon Author: {pack.icon_author}";
            desc.Text = pack.description;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pack = (IconStorePack)listView1.SelectedItems[0].Tag;
            using (var client = new WebClient())
            {
                client.DownloadFile($"https://raw.githubusercontent.com/customIcon/cIPacks/main/packs/{pack.filename}", $"{Environment.GetEnvironmentVariable("temp")}\\{pack.filename}");

                using (var zip = ZipFile.Open($"{Environment.GetEnvironmentVariable("temp")}\\{pack.filename}", ZipArchiveMode.Read))
                {
                    if (File.Exists($"{Environment.GetEnvironmentVariable("temp")}\\tempcIManifest.json"))
                        File.Delete($"{Environment.GetEnvironmentVariable("temp")}\\tempcIManifest.json");

                    foreach (var entry in zip.Entries)
                        if (entry.Name == "cIManifest.json")
                            entry.ExtractToFile(
                                $"{Environment.GetEnvironmentVariable("temp")}\\tempcIManifest.json");

                    if (File.Exists($"{Environment.GetEnvironmentVariable("temp")}\\tempcIManifest.json"))
                        using (var r = new StreamReader(
                                   $"{Environment.GetEnvironmentVariable("temp")}\\tempcIManifest.json"))
                        {
                            var manifestFile = r.ReadToEnd();
                            var manifest = JsonConvert.DeserializeObject<customIconsManifest>(manifestFile);

                            var d = MessageBox.Show(
                                $"Do you want to import this customIcons Pack?\r\n\r\nPack Name: {manifest.name}\r\nPack Author: {manifest.maker}",
                                "customIconPack Importer 3000", MessageBoxButtons.YesNo);
                            if (d == DialogResult.Yes)
                            {
                                zip.ExtractToDirectory(cIPreferences.ReadSetting("path"));
                                MessageBox.Show($"{manifest.name} by {manifest.maker} is now imported!");
                            }
                        }
                }
            }
        }
    }
}
