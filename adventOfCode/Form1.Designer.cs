namespace adventOfCode
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tbInputLines = new TextBox();
            bnRun = new Button();
            lbAnswer = new Label();
            bnRun2 = new Button();
            SuspendLayout();
            // 
            // tbInputLines
            // 
            tbInputLines.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbInputLines.Location = new Point(438, 26);
            tbInputLines.Multiline = true;
            tbInputLines.Name = "tbInputLines";
            tbInputLines.ScrollBars = ScrollBars.Both;
            tbInputLines.Size = new Size(350, 412);
            tbInputLines.TabIndex = 0;
            tbInputLines.WordWrap = false;
            // 
            // bnRun
            // 
            bnRun.Location = new Point(28, 40);
            bnRun.Name = "bnRun";
            bnRun.Size = new Size(250, 23);
            bnRun.TabIndex = 1;
            bnRun.Text = "Convert And Sum Lines";
            bnRun.UseVisualStyleBackColor = true;
            // 
            // lbAnswer
            // 
            lbAnswer.AutoSize = true;
            lbAnswer.Location = new Point(28, 215);
            lbAnswer.Name = "lbAnswer";
            lbAnswer.Size = new Size(67, 15);
            lbAnswer.TabIndex = 2;
            lbAnswer.Text = "Answer: ???";
            // 
            // bnRun2
            // 
            bnRun2.Location = new Point(28, 69);
            bnRun2.Name = "bnRun2";
            bnRun2.Size = new Size(250, 23);
            bnRun2.TabIndex = 1;
            bnRun2.Text = "Convert And Sum Lines v2";
            bnRun2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lbAnswer);
            Controls.Add(bnRun2);
            Controls.Add(bnRun);
            Controls.Add(tbInputLines);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbInputLines;
        private Button bnRun;
        private Label lbAnswer;
        private Button bnRun2;
    }
}