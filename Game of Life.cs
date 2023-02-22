using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game_of_Life
{
    public partial class GameOfLife : Form
    {
        //DEFAULT PROPERTIES

        //Set default values for Cell Width, Height and Interval
        private static int UniverseCellWidth = 30;
        private static int UniverseCellHeight = 30;
        private int TimerInterval = 100;

        //Default Drawing Colors
        private Color gridColor = Color.Black;
        private Color cellColor = Color.Gray;
        private Color gridColor10 = Color.Black;
        private Color backColor = Color.White;
        private Color tempGridColor;
        private Color tempGridColor10;

        //Initalize curent Seed
        int CurrentSeed;

        //GETTERS AND SETTERS

        //Sets UniverseCellWidth
        public void SetUniverseCellWidth(int _UniverseCellWidth)
        {
            UniverseCellWidth = _UniverseCellWidth;
        }

        //Sets UniverseCellHeight
        public void SetUniverseCellHeight(int _UniverseCellHeight)
        {
            UniverseCellHeight = _UniverseCellHeight;
        }

        //Sets TimerInterval
        public void SetTimerInterval(int _TimerInterval)
        {
            TimerInterval = _TimerInterval;
        }

        //Set cellColor
        public void SetCellColor(Color _color)
        {
            cellColor = _color;
        }

        //Set gridColor
        public void SetGridColor(Color _color)
        {
            gridColor = _color;
        }

        //Set gridColor10
        public void SetGridColor10(Color _color)
        {
            gridColor10 = _color;
        }

        //Set backColor
        public void SetBackColor(Color _color)
        {
            backColor = _color;

        }

        //Set tempgridColor10
        public void SetTempGridColor(Color _color)
        {
            tempGridColor = _color;
        }

        //Set tempgridColor10
        public void SetTempGridColor10(Color _color)
        {
            tempGridColor10 = _color;
        }

        //Gets TimerInterval
        public int GetTimerInterval()
        {
            return TimerInterval;
        }

        //Get UniverseCellWidth
        public int GetUniverseCellWidth()
        {
            return UniverseCellWidth;
        }

        //Get UniverseCellHeight
        public int GetUniverseCellHeight()
        {
            return UniverseCellHeight;
        }

        //Gets Width of Window
        public int GetClientSizeWidth()
        {
            return this.Size.Width;
        }

        //Gets Height of Window
        public int GetClientSizeHeight()
        {
            return this.Size.Height;
        }

        //Get CellColor
        public Color GetCellColor()
        {
            return cellColor;
        }

        //Get GridColor
        public Color GetGridColor()
        {
            return gridColor;
        }

        //Get GridColor10
        public Color GetGridColor10()
        {
            return gridColor10;
        }

        //Get CellColor
        public Color GetBackColor()
        {
            return backColor;
        }

        //Get tempgridColor
        public Color GetTempGridColor()
        {
            return tempGridColor;
        }

        //Get tempgridColor10
        public Color GetTempGridColor10()
        {
            return tempGridColor10;
        }

        //BOOLS
        bool isNeighborCountVisible = true;
        bool isHUDVisible = true;
        bool isTorodial = true;
        bool isGridVisible = true;
       
        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        //initialize bools
        bool[,] universe;
        bool[,] scratchpad;

        public GameOfLife()
        {
            
            InitializeComponent();

            // Loads the  properties
            LoadProperties();

            //Delcares the size of the arrays - This location allows us to use the saved properties for cellwidth and cellheight
            universe = new bool[UniverseCellWidth, UniverseCellHeight];
            scratchpad = new bool[UniverseCellWidth, UniverseCellHeight];

            //Randomize Current Seed
            Random rand = new Random();
            CurrentSeed = rand.Next(1, int.MaxValue);

            // Setup the timer
            timer.Interval = TimerInterval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            this.ToolStripStatusIntervalLabel.Text = "Interval: " + TimerInterval.ToString();

        }

        //Will Save the current properties when form closes
        private void GameOfLife_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveCurrentProperties();
        }

        //Used to Load previously saved Properties
        private void LoadProperties()
        {
            //Set Settings based on Settings File (last save)
            SetBackColor(Properties.Settings.Default.BackColor);
            SetCellColor(Properties.Settings.Default.CellColor);
            SetGridColor(Properties.Settings.Default.GridColor);
            SetGridColor10(Properties.Settings.Default.GridColor10);
            SetTimerInterval(Properties.Settings.Default.TimerInterval);
            SetUniverseCellWidth(Properties.Settings.Default.CellWidth);
            SetUniverseCellHeight(Properties.Settings.Default.CellHeight);
            graphicsPanel1.Invalidate();
         
        }

        //Used to Save Current Properties
        private void SaveCurrentProperties()
        {
            //We will use getters to store the current vaules into the properties
            Properties.Settings.Default.BackColor = GetBackColor();
            Properties.Settings.Default.CellColor = GetCellColor();
            Properties.Settings.Default.GridColor = GetGridColor();
            Properties.Settings.Default.GridColor10 = GetGridColor10();
            Properties.Settings.Default.TimerInterval = GetTimerInterval();
            Properties.Settings.Default.CellWidth = GetUniverseCellWidth();
            Properties.Settings.Default.CellHeight = GetUniverseCellHeight();

            //Saves the values
            Properties.Settings.Default.Save();
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

        //Renders the graphics panel
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Sets Backcolor to the panel
            graphicsPanel1.BackColor = GetBackColor();

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

            // Font Style for NeighborCount
            Font font = new Font("Arial", 12f);
            StringFormat stringFormat = new StringFormat();

            //Set Alignment
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //initialize Alive Count
            int aliveCount = 0;


            //GRAPHICS NESTED LOOP
            //Iterate through the universe in the y, top to bottom
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

                    //Draw the Grid if option is selected
                    if (isGridVisible == true)

                    {
                        //outline cells
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);


                        //IF STATMEMENTS BELOW WILL DRAW THE BOLD BORDER (X10)

                        //This if Statement will draw the borders
                        if (x == 0 && y == 0)
                        {
                            e.Graphics.DrawLine(gridPen10, 0, 0, graphicsPanel1.ClientSize.Width, 0);
                            e.Graphics.DrawLine(gridPen10, 0, 0, 0, graphicsPanel1.ClientSize.Height);
                            e.Graphics.DrawLine(gridPen10, graphicsPanel1.ClientSize.Width - 2, 0, graphicsPanel1.ClientSize.Width - 2, graphicsPanel1.ClientSize.Height);
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
                    }


                    //CALCULATE NEIGHBORS - Will calculate based on which check box option is checked

                    //Finite
                    if (isTorodial == true)
                    {
                        neighbors = CountNeighborsToroidal(x, y);
                    }

                    //Torodial
                    else
                    {
                        neighbors = CountNeighborsFinite(x, y);
                    }


                    // TOGGLE NEIGHBOR COUNT VIEW ON THE CELLS
                    if (isNeighborCountVisible == true)
                    {
                        // Below is the logic used to place numbers and the appropriate text color for the neighbor count

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


                    // Here we will count the cells that are alive for display
                    if (universe[x, y] == true)
                    {
                        aliveCount++;

                    }


                }


            }

            //Update Timer
            timer.Interval = GetTimerInterval();

            //Update Labels
            this.ToolStripStatusAliveLabel.Text = "Alive: " + aliveCount.ToString();
            this.ToolStripStatusIntervalLabel.Text = "Interval: " + timer.Interval.ToString();
            this.toolStripStatusSeedLabel.Text = "Seed: " + CurrentSeed.ToString();

            // Font Style for Hud
            font = new Font("Arial", 12f, FontStyle.Bold);

            //Draw HUD is option is true
            if (isHUDVisible == true)
            {
                //Determine string display if countneighbors is toroidal or finite
                string boundary;
                if (isTorodial == true)
                {
                    boundary = "Torodial";
                }

                else
                {
                    boundary = "Finite";
                }

                //write the text at the bottom left of the window
                e.Graphics.DrawString("Generations: " + generations + "\n" + "CellCount: " + aliveCount + "\n" + "BoundaryType: " + boundary + "\n" + "UniverseSize: {Width = " + universe.GetLength(0) + ", Height = " + universe.GetLength(1) + "}", font, Brushes.Blue, 0, GetClientSizeHeight() - 190);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        //Event for turning cells on an off with mo use
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

        //Count Neightbors Finite Method
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
                isTorodial = true;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Checked;
                this.finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
            }

            else if (this.toroidalToolStripMenuItem.Checked == true)
            {
                isTorodial = false;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.finiteToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();

            }
        }

        //See above comment
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.finiteToolStripMenuItem.Checked == false)
            {
                isTorodial = false;
                this.finiteToolStripMenuItem.CheckState = CheckState.Checked;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
            }

            else if (this.finiteToolStripMenuItem.Checked == true)
            {

                isTorodial = true;
                this.finiteToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.toroidalToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
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
                backColor = dlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        //Tool strip Back color option, will pop open a dialog box for the user to choose then update the color property for the graphics panel
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                backColor = dlg.Color;
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
        //We will store the old color since we have to make the current color properties transparent
        private void DisableGrid()
        {
            tempGridColor = GetGridColor();
            tempGridColor10 = GetGridColor10();
            gridColor = Color.Transparent;
            gridColor10 = Color.Transparent;
            graphicsPanel1.Invalidate();
        }

        //Enables Grid in the graphics Panel
        private void EnableGrid()
        {
            gridColor = GetTempGridColor();
            gridColor10 = GetTempGridColor10();
            graphicsPanel1.Invalidate();
        }

        //Changes Checkbox of GridToolStripMenuItem and GridToolStripMenuItem1 and toggles the state of the grid
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                isGridVisible = false;
                this.gridToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.gridToolStripMenuItem1.CheckState = CheckState.Unchecked;
                DisableGrid();
            }

            else if (gridToolStripMenuItem.Checked == false)
            {
                isGridVisible = true;
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
                isGridVisible = false;
                this.gridToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.gridToolStripMenuItem.CheckState = CheckState.Unchecked;
                DisableGrid();
            }

            else if (gridToolStripMenuItem1.Checked == false)
            {
                isGridVisible = true;
                this.gridToolStripMenuItem1.CheckState = CheckState.Checked;
                this.gridToolStripMenuItem.CheckState = CheckState.Checked;
                EnableGrid();
            }
        }

        //Menu Tool strip option that toggles the neighbor count on the cells
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (neighborCountToolStripMenuItem.Checked == true)
            {
                isNeighborCountVisible = false;
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
            }

            else if (neighborCountToolStripMenuItem.Checked == false)
            {
                isNeighborCountVisible = true;
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
                isNeighborCountVisible = false;
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();

            }

            else if (neighborCountToolStripMenuItem1.Checked == false)
            {
                isNeighborCountVisible = true;
                this.neighborCountToolStripMenuItem1.CheckState = CheckState.Checked;
                this.neighborCountToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
            }
        }

        //Options Menu that displays on Click
        //If Changes are made, essentially we need to copy the old parameters and create a new form since arrays are not resizeable
        //Then we copy the properties back to the new form.
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();

            //Retrieves the current values for each textbox
            dlg.numericUpDownWidth.Value = universe.GetLength(0);
            dlg.numericUpDownHeight.Value = universe.GetLength(1);
            dlg.numericUpDownInterval.Value = GetTimerInterval();

            //If user presses okay, We will update the parameters
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Update Options according to dialog box              
                SetUniverseCellWidth((int)dlg.numericUpDownWidth.Value);
                SetUniverseCellHeight((int)dlg.numericUpDownHeight.Value);
                SetTimerInterval((int)dlg.numericUpDownInterval.Value);
                
                //Copy Contents of current Universe into another Universe with the new size
                universe = ResizeArray(universe, GetUniverseCellWidth(), GetUniverseCellHeight());
                scratchpad = ResizeArray(scratchpad, GetUniverseCellWidth(), GetUniverseCellHeight());

                graphicsPanel1.Invalidate();
                dlg.Close();
            }

        }

        //ResizeArray - this will copy the contents from the old array into the new array so we no longer have sizing issues in the code.
        bool[,] ResizeArray(bool[,] array, int x, int y)
        {
            bool[,] ReplacementArray = new bool[x, y];
            int OriginalRowLength = array.GetLength(0);
            int OriginalColumnLength = array.GetLength(1);

            //if Sizing down
            if (x < OriginalRowLength || y < OriginalColumnLength)
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        ReplacementArray[i, j] = array[i, j];
                    }
                }

            }

            //if Sizing Up
            else
            {
                for (int i = 0; i < OriginalRowLength; i++)
                {
                    for (int j = 0; j < OriginalColumnLength; j++)
                    {
                        ReplacementArray[i, j] = array[i, j];
                    }
                }
            }


            return ReplacementArray;
        }

        //HUD enable / disable for Menu strip
        private void HUDToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (isHUDVisible == true)
            {
                isHUDVisible = false;
                this.HUDToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.HUDToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();
            }

            else if (isHUDVisible == false)
            {
                isHUDVisible = true;
                this.HUDToolStripMenuItem1.CheckState = CheckState.Checked;
                this.HUDToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
            }
        }

        //HUD enable / disable for Tool Strip
        private void HUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (isHUDVisible == true)
            {
                isHUDVisible = false;
                this.HUDToolStripMenuItem1.CheckState = CheckState.Unchecked;
                this.HUDToolStripMenuItem.CheckState = CheckState.Unchecked;
                graphicsPanel1.Invalidate();

            }

            else if (isHUDVisible == false)
            {
                isHUDVisible = true;
                this.HUDToolStripMenuItem1.CheckState = CheckState.Checked;
                this.HUDToolStripMenuItem.CheckState = CheckState.Checked;
                graphicsPanel1.Invalidate();
            }
        }

        //Saves the current cell state to a .cells file
        private void SaveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!Cells Save File");

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

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        //Menu Save Button
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        //Tool Strip Save Button
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        //Opens file and applies the cell file to the universe
        private void OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

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

                    if (row.Contains("!"))
                    {
                        reader.ReadLine().Skip(maxHeight);
                        
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                                     
                    maxHeight++;
                    
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.

                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = ResizeArray(universe, maxWidth, maxHeight);
                scratchpad = ResizeArray(universe, maxWidth, maxHeight);

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                //Reset Max height
                maxHeight = 0;

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                  
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.Contains('!'))
                    {
                        row = reader.ReadLine();
                        
                    }

                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O')
                        {
                            universe[xPos, maxHeight] = true;
                        }

                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.

                        if (row[xPos] == '.')
                        {
                            universe[xPos, maxHeight] = false;
                        }
                    }

                    maxHeight++;
                }

                // Close the file.
                reader.Close();

                graphicsPanel1.Invalidate();
            }
        }

        //Tool Strip Open Button - Opens a cells file and applies it to the grid
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        //Menu Strip Open - Opens a cells file and applies it to the grid
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        //Reset button, resets to default settings in the settings file
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            //Load saved Values
            LoadProperties();

            //Resize arrays based on settings
            universe = ResizeArray(universe, UniverseCellWidth, UniverseCellHeight);
            scratchpad = ResizeArray(scratchpad, UniverseCellWidth, UniverseCellHeight);

            //Redraw the panel with correct values
            graphicsPanel1.Invalidate();
            
        }

        //Reload button, reloads cached settings (last close)
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            //Load Saved Values
            LoadProperties();

            //Resize arrays based on settings
            universe = ResizeArray(universe, UniverseCellWidth, UniverseCellHeight);
            scratchpad = ResizeArray(scratchpad, UniverseCellWidth, UniverseCellHeight);

            //Redraw the panel with correct values
            graphicsPanel1.Invalidate();
        }

        //Randomizes values of the universe a pre-defined random that has been generated from seed value
        private void Randomize(Random rand)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int num = rand.Next(0, 5);

                    if (num == 1 || num == 0 || num == 2)
                    {
                        scratchpad[x, y] = false;
                    }

                    else
                    {
                        scratchpad[x, y] = true;
                    }
                }
            }

            universe = scratchpad;

            graphicsPanel1.Invalidate();
        }
        //Randomizes using Time as Seed
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create new Random using based on Time
            Random rand = new Random();

            //Randomize Values of the universe
            Randomize(rand);
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Creates a Random using the Current Seed
            Random rand = new Random(CurrentSeed);

            //Randomize Values of the universe
            Randomize(rand);
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create new SeedDialog
            SeedDialog dlg = new SeedDialog();

            //Random Generator for Random Seed
            Random randSeed = new Random();

            //Set Numeric Updown Min/Max
            dlg.SeedNumericUpDown.Minimum = 1;
            dlg.SeedNumericUpDown.Maximum = int.MaxValue;

            //Set SeedNumericUpDown Value to the current seed
            dlg.SeedNumericUpDown.Value = CurrentSeed;

           
            //if OK pressed, we will use the random number generator to randomize the universe values
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Stores value in SeedNumericUpDown as current seed
                CurrentSeed = (int)dlg.SeedNumericUpDown.Value;

                //Random Generator for Random values in Universe(based on seed)
                Random rand = new Random(CurrentSeed);

                //Randomize values in the universe
                Randomize(rand);

                //closes the dialogbox
                dlg.Close();
            }
        }
    }
    }



