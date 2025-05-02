namespace PatentApplicationManager
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.btnAddApplication = new System.Windows.Forms.Button();
            this.dgvPatentApplications = new System.Windows.Forms.DataGridView();
            this.colEdit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnCreateDatabase = new System.Windows.Forms.Button();
            this.btnDeleteDatabase = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExportDatabase = new System.Windows.Forms.Button();
            this.btnImportDatabase = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentApplications)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            this.btnGenerateReport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGenerateReport.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGenerateReport.Location = new System.Drawing.Point(289, 120);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(215, 71);
            this.btnGenerateReport.TabIndex = 9;
            this.btnGenerateReport.Text = "Сформировать отчёт (PDF)";
            this.btnGenerateReport.UseVisualStyleBackColor = false;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(51)))));
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(228)))), ((int)(((byte)(226)))));
            this.label1.Location = new System.Drawing.Point(-4, 287);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(586, 32);
            this.label1.TabIndex = 8;
            this.label1.Text = "Текущее состояние базы данных заявок на патенты:";
            // 
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(51)))));
            this.lblError.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(228)))), ((int)(((byte)(226)))));
            this.lblError.Location = new System.Drawing.Point(1089, 13);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(412, 70);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "ТЕМА ПРИЛОЖЕНИЯ: Заявки на патенты. АВТОР: Шалыгин А.В.";
            // 
            // btnAddApplication
            // 
            this.btnAddApplication.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            this.btnAddApplication.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddApplication.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAddApplication.Location = new System.Drawing.Point(12, 120);
            this.btnAddApplication.Name = "btnAddApplication";
            this.btnAddApplication.Size = new System.Drawing.Size(215, 71);
            this.btnAddApplication.TabIndex = 6;
            this.btnAddApplication.Text = "Добавить заявку";
            this.btnAddApplication.UseVisualStyleBackColor = false;
            this.btnAddApplication.Click += new System.EventHandler(this.btnAddApplication_Click);
            // 
            // dgvPatentApplications
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.AliceBlue;
            this.dgvPatentApplications.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPatentApplications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPatentApplications.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPatentApplications.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPatentApplications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPatentApplications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEdit});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPatentApplications.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPatentApplications.Location = new System.Drawing.Point(1, 334);
            this.dgvPatentApplications.Name = "dgvPatentApplications";
            this.dgvPatentApplications.RowHeadersWidth = 51;
            this.dgvPatentApplications.RowTemplate.Height = 24;
            this.dgvPatentApplications.Size = new System.Drawing.Size(1500, 51);
            this.dgvPatentApplications.TabIndex = 5;
            this.dgvPatentApplications.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPatentApplications_CellClick);
            this.dgvPatentApplications.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvPatentApplications_ColumnHeaderMouseClick);
            // 
            // colEdit
            // 
            this.colEdit.HeaderText = "Редактировать";
            this.colEdit.MinimumWidth = 6;
            this.colEdit.Name = "colEdit";
            this.colEdit.ReadOnly = true;
            this.colEdit.Text = "Редактировать";
            this.colEdit.UseColumnTextForButtonValue = true;
            this.colEdit.Width = 125;
            // 
            // btnCreateDatabase
            // 
            this.btnCreateDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            this.btnCreateDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCreateDatabase.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCreateDatabase.Location = new System.Drawing.Point(12, 12);
            this.btnCreateDatabase.Name = "btnCreateDatabase";
            this.btnCreateDatabase.Size = new System.Drawing.Size(215, 71);
            this.btnCreateDatabase.TabIndex = 10;
            this.btnCreateDatabase.Text = "Создать базу данных";
            this.btnCreateDatabase.UseVisualStyleBackColor = false;
            this.btnCreateDatabase.Click += new System.EventHandler(this.btnCreateDatabase_Click);
            // 
            // btnDeleteDatabase
            // 
            this.btnDeleteDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.btnDeleteDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteDatabase.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDeleteDatabase.Location = new System.Drawing.Point(838, 12);
            this.btnDeleteDatabase.Name = "btnDeleteDatabase";
            this.btnDeleteDatabase.Size = new System.Drawing.Size(215, 71);
            this.btnDeleteDatabase.TabIndex = 11;
            this.btnDeleteDatabase.Text = "Удалить базу данных";
            this.btnDeleteDatabase.UseVisualStyleBackColor = false;
            this.btnDeleteDatabase.Click += new System.EventHandler(this.btnDeleteDatabase_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(51)))));
            this.txtSearch.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(228)))), ((int)(((byte)(226)))));
            this.txtSearch.Location = new System.Drawing.Point(561, 154);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(492, 37);
            this.txtSearch.TabIndex = 13;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            this.txtSearch.Enter += new System.EventHandler(this.txtSearch_Enter);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(1, 197);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1500, 10);
            this.panel1.TabIndex = 14;
            // 
            // btnExportDatabase
            // 
            this.btnExportDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            this.btnExportDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExportDatabase.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExportDatabase.Location = new System.Drawing.Point(289, 13);
            this.btnExportDatabase.Name = "btnExportDatabase";
            this.btnExportDatabase.Size = new System.Drawing.Size(215, 71);
            this.btnExportDatabase.TabIndex = 17;
            this.btnExportDatabase.Text = "Экспорт БД";
            this.btnExportDatabase.UseVisualStyleBackColor = false;
            this.btnExportDatabase.Click += new System.EventHandler(this.btnExportDatabase_Click_1);
            // 
            // btnImportDatabase
            // 
            this.btnImportDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(236)))), ((int)(((byte)(182)))));
            this.btnImportDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportDatabase.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnImportDatabase.Location = new System.Drawing.Point(561, 12);
            this.btnImportDatabase.Name = "btnImportDatabase";
            this.btnImportDatabase.Size = new System.Drawing.Size(215, 71);
            this.btnImportDatabase.TabIndex = 18;
            this.btnImportDatabase.Text = "Импорт БД";
            this.btnImportDatabase.UseVisualStyleBackColor = false;
            this.btnImportDatabase.Click += new System.EventHandler(this.BtnImportDatabase_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(33)))), ((int)(((byte)(61)))));
            this.ClientSize = new System.Drawing.Size(1496, 622);
            this.Controls.Add(this.btnImportDatabase);
            this.Controls.Add(this.btnExportDatabase);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnDeleteDatabase);
            this.Controls.Add(this.btnCreateDatabase);
            this.Controls.Add(this.btnGenerateReport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnAddApplication);
            this.Controls.Add(this.dgvPatentApplications);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatentApplications)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnAddApplication;
        private System.Windows.Forms.DataGridView dgvPatentApplications;
        private System.Windows.Forms.DataGridViewButtonColumn colEdit;
        private System.Windows.Forms.Button btnCreateDatabase;
        private System.Windows.Forms.Button btnDeleteDatabase;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExportDatabase;
        private System.Windows.Forms.Button btnImportDatabase;
    }
}

