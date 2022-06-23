﻿using System;
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
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }

        public int GetSeed()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetSeed(int seedling)
        {
            numericUpDown1.Value = seedling;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random _seeds = new Random();
            int plantseeds = _seeds.Next(int.MinValue, int.MaxValue);
            SetSeed(plantseeds);
        }
    }
}
