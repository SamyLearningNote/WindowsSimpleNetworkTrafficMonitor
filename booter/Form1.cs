using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace booter
{
    public partial class Booter : Form
    {
        CommonSet commonSet = new CommonSet();

        public Booter()
        {
            InitializeComponent();
        }

        private void Booter_Load(object sender, EventArgs e)
        {
            if (!commonSet.StartProcess(commonSet.baseProgramExecutableName))
            {
                MessageBox.Show("Main program may be corrupted\nPlease download this program again", "SNTM Error");
                this.Close();
            }

            // hide the booting window
            this.ShowInTaskbar = false;
            this.Visible = false;
        }
    }
}
