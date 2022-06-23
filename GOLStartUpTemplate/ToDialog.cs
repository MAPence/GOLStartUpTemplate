using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate
{
    public partial class ToDialog : Form
    {
        public ToDialog()
        {
            InitializeComponent();
        }
        public int GenerationTo 
        { 
            get { return (int)numericUpDown1.Value; } 
            set { numericUpDown1.Value = value; } 
        }
        
    }
}
