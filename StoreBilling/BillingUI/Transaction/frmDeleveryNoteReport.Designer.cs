﻿namespace AC.Billing.UI.Transaction
{
    partial class frmDeleveryNoteReport
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Customer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryMode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstimatedDeliveryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaxAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DiscountAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnPrintPreview = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtEndDate = new System.Windows.Forms.DateTimePicker();
            this.txtStartDate = new System.Windows.Forms.DateTimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Customer,
            this.DeliveryMode,
            this.DeliveryDate,
            this.EstimatedDeliveryDate,
            this.TaxAmount,
            this.DiscountAmount,
            this.TotalAmount});
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1045, 152);
            this.dataGridView1.TabIndex = 130;
            // 
            // Customer
            // 
            this.Customer.DataPropertyName = "CustomerName";
            this.Customer.HeaderText = "Customer";
            this.Customer.Name = "Customer";
            this.Customer.ReadOnly = true;
            // 
            // DeliveryMode
            // 
            this.DeliveryMode.DataPropertyName = "DeliveryMode";
            this.DeliveryMode.HeaderText = "Delivery Mode";
            this.DeliveryMode.Name = "DeliveryMode";
            this.DeliveryMode.ReadOnly = true;
            // 
            // DeliveryDate
            // 
            this.DeliveryDate.DataPropertyName = "DeliveryDate";
            this.DeliveryDate.HeaderText = "Delivery Date";
            this.DeliveryDate.Name = "DeliveryDate";
            this.DeliveryDate.ReadOnly = true;
            this.DeliveryDate.Width = 110;
            // 
            // EstimatedDeliveryDate
            // 
            this.EstimatedDeliveryDate.DataPropertyName = "EstimatedDeliveryDate";
            this.EstimatedDeliveryDate.HeaderText = "EstimatedDeliveryDate";
            this.EstimatedDeliveryDate.Name = "EstimatedDeliveryDate";
            this.EstimatedDeliveryDate.ReadOnly = true;
            this.EstimatedDeliveryDate.Width = 130;
            // 
            // TaxAmount
            // 
            this.TaxAmount.DataPropertyName = "TaxAmount";
            this.TaxAmount.HeaderText = "TaxAmount";
            this.TaxAmount.Name = "TaxAmount";
            this.TaxAmount.ReadOnly = true;
            // 
            // DiscountAmount
            // 
            this.DiscountAmount.DataPropertyName = "DiscountAmount";
            this.DiscountAmount.HeaderText = "DiscountAmount";
            this.DiscountAmount.Name = "DiscountAmount";
            this.DiscountAmount.ReadOnly = true;
            // 
            // TotalAmount
            // 
            this.TotalAmount.DataPropertyName = "TotalAmount";
            this.TotalAmount.HeaderText = "TotalAmount";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.ReadOnly = true;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Location = new System.Drawing.Point(9, 174);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1091, 212);
            this.panel3.TabIndex = 133;
            // 
            // btnPrintPreview
            // 
            this.btnPrintPreview.Location = new System.Drawing.Point(102, 16);
            this.btnPrintPreview.Name = "btnPrintPreview";
            this.btnPrintPreview.Size = new System.Drawing.Size(90, 23);
            this.btnPrintPreview.TabIndex = 67;
            this.btnPrintPreview.Text = "Print Preview";
            this.btnPrintPreview.UseVisualStyleBackColor = true;
            this.btnPrintPreview.Click += new System.EventHandler(this.btnPrintPreview_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(208, 16);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 66;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(12, 16);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 48;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.btnPrintPreview);
            this.panel2.Controls.Add(this.btnPrint);
            this.panel2.Controls.Add(this.btnView);
            this.panel2.Location = new System.Drawing.Point(9, 106);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1091, 53);
            this.panel2.TabIndex = 132;
            // 
            // txtEndDate
            // 
            this.txtEndDate.Location = new System.Drawing.Point(426, 35);
            this.txtEndDate.Name = "txtEndDate";
            this.txtEndDate.Size = new System.Drawing.Size(118, 20);
            this.txtEndDate.TabIndex = 7;
            // 
            // txtStartDate
            // 
            this.txtStartDate.Location = new System.Drawing.Point(133, 35);
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.Size = new System.Drawing.Size(118, 20);
            this.txtStartDate.TabIndex = 8;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 35);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 13);
            this.label15.TabIndex = 61;
            this.label15.Text = "From Date";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(313, 35);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(46, 13);
            this.label16.TabIndex = 49;
            this.label16.Text = "To Date";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtEndDate);
            this.groupBox1.Controls.Add(this.txtStartDate);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1088, 88);
            this.groupBox1.TabIndex = 131;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delivery Note Report";
            // 
            // frmDeleveryNoteReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(1144, 632);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmDeleveryNoteReport";
            this.Text = "Delivery Note Report";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmDeleveryNoteReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnPrintPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker txtEndDate;
        private System.Windows.Forms.DateTimePicker txtStartDate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Customer;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstimatedDeliveryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaxAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiscountAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
    }
}