namespace AmIAnA.UI
{
    partial class Detect
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.runNeuralNet = new System.Windows.Forms.Button();
            this.reset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.GhostWhite;
            this.panel1.Location = new System.Drawing.Point(29, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(682, 476);
            this.panel1.TabIndex = 0;
            // 
            // runNeuralNet
            // 
            this.runNeuralNet.Location = new System.Drawing.Point(29, 530);
            this.runNeuralNet.Name = "runNeuralNet";
            this.runNeuralNet.Size = new System.Drawing.Size(156, 66);
            this.runNeuralNet.TabIndex = 1;
            this.runNeuralNet.Text = "Am I An A";
            this.runNeuralNet.UseVisualStyleBackColor = true;
            this.runNeuralNet.Click += new System.EventHandler(this.runNeuralNet_Click);
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(29, 530);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(156, 66);
            this.reset.TabIndex = 2;
            this.reset.Text = "Reset";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Visible = false;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // Detect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 669);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.runNeuralNet);
            this.Controls.Add(this.panel1);
            this.Name = "Detect";
            this.Text = "Detect";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button runNeuralNet;
        private System.Windows.Forms.Button reset;
    }
}