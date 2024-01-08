namespace plc.monitor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            app_pn_top = new Panel();
            app_tabs = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            button1 = new Button();
            app_pn_top.SuspendLayout();
            app_tabs.SuspendLayout();
            SuspendLayout();
            // 
            // app_pn_top
            // 
            app_pn_top.Controls.Add(button1);
            app_pn_top.Dock = DockStyle.Top;
            app_pn_top.Location = new Point(3, 3);
            app_pn_top.Name = "app_pn_top";
            app_pn_top.Size = new Size(1202, 65);
            app_pn_top.TabIndex = 0;
            // 
            // app_tabs
            // 
            app_tabs.Controls.Add(tabPage1);
            app_tabs.Controls.Add(tabPage2);
            app_tabs.Dock = DockStyle.Fill;
            app_tabs.Location = new Point(3, 68);
            app_tabs.Name = "app_tabs";
            app_tabs.SelectedIndex = 0;
            app_tabs.Size = new Size(1202, 513);
            app_tabs.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1194, 483);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1194, 483);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(55, 23);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1208, 584);
            Controls.Add(app_tabs);
            Controls.Add(app_pn_top);
            Name = "Form1";
            Padding = new Padding(3);
            app_pn_top.ResumeLayout(false);
            app_tabs.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel app_pn_top;
        private TabControl app_tabs;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
    }
}