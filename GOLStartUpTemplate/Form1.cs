using System;
using System.IO;
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

    public partial class Form1 : Form
    {
        static int beanstalk = Properties.Settings.Default.giant;
        static int wideload = Properties.Settings.Default.doublewide;
        static int livenCells = 0;
        int seeds = new Random().Next();
        // The universe array
        bool[,] universe = new bool[wideload, beanstalk];

        bool[,] scratchPad = new bool[wideload, beanstalk];


        // Drawing colors
        Color gridColor = Properties.Settings.Default.gridiron;
        Color cellColor = Properties.Settings.Default.cellular;
        Color huddlecolor = Color.DarkViolet;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            graphicsPanel1.BackColor = Properties.Settings.Default.backhair;
            // Setup the timer
            timer.Interval = Properties.Settings.Default.sundial; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int liverCells = livenCells;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                //Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = 0;
                    if (finiteToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    else
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }


                    //Apply the rules
                    if (universe[x, y] == true)
                    {
                        if (count < 2)
                        {
                            scratchPad[x, y] = false;
                        }
                        else if (count > 3)
                        {
                            scratchPad[x, y] = false;
                        }
                        else if (count == 2 || count == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (count == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    //Turn in on/off the scratchPad
                }
            }
            // Copy for scratchPad to universe
            bool[,] zombie = universe;
            universe = scratchPad;
            scratchPad = zombie;

            bool[,] empty = new bool[wideload, beanstalk];
            scratchPad = empty;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel2.Text = "Living Cells = " + liverCells.ToString();
            toolStripStatusLabel1.Text = "Interval = " + timer.Interval.ToString();
            toolStripStatusLabel3.Text = "Seed = " + seeds;
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        //graphics panel section
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Arial", 20f);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 2);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush hudbrush = new SolidBrush(huddlecolor);
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //RectangleF

                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    int neighbors = 0;
                    if (finiteToolStripMenuItem.Checked == true)// boundary types
                    {
                        neighbors = CountNeighborsFinite(x, y);
                    }
                    else
                    {
                        neighbors = CountNeighborsToroidal(x, y);
                    }

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if (gridToolStripMenuItem.Checked == true) // grid on\off
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    if (neighborCountToolStripMenuItem.Checked == true) //neighbor on\off
                    {
                        if (neighbors != 0)
                        {
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.DarkSlateGray, cellRect, stringFormat);
                        }
                    }


                }
            }
            int liverCells = 0;
            for (int y = 0; y < universe.GetLength(1); y++) //cell counter
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }
                }
            }
            string bound;
            if (finiteToolStripMenuItem.Checked == true)// boundary types
            {
                bound = "Finite";
            }
            else
            {
                bound = "Toroidal";
            }
            if (hUDToolStripMenuItem.Checked == true) // HUD on\off
            {
                RectangleF hudsie = RectangleF.Empty;
                string hudinfo = "Generations = " + generations + "\n"
                               + "Living Cells = " + liverCells.ToString() + "\n" 
                               + "Universe Size = " + wideload + ", " + beanstalk + "\n"
                               + "Boundary Type = " + bound;
                hudsie.Width = graphicsPanel1.Width;
                hudsie.Height = graphicsPanel1.Height;
                hudsie.X = 0;
                hudsie.Y = (universe.GetLength(1) - 20) * cellHeight;
                e.Graphics.DrawString(hudinfo.ToString(), font, hudbrush, hudsie);

               
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                //FLOATS
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        //start, pause, next, to
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true; //play
        } //start 

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false; //pause
        } //pause

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration(); //next
        } //next

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToDialog torun = new ToDialog();
            torun.GenerationTo = generations;
            if (DialogResult.OK == torun.ShowDialog())
            {
                while (generations != torun.GenerationTo)
                {
                    NextGeneration();
                }
            }
            int liverCells = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        } //to

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        } //start

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        } //pause

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        } //next
        //exit program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); //exit
        }
        //Count Neighbor Methods
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    else if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    else if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    else if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    else if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        //new, save, and open
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        } //new

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        } //new

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamWriter writer = new StreamWriter(dlg.FileName))
                {
                    // Write any comments you want to include first.
                    // Prefix all comment strings with an exclamation point.
                    // Use WriteLine to write the strings to the file. 
                    // It appends a CRLF for you.
                    writer.WriteLine("Save As!");

                    // Iterate through the universe one row at a time.
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Create a string to represent the current row.
                        String currentRow = string.Empty;

                        // Iterate through the current row one cell at a time.
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // If the universe[x,y] is alive then append 'O' (capital O)
                            // to the row string.
                            if (universe[x, y] == true)
                            {
                                currentRow += "O";
                            }
                            // Else if the universe[x,y] is dead then append '.' (period)
                            // to the row string.
                            else if (universe[x, y] == false)
                            {
                                currentRow += ".";
                            }

                        }
                        writer.WriteLine(currentRow);
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.

                }

            }
            graphicsPanel1.Invalidate();
        } //save

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamReader reader = new StreamReader(dlg.FileName))
                {
                    // Create a couple variables to calculate the width and height
                    // of the data in the file.
                    int maxWidth = 0;
                    int maxHeight = 0;

                    // Iterate through the file once to get its size.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then it is a comment
                        // and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then it is a row of cells.
                        // Increment the maxHeight variable for each row read.
                        if (row.Contains(".") || row.Contains("O"))
                        {
                            maxHeight++;
                        }

                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }

                    // Resize the current universe and scratchPad
                    // to the width and height of the file calculated above.
                    wideload = maxWidth;
                    beanstalk = maxHeight;
                    universe = new bool[wideload, beanstalk];
                    scratchPad = new bool[wideload, beanstalk];

                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Iterate through the file again, this time reading in the cells.


                    int y = 0;
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then
                        // it is a comment and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        if (row.Contains(".") || row.Contains("O"))
                        {
                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {

                                // If row[xPos] is a 'O' (capital O) then
                                // set the corresponding cell in the universe to alive.
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, y] = true;
                                }

                                // If row[xPos] is a '.' (period) then
                                // set the corresponding cell in the universe to dead.
                                if (row[xPos] == '.')
                                {
                                    universe[xPos, y] = false;
                                }

                            }
                            y++;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();
        } //open

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamWriter writer = new StreamWriter(dlg.FileName))
                {
                    // Write any comments you want to include first.
                    // Prefix all comment strings with an exclamation point.
                    // Use WriteLine to write the strings to the file. 
                    // It appends a CRLF for you.
                    writer.WriteLine("Save As!");

                    // Iterate through the universe one row at a time.
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Create a string to represent the current row.
                        String currentRow = string.Empty;

                        // Iterate through the current row one cell at a time.
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // If the universe[x,y] is alive then append 'O' (capital O)
                            // to the row string.
                            if (universe[x, y] == true)
                            {
                                currentRow += "O";
                            }
                            // Else if the universe[x,y] is dead then append '.' (period)
                            // to the row string.
                            else if (universe[x, y] == false)
                            {
                                currentRow += ".";
                            }

                        }
                        writer.WriteLine(currentRow);
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.

                }

            }
            graphicsPanel1.Invalidate();
        } //save

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamReader reader = new StreamReader(dlg.FileName))
                {

                    // Create a couple variables to calculate the width and height
                    // of the data in the file.
                    int maxWidth = 0;
                    int maxHeight = 0;

                    // Iterate through the file once to get its size.
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then it is a comment
                        // and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then it is a row of cells.
                        // Increment the maxHeight variable for each row read.
                        if (row.Contains(".") || row.Contains("O"))
                        {
                            maxHeight++;
                        }

                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }

                    // Resize the current universe and scratchPad
                    // to the width and height of the file calculated above.
                    wideload = maxWidth;
                    beanstalk = maxHeight;
                    universe = new bool[wideload, beanstalk];
                    scratchPad = new bool[wideload, beanstalk];

                    // Reset the file pointer back to the beginning of the file.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Iterate through the file again, this time reading in the cells.


                    int y = 0;
                    while (!reader.EndOfStream)
                    {
                        // Read one row at a time.
                        string row = reader.ReadLine();

                        // If the row begins with '!' then
                        // it is a comment and should be ignored.
                        if (row.StartsWith("!"))
                        {
                            continue;
                        }

                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        if (row.Contains(".") || row.Contains("O"))
                        {
                            for (int xPos = 0; xPos < row.Length; xPos++)
                            {

                                // If row[xPos] is a 'O' (capital O) then
                                // set the corresponding cell in the universe to alive.
                                if (row[xPos] == 'O')
                                {
                                    universe[xPos, y] = true;
                                }

                                // If row[xPos] is a '.' (period) then
                                // set the corresponding cell in the universe to dead.
                                if (row[xPos] == '.')
                                {
                                    universe[xPos, y] = false;
                                }

                            }
                            y++;
                        }
                    }
                }
            }
            graphicsPanel1.Invalidate();
        } //open

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                using (StreamWriter writer = new StreamWriter(dlg.FileName))
                {
                    // Write any comments you want to include first.
                    // Prefix all comment strings with an exclamation point.
                    // Use WriteLine to write the strings to the file. 
                    // It appends a CRLF for you.
                    writer.WriteLine("Save As!");

                    // Iterate through the universe one row at a time.
                    for (int y = 0; y < universe.GetLength(1); y++)
                    {
                        // Create a string to represent the current row.
                        String currentRow = string.Empty;

                        // Iterate through the current row one cell at a time.
                        for (int x = 0; x < universe.GetLength(0); x++)
                        {
                            // If the universe[x,y] is alive then append 'O' (capital O)
                            // to the row string.
                            if (universe[x, y] == true)
                            {
                                currentRow += "O";
                            }
                            // Else if the universe[x,y] is dead then append '.' (period)
                            // to the row string.
                            else if (universe[x, y] == false)
                            {
                                currentRow += ".";
                            }

                        }
                        writer.WriteLine(currentRow);
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.

                }

            }
            graphicsPanel1.Invalidate();
        } //save
        //seed options
        private void currentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random seed = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    seeds = seed.Next(0, 2);
                    if (seeds == 0)
                    {
                        universe[x, y] = true;
                    }
                    else
                    {
                        universe[x, y] = false;
                    }
                }
            }
            graphicsPanel1.Invalidate();

        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalDialog boxer = new ModalDialog();
            boxer.SetSeed(seeds);
            if (DialogResult.OK == boxer.ShowDialog())
            {
                seeds = boxer.GetSeed();
                Random seed = new Random(seeds);
                int seedling;
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        seedling = seed.Next(0, 2);
                        if (seedling == 0)
                        {
                            universe[x, y] = true;
                        }
                        else
                        {
                            universe[x, y] = false;
                        }
                    }
                }

            }
            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime clockwork = new DateTime();
            int tiktok = clockwork.Hour + clockwork.Minute + clockwork.Second;
            seeds = tiktok;
            Random cogsworth = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    seeds = cogsworth.Next(0, 2);
                    if (seeds == 0)
                    {
                        universe[x, y] = true;
                    }
                    else
                    {
                        universe[x, y] = false;
                    }
                }
            }

            int liverCells = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }
        //boundary types
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Checked = true;
            if (finiteToolStripMenuItem.Checked == true)
            {
                toroidalToolStripMenuItem.Checked = false;
            }
            graphicsPanel1.Invalidate();
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = true;
            if (toroidalToolStripMenuItem.Checked == true)
            {
                finiteToolStripMenuItem.Checked = false;
            }
            graphicsPanel1.Invalidate();
        }
        //options - universe
        private void universeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            UniverseResize size = new UniverseResize();
            size.SetWidth(wideload);
            size.SetHeight(beanstalk);
            size.SetTimer(timer.Interval);
            if (DialogResult.OK == size.ShowDialog())
            {
                timer.Interval = size.GetTimer();
                wideload = size.GetWidth();
                beanstalk = size.GetHeight();
                universe = new bool[wideload, beanstalk];
                scratchPad = new bool[wideload, beanstalk];
            }
            graphicsPanel1.Invalidate();

        }
        //view menu
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                gridToolStripMenuItem.Checked = false;
            }
            else
            {
                gridToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hUDToolStripMenuItem.Checked == true)
            {
                hUDToolStripMenuItem.Checked = false;
            }
            else
            {
                hUDToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
        //color options
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog gdlg = new ColorDialog();
            gdlg.Color = gridColor;
            if (DialogResult.OK == gdlg.ShowDialog())
            {
                gridColor = gdlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog gdlg = new ColorDialog();
            gdlg.Color = cellColor;
            if (DialogResult.OK == gdlg.ShowDialog())
            {
                cellColor = gdlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog gdlg = new ColorDialog();
            gdlg.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == gdlg.ShowDialog())
            {
               graphicsPanel1.BackColor = gdlg.Color;
            }
        }
        //reset\reload
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            wideload = Properties.Settings.Default.doublewide;
            beanstalk = Properties.Settings.Default.giant;
            graphicsPanel1.BackColor = Properties.Settings.Default.backhair;
            gridColor = Properties.Settings.Default.gridiron;
            cellColor = Properties.Settings.Default.cellular;
            timer.Interval = Properties.Settings.Default.sundial;
            universe = new bool[wideload, beanstalk];
            scratchPad = new bool[wideload, beanstalk];
            int liverCells = 0;
            for (int y = 0; y < universe.GetLength(1); y++) //cell counter
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }
                }
            }
            graphicsPanel1.Invalidate();

        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            wideload = Properties.Settings.Default.doublewide;
            beanstalk = Properties.Settings.Default.giant;
            graphicsPanel1.BackColor = Properties.Settings.Default.backhair;
            gridColor = Properties.Settings.Default.gridiron;
            cellColor = Properties.Settings.Default.cellular;
            timer.Interval = Properties.Settings.Default.sundial;
            universe = new bool[wideload, beanstalk];
            scratchPad = new bool[wideload, beanstalk];
            int liverCells = 0;
            for (int y = 0; y < universe.GetLength(1); y++) //cell counter
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        liverCells++;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }
        //save settings
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.doublewide = wideload;
            Properties.Settings.Default.giant = beanstalk;
            Properties.Settings.Default.backhair = graphicsPanel1.BackColor;
            Properties.Settings.Default.gridiron = gridColor;
            Properties.Settings.Default.cellular = cellColor;
            Properties.Settings.Default.sundial = timer.Interval;
            Properties.Settings.Default.Save();
        }
    }

}
