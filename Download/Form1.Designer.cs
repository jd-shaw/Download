namespace Download
{
    partial class DownloadWin
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadWin));
            this.scheduleLabel1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.downloadTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serverUrlText = new System.Windows.Forms.TextBox();
            this.requestBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.RequestHeaderGridView = new System.Windows.Forms.DataGridView();
            this.RequestHanderGridAdd = new System.Windows.Forms.Button();
            this.RequestType = new System.Windows.Forms.ComboBox();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequestHanderGridDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.RequestHeaderGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // scheduleLabel1
            // 
            this.scheduleLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.scheduleLabel1.AutoSize = true;
            this.scheduleLabel1.Location = new System.Drawing.Point(447, 417);
            this.scheduleLabel1.Name = "scheduleLabel1";
            this.scheduleLabel1.Size = new System.Drawing.Size(35, 12);
            this.scheduleLabel1.TabIndex = 25;
            this.scheduleLabel1.Text = "(0/0)";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 417);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "下载进度：";
            // 
            // downloadProgressBar
            // 
            this.downloadProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadProgressBar.Location = new System.Drawing.Point(99, 410);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(342, 23);
            this.downloadProgressBar.TabIndex = 23;
            // 
            // downloadTextBox
            // 
            this.downloadTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.downloadTextBox.Location = new System.Drawing.Point(99, 207);
            this.downloadTextBox.Multiline = true;
            this.downloadTextBox.Name = "downloadTextBox";
            this.downloadTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.downloadTextBox.Size = new System.Drawing.Size(401, 197);
            this.downloadTextBox.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 207);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "文件路径：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 20;
            this.label1.Text = "服务器地址：";
            // 
            // serverUrlText
            // 
            this.serverUrlText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverUrlText.Location = new System.Drawing.Point(99, 39);
            this.serverUrlText.Name = "serverUrlText";
            this.serverUrlText.Size = new System.Drawing.Size(345, 21);
            this.serverUrlText.TabIndex = 19;
            // 
            // requestBtn
            // 
            this.requestBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.requestBtn.Location = new System.Drawing.Point(224, 453);
            this.requestBtn.Name = "requestBtn";
            this.requestBtn.Size = new System.Drawing.Size(75, 23);
            this.requestBtn.TabIndex = 18;
            this.requestBtn.Text = "确定";
            this.requestBtn.UseVisualStyleBackColor = true;
            this.requestBtn.Click += new System.EventHandler(this.requestBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "服务器参数：";
            // 
            // RequestHeaderGridView
            // 
            this.RequestHeaderGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RequestHeaderGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.RequestHeaderGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.RequestHeaderGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RequestHeaderGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.name});
            this.RequestHeaderGridView.Location = new System.Drawing.Point(99, 66);
            this.RequestHeaderGridView.Name = "RequestHeaderGridView";
            this.RequestHeaderGridView.RowTemplate.Height = 23;
            this.RequestHeaderGridView.Size = new System.Drawing.Size(401, 135);
            this.RequestHeaderGridView.TabIndex = 27;
            // 
            // RequestHanderGridAdd
            // 
            this.RequestHanderGridAdd.Location = new System.Drawing.Point(67, 97);
            this.RequestHanderGridAdd.Name = "RequestHanderGridAdd";
            this.RequestHanderGridAdd.Size = new System.Drawing.Size(26, 23);
            this.RequestHanderGridAdd.TabIndex = 28;
            this.RequestHanderGridAdd.Text = "+";
            this.RequestHanderGridAdd.UseVisualStyleBackColor = true;
            this.RequestHanderGridAdd.Click += new System.EventHandler(this.RequestHanderGridAdd_Click);
            // 
            // RequestType
            // 
            this.RequestType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RequestType.DisplayMember = "GET";
            this.RequestType.FormattingEnabled = true;
            this.RequestType.Items.AddRange(new object[] {
            "GET",
            "POST"});
            this.RequestType.Location = new System.Drawing.Point(450, 39);
            this.RequestType.Name = "RequestType";
            this.RequestType.Size = new System.Drawing.Size(50, 20);
            this.RequestType.TabIndex = 29;
            this.RequestType.ValueMember = "GET";
            // 
            // type
            // 
            this.type.HeaderText = "属性";
            this.type.Name = "type";
            this.type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // name
            // 
            this.name.HeaderText = "值";
            this.name.Name = "name";
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RequestHanderGridDelete
            // 
            this.RequestHanderGridDelete.Location = new System.Drawing.Point(67, 126);
            this.RequestHanderGridDelete.Name = "RequestHanderGridDelete";
            this.RequestHanderGridDelete.Size = new System.Drawing.Size(26, 23);
            this.RequestHanderGridDelete.TabIndex = 30;
            this.RequestHanderGridDelete.Text = "-";
            this.RequestHanderGridDelete.UseVisualStyleBackColor = true;
            this.RequestHanderGridDelete.Click += new System.EventHandler(this.RequestHanderGridDelete_Click);
            // 
            // DownloadWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 505);
            this.Controls.Add(this.RequestHanderGridDelete);
            this.Controls.Add(this.RequestType);
            this.Controls.Add(this.RequestHanderGridAdd);
            this.Controls.Add(this.RequestHeaderGridView);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.scheduleLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.downloadProgressBar);
            this.Controls.Add(this.downloadTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverUrlText);
            this.Controls.Add(this.requestBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DownloadWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下载器";
            this.Load += new System.EventHandler(this.DownloadWin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RequestHeaderGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label scheduleLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
        private System.Windows.Forms.TextBox downloadTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverUrlText;
        private System.Windows.Forms.Button requestBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView RequestHeaderGridView;
        private System.Windows.Forms.Button RequestHanderGridAdd;
        private System.Windows.Forms.ComboBox RequestType;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.Button RequestHanderGridDelete;
    }
}

