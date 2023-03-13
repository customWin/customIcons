using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using customIcons.Exceptions;
using customIcons.Extensions;
using customIcons.Types;
using Newtonsoft.Json;
using TsudaKageyu;

namespace customIcons.Forms
{
    public partial class customIcons_Form1 : Form
    {
        public static string iconPath;
        public static List<string> imageL = new List<string>();
        public static Dictionary<string, string> imageKey = new Dictionary<string, string>();

        private Icon[] icons;

        public customIcons_Form1()
        {
            if (ReadSetting("path") == "")
            {
                AddUpdateAppSettings("path", $"{Environment.GetEnvironmentVariable("appdata")}\\customIcons\\Packs");

                try
                {
                    var nicon = new NotifyIcon();
                    nicon.ShowBalloonTip(1000, "customIcons", "Getting ready for first use...", ToolTipIcon.None);

                    var icone = new IconExtractor($"{Environment.GetEnvironmentVariable("systemroot")}\\SystemResources\\imageres.dll.mun");
                    var icons = icone.GetAllIcons();

                    if (!Directory.Exists(ReadSetting("path") + "\\cI.imageres.dll"))
                        Directory.CreateDirectory(ReadSetting("path") + "\\cI.imageres.dll");

                    customIconsManifest manifest = new customIconsManifest
                    {
                        name = "ImageRes.DLL",
                        maker = "Microsoft"
                    };
                    var jsonstr = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                    using (StreamWriter streamWriter =
                           File.AppendText(ReadSetting("path") + "\\cI.imageres.dll\\cIManifest.json"))
                    {
                        streamWriter.WriteLine(jsonstr);
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }

                    int i = 0;
                    foreach (var icon in icons)
                    {
                        using (FileStream fileStream = new FileStream(ReadSetting("path") + $"\\cI.imageres.dll\\ImageRes.DLL [{i}].ico", FileMode.OpenOrCreate))
                        {
                            icon.Save(fileStream);
                            fileStream.Close();
                            fileStream.Dispose();
                        }

                        i++;
                    }
                }
                catch (Exception e) { }
            }

            InitializeComponent();
            loopAndAddFolders(ReadSetting("path"));

            if (!Directory.Exists(ReadSetting("path")))
                Directory.CreateDirectory(ReadSetting("path"));
        }

        private string ReadSetting(string key)
        {
            return cIPreferences.ReadSetting(key);
        }

        private void AddUpdateAppSettings(string key, string value)
        {
            cIPreferences.AddUpdateAppSettings(key, value);
        }

        private void loopAndAddFolders(string path)
        {
            label2.Visible = true;
            pictureBox1.Visible = false;

            var folders = Directory.GetDirectories(path, "cI.*", SearchOption.TopDirectoryOnly);
            var imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            listView1.Clear();
            imageL.Clear();
            imageKey.Clear();
            var i = 0;
            foreach (var folder in folders)
                try
                {
                    using (var r = new StreamReader($@"{folder}\cIManifest.json"))
                    {
                        var manifestFile = r.ReadToEnd();
                        var manifest = JsonConvert.DeserializeObject<customIconsManifest>(manifestFile);

                        var files = Directory.GetFiles(folder, "*.ico", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            imageKey.Add(file, $"{file}~{manifest.name}~{manifest.maker}~{folder}");
                            imageL.Add(file);
                            imageList.Images.Add(new Icon(file, 32, 32));
                            var item = new ListViewItem();
                            item.Text = file.Replace(".ico", "").Replace(folder, "").Replace("\\", "");
                            item.ToolTipText = $"{folder}";
                            item.Tag = $"{manifest.name}~{manifest.maker}";
                            listView1.Items.Add(item);
                        }
                    }

                    i++;
                    Console.WriteLine(i);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"Cannot read {folder}, there is no manifest.");
                }

            listView1.LargeImageList = imageList;

            var count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                item.ImageIndex = count;
                count++;
            }
            //if (count <= 0) label3.Visible = true; listView1.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = icons[comboBox1.SelectedIndex].ToBitmap();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                label2.Visible = false;
                pictureBox1.Visible = true;

                icons = new Icon($@"{listView1.SelectedItems[0].ToolTipText}\{listView1.SelectedItems[0].Text}.ico", -1,
                    -1).Split();
                comboBox1.Items.Clear();
                foreach (var icon in icons)
                    comboBox1.Items.Add($"{icon.Width}x{icon.Height} [{icon.GetBitCount()} bit]");
                var man = listView1.SelectedItems[0].Tag.ToString().Split('~');
                label1.Text = $"Name   : {listView1.SelectedItems[0].Text}\r\ncI Pack: {man[0]}\r\nMaker  : {man[1]}";
                iconPath = $@"{listView1.SelectedItems[0].ToolTipText}\{listView1.SelectedItems[0].Text}.ico";

                comboBox1.SelectedIndex = 0;
                comboBox1_SelectedIndexChanged(sender, e);
            }
            catch (Exception)
            {
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var i = new iconViewerAndExtractor();
            i.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripTextBox1_TextChanged(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                var list = new ImageList();
                list.ImageSize = new Size(32, 32);
                list.ColorDepth = ColorDepth.Depth32Bit;
                var imgKey = new List<string>();
                foreach (var image in imageL)
                {
                    var tags = imageKey[image].Split('~');
                    if (tags[0].Contains(toolStripTextBox1.Text, StringComparison.CurrentCultureIgnoreCase) ||
                        tags[1].Contains(toolStripTextBox1.Text, StringComparison.CurrentCultureIgnoreCase) ||
                        tags[2].Contains(toolStripTextBox1.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        list.Images.Add(new Icon(image, 32, 32));
                        imgKey.Add(imageKey[image]);
                    }
                }

                listView1.Clear();
                listView1.LargeImageList = list;
                var i = 0;
                foreach (Image image in list.Images)
                {
                    var tags = imgKey[i].Split('~');
                    var item = new ListViewItem();
                    item.Text = tags[0].Replace(tags[3], "").Replace("\\", "").Replace(".ico", "");
                    item.ToolTipText = tags[3];
                    item.Tag = $"{tags[1]}~{tags[2]}";
                    item.ImageIndex = i;
                    i++;
                    listView1.Items.Add(item);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fd = folderBrowserDialog1.ShowDialog();
            if (fd == DialogResult.OK)
            {
                var path = folderBrowserDialog1.SelectedPath;
                var d = new DirectoryInfo(path);
                if (d.Parent != null)
                {
                    path += "\\desktop.ini";

                    if (!File.Exists(path))
                    {
                        using (var sw = File.CreateText(path))
                        {
                            sw.WriteLine("[.ShellClassInfo]");
                            sw.WriteLine(
                                $"IconFile={listView1.SelectedItems[0].ToolTipText}\\{listView1.SelectedItems[0].Text}.ico");
                            sw.WriteLine("IconIndex=0");
                        }
                    }
                    else
                    {
                        File.Delete(path);
                        using (var sw = File.CreateText(path))
                        {
                            sw.WriteLine("[.ShellClassInfo]");
                            sw.WriteLine(
                                $"IconFile={listView1.SelectedItems[0].ToolTipText}\\{listView1.SelectedItems[0].Text}.ico");
                            sw.WriteLine("IconIndex=0");
                        }
                    }

                    File.SetAttributes(path, FileAttributes.System | FileAttributes.Hidden | FileAttributes.ReadOnly);
                    File.SetAttributes(path.Replace("\\desktop.ini", ""), FileAttributes.ReadOnly);

                    if (Boolean.Parse((ReadSetting("restartExplorer") != "") ? ReadSetting("restartExplorer") : "true"))
                    {
                        Process.Start(@"taskkill.exe", "/f /im explorer.exe");
                        Thread.Sleep(2500);
                        Process.Start($@"{Environment.GetEnvironmentVariable("systemroot")}\sysnative\cmd.exe",
                            "/c start explorer.exe & exit");
                    }
                }
                else
                {
                    if (!File.Exists(Environment.GetEnvironmentVariable("appdata") + "\\customIcons\\driveIconPatcher"))
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(
                                    "https://github.com/customIcon/driveIconPatcher/releases/latest/download/driveIconPatcherPortable.zip",
                                    Environment.GetEnvironmentVariable("appdata") +
                                    "\\customIcons\\driveIconPatcher.zip");
                            }

                            using (var zip = ZipFile.OpenRead(Environment.GetEnvironmentVariable("appdata") +
                                                              "\\customIcons\\driveIconPatcher.zip"))
                            {
                                zip.ExtractToDirectory(Environment.GetEnvironmentVariable("appdata") +
                                                       "\\customIcons\\driveIconPatcher");
                            }

                            File.Delete(Environment.GetEnvironmentVariable("appdata") +
                                        "\\customIcons\\driveIconPatcher.zip");
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Process.Start(Environment.GetEnvironmentVariable("appdata") + "\\customIcons\\driveIconPatcher\\driveIconPatcher.exe",
                        $"{folderBrowserDialog1.SelectedPath} {listView1.SelectedItems[0].ToolTipText}\\{listView1.SelectedItems[0].Text}.ico");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void aboutCustomIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new about();
            about.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "This will show you Windows' built-in Desktop Icons Control Panel settings, because registry edit is still ongoing. The path of your selected icon will be copied to the clipboard.\r\n\r\nDo you want to continue?",
                    "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Clipboard.SetText($"{listView1.SelectedItems[0].ToolTipText}\\{listView1.SelectedItems[0].Text}.ico");
                Process.Start("Rundll32.exe", "shell32.dll,Control_RunDLL desk.cpl,,0");
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
        }

        private void importCustomIconsPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "customIcons Pack file (*.cIPack)|*.cIPack|Compressed file (*.zip)|*.zip";
            openFileDialog1.DefaultExt = "zip";

            var fd = openFileDialog1.ShowDialog();
            if (fd == DialogResult.OK)
                try
                {
                    if (openFileDialog1.FileName.Contains(".cIPack"))
                    {
                        if (File.Exists($"{Environment.GetEnvironmentVariable("temp")}\\tempcI.zip"))
                            File.Delete($"{Environment.GetEnvironmentVariable("temp")}\\tempcI.zip");

                        File.Copy(openFileDialog1.FileName,
                            $"{Environment.GetEnvironmentVariable("temp")}\\tempcI.zip");
                        openFileDialog1.FileName = $"{Environment.GetEnvironmentVariable("temp")}\\tempcI.zip";
                    }

                    using (var zip = ZipFile.Open(openFileDialog1.FileName, ZipArchiveMode.Read))
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
                                    zip.ExtractToDirectory(ReadSetting("path"));
                                    MessageBox.Show($"{manifest.name} by {manifest.maker} is now imported!");
                                    loopAndAddFolders(ReadSetting("path"));
                                }
                            }
                        else
                            throw new customIconPackException(
                                $"[{openFileDialog1.FileName}\\cIManifest.json] - The system could not find the file specified.");
                    }
                }
                catch (Exception s)
                {
                    throw new customIconPackException(s.Message);
                }
        }

        public static string ToSafeFileName(string s)
        {
            return s
                .Replace("\\", "_")
                .Replace("/", "_")
                .Replace("\"", "_")
                .Replace("*", "_")
                .Replace(":", "_")
                .Replace("?", "_")
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace("|", "_");
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pref = new cIPreferences();
            pref.ShowDialog();

            loopAndAddFolders(ReadSetting("path"));
        }
    }
}