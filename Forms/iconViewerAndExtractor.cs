using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TsudaKageyu;

namespace customIcons.Forms
{
    public partial class iconViewerAndExtractor : Form
    {
        private IconExtractor iconE;
        private Icon[] icons;

        public iconViewerAndExtractor()
        {
            InitializeComponent();
            iconList1.HideSelection = false;
            changeList(@"C:\Windows\SystemResources\imageres.dll.mun");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var icon = iconE.GetIcon(iconList1.SelectedItems[0].ImageIndex);
                icons = icon.Split();

                comboBox1.Items.Clear();
                foreach (Icon i in icons)
                    comboBox1.Items.Add($"{i.Width}x{i.Height} [{IconUtil.GetBitCount(i)} bit]");

                comboBox1.SelectedIndex = 0;
                comboBox1_SelectedIndexChanged(sender, e);
            }
            catch (Exception) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var icon = iconE.GetIcon(iconList1.SelectedItems[0].ImageIndex);
            icons = icon.Split();

            var selicon = icons.Where(x => (IconUtil.GetBitCount(x) == IconUtil.GetBitCount(icon))).ToArray();

            saveFileDialog1.Filter = "Photo file (*.png)|*.png";
            saveFileDialog1.DefaultExt = "png";
            var fd = saveFileDialog1.ShowDialog();
            if (fd == DialogResult.OK)
            {
                selicon[0].ToBitmap().Save(saveFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var icon = iconE.GetIcon(iconList1.SelectedItems[0].ImageIndex);

            saveFileDialog1.Filter = "Icon file (*.ico)|*.ico";
            saveFileDialog1.DefaultExt = "ico";
            var fd = saveFileDialog1.ShowDialog();
            if (fd == DialogResult.OK)
            {
                using (FileStream fileStream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate))
                {
                    icon.Save(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var icon = iconE.GetIcon(iconList1.SelectedItems[0].ImageIndex);
            icons = icon.Split();

            var selicon = icons.Where(x => (IconUtil.GetBitCount(x) == IconUtil.GetBitCount(icon))).ToArray();

            saveFileDialog1.Filter = "Photo file (*.png)|*.png";
            saveFileDialog1.DefaultExt = "png";
            Clipboard.SetData(DataFormats.Bitmap, selicon[0].ToBitmap());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = icons[comboBox1.SelectedIndex].ToBitmap();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            var fd = openFileDialog1.ShowDialog();
            if (fd == DialogResult.OK)
            {
                iconList1.Clear();

                textBox1.Text = openFileDialog1.FileName;
                changeList(openFileDialog1.FileName);
                iconList1.Select();
                listView1_SelectedIndexChanged(sender, e);
            }
        }

        private void changeList(string path)
        {
            try
            {
                iconE = new IconExtractor(path);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "There is an error!", MessageBoxButtons.OK);
            }
            var icons = iconE.GetAllIcons();

            var list = new ImageList();
            list.ColorDepth = ColorDepth.Depth32Bit;
            list.ImageSize = new Size(32, 32);
            list.Images.AddRange(icons.Select(x => x.ToBitmap()).ToArray());

            iconList1.LargeImageList = list;

            int ic = 0;
            foreach (Icon i in icons)
            {
                iconList1.Items.Add($"#{ic}", ic);
                ic++;
            }

            textBox1.Text = path;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("It looks like you've pressed the secret button! Do you want to proceed?", "Hey! This is a easter egg.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(Encoding.UTF8.GetString(Convert.FromBase64String("aHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQ==")));
            }
        }
    }
}
