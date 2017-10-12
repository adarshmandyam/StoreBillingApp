using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BillingUI;
using AC.Billing.Business;
using System.Configuration;
namespace AC.Billing.UI.Transaction
{
    public partial class frmQuotation : BaseClass
    {
        #region Member Variables
        // for PrintDialog, PrintPreviewDialog and PrintDocument:
        private System.Windows.Forms.PrintDialog prnDialog;
        private System.Windows.Forms.PrintPreviewDialog prnPreview;
        private System.Drawing.Printing.PrintDocument prnDocument;
        // private System.ComponentModel.Container component = null;

        // for Invoice Head:
        private string InvTitle;
        private string InvSubTitle1;
        private string InvSubTitle2;
        private string InvSubTitle3;
        private string InvImage;

        // for Report:
        private int CurrentY;
        private int CurrentX;
        private int leftMargin;
        private int rightMargin;
        private int topMargin;
        private int bottomMargin;
        private int InvoiceWidth;
        private int InvoiceHeight;
        private string CustomerNameQuote;
        private string CustomerCity;
        private string SellerName;
        private string SaleID;
        private string SaleDate;
        private decimal SaleFreight;
        private decimal SubTotal;
        private decimal InvoiceTotal;
        private bool ReadInvoice;
        private int AmountPosition;

        // Font and Color:------------------
        // Title Font
        private Font InvTitleFont = new Font("Arial", 24, FontStyle.Regular);
        // Title Font height
        private int InvTitleHeight;
        // SubTitle Font
        private Font InvSubTitleFont = new Font("Arial", 14, FontStyle.Regular);
        // SubTitle Font height
        private int InvSubTitleHeight;
        // Invoice Font
        private Font InvoiceFont = new Font("Arial", 12, FontStyle.Regular);
        // Invoice Font height
        private int InvoiceFontHeight;
        // Blue Color
        private SolidBrush BlueBrush = new SolidBrush(Color.Blue);
        // Red Color
        private SolidBrush RedBrush = new SolidBrush(Color.Red);
        // Black Color
        private SolidBrush BlackBrush = new SolidBrush(Color.Black);

        #endregion


        #region [Variable Declaration]  

        QuotationBLL quotationBLL;
        SizeBLL sizeBLL;
        int UserId = 1;
        int QuotationID=0;
        DataTable dtQuotationItem;
        ProductBLL productBLL;
        #endregion [Variable Declaration]  
        #region [Events] 

        public frmQuotation()
        {
            this.prnDialog = new System.Windows.Forms.PrintDialog();
            this.prnPreview = new System.Windows.Forms.PrintPreviewDialog();
            this.prnDocument = new System.Drawing.Printing.PrintDocument();
            // The Event of 'PrintPage'
            prnDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(prnDocument_PrintPage);
            sizeBLL = new SizeBLL(dbHelper);
            quotationBLL = new QuotationBLL(dbHelper);
            productBLL = new ProductBLL(dbHelper);
            InitializeComponent();
            gvQuotation.CellContentClick += gvQuotation_CellContentClick;
            FillCombo();
            FillGrid();
        }

        private void FillDataGridForPrint()
        {
            DataTable dtInvoice = new DataTable();
            dtInvoice = quotationBLL.GetAllQuotation(QuotationID);
            if (dtInvoice.Rows.Count > 0)
            {

                CustomerNameQuote = Convert.ToString(dtInvoice.Rows[0]["CustomerName"]);
                //txtContactNumber.Text = Convert.ToString(dtInvoice.Rows[0]["ContactNumber"]);
                //txtEmail.Text = Convert.ToString(dtInvoice.Rows[0]["Email"]);
                //TxtWebSite.Text = Convert.ToString(dtInvoice.Rows[0]["Website"]);
                //dtInvoiceDate.Text = Convert.ToString(dtInvoice.Rows[0]["InvoiceDate"]);
                //dtPaymentExDate.Text = Convert.ToString(dtInvoice.Rows[0]["PaymentExpectedBy"]);
                //txtRemark.Text = Convert.ToString(dtInvoice.Rows[0]["Remarks"]);
                //cmbPaymentMode.SelectedValue = Convert.ToString(dtInvoice.Rows[0]["PaymentModeID"]);
                //cmbIsOnCredit.SelectedValue = dtInvoice.Rows[0]["IsOnCredit"].ToString() == "Yes" ? "1" : "0";
                //cmbIsPaid.SelectedValue = dtInvoice.Rows[0]["IsPaid"].ToString() == "Yes" ? "1" : "0";
            }

            DataTable dTable = new DataTable();
            dTable = quotationBLL.GetAllQuotationItem(QuotationID, 0);

            ordGrid.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();

            foreach (DataColumn dc in dTable.Columns)
            {
                DataGridTextBoxColumn txtColumn = new DataGridTextBoxColumn();
                txtColumn.MappingName = dc.ColumnName;
                txtColumn.HeaderText = dc.Caption;
                switch (dc.ColumnName.ToString())
                {
                    case "ProductID":   // Product ID 
                        txtColumn.HeaderText = "Product ID";
                        txtColumn.Width = 60;
                        break;

                    case "ProductDesc":   // Product Name 
                        txtColumn.HeaderText = "Product Name";
                        txtColumn.Width = 110;
                        break;
                    case "RatePerUnit":   // Unit Price 
                        txtColumn.HeaderText = "Rate Per Unit";
                        txtColumn.Format = "0.00";
                        txtColumn.Alignment = HorizontalAlignment.Right;
                        txtColumn.Width = 60;
                        break;
                    case "Discount":   // Discount 
                        txtColumn.HeaderText = "Discount";
                        txtColumn.Format = "p"; // Percent
                        txtColumn.Alignment = HorizontalAlignment.Right;
                        txtColumn.Width = 60;
                        break;
                    case "Quantity":   // Quantity 
                        txtColumn.HeaderText = "Quantity";
                        txtColumn.Alignment = HorizontalAlignment.Right;
                        txtColumn.Width = 50;
                        break;
                    case "TaxAmount":   // Extended Price 
                        txtColumn.HeaderText = "Tax Amount";
                        txtColumn.Format = "0.00";
                        txtColumn.Alignment = HorizontalAlignment.Right;
                        txtColumn.Width = 90;
                        break;
                    case "TotalAmount":   // Extended Price 
                        txtColumn.HeaderText = "Total Amount";
                        txtColumn.Format = "0.00";
                        txtColumn.Alignment = HorizontalAlignment.Right;
                        txtColumn.Width = 90;
                        break;
                }
                tableStyle.GridColumnStyles.Add(txtColumn);
            }

            tableStyle.MappingName = dTable.TableName;
            ordGrid.TableStyles.Add(tableStyle);
            //set DataSource of DataGrid 
            ordGrid.DataSource = dTable.DefaultView;
        }

        private void SetInvoiceHead(Graphics g)
        {
            ReadInvoiceHead();

            CurrentY = topMargin;
            CurrentX = leftMargin;
            int ImageHeight = 0;

            // Draw Invoice image:
            if (System.IO.File.Exists(InvImage))
            {
                Bitmap oInvImage = new Bitmap(InvImage);
                // Set Image Left to center Image:
                int xImage = CurrentX + (InvoiceWidth - oInvImage.Width) / 2;
                ImageHeight = oInvImage.Height; // Get Image Height
                g.DrawImage(oInvImage, xImage, CurrentY);
            }

            InvTitleHeight = (int)(InvTitleFont.GetHeight(g));
            InvSubTitleHeight = (int)(InvSubTitleFont.GetHeight(g));

            // Get Titles Length:
            int lenInvTitle = (int)g.MeasureString(InvTitle, InvTitleFont).Width;
            int lenInvSubTitle1 = (int)g.MeasureString(InvSubTitle1, InvSubTitleFont).Width;
            int lenInvSubTitle2 = (int)g.MeasureString(InvSubTitle2, InvSubTitleFont).Width;
            int lenInvSubTitle3 = (int)g.MeasureString(InvSubTitle3, InvSubTitleFont).Width;
            // Set Titles Left:
            int xInvTitle = CurrentX + (InvoiceWidth - lenInvTitle) / 2;
            int xInvSubTitle1 = CurrentX + (InvoiceWidth - lenInvSubTitle1) / 2;
            int xInvSubTitle2 = CurrentX + (InvoiceWidth - lenInvSubTitle2) / 2;
            int xInvSubTitle3 = CurrentX + (InvoiceWidth - lenInvSubTitle3) / 2;

            // Draw Invoice Head:
            if (InvTitle != "")
            {
                CurrentY = CurrentY + ImageHeight;
                g.DrawString(InvTitle, InvTitleFont, BlueBrush, xInvTitle, CurrentY);
            }
            if (InvSubTitle1 != "")
            {
                CurrentY = CurrentY + InvTitleHeight;
                g.DrawString(InvSubTitle1, InvSubTitleFont, BlueBrush, xInvSubTitle1, CurrentY);
            }
            if (InvSubTitle2 != "")
            {
                CurrentY = CurrentY + InvSubTitleHeight;
                g.DrawString(InvSubTitle2, InvSubTitleFont, BlueBrush, xInvSubTitle2, CurrentY);
            }
            if (InvSubTitle3 != "")
            {
                CurrentY = CurrentY + InvSubTitleHeight;
                g.DrawString(InvSubTitle3, InvSubTitleFont, BlueBrush, xInvSubTitle3, CurrentY);
            }

            // Draw line:
            CurrentY = CurrentY + InvSubTitleHeight + 8;
            g.DrawLine(new Pen(Brushes.Black, 2), CurrentX, CurrentY, rightMargin, CurrentY);
        }

        private void SetOrderData(Graphics g)
        {// Set Company Name, City, Salesperson, Order ID and Order Date
            string FieldValue = "";
            InvoiceFontHeight = (int)(InvoiceFont.GetHeight(g));
            // Set Company Name:
            CurrentX = leftMargin;
            CurrentY = CurrentY + 8;
            FieldValue = "Customer Name: " + CustomerNameQuote;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            //// Set City:
            //CurrentX = CurrentX + (int)g.MeasureString(FieldValue, InvoiceFont).Width + 16;
            //FieldValue = "City: " + CustomerCity;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            //// Set Salesperson:
            //CurrentX = leftMargin;
            //CurrentY = CurrentY + InvoiceFontHeight;
            //FieldValue = "Salesperson: " + SellerName;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            //// Set Order ID:
            //CurrentX = leftMargin;
            //CurrentY = CurrentY + InvoiceFontHeight;
            //FieldValue = "Order ID: " + SaleID;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            //// Set Order Date:
            //CurrentX = CurrentX + (int)g.MeasureString(FieldValue, InvoiceFont).Width + 16;
            //FieldValue = "Order Date: " + SaleDate;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);

            // Draw line:
            CurrentY = CurrentY + InvoiceFontHeight + 8;
            g.DrawLine(new Pen(Brushes.Black), leftMargin, CurrentY, rightMargin, CurrentY);
        }

        private void SetInvoiceData(Graphics g, System.Drawing.Printing.PrintPageEventArgs e)
        {// Set Invoice Table:
            string FieldValue = "";
            // int CurrentRecord = 0;
            //  int RecordsPerPage = 20; // twenty items in a page
            decimal Amount = 0;
            // bool StopReading = false;
            // Product,RatePerUnit,Discount,Quantity,TaxAmount
            // Set Table Head:

            int xProductName = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            g.DrawString("Product Name", InvoiceFont, BlueBrush, xProductName, CurrentY);

            int xQuantity = xProductName + (int)g.MeasureString("Product Name", InvoiceFont).Width + 20;
            g.DrawString("Quantity", InvoiceFont, BlueBrush, xQuantity, CurrentY);

            int xUnitPrice = xQuantity + (int)g.MeasureString("Quantity", InvoiceFont).Width + 20;
            g.DrawString("Rate Per Unit", InvoiceFont, BlueBrush, xUnitPrice, CurrentY);

            int xDiscount = xUnitPrice + (int)g.MeasureString("Rate Per Unit", InvoiceFont).Width + 20;
            g.DrawString("Discount", InvoiceFont, BlueBrush, xDiscount, CurrentY);

            int xTaxAmt = xDiscount + (int)g.MeasureString("Discount", InvoiceFont).Width + 20;
            g.DrawString("Tax Amount", InvoiceFont, BlueBrush, xTaxAmt, CurrentY);


            AmountPosition = xTaxAmt + (int)g.MeasureString("Tax Amount", InvoiceFont).Width + 20;
            g.DrawString("Total Amount", InvoiceFont, BlueBrush, AmountPosition, CurrentY);


            //int xProductID = leftMargin;
            //CurrentY = CurrentY + InvoiceFontHeight;
            //g.DrawString("Product ID", InvoiceFont, BlueBrush, xProductID, CurrentY);

            //int xProductName = xProductID + (int)g.MeasureString("Product ID", InvoiceFont).Width + 4;
            //g.DrawString("Product Name", InvoiceFont, BlueBrush, xProductName, CurrentY);

            //int xUnitPrice = xProductName + (int)g.MeasureString("Product Name", InvoiceFont).Width + 10;
            //g.DrawString("Rate Per Unit", InvoiceFont, BlueBrush, xUnitPrice, CurrentY);

            //int xQuantity = xUnitPrice + (int)g.MeasureString("Rate Per Unit", InvoiceFont).Width + 4;
            //g.DrawString("Discount", InvoiceFont, BlueBrush, xQuantity, CurrentY);

            //int xDiscount = xQuantity + (int)g.MeasureString("Discount", InvoiceFont).Width + 4;
            //g.DrawString("Quantity", InvoiceFont, BlueBrush, xDiscount, CurrentY);


            //AmountPosition = xDiscount + (int)g.MeasureString("Quantity", InvoiceFont).Width + 4;
            //g.DrawString("Tax Amount", InvoiceFont, BlueBrush, AmountPosition, CurrentY);

            // Set Invoice Table:
            CurrentY = CurrentY + InvoiceFontHeight + 8;
            dtQuotationItem = quotationBLL.GetAllQuotationItem(QuotationID, 0);
            for (int i = 0; i < dtQuotationItem.Rows.Count; i++)
            {
                FieldValue = dtQuotationItem.Rows[i]["ProductDesc"].ToString();
                // if Length of (Product Name) > 20, Draw 20 character only
                if (FieldValue.Length > 20)
                    FieldValue = FieldValue.Remove(20, FieldValue.Length - 20);
                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xProductName, CurrentY);
                FieldValue = dtQuotationItem.Rows[i]["Quantity"].ToString();

                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xQuantity, CurrentY);
                FieldValue = String.Format("{0:0.00}", dtQuotationItem.Rows[i]["RatePerUnit"]);
                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xUnitPrice, CurrentY);
                FieldValue = dtQuotationItem.Rows[i]["Quantity"].ToString();
                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xQuantity, CurrentY);
                FieldValue = dtQuotationItem.Rows[i]["DiscountAmount"].ToString();
                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xDiscount, CurrentY);

                FieldValue = dtQuotationItem.Rows[i]["TaxAmount"].ToString();
                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xTaxAmt, CurrentY);

                Amount = Convert.ToDecimal(dtQuotationItem.Rows[i]["TotalAmount"]);
                // Format Extended Price and Align to Right:
                FieldValue = String.Format("{0:0.00}", Amount);
                int xAmount = AmountPosition + (int)g.MeasureString("Total Amount", InvoiceFont).Width;
                xAmount = xAmount - (int)g.MeasureString(FieldValue, InvoiceFont).Width;


                g.DrawString(FieldValue, InvoiceFont, BlackBrush, xAmount, CurrentY);
                CurrentY = CurrentY + InvoiceFontHeight;

            }
            SetInvoiceTotal(g);
            g.Dispose();
        }

        private void SetInvoiceTotal(Graphics g)
        {// Set Invoice Total:
         // Draw line:
            CurrentY = CurrentY + 8;
            g.DrawLine(new Pen(Brushes.Black), leftMargin, CurrentY, rightMargin, CurrentY);
            // Get Right Edge of Invoice:
            int xRightEdg = AmountPosition + (int)g.MeasureString("Tax Amount", InvoiceFont).Width;

            // Write Sub Total:
            //int xSubTotal = AmountPosition - (int)g.MeasureString("Sub Total", InvoiceFont).Width;
            //CurrentY = CurrentY + 8;
            //g.DrawString("Sub Total", InvoiceFont, RedBrush, xSubTotal, CurrentY);
            //string TotalValue = String.Format("{0:0.00}", SubTotal);
            //int xTotalValue = xRightEdg - (int)g.MeasureString(TotalValue, InvoiceFont).Width;
            //g.DrawString(TotalValue, InvoiceFont, BlackBrush, xTotalValue, CurrentY);

            //// Write Order Freight:
            //int xOrderFreight = AmountPosition - (int)g.MeasureString("Order Freight", InvoiceFont).Width;
            //CurrentY = CurrentY + InvoiceFontHeight;
            //g.DrawString("Order Freight", InvoiceFont, RedBrush, xOrderFreight, CurrentY);
            //string FreightValue = String.Format("{0:0.00}", SaleFreight);
            //int xFreight = xRightEdg - (int)g.MeasureString(FreightValue, InvoiceFont).Width;
            //g.DrawString(FreightValue, InvoiceFont, BlackBrush, xFreight, CurrentY);

            // Write Invoice Total:
            int xInvoiceTotal = AmountPosition - (int)g.MeasureString("Invoice Total", InvoiceFont).Width;
            CurrentY = CurrentY + InvoiceFontHeight;
            g.DrawString("Invoice Total", InvoiceFont, RedBrush, xInvoiceTotal, CurrentY);
            string InvoiceValue = String.Format("{0:0.00}", InvoiceTotal);
            int xInvoiceValue = xRightEdg - (int)g.MeasureString(InvoiceValue, InvoiceFont).Width;
            g.DrawString(InvoiceValue, InvoiceFont, BlackBrush, xInvoiceValue, CurrentY);
        }

        private void DisplayDialog()
        {
            try
            {
                prnDialog.Document = this.prnDocument;
                DialogResult ButtonPressed = prnDialog.ShowDialog();
                // If user Click 'OK', Print Invoice
                if (ButtonPressed == DialogResult.OK)
                    prnDocument.Print();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void DisplayInvoice()
        {
            prnPreview.Document = this.prnDocument;

            try
            {
                prnPreview.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void PrintReport()
        {
            try
            {
                prnDocument.Print();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void ReadInvoiceHead()
        {
            //Titles and Image of invoice:
            InvTitle = "Company Name";
            InvSubTitle1 = "Address 1";
            InvSubTitle2 = "Address 2";
            InvSubTitle3 = "Phone 2233445566";
            InvImage = Application.StartupPath + @"\Images\" + "InvPic.jpg";
        }
        private void prnDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            leftMargin = (int)e.MarginBounds.Left;
            rightMargin = (int)e.MarginBounds.Right;
            topMargin = (int)e.MarginBounds.Top;
            bottomMargin = (int)e.MarginBounds.Bottom;
            InvoiceWidth = (int)e.MarginBounds.Width;
            InvoiceHeight = (int)e.MarginBounds.Height;

            if (!ReadInvoice)
                FillDataGridForPrint();

            SetInvoiceHead(e.Graphics); // Draw Invoice Head
            SetOrderData(e.Graphics); // Draw Order Data
            SetInvoiceData(e.Graphics, e); // Draw Invoice Data

            ReadInvoice = true;
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count >= 1)
            {
                ReadInvoice = false;
                PrintReport(); // Print Invoice
            }
            else
            {
                MessageBox.Show("Please add an item and then click on Print.");
            }

        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count >= 1)
            {
                ReadInvoice = false;
                DisplayInvoice(); // Print Invoice
            }
            else
            {
                MessageBox.Show("Please add an item and then click on Print Preview");
            }


        }
        private void frmQuotation_Load(object sender, EventArgs e)
        {

            dtQuotationDate.Format = DateTimePickerFormat.Custom;
            dtQuotationDate.CustomFormat = ConfigurationManager.AppSettings["DateFormat"];

            dtPurchaseDate.Format = DateTimePickerFormat.Custom;
            dtPurchaseDate.CustomFormat = ConfigurationManager.AppSettings["DateFormat"];

        }

        private void gvQuotation_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView2.DataSource = null;
            var senderGrid = (DataGridView)sender;

            var aa = gvQuotation.CurrentCell.RowIndex;
            if (senderGrid.Columns[e.ColumnIndex].DisplayIndex == 7)
            {
                QuotationID = Convert.ToInt32(gvQuotation.Rows[aa].Cells[0].Value);

                DataTable dtQuotation = new DataTable();
                dtQuotation = quotationBLL.GetAllQuotation(QuotationID);
                if (dtQuotation != null)
                {
                    if (dtQuotation.Rows.Count > 0)
                    {
                        txtName.Text = Convert.ToString(dtQuotation.Rows[0]["CustomerName"]);
                        txtRemarks.Text = Convert.ToString(dtQuotation.Rows[0]["Remarks"]);                        
                        cmbPaymentMode.SelectedValue = Convert.ToString(dtQuotation.Rows[0]["PaymentModeID"]);

                        dtQuotationDate.Text = Convert.ToString(dtQuotation.Rows[0]["QuotationDate"]);
                        dtPurchaseDate.Text = Convert.ToString(dtQuotation.Rows[0]["PurchaseExpectedBy"]);

                    }

                    dtQuotationItem = new DataTable();
                    dtQuotationItem = quotationBLL.GetAllQuotationItem(QuotationID, 0);
                    dataGridView2.AutoGenerateColumns = false;
                    dataGridView2.DataSource = dtQuotationItem;
                    if (dtQuotationItem.Rows.Count > 0)
                    {
                        dataGridView2.Columns[3].ReadOnly = true;

                    }
                    txtTotal.Text = "";
                    decimal tltValue = 0;
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                        tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    }
                    txtTotal.Text = tltValue.ToString();
                    InvoiceTotal = Convert.ToDecimal(txtTotal.Text);
                    this.gvQuotation.AllowUserToAddRows = false;

                    GetProduct();
                    GetUnit();

                }
            }
            else if ((senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (senderGrid.Columns[e.ColumnIndex].DisplayIndex == 8 && e.RowIndex >= 0) && (!(String.IsNullOrEmpty(gvQuotation.Rows[aa].Cells[0].Value.ToString()))))
            {
                
                QuotationID = Convert.ToInt32(gvQuotation.Rows[aa].Cells[0].Value);
                bool check = quotationBLL.DeleteQuotation(QuotationID, 1);
                if (check == true)
                    MessageBox.Show("Selected Quotation Deleted Successfully.");
                else
                    MessageBox.Show("Selected Quotation cannot be deleted due to maaping with other masters.");

                ClearFields();
            }
            FillGrid();
        }
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            DataRow newRow;
            DataTable dtItem = new DataTable();
            DataTable tdExistRows = (DataTable)dataGridView2.DataSource;
         
            if (dtQuotationItem == null)
            {
                dtItem = new DataTable();
                dtItem.TableName = "InvoiceDetails";
                dtItem.Columns.Add("QuotationItemID", typeof(Int32));
                dtItem.Columns.Add("QuotationID", typeof(Int32));
                dtItem.Columns.Add("ProductID", typeof(Int32));
                dtItem.Columns.Add("Product", typeof(String));
                dtItem.Columns.Add("Make", typeof(String));
                dtItem.Columns.Add("Quantity", typeof(Decimal));
                dtItem.Columns.Add("UnitID", typeof(String));
                dtItem.Columns.Add("SizeId", typeof(Int32));
                dtItem.Columns.Add("Size", typeof(String));
                dtItem.Columns.Add("RatePerUnit", typeof(Decimal));
                dtItem.Columns.Add("TaxID", typeof(Int32));
                dtItem.Columns.Add("TaxPercentage", typeof(Int32));
                dtItem.Columns.Add("TaxAmount", typeof(Decimal));
                dtItem.Columns.Add("Discount", typeof(String));
                dtItem.Columns.Add("DiscountAmount", typeof(Decimal));
                dtItem.Columns.Add("TotalAmount", typeof(Decimal));
                dtItem.Columns.Add("Remark", typeof(String));
                dtItem.Columns.Add("Delete", typeof(Boolean));
                newRow = dtItem.NewRow();
                dtItem.Rows.Add(newRow);

                dataGridView2.AutoGenerateColumns = false;
              

                if (tdExistRows != null)
                {
                    newRow = tdExistRows.NewRow();
                    tdExistRows.Rows.Add(newRow);
                }
                else
                {
                    tdExistRows = dtItem;
                }
                dataGridView2.DataSource = tdExistRows;

                this.gvQuotation.AllowUserToAddRows = false;
                GetSingleProduct(dataGridView2.Rows.Count - 1);
                GetSingleUnit(dataGridView2.Rows.Count - 1);
            }
            else
            {
                newRow = tdExistRows.NewRow();
                tdExistRows.Rows.Add(newRow);
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.DataSource = tdExistRows;
                this.gvQuotation.AllowUserToAddRows = false;
                GetSingleProduct(dataGridView2.Rows.Count - 1);
                GetSingleUnit(dataGridView2.Rows.Count - 1);
            }
          

        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> selectedRows = (from row in dataGridView2.Rows.Cast<DataGridViewRow>()
                                                  select row).ToList();

            foreach (DataGridViewRow item in selectedRows)
            {
                if (item.Cells[0].Value != null)
                {
                    if (Convert.ToBoolean(item.Cells[0].Value) == true)
                        dataGridView2.Rows.RemoveAt(item.Index);
                }

            }
            txtTotal.Text = "";
            decimal tltValue = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                if (dataGridView2.Rows[i].Cells["TotalAmt"].Value.ToString() != "")
                    tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
            }
            txtTotal.Text = tltValue.ToString();
            InvoiceTotal = Convert.ToDecimal(txtTotal.Text);

        }


        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)e.Control).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)e.Control).AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == 6 && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += LastColumnComboSelectionChanged;
            }
            if ((dataGridView2.CurrentCell.ColumnIndex == 8) && e.Control is TextBox)
            {
                TextBox txtQuantity = e.Control as TextBox;
                txtQuantity.TextChanged += txtQuantity_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == 10 && e.Control is TextBox)
            {
                TextBox txtRate = e.Control as TextBox;
                txtRate.TextChanged += txtRate_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == 14 && e.Control is TextBox)
            {
                TextBox txtDiscount = e.Control as TextBox;
                txtDiscount.TextChanged += txtDiscount_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == 12 && e.Control is TextBox)
            {
                TextBox txtTax = e.Control as TextBox;
                txtTax.TextChanged += txtTax_TextChanged;
            }

            if (dataGridView2.CurrentCell.ColumnIndex == 9 && e.Control is ComboBox)
            {
                ComboBox comboBoxUnit = e.Control as ComboBox;
                comboBoxUnit.SelectedIndexChanged += LastColumnComboUnitSelectionChanged;
            }

        }
        private void LastColumnComboUnitSelectionChanged(object sender, EventArgs e)
        {
            var currentcell = dataGridView2.CurrentCellAddress;
            var sendingCB = sender as DataGridViewComboBoxEditingControl;

            DataGridViewComboBoxCell sizedll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[11];
            if (currentcell.X == 9)
            {
                if (sendingCB.SelectedValue != null && sendingCB.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    DataTable dt = new DataTable();
                    dt = sizeBLL.GetAllSize(null, Convert.ToInt32(sendingCB.SelectedValue));
                    dt.Rows.Add(new object[] { 0, "Select" });
                    dt.DefaultView.Sort = "SizeId ASC";

                    var selectValue = 0;

                    sizedll.DataSource = dt;
                    sizedll.ValueMember = "SizeID";
                    sizedll.DisplayMember = "SizeName";
                    sizedll.Value = selectValue;
                }
            }
        }

        // TextBox TextChanged Event
        private void txtTax_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 12)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[10];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[14];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[15];
                DataGridViewTextBoxCell Quantity = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[8];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[13];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[16];
                decimal outValue = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);

                decimal QuantityRate = Convert.ToDecimal(Quantity.Value) * Convert.ToDecimal(Rate.Value);
                decimal dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;

                decimal totalTax = totalafterDiscount / 100 * Convert.ToDecimal(outValue);
                TaxAmount.Value = totalTax;
                Total.Value = Convert.ToDecimal(totalafterDiscount + totalTax);
                txtTotal.Text = "";
                decimal tltValue = 0;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                }
                txtTotal.Text = tltValue.ToString();
                InvoiceTotal = Convert.ToDecimal(txtTotal.Text);
            }
        }
        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 14)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[10];
                DataGridViewTextBoxCell Quantity = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[8];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[15];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[12];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[13];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[16];

                decimal outValue = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);

                decimal QuantityRate = Convert.ToDecimal(Quantity.Value) * Convert.ToDecimal(Rate.Value);
                decimal dicountPer = QuantityRate / 100 * Convert.ToDecimal(outValue);
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;

                decimal totalTax = totalafterDiscount / 100 * Convert.ToDecimal(Tax.Value);
                TaxAmount.Value = totalTax;
                Total.Value = Convert.ToDecimal(totalafterDiscount + totalTax);
                txtTotal.Text = "";
                decimal tltValue = 0;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                }
                txtTotal.Text = tltValue.ToString();
                InvoiceTotal = Convert.ToDecimal(txtTotal.Text);
            }
        }
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 8)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[10];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[14];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[15];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[12];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[13];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[16];
                decimal outValue = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);
                decimal QuantityRate = outValue * Convert.ToDecimal(Rate.Value);
                decimal dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;

                decimal totalTax = totalafterDiscount / 100 * Convert.ToDecimal(Tax.Value);
                TaxAmount.Value = totalTax;
                Total.Value = Convert.ToDecimal(totalafterDiscount + totalTax);
                txtTotal.Text = "";
                decimal tltValue = 0;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                }
                txtTotal.Text = tltValue.ToString();
                InvoiceTotal = Convert.ToDecimal(txtTotal.Text);

            }
        }
        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 10)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var Rate = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell sendingTB = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[8];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[14];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[15];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[12];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[13];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[16];
                decimal outValue = 0;
                Decimal.TryParse(Rate.Text, out outValue);

                decimal QuantityRate = Convert.ToDecimal(sendingTB.Value) * outValue;
                decimal dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;

                decimal totalTax = totalafterDiscount / 100 * Convert.ToDecimal(Tax.Value);
                TaxAmount.Value = totalTax;
                Total.Value = Convert.ToDecimal(totalafterDiscount + totalTax);
                txtTotal.Text = "";
                decimal tltValue = 0;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    // txtTotal.Text += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    tltValue += Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                }
                txtTotal.Text = tltValue.ToString();
                InvoiceTotal = Convert.ToDecimal(txtTotal.Text);
            }
        }

        private void LastColumnComboSelectionChanged(object sender, EventArgs e)
        {
            var currentcell = dataGridView2.CurrentCellAddress;
            var sendingCB = sender as DataGridViewComboBoxEditingControl;
            DataGridViewTextBoxCell cel = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[4];
            DataGridViewComboBoxCell unitdll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[9];
            DataGridViewComboBoxCell sizedll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[11];
            if (currentcell.X == 6)
            {
                if (sendingCB.SelectedValue != null && sendingCB.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    cel.Value = sendingCB.SelectedValue;
                    DataTable dtProduct = new DataTable();
                    if (Convert.ToInt32(cel.Value) > 0)
                    {
                        dtProduct = productBLL.GetAllProduct(Convert.ToInt32(cel.Value));
                        var row = this.dataGridView2.Rows[currentcell.Y];
                        if (dtProduct != null)
                        {
                            if (dtProduct.Rows.Count > 0)
                            {
                                row.Cells["ProductID"].Value = dtProduct.Rows[0]["ProductID"];
                                row.Cells["SizeID"].Value = dtProduct.Rows[0]["SizeID"];
                                row.Cells["TaxID"].Value = dtProduct.Rows[0]["TaxID"];
                                row.Cells["Make"].Value = dtProduct.Rows[0]["Make"];
                                row.Cells[8].Value = 0;
                                row.Cells["UnitID"].Value = dtProduct.Rows[0]["UnitID"];
                                unitdll.Value = dtProduct.Rows[0]["UnitID"];

                                DataTable dt = new DataTable();
                                dt = sizeBLL.GetAllSize(null, Convert.ToInt32(dtProduct.Rows[0]["UnitID"]));
                                dt.Rows.Add(new object[] { 0, "Select" });
                                dt.DefaultView.Sort = "SizeId ASC";



                                sizedll.DataSource = dt;
                                sizedll.ValueMember = "SizeID";
                                sizedll.DisplayMember = "SizeName";


                                sizedll.Value = dtProduct.Rows[0]["SizeID"];
                                // row.Cells[10].Value = 0;
                                row.Cells["RatePerUnit"].Value = dtProduct.Rows[0]["RatePerUnit"];
                                //
                                // row.Cells[11].Value = dtProduct.Rows[0]["SizeName"];
                                row.Cells[12].Value = dtProduct.Rows[0]["TaxPercentage"];
                                row.Cells[13].Value = 0;
                                row.Cells[14].Value = dtProduct.Rows[0]["Discount"];
                                row.Cells[15].Value = 0;
                                row.Cells[16].Value = 0;
                                row.Cells[17].Value = dtProduct.Rows[0]["Remark"];

                            }
                        }

                    }
                }
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            //if (validation() == true)
            {

                DataTable dtQuotation = new DataTable();
                dtQuotation.TableName = "Quotation";
                dtQuotation.Columns.Add("CustomerName", typeof(String));             
                dtQuotation.Columns.Add("PaymentModeID", typeof(Int32));
                dtQuotation.Columns.Add("PaymentMode", typeof(String));
                dtQuotation.Columns.Add("QuotationDate", typeof(String));
                dtQuotation.Columns.Add("PurchaseExpectedBy", typeof(String));
                dtQuotation.Columns.Add("Remarks", typeof(String));

                DataRow rowQuotation = dtQuotation.NewRow();
                rowQuotation["CustomerName"] = txtName.Text.Trim();
                rowQuotation["PaymentModeID"] = cmbPaymentMode.SelectedValue;
                rowQuotation["PaymentMode"] = "Self";
                rowQuotation["QuotationDate"] = Convert.ToDateTime(dtQuotationDate.Value).ToString("MM-dd-yy");
                rowQuotation["PurchaseExpectedBy"] = Convert.ToDateTime(dtPurchaseDate.Value).ToString("MM-dd-yy");
                rowQuotation["Remarks"] = txtRemarks.Text.Trim();
                dtQuotation.Rows.Add(rowQuotation);


                DataTable dt1 = new DataTable();
                dt1.TableName = "QuotationDetails";
                dt1.Columns.Add("QuotationItemID", typeof(Int32));
                dt1.Columns.Add("QuotationID", typeof(Int32));
                dt1.Columns.Add("ProductID", typeof(Int32));
                dt1.Columns.Add("Product", typeof(String));
                dt1.Columns.Add("Make", typeof(String));
                dt1.Columns.Add("Quantity", typeof(Decimal));
                dt1.Columns.Add("UnitID", typeof(String));
                dt1.Columns.Add("SizeId", typeof(Int32));
                dt1.Columns.Add("Size", typeof(String));
                dt1.Columns.Add("RatePerUnit", typeof(Decimal));
                dt1.Columns.Add("TaxID", typeof(Int32));
                dt1.Columns.Add("TaxPercentage", typeof(Int32));
                dt1.Columns.Add("TaxAmount", typeof(Decimal));
                dt1.Columns.Add("Discount", typeof(Decimal));
                dt1.Columns.Add("DiscountAmount", typeof(Decimal));
                dt1.Columns.Add("TotalAmount", typeof(Decimal));
                dt1.Columns.Add("Remark", typeof(String));
                dt1.Columns.Add("Delete", typeof(Boolean));

                DataRow row;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    row = dt1.NewRow();
                    row["QuotationItemID"] = dataGridView2.Rows[i].Cells["QuotationItemID"].Value.ToString() == "" ? "0" : dataGridView2.Rows[i].Cells["QuotationItemID"].Value;
                    row["QuotationID"] = QuotationID;
                    row["ProductID"] = dataGridView2.Rows[i].Cells["Product"].Value;
                    row["Product"] = Convert.ToString(dataGridView2.Rows[i].Cells["Product"].Value);
                    row["Make"] = Convert.ToString(dataGridView2.Rows[i].Cells["Make"].Value);
                    row["Quantity"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Quantity"].Value);
                    row["UnitID"] = Convert.ToString(dataGridView2.Rows[i].Cells["Unit"].Value);
                    row["TaxID"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["TaxID"].Value);
                    row["SizeId"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["SizeID"].Value);
                    row["RatePerUnit"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["RateperUnit"].Value);

                    row["TaxPercentage"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Tax"].Value);
                    row["Discount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Discount"].Value);

                    row["TaxAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["TaxAmt"].Value);
                    row["DiscountAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DiscountAmt"].Value);
                    row["TotalAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    row["Remark"] = Convert.ToString(dataGridView2.Rows[i].Cells["Remark"].Value);
                    DataGridViewCheckBoxCell ch1 = new DataGridViewCheckBoxCell();
                    ch1 = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                    row["Delete"] = ch1.Value == null ? false : true;
                    dt1.Rows.Add(row);

                }

                 System.IO.StringWriter swSQL;
                StringBuilder sbSQL1 = new StringBuilder();
                swSQL = new System.IO.StringWriter(sbSQL1);
                dt1.WriteXml(swSQL);
                swSQL.Dispose();

                System.IO.StringWriter swSQL2;
                StringBuilder sbSQL2 = new StringBuilder();
                swSQL2 = new System.IO.StringWriter(sbSQL2);
                dtQuotation.WriteXml(swSQL2);
                swSQL2.Dispose();

                quotationBLL.QuotationSave(QuotationID, sbSQL2.ToString(), sbSQL1.ToString(), UserId);

                QuotationID = 0;
                ClearFields();
                FillGrid();
                this.dataGridView2.DataSource = null;

            }
        }
        #endregion [Events] 

        #region [FillGrid] 

        private void FillGrid()
        {
            DataTable dt = new DataTable();
            dt = quotationBLL.GetAllQuotation(0);
            GridBind(dt);
        }

        private void GridBind(DataTable dt)
        {
            gvQuotation.AutoGenerateColumns = false;
            dt.Columns.Add("Edit");
            gvQuotation.DataSource = dt;
            this.gvQuotation.AllowUserToAddRows = false;
        }

        #endregion [FillGrid]

        #region [Combo Fill] 

        private void GetAllPaymentMode()
        {
            DataTable dt = new DataTable();
            dt = quotationBLL.GetAllPaymentMode();
            cmbPaymentMode.DataSource = dt;
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "PaymentModeID";
            cmbPaymentMode.ValueMember = "PaymentModeID";
            cmbPaymentMode.DisplayMember = "PaymentMode";
            cmbPaymentMode.SelectedValue = 0;
        }

        private void GetProduct()
        {
            DataTable dt = new DataTable();
            dt = quotationBLL.GetProduct();
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "ProductID ASC";
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
                obj = (DataGridViewComboBoxCell)dataGridView2.Rows[i].Cells["Product"];
                var data = dataGridView2.Rows[i].Cells["ProductID"].Value;
                var selectValue = 0;
                if (data.ToString() != string.Empty)
                    selectValue = Convert.ToInt32(data);
                obj.DataSource = dt;
                obj.ValueMember = "ProductID";
                obj.DisplayMember = "ProductName";
                obj.Value = selectValue;
            }
        }
        private void FillCombo()
        {
            GetAllPaymentMode();            
            GetProduct();
            GetUnit();
        }
        private void GetSingleProduct(int index)
        {
            DataTable dt = new DataTable();
            dt = quotationBLL.GetProduct();
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "ProductID ASC";

            DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
            obj = (DataGridViewComboBoxCell)dataGridView2.Rows[index].Cells["Product"];
            var data = dataGridView2.Rows[index].Cells["ProductID"].Value;
            var selectValue = 0;
            if (data.ToString() != string.Empty)
                selectValue = Convert.ToInt32(data);
            obj.DataSource = dt;
            obj.ValueMember = "ProductID";
            obj.DisplayMember = "ProductName";
            obj.Value = selectValue;


        }
        private void GetSingleUnit(int Index)
        {
            DataTable dt = new DataTable();
            dt = productBLL.GetAllUnit(null);
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "UnitID ASC";

            DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
            obj = (DataGridViewComboBoxCell)dataGridView2.Rows[Index].Cells["Unit"];
            var data = dataGridView2.Rows[Index].Cells["UnitID"].Value;
            var selectValue = 0;
            if (data != null)
            {
                if (data.ToString() != string.Empty)
                    selectValue = Convert.ToInt32(data);
            }
            obj.DataSource = dt;
            obj.ValueMember = "UnitID";
            obj.DisplayMember = "Name";
            obj.Value = selectValue;
            GetSingleSize(selectValue, Index);


        }
        private void GetSize(int? unitId)
        {
            DataTable dt = new DataTable();
            dt = sizeBLL.GetAllSize(null, unitId);
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "SizeId ASC";
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
                obj = (DataGridViewComboBoxCell)dataGridView2.Rows[i].Cells["Size"];
                var data = dataGridView2.Rows[i].Cells["SizeId"].Value;
                var selectValue = 0;
                if (data != null)
                {
                    if (data.ToString() != string.Empty)
                        selectValue = Convert.ToInt32(data);
                }
                obj.DataSource = dt;
                obj.ValueMember = "SizeID";
                obj.DisplayMember = "SizeName";
                obj.Value = selectValue;

            }
        }
        private void GetSingleSize(int? unitId, int index)
        {
            DataTable dt = new DataTable();
            dt = sizeBLL.GetAllSize(null, unitId);
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "SizeId ASC";

            DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
            obj = (DataGridViewComboBoxCell)dataGridView2.Rows[index].Cells["Size"];
            var data = dataGridView2.Rows[index].Cells["SizeId"].Value;
            var selectValue = 0;
            if (data != null)
            {
                if (data.ToString() != string.Empty)
                    selectValue = Convert.ToInt32(data);
            }
            obj.DataSource = dt;
            obj.ValueMember = "SizeID";
            obj.DisplayMember = "SizeName";
            obj.Value = selectValue;


        }
        private void GetUnit()
        {
            DataTable dt = new DataTable();
            dt = productBLL.GetAllUnit(null);
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "UnitID ASC";
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewComboBoxCell obj = new DataGridViewComboBoxCell();
                obj = (DataGridViewComboBoxCell)dataGridView2.Rows[i].Cells["Unit"];
                var data = dataGridView2.Rows[i].Cells["UnitID"].Value;
                var selectValue = 0;
                if (data != null)
                {
                    if (data.ToString() != string.Empty)
                        selectValue = Convert.ToInt32(data);
                }
                obj.DataSource = dt;
                obj.ValueMember = "UnitID";
                obj.DisplayMember = "Name";
                obj.Value = selectValue;
                GetSize(selectValue);

            }
        }

        #endregion [Combo Fill] 

        #region [ClearFields] 

        private void ClearFields()
        {
            txtName.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            cmbPaymentMode.SelectedValue = 0;            
            dtQuotationDate.Value = DateTime.Now;
            dtPurchaseDate.Value = DateTime.Now;
            QuotationID = 0;
        }

        #endregion [ClearFields] 
        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Error happened " + e.ThrowException.ToString());
        }
        #region [Validation] 

        public void onlynumwithsinglepoint(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == '.'))
            { e.Handled = true; }
            TextBox txtDecimal = sender as TextBox;
            if (e.KeyChar == '.' && txtDecimal.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        public void onlynumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }




        #endregion [Validation] 

    }
}
