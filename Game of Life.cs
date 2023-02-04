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
        bool[,] universe = new bool[20, 20];

        //scratchpad array
        bool[,] scratchpad = new bool[20,20];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

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

                    //Turn in on//off Scratch pad
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
            //FLOATS**
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (float y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (float x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //RectangleF
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = (int)(x * cellWidth);
                    cellRect.Y = (int)(y * cellHeight);
                    cellRect.Width = (int)cellWidth;
                    cellRect.Height = (int)cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[(int)x, (int)y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
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
                float cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

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

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        // New button in the menu button stip that will clear the universe
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            ClearUniverse();            
            NextGeneration();
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
        }

        //New Button in the Menu that will clear the universe 
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearUniverse();
            NextGeneration();
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
        }
    }

   

}
    

