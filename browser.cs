using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;

namespace ClickShare
{
    public partial class browser : Form
    {
        public FacebookOAuthResult result; 

        public browser(Uri url)
        {
            InitializeComponent();
            wb.Navigate(url);
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void wb_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
			
            var fb=new FacebookClient();
            if (fb.TryParseOAuthCallbackUrl(e.Url, out result)) {
                if (result.IsSuccess) { 
                    this.Hide();
                } else { 
                    var errorDescription = result.ErrorDescription; var errorReason = result.ErrorReason; 
                } 
            } 
		}
    }
}
