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
    public partial class frmSelectAlbum : Form
    {
        private Form1 f1;
        public frmSelectAlbum(Form1 f)
        {
            InitializeComponent();
            f1 = f;
        }

        private String[] imgs; string[] l2; string[] l1;

        private void frmSelectAlbum_Load(object sender, EventArgs e)
        {
            try
            {
                imgs = new String[f1.albums.data.Count];
                l2 = new string[f1.albums.data.Count];
                l1 = new string[f1.albums.data.Count];
                panel.Controls.Clear();
                for (int i = 0; i < f1.albums.data.Count; i++)
                {
                    dynamic a = f1.albums.data[i];
                    dynamic a1 = f1.fb.Get("/" + a.id + "/");

                    l1[i] = a.name;
                    l2[i] = a1.description != null ? a1.description + "\n" + a1.location + "\n" + a1.count : null;
                    imgs[i] = "https://graph.facebook.com/" + a1.cover_photo + "/picture?type=thumbnail&access_token=" + f1.fb.AccessToken;

                    ctlAlbum c = new ctlAlbum(); c.name.Text = l1[i]; c.description.Text = l2[2]; c.pictureBox.ImageLocation = imgs[i];
                    c.Size = new System.Drawing.Size(panel.Width - (panel.Padding.Left + panel.Padding.Right), 60);
                    c.Location = new Point(0, 60 * i);
                    c.Index = i;
                    c.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    c.OnSelected += setSelected;
                    panel.Controls.Add(c);
                    if (a.id == f1.defaultAlbumId) setSelected((ctlAlbum)panel.Controls[i]);
                }
                
                {
                    ctlAlbum c = new ctlAlbum(); c.name.Text = "Create new album"; c.name.ForeColor = Color.Gray;
                    c.description.Text = "Select this option to create a new album."; c.description.ForeColor = Color.Gray;
                    c.pictureBox.Image = new Bitmap(Properties.Resources.appbar_add);
                    c.Size = new System.Drawing.Size(panel.Width - (panel.Padding.Left + panel.Padding.Right), 60);
                    c.Location = new Point(0, 60 * f1.albums.data.Count);
                    c.Index = f1.albums.data.Count;
                    c.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    c.OnSelected += setSelected;
                    panel.Controls.Add(c);
                }
                
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        public ctlAlbum currSel; private Color c2;
        void setSelected(ctlAlbum c)
        {
            if (currSel != null) currSel.BackColor = c2;
            if (c2 == null) c2 = currSel.BackColor;
            c.BackColor = Color.LightGray;
            currSel = c; 
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (currSel.Index == f1.albums.data.Count)
            {
                var f = new frmNewAlbum(f1.fb);
                f.ShowDialog();
                f1.getAlbums();
                frmSelectAlbum_Load(this, new EventArgs());
            }
            else
            {
                f1.defaultAlbumId = f1.albums.data[currSel.Index].id; 
                this.Hide();
            }
        }

        private void panel_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("c00");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f1.getAlbums();
            frmSelectAlbum_Load(this, new EventArgs());
        }

    }
}
