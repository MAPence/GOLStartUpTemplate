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
    public partial class UniverseResize : Form
    {
        public UniverseResize()
        {
            InitializeComponent();
        }

        public int GetWidth()
        {
            return (int)numericUpDown1.Value;
        }
        public int GetHeight()
        {
            return (int)numericUpDown2.Value;
        }
        public void SetWidth(int widen)
        {
            numericUpDown1.Value = widen;
        }
        public void SetHeight(int tallman)
        {
            numericUpDown2.Value = tallman;
        }
        public int GetTimer()
        {
            return (int)numericUpDown3.Value;
        }
        public void SetTimer(int timezone)
        {
            numericUpDown3.Value = timezone;
        }

    }
}
