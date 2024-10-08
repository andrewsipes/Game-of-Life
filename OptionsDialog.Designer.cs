﻿namespace Game_of_Life
{
    partial class OptionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OkayButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.IntervalLabel = new System.Windows.Forms.Label();
            this.CellWidthLabel = new System.Windows.Forms.Label();
            this.CellHeightLabel = new System.Windows.Forms.Label();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // OkayButton
            // 
            this.OkayButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkayButton.Location = new System.Drawing.Point(50, 178);
            this.OkayButton.Name = "OkayButton";
            this.OkayButton.Size = new System.Drawing.Size(87, 33);
            this.OkayButton.TabIndex = 0;
            this.OkayButton.Text = "OK";
            this.OkayButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(178, 178);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(87, 33);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // IntervalLabel
            // 
            this.IntervalLabel.AutoSize = true;
            this.IntervalLabel.Location = new System.Drawing.Point(36, 40);
            this.IntervalLabel.Name = "IntervalLabel";
            this.IntervalLabel.Size = new System.Drawing.Size(142, 13);
            this.IntervalLabel.TabIndex = 5;
            this.IntervalLabel.Text = "Timer Interval in Milliseconds";
            // 
            // CellWidthLabel
            // 
            this.CellWidthLabel.AutoSize = true;
            this.CellWidthLabel.Location = new System.Drawing.Point(50, 81);
            this.CellWidthLabel.Name = "CellWidthLabel";
            this.CellWidthLabel.Size = new System.Drawing.Size(128, 13);
            this.CellWidthLabel.TabIndex = 6;
            this.CellWidthLabel.Text = "Width of Universe in Cells";
            // 
            // CellHeightLabel
            // 
            this.CellHeightLabel.AutoSize = true;
            this.CellHeightLabel.Location = new System.Drawing.Point(47, 121);
            this.CellHeightLabel.Name = "CellHeightLabel";
            this.CellHeightLabel.Size = new System.Drawing.Size(131, 13);
            this.CellHeightLabel.TabIndex = 7;
            this.CellHeightLabel.Text = "Height of Universe in Cells";
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(194, 38);
            this.numericUpDownInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownInterval.TabIndex = 8;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(194, 79);
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownWidth.TabIndex = 9;
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(194, 119);
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownHeight.TabIndex = 10;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.OkayButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 236);
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.numericUpDownInterval);
            this.Controls.Add(this.CellHeightLabel);
            this.Controls.Add(this.CellWidthLabel);
            this.Controls.Add(this.IntervalLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkayButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OptionsDialog";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkayButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label IntervalLabel;
        private System.Windows.Forms.Label CellWidthLabel;
        private System.Windows.Forms.Label CellHeightLabel;
        public System.Windows.Forms.NumericUpDown numericUpDownWidth;
        public System.Windows.Forms.NumericUpDown numericUpDownHeight;
        public System.Windows.Forms.NumericUpDown numericUpDownInterval;
    }
}