using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EduFocus
{
    public partial class frmSplash : Form
    {
        int move = 0;
        int i = 0;
        public frmSplash()
        {
            InitializeComponent();
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            
            tmrSplash.Start();

        }
        
        private void tmrSplash_Tick(object sender, EventArgs e)
        {
            panelSlide.Width += 1;
            if(panelSlide.Width > 300)
            {
                panelSlide.Width = 0;
            }
            if(panelSlide.Width < 0)
            {
                move = 2;
               
            }
            i++;
            if (i == 300)
            {
                frmLogin Login = new frmLogin(this);
                Login.Show();
                this.Hide();
                tmrSplash.Enabled = false;
            }
            else
                return;

        }

    }
}
