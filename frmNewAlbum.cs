using Facebook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickShare
{
    public partial class frmNewAlbum : Form
    {
        private FacebookClient fb;

        public frmNewAlbum(        FacebookClient _fb)
        {
            InitializeComponent();
            fb = _fb;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dynamic newalbum=fb.Post("me/albums", new { name = textBox1.Text, description = textBox2.Text, privacy = new { value = comboBox1.Text } });
                
                this.Close();
            }
            catch (Facebook.FacebookApiException x)
            {
                MessageBox.Show(x.Message);
            }
        }
    }
}
