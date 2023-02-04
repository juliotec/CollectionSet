namespace CollectionSet
{
    partial class TestForm
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.cambiarListButton = new System.Windows.Forms.Button();
            this.CambiarCollectionSetButton = new System.Windows.Forms.Button();
            this.allowNewFalseButton = new System.Windows.Forms.Button();
            this.allowNewTrueButton = new System.Windows.Forms.Button();
            this.allowEditFalseButton = new System.Windows.Forms.Button();
            this.allowEditTrueButton = new System.Windows.Forms.Button();
            this.filtrarLabel = new System.Windows.Forms.Label();
            this.filtarTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(4, 134);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 51;
            this.dataGridView.RowTemplate.Height = 29;
            this.dataGridView.Size = new System.Drawing.Size(1133, 304);
            this.dataGridView.TabIndex = 0;
            // 
            // cambiarListButton
            // 
            this.cambiarListButton.Location = new System.Drawing.Point(12, 12);
            this.cambiarListButton.Name = "cambiarListButton";
            this.cambiarListButton.Size = new System.Drawing.Size(197, 29);
            this.cambiarListButton.TabIndex = 1;
            this.cambiarListButton.Text = "Cambiar a List";
            this.cambiarListButton.UseVisualStyleBackColor = true;
            this.cambiarListButton.Click += new System.EventHandler(this.CambiarListButtonClick);
            // 
            // CambiarCollectionSetButton
            // 
            this.CambiarCollectionSetButton.Location = new System.Drawing.Point(12, 47);
            this.CambiarCollectionSetButton.Name = "CambiarCollectionSetButton";
            this.CambiarCollectionSetButton.Size = new System.Drawing.Size(197, 29);
            this.CambiarCollectionSetButton.TabIndex = 2;
            this.CambiarCollectionSetButton.Text = "Cambiar a CollectionSet";
            this.CambiarCollectionSetButton.UseVisualStyleBackColor = true;
            this.CambiarCollectionSetButton.Click += new System.EventHandler(this.CambiarCollectionSetButtonClick);
            // 
            // allowNewFalseButton
            // 
            this.allowNewFalseButton.Location = new System.Drawing.Point(215, 12);
            this.allowNewFalseButton.Name = "allowNewFalseButton";
            this.allowNewFalseButton.Size = new System.Drawing.Size(254, 29);
            this.allowNewFalseButton.TabIndex = 3;
            this.allowNewFalseButton.Text = "No permitir nuevo CollectionSet";
            this.allowNewFalseButton.UseVisualStyleBackColor = true;
            this.allowNewFalseButton.Click += new System.EventHandler(this.AllowNewFalseButtonClick);
            // 
            // allowNewTrueButton
            // 
            this.allowNewTrueButton.Location = new System.Drawing.Point(215, 47);
            this.allowNewTrueButton.Name = "allowNewTrueButton";
            this.allowNewTrueButton.Size = new System.Drawing.Size(254, 29);
            this.allowNewTrueButton.TabIndex = 4;
            this.allowNewTrueButton.Text = "Permitir nuevo CollectionSet";
            this.allowNewTrueButton.UseVisualStyleBackColor = true;
            this.allowNewTrueButton.Click += new System.EventHandler(this.AllowNewTrueButtonClick);
            // 
            // allowEditFalseButton
            // 
            this.allowEditFalseButton.Location = new System.Drawing.Point(475, 12);
            this.allowEditFalseButton.Name = "allowEditFalseButton";
            this.allowEditFalseButton.Size = new System.Drawing.Size(254, 29);
            this.allowEditFalseButton.TabIndex = 5;
            this.allowEditFalseButton.Text = "No permitir editar CollectionSet";
            this.allowEditFalseButton.UseVisualStyleBackColor = true;
            this.allowEditFalseButton.Click += new System.EventHandler(this.AllowEditFalseButtonClick);
            // 
            // allowEditTrueButton
            // 
            this.allowEditTrueButton.Location = new System.Drawing.Point(475, 47);
            this.allowEditTrueButton.Name = "allowEditTrueButton";
            this.allowEditTrueButton.Size = new System.Drawing.Size(254, 29);
            this.allowEditTrueButton.TabIndex = 6;
            this.allowEditTrueButton.Text = "Permitir editar CollectionSet";
            this.allowEditTrueButton.UseVisualStyleBackColor = true;
            this.allowEditTrueButton.Click += new System.EventHandler(this.AllowEditTrueButtonClick);
            // 
            // filtrarLabel
            // 
            this.filtrarLabel.AutoSize = true;
            this.filtrarLabel.Location = new System.Drawing.Point(12, 82);
            this.filtrarLabel.Name = "filtrarLabel";
            this.filtrarLabel.Size = new System.Drawing.Size(47, 20);
            this.filtrarLabel.TabIndex = 7;
            this.filtrarLabel.Text = "Filtrar";
            // 
            // filtarTextBox
            // 
            this.filtarTextBox.Location = new System.Drawing.Point(65, 79);
            this.filtarTextBox.Name = "filtarTextBox";
            this.filtarTextBox.Size = new System.Drawing.Size(197, 27);
            this.filtarTextBox.TabIndex = 8;
            this.filtarTextBox.TextChanged += new System.EventHandler(this.FiltarTextBoxTextChanged);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 450);
            this.Controls.Add(this.filtarTextBox);
            this.Controls.Add(this.filtrarLabel);
            this.Controls.Add(this.allowEditTrueButton);
            this.Controls.Add(this.allowEditFalseButton);
            this.Controls.Add(this.allowNewTrueButton);
            this.Controls.Add(this.allowNewFalseButton);
            this.Controls.Add(this.CambiarCollectionSetButton);
            this.Controls.Add(this.cambiarListButton);
            this.Controls.Add(this.dataGridView);
            this.Name = "TestForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.TestFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView dataGridView;
        private Button cambiarListButton;
        private Button CambiarCollectionSetButton;
        private Button allowNewFalseButton;
        private Button allowNewTrueButton;
        private Button allowEditFalseButton;
        private Button allowEditTrueButton;
        private Label filtrarLabel;
        private TextBox filtarTextBox;
    }
}