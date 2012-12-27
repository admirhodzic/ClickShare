using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ClickShare
{
    public partial class ctlAlbum : UserControl
    {
        
        public ctlAlbum()
        {
            InitializeComponent();
        }

        public delegate void SelectedEvent(ctlAlbum sender);
        public event SelectedEvent OnSelected;
        public int Index;
        private void name_Click(object sender, EventArgs e)
        {
            if (OnSelected != null) OnSelected(this);
        }

        private void ctlAlbum_Paint(object sender, PaintEventArgs e)
        {
            
            e.Graphics.Clear(this.BackColor);
            var r = this.ClientRectangle; r.Inflate(0, -1);
            e.Graphics.DrawLine(Pens.LightGray , new Point(0,r.Height),new Point(r.Width,r.Height));
        }
    }
}
