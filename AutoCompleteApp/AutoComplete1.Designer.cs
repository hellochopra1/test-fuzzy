namespace AutoCompleteApp
{
    partial class AutoComplete1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoComplete1));
            this.txtHomeWait = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBrowser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.ddlCountry = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLocalIp = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.chkIsTesting = new System.Windows.Forms.CheckBox();
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon2 = new System.Windows.Forms.NotifyIcon(this.components);
            this.chkChrome = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkMozilla = new System.Windows.Forms.CheckBox();
            this.chkSafari = new System.Windows.Forms.CheckBox();
            this.chkIE = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Mozilla = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtHomeWait
            // 
            this.txtHomeWait.Enabled = false;
            this.txtHomeWait.Location = new System.Drawing.Point(447, 135);
            this.txtHomeWait.Margin = new System.Windows.Forms.Padding(4);
            this.txtHomeWait.Name = "txtHomeWait";
            this.txtHomeWait.Size = new System.Drawing.Size(132, 22);
            this.txtHomeWait.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(60, 135);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(287, 20);
            this.label5.TabIndex = 25;
            this.label5.Text = "Enter Wait for Home(in seconds)";
            // 
            // txtBrowser
            // 
            this.txtBrowser.Enabled = false;
            this.txtBrowser.Location = new System.Drawing.Point(447, 94);
            this.txtBrowser.Margin = new System.Windows.Forms.Padding(4);
            this.txtBrowser.Name = "txtBrowser";
            this.txtBrowser.Size = new System.Drawing.Size(132, 22);
            this.txtBrowser.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(60, 94);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(327, 20);
            this.label4.TabIndex = 23;
            this.label4.Text = "Enter Number of Change(For Bowser)";
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Location = new System.Drawing.Point(743, 54);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 18;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(613, 54);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 16;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // ddlCountry
            // 
            this.ddlCountry.Enabled = false;
            this.ddlCountry.FormattingEnabled = true;
            this.ddlCountry.Location = new System.Drawing.Point(447, 54);
            this.ddlCountry.Margin = new System.Windows.Forms.Padding(4);
            this.ddlCountry.Name = "ddlCountry";
            this.ddlCountry.Size = new System.Drawing.Size(132, 24);
            this.ddlCountry.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(63, 54);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(196, 20);
            this.label8.TabIndex = 36;
            this.label8.Text = "Please Select Country";
            // 
            // txtLocalIp
            // 
            this.txtLocalIp.Location = new System.Drawing.Point(447, 13);
            this.txtLocalIp.Margin = new System.Windows.Forms.Padding(4);
            this.txtLocalIp.Name = "txtLocalIp";
            this.txtLocalIp.Size = new System.Drawing.Size(132, 22);
            this.txtLocalIp.TabIndex = 40;
            this.txtLocalIp.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(64, 12);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(129, 20);
            this.label10.TabIndex = 39;
            this.label10.Text = "Enter Local IP";
            this.label10.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(60, 181);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(179, 20);
            this.label11.TabIndex = 42;
            this.label11.Text = "Is Testing Keywords";
            this.label11.Visible = false;
            // 
            // chkIsTesting
            // 
            this.chkIsTesting.AutoSize = true;
            this.chkIsTesting.Location = new System.Drawing.Point(447, 182);
            this.chkIsTesting.Margin = new System.Windows.Forms.Padding(4);
            this.chkIsTesting.Name = "chkIsTesting";
            this.chkIsTesting.Size = new System.Drawing.Size(18, 17);
            this.chkIsTesting.TabIndex = 43;
            this.chkIsTesting.UseVisualStyleBackColor = true;
            this.chkIsTesting.Visible = false;
            // 
            // timer4
            // 
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timer5
            // 
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // notifyIcon2
            // 
            this.notifyIcon2.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon2.Icon")));
            this.notifyIcon2.Text = "AutoCompleteApp";
            this.notifyIcon2.Visible = true;
            this.notifyIcon2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon2_MouseDoubleClick);
            // 
            // chkChrome
            // 
            this.chkChrome.AutoSize = true;
            this.chkChrome.Location = new System.Drawing.Point(447, 218);
            this.chkChrome.Margin = new System.Windows.Forms.Padding(4);
            this.chkChrome.Name = "chkChrome";
            this.chkChrome.Size = new System.Drawing.Size(18, 17);
            this.chkChrome.TabIndex = 47;
            this.chkChrome.UseVisualStyleBackColor = true;
            this.chkChrome.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 217);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 20);
            this.label1.TabIndex = 46;
            this.label1.Text = "Select Browsers";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Visible = false;
            // 
            // chkMozilla
            // 
            this.chkMozilla.AutoSize = true;
            this.chkMozilla.Location = new System.Drawing.Point(581, 218);
            this.chkMozilla.Margin = new System.Windows.Forms.Padding(4);
            this.chkMozilla.Name = "chkMozilla";
            this.chkMozilla.Size = new System.Drawing.Size(18, 17);
            this.chkMozilla.TabIndex = 48;
            this.chkMozilla.UseVisualStyleBackColor = true;
            this.chkMozilla.Visible = false;
            // 
            // chkSafari
            // 
            this.chkSafari.AutoSize = true;
            this.chkSafari.Location = new System.Drawing.Point(695, 217);
            this.chkSafari.Margin = new System.Windows.Forms.Padding(4);
            this.chkSafari.Name = "chkSafari";
            this.chkSafari.Size = new System.Drawing.Size(18, 17);
            this.chkSafari.TabIndex = 49;
            this.chkSafari.UseVisualStyleBackColor = true;
            this.chkSafari.Visible = false;
            // 
            // chkIE
            // 
            this.chkIE.AutoSize = true;
            this.chkIE.Location = new System.Drawing.Point(793, 217);
            this.chkIE.Margin = new System.Windows.Forms.Padding(4);
            this.chkIE.Name = "chkIE";
            this.chkIE.Size = new System.Drawing.Size(18, 17);
            this.chkIE.TabIndex = 50;
            this.chkIE.UseVisualStyleBackColor = true;
            this.chkIE.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(343, 217);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 20);
            this.label2.TabIndex = 51;
            this.label2.Text = "Chrome";
            this.label2.Visible = false;
            // 
            // Mozilla
            // 
            this.Mozilla.AutoSize = true;
            this.Mozilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mozilla.Location = new System.Drawing.Point(504, 217);
            this.Mozilla.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Mozilla.Name = "Mozilla";
            this.Mozilla.Size = new System.Drawing.Size(69, 20);
            this.Mozilla.TabIndex = 52;
            this.Mozilla.Text = "Mozilla";
            this.Mozilla.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(628, 215);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 20);
            this.label3.TabIndex = 53;
            this.label3.Text = "Safari";
            this.label3.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(749, 215);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 20);
            this.label6.TabIndex = 54;
            this.label6.Text = "IE";
            this.label6.Visible = false;
            // 
            // AutoComplete1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(869, 178);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Mozilla);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkIE);
            this.Controls.Add(this.chkSafari);
            this.Controls.Add(this.chkMozilla);
            this.Controls.Add(this.chkChrome);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkIsTesting);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtLocalIp);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ddlCountry);
            this.Controls.Add(this.txtHomeWait);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBrowser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AutoComplete1";
            this.Text = "AutoComplete";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtHomeWait;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBrowser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.ComboBox ddlCountry;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLocalIp;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkIsTesting;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.NotifyIcon notifyIcon2;
        private System.Windows.Forms.CheckBox chkChrome;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkMozilla;
        private System.Windows.Forms.CheckBox chkSafari;
        private System.Windows.Forms.CheckBox chkIE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Mozilla;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
    }
}

