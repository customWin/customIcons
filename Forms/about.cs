using System.Diagnostics;
using System.Windows.Forms;

namespace customIcons.Forms
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.elevenforum.com/t/custom-icons-for-windows-11-thread-folders-dropbox-google-drive-podcasts-nvme-drive-steam-adobe.448/");
        }
    }
}
