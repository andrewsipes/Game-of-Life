using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game_of_Life
{
    public partial class GameOfLife : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];

        //scratchpad array
        bool[,] scratchpad = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color gridColor10 = Color.Black;

        // The Timer class
        Timer timer = new Timer();


        // Generation count
        int generations = 0;

        public GameOfLife()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            //Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int CountNeighbors = 0;
                    int Count;

                    //Calculates CountNeighbor based on if Finite or Torodial is checked;
                    if (this.finiteToolStripMenuItem.Checked == true)
                    {
                        CountNeighbors = CountNeighborsFinite(x, y);
                    }

                    else if (this.toroidalToolStripMenuItem.Checked == true)
                    {
                        CountNeighbors = CountNeighborsToroidal(x, y);
                    }

                    Count = CountNeighbors;

                    //Apply the rules of life
                    //If check that runs if cell is alive
                    if (universe[x, y] == true)
                    {
                        // if number of neighboring cells is less than 2 or great than 3 then the cell dies
                        if (CountNeighbors < 2 || CountNeighbors > 3)
                        {
                            scratchpad[x, y] = false;
                        }

                        //if number of neighboring cells is 3 (only other option) then the cell is alive
                        else
                        {
                            scratchpad[x, y] = true;
                        }
                    }

                    //If check that runs if the cell is dead
                    else if (universe[x, y] == false)
                    {
                        //If neighobring cells is = 3, then the cell is now  alive
                        if (CountNeighbors == 3)
                        {
                            scratchpad[x, y] = true;
                        }

                        //If not, then cell remains dead
                        else
                        {
                            scratchpad[x, y] = false;
                        }
                    }

                }
            }

            //copy from scratchPad to universe
            bool[,] temp = universe;
            universe = scratchpad;
            scratchpad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            this.toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //Invalidate
            graphicsPanel1.Invalidate();
        }

        // Call NextGeneration with each timer tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {

            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Pen for drawing the grid 10 lines (color, width)
            Pen gridPen10 = new Pen(gridColor10, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //Code that will show # of Neighbors if > than 0
            Font font = new Font("Arial", 12f); // Set Font

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //initialize neighbors
                    int neighbors = 0;

                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = (x * cellWidth);
                    cellRect.Y = (y * cellHeight);
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    //outline cells
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    
                    //IF STATMEMENTS BELOW WILL DRAW THE BOLD BORDER (X10)

                        //This if Statement will draw the borders
                        if (x == 0 && y == 0)
                        {
                            e.Graphics.DrawLine(gridPen10, 0, 0, graphicsPanel1.ClientSize.Width, 0);
                            e.Graphics.DrawLine(gridPen10, 0, 0, 0, graphicsPanel1.ClientSize.Height);
                            e.Graphics.DrawLine(gridPen10, graphicsPanel1.ClientSize.Width, 0, graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height);
                            e.Graphics.DrawLine(gridPen10, 0, graphicsPanel1.ClientSize.Height, graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height);
                        }
                    
                        //This if Statement will draw the vertical lines
                        if (x % 10 == 0)
                        {
                            e.Graphics.DrawLine(gridPen10, cellRect.X, cellRect.Y, cellRect.X, graphicsPanel1.ClientSize.Height);
                        }

                        //This if Statement will draw the horizontal lines
                        if (y % 10 == 0)
                        {
                            e.Graphics.DrawLine(gridPen10, cellRect.X, cellRect.Y, graphicsPanel1.ClientSize.Width, cellRect.Y);
                        }

                    //CALCULATE NEIGHBORS - Will calculate based on which check box option is checked

                        //Finite
                        if (this.finiteToolStripMenuItem.Checked == true)
                        {
                            neighbors = CountNeighborsFinite(x, y);
                        }

                        //Torodial
                        else if (this.toroidalToolStripMenuItem.Checked == true)
                        {
                            neighbors = CountNeighborsToroidal(x, (y));
                        }


                    // Here we will write the neighbor count on the cells IF the checkbox state for the Neighbors is checked
                    if (neighborCountToolStripMenuItem.Checked == true)
                    {
                        // if statement that determine Neighbor text color for cells that are alive
                        if (universe[x, y] == true && neighbors > 0)
                        {
                            if (neighbors >= 2 && neighbors < 4)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }

                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                            }
                        }


                        //if statement that will determine Neighbor text color for cells that are dead 
                        else if (universe[x, y] == false && neighbors > 0)
                        {
                            if (neighbors == 3)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }

                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                            }
                        }


                    }

                }
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
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

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

        private void ClearUniverse()
        {
            //Nested Loop that will set everything in the scratchpad to false.
            //Afterwards we will then copy the scratchpad to the universe making all cells dead
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchpad[x, y] = false;
                }

            }

            //copy from scratchPad to universe
            bool[,] temp = universe;
            universe = scratchpad;
            scratchpad = temp;

            //Reset Generations to 0
            generations = 0;

            //Update label Text
            toolStripStatusLabelGenerations.Text = "Generations = 0";

            //Redraw Panel
            graphicsPanel1.Invalidate();
        }
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
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        //Count Neighbors Toroidal Method
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

        //Exit method that will close the form once initiated, should be assigned to the Exit menu option
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exits the program
            this.Close();
        }

        //Programs Next button to run Next Generation
        private void NextButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        //Programs Pause button to stop the timer
        private void PauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        //Programs the Play button to Play the timer
        private void PlayButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        //The two methods below will inversely change the checkbox sates of Torodial and Finite menu tool strips
        //Essentially when either is clicked it will change the state of each checkbox at the same time so that only 1 of them is actually checked at any given time.
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.toroidalToolStripMenuItem.Checked == false)
            {
                this.toroidalToolStripMenuItem.CheckState = CheckState.Checked;
                this.finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
            }

            else if (this.toroidalToolStripMenuItem.Checked == true)
            {
                this.toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.finiteToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }

        //See above comment
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.finiteToolStripMenuItem.Checked == false)
            {
                this.finiteToolStripMenuItem.CheckState = CheckState.Checked;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
            }

            else if (this.finiteToolStripMenuItem.Checked == true)
            {
                this.finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }

        // New button in the menu button stip that will clear the universe
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            ClearUniverse();

        }

        //New Button in the Menu that will clear the universe 
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUniverse();

        }

        //Play / Start button in the Menu Strip
        //Starts the Timer
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        //Pause Button in the Menu Strip
        //Stops the Timer
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        //Next Generation Menu Strip button
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        //Menu Strip Cell Color option, will pop open a dialog box for the user to choose then update the color property for cells
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Tool Strip Cell Color option, will pop open a dialog box for the user to choose then update the color property for cells
        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Menu strip Back color option, will pop open a dialog box for the user to choose then update the color property for the graphics panel
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Tool strip Back color option, will pop open a dialog box for the user to choose then update the color property for the graphics panel
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Menu Strip Grid Color option, will pop open a dialog box for the user to choose then update the color property for the grid
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Tool Strip Grid Color option, will pop open a dialog box for the user to choose then update the color property for the grid
        private void gridColorToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Menu Strip Grid Color 10 option, will pop open a dialog box for the user to choose then update the color property for the grid 10
        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor10 = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Tool Strip Grid Color 10 option, will pop open a dialog box for the user to choose then update the color property for the grid 10
        private void gridX10ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor10 = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Disables Grid in the graphics Panel
        private void DisableGrid()
        {           
            gridColor = Color.Transparent;
            gridColor10 = Color.Transparent;
            graphicsPanel1.Invalidate();
        }

        //Enables Grid in the graphics Panel
        private void EnableGrid()
        {
            gridColor = Color.Black;
            gridColor10 = Color.Black;
            graphicsPanel1.Invalidate();
        }

        //Changes Checkbox of GridToolStripMenuItem and GridToolStripMenuItem1 and toggles the state of the grid
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                this.gridToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.gridToolStripMenuItem1.CheckState = CheckState.Unchecked;
                DisableGrid();
            }

            else if (gridToolStripMenuItem.Checked == false)
            {
                this.gridToolStripMenuItem.CheckState = CheckState.Checked;
                this.gridToolStripMenuItem1.CheckState = CheckState.Checked;
                EnableGrid();
            }
        }
        //Changes Checkbox of GridToolStripMenuItem1 and GridToolStripMenuItem and toggles the state of the grid
        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem1.Checked == true)
            {
                this.gridToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.gridToolStripMenuItem.CheckState = CheckState.Unchecked;
                DisableGrid();
            }

            else if (gridToolStripMenuItem1.Checked == false)
            {
                this.gridToolStripMenuItem1.CheckState = CheckState.Checked;
                this.gridToolStripMenuItem.CheckState = CheckState.Checked;
                EnableGrid();
            }
        }

        //Shows Options Dialog Box when Options is clicked
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();
            dlg.ShowDialog();
        }

        //Menu Tool strip option that toggles the neighbor count on the cells
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (neighborCountToolStripMenuItem.Checked == true)
            {
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
            }

            else if (neighborCountToolStripMenuItem.Checked == false)
            {
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Checked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
            }
        }

        //Context Menu Tool strip option that toggles the neighbor count on the cells
        private void neighborCountToolStripMenuItem1_Click(object sender, EventArgs e)
        {


            if (neighborCountToolStripMenuItem1.Checked == true)
            {
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
                
            }

            else if (neighborCountToolStripMenuItem1.Checked == false)
            {
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Checked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
            }
        }
    }
}
    

