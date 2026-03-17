namespace HastaneOtomasyonSistemi
{
    partial class FrmDuyurular
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDuyurular));
            this.RchDuyuruListesi = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // RchDuyuruListesi
            // 
            this.RchDuyuruListesi.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.RchDuyuruListesi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RchDuyuruListesi.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.RchDuyuruListesi.Location = new System.Drawing.Point(0, 0);
            this.RchDuyuruListesi.Name = "RchDuyuruListesi";
            this.RchDuyuruListesi.ReadOnly = true;
            this.RchDuyuruListesi.Size = new System.Drawing.Size(753, 503);
            this.RchDuyuruListesi.TabIndex = 0;
            this.RchDuyuruListesi.Text = "";
            // 
            // FrmDuyurular
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(753, 503);
            this.Controls.Add(this.RchDuyuruListesi);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmDuyurular";
            this.Text = "Duyurular";
            this.Load += new System.EventHandler(this.FrmDuyurular_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RchDuyuruListesi;
    }
}