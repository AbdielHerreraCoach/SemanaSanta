namespace SemanaSanta
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
            btnCargarArchivo = new Button();
            dgvDatos = new DataGridView();
            btnCargarSQL = new Button();
            btnAgrupar = new Button();
            txtBuscar = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvDatos).BeginInit();
            SuspendLayout();
            // 
            // btnCargarArchivo
            // 
            btnCargarArchivo.Location = new Point(39, 12);
            btnCargarArchivo.Name = "btnCargarArchivo";
            btnCargarArchivo.Size = new Size(75, 23);
            btnCargarArchivo.TabIndex = 0;
            btnCargarArchivo.Text = "Cargar";
            btnCargarArchivo.UseVisualStyleBackColor = true;
            btnCargarArchivo.Click += btnCargarArchivo_Click;
            // 
            // dgvDatos
            // 
            dgvDatos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDatos.Location = new Point(12, 41);
            dgvDatos.Name = "dgvDatos";
            dgvDatos.Size = new Size(776, 397);
            dgvDatos.TabIndex = 1;
            dgvDatos.ColumnHeaderMouseClick += dgvDatos_ColumnHeaderMouseClick;
            // 
            // btnCargarSQL
            // 
            btnCargarSQL.Location = new Point(120, 12);
            btnCargarSQL.Name = "btnCargarSQL";
            btnCargarSQL.Size = new Size(75, 23);
            btnCargarSQL.TabIndex = 2;
            btnCargarSQL.Text = "SQL";
            btnCargarSQL.UseVisualStyleBackColor = true;
            btnCargarSQL.Click += btnCargarSQL_Click;
            // 
            // btnAgrupar
            // 
            btnAgrupar.Location = new Point(201, 12);
            btnAgrupar.Name = "btnAgrupar";
            btnAgrupar.Size = new Size(75, 23);
            btnAgrupar.TabIndex = 3;
            btnAgrupar.Text = "Agrupar";
            btnAgrupar.UseVisualStyleBackColor = true;
            btnAgrupar.Click += btnAgrupar_Click;
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(586, 12);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(202, 23);
            txtBuscar.TabIndex = 4;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(538, 15);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 5;
            label1.Text = "Buscar";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(txtBuscar);
            Controls.Add(btnAgrupar);
            Controls.Add(btnCargarSQL);
            Controls.Add(dgvDatos);
            Controls.Add(btnCargarArchivo);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvDatos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCargarArchivo;
        private DataGridView dgvDatos;
        private Button btnCargarSQL;
        private Button btnAgrupar;
        private TextBox txtBuscar;
        private Label label1;
    }
}
