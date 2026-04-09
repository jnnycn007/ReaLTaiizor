namespace ReaLTaiizor_CR
{
    partial class Debug
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
            this.foreverNotification1 = new ReaLTaiizor.Controls.ForeverNotification();
            this.SuspendLayout();
            // 
            // foreverNotification1
            // 
            this.foreverNotification1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(73)))));
            this.foreverNotification1.Close = true;
            this.foreverNotification1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.foreverNotification1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.foreverNotification1.Kind = ReaLTaiizor.Controls.ForeverNotification._Kind.Success;
            this.foreverNotification1.Location = new System.Drawing.Point(151, 175);
            this.foreverNotification1.Name = "foreverNotification1";
            this.foreverNotification1.Size = new System.Drawing.Size(334, 157);
            this.foreverNotification1.TabIndex = 0;
            this.foreverNotification1.Text = "Abur cubur panpiş pompiş sikiş mikiş takış tukuş";
            this.foreverNotification1.Visible = false;
            // 
            // Debug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.foreverNotification1);
            this.Name = "Debug";
            this.Text = "Debug";
            this.ResumeLayout(false);

        }

        #endregion

        private ReaLTaiizor.Controls.ForeverNotification foreverNotification1;
    }
}