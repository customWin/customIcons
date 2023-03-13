using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using TsudaKageyu;

namespace customIcons.Forms
{
    public partial class patchResourceIconFile : Form
    {
        public string source;
        public string destination;
        public int index;

        public patchResourceIconFile(string source, string destination, int index)
        {
            this.source = source;
            this.destination = destination;
            this.index = index;

            InitializeComponent();
            InitPatch();
        }

        private void InitPatch()
        {
            try
            {
                var sourIcon = new Icon(source);
                var destIcon = new IconExtractor(destination);

                var icons = destIcon.GetAllIcons();
                var icon = icons[index];

                pictureBox1.Image = sourIcon.ToBitmap();
                pictureBox2.Image = icon.ToBitmap();

                label3.Text = source;
                label4.Text = $"{destination} [{index}]";
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "There is an error!", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = HttpUtility.UrlDecode(GetExecutingDirectoryName().FullName);
                process.StartInfo.Arguments = $"--patch {destination} {index} {source}";
                process.StartInfo.UseShellExecute = true;

                process.Start();
            }
        }

        public static FileInfo GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath);
        }
    }
}
