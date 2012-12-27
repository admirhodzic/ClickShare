using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;

namespace ClickShare
{
    public partial class Form1 : Form
    {
        private string[] Args;
        public Form1(string[] _Args)
        {
            Args = _Args;
            InitializeComponent();
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public dynamic albums;
        Properties.Settings s = Properties.Settings.Default;
        public string defaultAlbumId;
        public FacebookClient fb;

        private void Form1_Load(object sender, EventArgs e)
        {
            var accesstoken = s.accesstoken;
            
            if (Args.Length == 0) { MessageBox.Show("Error: No picture(s) specified on open!"); this.Close(); return; }

            if(!(((string)s["accesstoken"])!="" && (long)s["accesstoken_expires"]>DateTime.Now.Ticks)){
                var fb1 = new FacebookClient { };
                Uri loginUrl = fb1.GetLoginUrl(new { client_id = "180571752085804", response_type = "token", display = "popup", scope = "user_photos,publish_stream,offline_access", redirect_uri = "https://www.facebook.com/connect/login_success.html" });
                var b = new browser(loginUrl);
                b.ShowDialog();
                if (b.result.AccessToken == null) this.Close();
                accesstoken = b.result.AccessToken;
                s["accesstoken"] = accesstoken;
                s["accesstoken_expires"] = b.result.Expires.Ticks;
                s.Save();
            }
            fb = new FacebookClient(accesstoken.ToString());
            fb.PostCompleted += fb_PostCompleted;
            fb.UploadProgressChanged += fb_UploadProgressChanged;

            defaultAlbumId=(string)s["selAlbum"];

            getAlbums();
            frmSelectAlbum f = new frmSelectAlbum(this);
            f.ShowDialog();
            idxAlbum = f.currSel.Index;
            msgs=new string[Args.Length];
            setImage(0);
            textBox1_Leave(textBox1,new EventArgs());
        }

        private int curr_n=-1;
        private void setImage(int n)
        {
            if (curr_n != -1) { msgs[curr_n] = textBox1.Text; }
            curr_n = n;
            pictureBox2.Image=Image.FromFile(Args[n]);
            textBox1.Text=msgs[n];
            if (curr_n == Args.Length - 1) button2.Text = "&Finish"; else button2.Text = "&Next";
            this.Text = "Click and Share to Facebook - " + (curr_n+1).ToString() + " of " + Args.Length.ToString();
        }

        public  void getAlbums()
        {
            try
            {
                albums = fb.Get("me/albums");
            }
            catch (Facebook.FacebookApiException x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void fb_UploadProgressChanged(object sender, FacebookUploadProgressChangedEventArgs e)
        {
            BeginInvoke(new MethodInvoker(() => { var totalBytesToSend = e.TotalBytesToSend; var bytesSent = e.BytesSent; var state = e.UserState; toolStripProgressBar1.Value = e.ProgressPercentage; }));
            //toolStripProgressBar1.Value = e.ProgressPercentage; 
        }

        private void fb_PostCompleted(object sender, FacebookApiEventArgs e)
        {
            if (e.Cancelled)    {        
                var cancellationError = e.Error;        
                MessageBox.Show("Upload cancelled");    
            }    
            else if (e.Error == null)    
            {
                // upload successful.        

                BeginInvoke(new MethodInvoker(() => { MessageBox.Show("Upload successful!"); this.Close(); }));
            }    
            else    
            {        // upload failed        
                MessageBox.Show(e.Error.Message);    
            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                s["selAlbum"] = this.defaultAlbumId;
                s.Save();
                fb.CancelAsync();
            }
            catch (Exception x)
            {
            }

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private string[] msgs;
        public int idxAlbum = 0;
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (button2.Text == "Next"){
                if (curr_n < Args.Length - 1) { 
                    setImage(curr_n + 1); 
                    return; 
                }
                
            }


            if (button2.Text == "Cancel")
            {
                fb.CancelAsync();
                return;
            }

            button2.Text = "Cancel";
            msgs[curr_n] = textBox1.Text;

            try
            {
                //foreach (string fn in Args)
                for(int i=0;i<Args.Length;i++)
                {

                    string attachementPath = Args[i];
                    dynamic result = fb.PostTaskAsync(albums.data[idxAlbum].id + "/photos",
                    new
                    {
                        message = msgs[i],
                        file = new FacebookMediaObject
                        {
                            ContentType = GetMimeType(attachementPath),
                            FileName = Path.GetFileName(attachementPath)

                        }.SetValue(File.ReadAllBytes(attachementPath))

                    });
                }
            }
            catch (Facebook.FacebookApiException x)
            {
                MessageBox.Show(x.Message);
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
        }

    }
}
