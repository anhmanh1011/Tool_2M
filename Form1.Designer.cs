namespace Tool_2M
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
            this.lb_page_name = new System.Windows.Forms.Label();
            this.txt_page_name = new System.Windows.Forms.TextBox();
            this.login = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.STT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.haifa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.token = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cookie = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_request = new System.Windows.Forms.Button();
            this.btn_reg_page = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nb_number_thread = new System.Windows.Forms.NumericUpDown();
            this.btn_reg_page_request = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_file_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_add_camp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nb_number_thread)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_page_name
            // 
            this.lb_page_name.AutoSize = true;
            this.lb_page_name.Location = new System.Drawing.Point(39, 25);
            this.lb_page_name.Name = "lb_page_name";
            this.lb_page_name.Size = new System.Drawing.Size(85, 20);
            this.lb_page_name.TabIndex = 0;
            this.lb_page_name.Text = "Page Name";
            // 
            // txt_page_name
            // 
            this.txt_page_name.Location = new System.Drawing.Point(131, 21);
            this.txt_page_name.Name = "txt_page_name";
            this.txt_page_name.Size = new System.Drawing.Size(125, 27);
            this.txt_page_name.TabIndex = 1;
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(885, 37);
            this.login.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(86, 31);
            this.login.TabIndex = 4;
            this.login.Text = "Login";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv.ColumnHeadersHeight = 29;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.STT,
            this.uid,
            this.pass,
            this.haifa,
            this.token,
            this.cookie,
            this.status});
            this.dgv.Location = new System.Drawing.Point(1, 165);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidth = 51;
            this.dgv.RowTemplate.Height = 29;
            this.dgv.Size = new System.Drawing.Size(979, 340);
            this.dgv.TabIndex = 5;
            this.dgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellContentClick);
            this.dgv.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgv_KeyUp);
            // 
            // STT
            // 
            this.STT.HeaderText = "STT";
            this.STT.MinimumWidth = 6;
            this.STT.Name = "STT";
            // 
            // uid
            // 
            this.uid.HeaderText = "UID";
            this.uid.MinimumWidth = 6;
            this.uid.Name = "uid";
            // 
            // pass
            // 
            this.pass.HeaderText = "PASS";
            this.pass.MinimumWidth = 6;
            this.pass.Name = "pass";
            // 
            // haifa
            // 
            this.haifa.HeaderText = "2FA";
            this.haifa.MinimumWidth = 6;
            this.haifa.Name = "haifa";
            // 
            // token
            // 
            this.token.HeaderText = "Token";
            this.token.MinimumWidth = 6;
            this.token.Name = "token";
            // 
            // cookie
            // 
            this.cookie.HeaderText = "Cookie";
            this.cookie.MinimumWidth = 6;
            this.cookie.Name = "cookie";
            // 
            // status
            // 
            this.status.HeaderText = "Trạng thái";
            this.status.MinimumWidth = 6;
            this.status.Name = "status";
            // 
            // btn_request
            // 
            this.btn_request.Location = new System.Drawing.Point(278, 63);
            this.btn_request.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_request.Name = "btn_request";
            this.btn_request.Size = new System.Drawing.Size(125, 31);
            this.btn_request.TabIndex = 6;
            this.btn_request.Text = "login_request";
            this.btn_request.UseVisualStyleBackColor = true;
            this.btn_request.Click += new System.EventHandler(this.btn_request_Click);
            // 
            // btn_reg_page
            // 
            this.btn_reg_page.Location = new System.Drawing.Point(885, 76);
            this.btn_reg_page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_reg_page.Name = "btn_reg_page";
            this.btn_reg_page.Size = new System.Drawing.Size(86, 31);
            this.btn_reg_page.TabIndex = 7;
            this.btn_reg_page.Text = "Reg Page";
            this.btn_reg_page.UseVisualStyleBackColor = true;
            this.btn_reg_page.Click += new System.EventHandler(this.btn_reg_page_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Số luồng";
            // 
            // nb_number_thread
            // 
            this.nb_number_thread.Location = new System.Drawing.Point(131, 63);
            this.nb_number_thread.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nb_number_thread.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nb_number_thread.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nb_number_thread.Name = "nb_number_thread";
            this.nb_number_thread.Size = new System.Drawing.Size(126, 27);
            this.nb_number_thread.TabIndex = 10;
            this.nb_number_thread.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // btn_reg_page_request
            // 
            this.btn_reg_page_request.Location = new System.Drawing.Point(278, 21);
            this.btn_reg_page_request.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_reg_page_request.Name = "btn_reg_page_request";
            this.btn_reg_page_request.Size = new System.Drawing.Size(125, 31);
            this.btn_reg_page_request.TabIndex = 11;
            this.btn_reg_page_request.Text = "reg page request";
            this.btn_reg_page_request.UseVisualStyleBackColor = true;
            this.btn_reg_page_request.Click += new System.EventHandler(this.btn_reg_page_request_Click);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(278, 101);
            this.btn_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(125, 31);
            this.btn_save.TabIndex = 12;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_file_name
            // 
            this.txt_file_name.Location = new System.Drawing.Point(131, 103);
            this.txt_file_name.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_file_name.Name = "txt_file_name";
            this.txt_file_name.Size = new System.Drawing.Size(125, 27);
            this.txt_file_name.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "File Name";
            // 
            // btn_add_camp
            // 
            this.btn_add_camp.Location = new System.Drawing.Point(444, 25);
            this.btn_add_camp.Name = "btn_add_camp";
            this.btn_add_camp.Size = new System.Drawing.Size(94, 29);
            this.btn_add_camp.TabIndex = 15;
            this.btn_add_camp.Text = "Len camp";
            this.btn_add_camp.UseVisualStyleBackColor = true;
            this.btn_add_camp.Click += new System.EventHandler(this.btn_add_camp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 508);
            this.Controls.Add(this.btn_add_camp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_file_name);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_reg_page_request);
            this.Controls.Add(this.nb_number_thread);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_reg_page);
            this.Controls.Add(this.btn_request);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.login);
            this.Controls.Add(this.txt_page_name);
            this.Controls.Add(this.lb_page_name);
            this.Name = "Form1";
            this.Text = "Tool_khoa_hoc";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nb_number_thread)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lb_page_name;
        private TextBox txt_page_name;
        private Button login;
        private DataGridView dgv;
        private Button btn_request;
        private DataGridViewTextBoxColumn STT;
        private DataGridViewTextBoxColumn uid;
        private DataGridViewTextBoxColumn pass;
        private DataGridViewTextBoxColumn haifa;
        private DataGridViewTextBoxColumn token;
        private DataGridViewTextBoxColumn cookie;
        private DataGridViewTextBoxColumn status;
        private Button btn_reg_page;
        private Label label1;
        public NumericUpDown nb_number_thread;
        private Button btn_reg_page_request;
        private Button btn_save;
        private TextBox txt_file_name;
        private Label label2;
        private Button btn_add_camp;
    }
}