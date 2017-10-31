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
using AC.Billing.DataAccess;
using System.Configuration;
using System.Collections;

namespace AC.Billing.UI.Transaction
{
    public partial class Inventory : BaseClass
    {
        #region Member Variables

        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        bool bNewPage = false;// Used to check whether we are printing a new page
        int iHeaderHeight = 0; //Used for the header height
        //Grid Column
        public const int ConstIsDelete = 0;
        public const int ConstInvoiceItemID = 1;
        public const int ConstInvoiceID = 2;
        public const int ConstProductID = 3;
        public const int ConstTaxID = 4;
        public const int ConstSizeID = 5;
        public const int ConstProduct = 6;
        public const int ConstHAN = 7;
        public const int ConstMake = 8;
        public const int ConstQuantity = 9;
        public const int ConstUnit = 10;
        public const int ConstRate = 11;
        public const int ConstSize = 12;
        public const int ConstTax = 13;
        public const int ConstTaxAmount = 14;
        public const int ConstDiscount = 15;
        public const int ConstDiscountAmount = 16;
        public const int ConstTotal = 17;
        public const int ConstRemark = 18;
        public const int ConstUnitID = 19;
        //end
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
        private string InvSubTitle4;
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
        private string CustomerName;
        private string BillingAddress;
        private string ShipingAddress;
        private string InvoiceNo;
        private string MobileNo;
        private string InvoicePrintDate;
        private decimal InvoiceTotal;
        private bool ReadInvoice;
        private int AmountPosition;

        // Font and Color:------------------
        // Title Font
        private Font InvTitleFont = new Font("Arial", 16, FontStyle.Regular);
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

        InventoryBLL inventoryBLL;
        SizeBLL sizeBLL;
        int  UserId = 1;
        int InventoryID;
        ProductBLL productBLL;       
        DataTable dtInventoryItem;

        #endregion [Variable Declaration]  

        #region [Events] 

        public Inventory()
        {
            this.prnDialog = new System.Windows.Forms.PrintDialog();
            this.prnPreview = new System.Windows.Forms.PrintPreviewDialog();
            this.prnDocument = new System.Drawing.Printing.PrintDocument();
            // The Event of 'PrintPage'
            prnDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(prnDocument_PrintPage);
            inventoryBLL = new InventoryBLL(dbHelper);
            productBLL = new ProductBLL(dbHelper);
            sizeBLL = new SizeBLL(dbHelper);
            InitializeComponent();
            gvInventory.CellContentClick += gvInventory_CellContentClick;
            FillCombo();
            FillGrid();
            this.gvInventory.AllowUserToAddRows = false;

        }
        private void FillDataGridForPrint()
        {
            DataTable dtInvoice = new DataTable();
            dtInvoice = inventoryBLL.GetAllInventory(InventoryID);
            if (dtInvoice.Rows.Count > 0)
            {

                CustomerName = Convert.ToString(dtInvoice.Rows[0]["CompanyName"]);
                BillingAddress = Convert.ToString(dtInvoice.Rows[0]["BillingAddress"]);
                ShipingAddress = Convert.ToString(dtInvoice.Rows[0]["ShippingAddress"]);
                MobileNo = Convert.ToString(dtInvoice.Rows[0]["ContactNumber"]);
                InvoicePrintDate = Convert.ToString(dtInvoice.Rows[0]["InventoryDate"]);
                InvoiceNo = Convert.ToString(dtInvoice.Rows[0]["InvoiceNo"]);              

      
            }

            DataTable dTable = new DataTable();
            dTable = inventoryBLL.GetAllInventoryItemReport(InventoryID, 0);

            PrintGrid.DataSource = dTable;
            PrintGrid.AutoGenerateColumns = false;
        }
        private void SetInvoiceHead(Graphics g, System.Drawing.Printing.PrintPageEventArgs e)
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
            int lenInvSubTitle4 = (int)g.MeasureString(InvSubTitle4, InvSubTitleFont).Width;
            // Set Titles Left:
            int xInvTitle = CurrentX + (InvoiceWidth - lenInvTitle) / 2;
            int xInvSubTitle1 = CurrentX + (InvoiceWidth - lenInvSubTitle1) / 2;
            int xInvSubTitle2 = CurrentX + (InvoiceWidth - lenInvSubTitle2) / 2;
            int xInvSubTitle3 = CurrentX + (InvoiceWidth - lenInvSubTitle3) / 2;
            int xInvSubTitle4 = CurrentX + (InvoiceWidth - lenInvSubTitle4) / 2;
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
            if (InvSubTitle4 != "")
            {
                CurrentY = CurrentY + InvSubTitleHeight;
                g.DrawString(InvSubTitle4, InvSubTitleFont, BlueBrush, xInvSubTitle4, CurrentY);
            }
            // Draw line:
            CurrentY = CurrentY + InvSubTitleHeight + 8;
            g.DrawLine(new Pen(Brushes.Black, 2), CurrentX, CurrentY, rightMargin, CurrentY);

            SetOrderData(g, e);
        }

        private void SetOrderData(Graphics g, System.Drawing.Printing.PrintPageEventArgs e)
        {// Set Company Name, City, Salesperson, Order ID and Order Date


            string FieldValue = "";
            InvoiceFontHeight = (int)(InvoiceFont.GetHeight(g));
            // Set Company Name:
            CurrentX = leftMargin;
            CurrentY = CurrentY + 8;
            FieldValue = "Invoice No: " + InvoiceNo;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            ////// Set City:
            //CurrentX = CurrentX + (int)g.MeasureString(FieldValue, InvoiceFont).Width + 16;
            //FieldValue = "Customer Name: " + CustomerName;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            //// Set Salesperson:

            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Customer Name: " + CustomerName;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);


            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Contact Number: " + MobileNo;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);
            // Set Order ID:
            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Inventory Date: " + InvoicePrintDate;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);

            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Billing Address: " + BillingAddress;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);

            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Shipping Address: " + ShipingAddress;
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);

            //// Set Order Date:
            //CurrentX = CurrentX + (int)g.MeasureString(FieldValue, InvoiceFont).Width + 16;
            //FieldValue = "Order Date: " + SaleDate;
            //g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);

            // Draw line:
            CurrentY = CurrentY + InvoiceFontHeight + 8;
            g.DrawLine(new Pen(Brushes.Black), leftMargin, CurrentY, rightMargin, CurrentY);


            CurrentX = leftMargin;
            CurrentY = CurrentY + InvoiceFontHeight;
            FieldValue = "Invoice Details:";
            g.DrawString(FieldValue, InvoiceFont, BlackBrush, CurrentX, CurrentY);



            strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Near;
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Trimming = StringTrimming.EllipsisCharacter;

            arrColumnLefts.Clear();
            arrColumnWidths.Clear();
            iCellHeight = 0;
            iRow = 0;
            bFirstPage = true;
            bNewPage = true;

            // Calculating Total Widths
            iTotalWidth = 0;
            foreach (DataGridViewColumn dgvGridCol in PrintGrid.Columns)
            {
                iTotalWidth += dgvGridCol.Width;
            }

            //Set the left margin
            int iLeftMargin = e.MarginBounds.Left;
            //Set the top margin
            int iTopMargin = e.MarginBounds.Top;
            //Whether more pages have to print or not
            bool bMorePagesToPrint = false;
            int iTmpWidth = 0;

            //For the first page to print set the cell width and header height
            if (bFirstPage)
            {
                foreach (DataGridViewColumn GridCol in PrintGrid.Columns)
                {
                    iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                   (double)iTotalWidth * (double)iTotalWidth *
                                   ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                    iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                    // Save width and height of headres
                    arrColumnLefts.Add(iLeftMargin);
                    arrColumnWidths.Add(iTmpWidth);
                    iLeftMargin += iTmpWidth;
                }
            }
            //Loop till all the grid rows not get printed
            while (iRow <= PrintGrid.Rows.Count - 1)
            {
                DataGridViewRow GridRow = PrintGrid.Rows[iRow];
                //Set the cell height
                iCellHeight = GridRow.Height + 5;
                int iCount = 0;
                //Check whether the current page settings allo more rows to print
                if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                {
                    bNewPage = true;
                    bFirstPage = false;
                    bMorePagesToPrint = true;
                    break;
                }
                else
                {
                    if (bNewPage)
                    {

                        InvoiceFontHeight = (int)(InvoiceFont.GetHeight(g));
                        // Set Company Name:

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
                        // CurrentY = CurrentY + InvoiceFontHeight + 8;
                        CurrentX = leftMargin;
                        CurrentY = CurrentY + InvoiceFontHeight;

                        g.DrawLine(new Pen(Brushes.Black), leftMargin, CurrentY, rightMargin, CurrentY);

                        iTopMargin = e.MarginBounds.Top + 310;
                        foreach (DataGridViewColumn GridCol in PrintGrid.Columns)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                (int)arrColumnWidths[iCount], iHeaderHeight));

                            e.Graphics.DrawRectangle(Pens.Black,
                                new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                (int)arrColumnWidths[iCount], iHeaderHeight));

                            e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                            iCount++;
                        }
                        bNewPage = false;
                        iTopMargin += iHeaderHeight;
                    }
                    iCount = 0;
                    //Draw Columns Contents                
                    foreach (DataGridViewCell Cel in GridRow.Cells)
                    {
                        if (Cel.Value != null)
                        {
                            e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                        new SolidBrush(Cel.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                        (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                        }
                        //Drawing Cells Borders 
                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                        iCount++;
                    }
                }
                iRow++;
                iTopMargin += iCellHeight;
            }

            //If more lines exist, print another page.
            if (bMorePagesToPrint)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;


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
            InvTitle = ConfigurationManager.AppSettings["Company"].ToString();
            InvSubTitle1 = ConfigurationManager.AppSettings["Address"].ToString();
            InvSubTitle2 = ConfigurationManager.AppSettings["Phone"].ToString();
            InvSubTitle3 = ConfigurationManager.AppSettings["Email"].ToString();
            InvSubTitle4 = ConfigurationManager.AppSettings["GSTIN"].ToString();
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

            SetInvoiceHead(e.Graphics, e);
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
                MessageBox.Show("Please add an item and then click on Print or Print ");
            }

        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count >= 1)
            {
                ReadInvoice = false;
                DisplayInvoice(); // Print Preview
            }
            else
            {
                MessageBox.Show("Please add an item and then click on Print Preview.");
            }
        }


        private void frmInventory_Load(object sender, EventArgs e)
        {
           
            dtpInventoryDate.Format = DateTimePickerFormat.Custom;
            dtPaymentDate.Format = DateTimePickerFormat.Custom;
            dtReceivedDate.Format = DateTimePickerFormat.Custom;

            
            dtpInventoryDate.CustomFormat = ConfigurationManager.AppSettings["DateFormat"];
            dtPaymentDate.CustomFormat = ConfigurationManager.AppSettings["DateFormat"];
            dtReceivedDate.CustomFormat = ConfigurationManager.AppSettings["DateFormat"];
            string invPrefix = ConfigurationManager.AppSettings["InvoicePrefix"];

            string maxinvoiceID;

            DataTable dt = inventoryBLL.GetMax_Inventory();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    int maxID = 0;
                    int.TryParse(Convert.ToString(dt.Rows[0]["InvoiceID"]), out maxID);
                    maxinvoiceID = Convert.ToString(maxID + 1);
                    if (maxinvoiceID.Length == 1)
                    {
                        maxinvoiceID = invPrefix + "00" + maxinvoiceID;
                    }
                    else if (maxinvoiceID.Length == 2)
                    {
                        maxinvoiceID = invPrefix + "0" + maxinvoiceID;
                    }
                    else
                    {
                        maxinvoiceID = invPrefix + maxinvoiceID;
                    }
                }
                else
                {
                    maxinvoiceID = invPrefix + "001";
                }
                txtInvoiceNo.Text = maxinvoiceID;
            }
        }

        private void txtContactNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlynumber(sender, e);
        }

        private void txtTaxAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlynumwithsinglepoint(sender, e);
        }

        private void txtTotalAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlynumwithsinglepoint(sender, e);
        }

        private void gvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView2.DataSource = null;
            var senderGrid = (DataGridView)sender;

            var aa = gvInventory.CurrentCell.RowIndex;
            if (senderGrid.Columns[e.ColumnIndex].DisplayIndex == 16)
            {
                InventoryID = Convert.ToInt32(gvInventory.Rows[aa].Cells[0].Value);

                DataTable dtInventory = new DataTable();
                dtInventory = inventoryBLL.GetAllInventory(InventoryID);
                if (dtInventory != null)
                {
                    if (dtInventory.Rows.Count > 0)
                    {
                        txtCompany.Text = Convert.ToString(dtInventory.Rows[0]["CompanyName"]);
                        txtContactPerson.Text = Convert.ToString(dtInventory.Rows[0]["ContactPersonName"]);
                        txtContactNumber.Text = Convert.ToString(dtInventory.Rows[0]["ContactNumber"]);
                        txtEmail.Text = Convert.ToString(dtInventory.Rows[0]["Email"]);
                        txtWebsite.Text = Convert.ToString(dtInventory.Rows[0]["Website"]);
                        cmbPaymentMode.SelectedValue = Convert.ToString(dtInventory.Rows[0]["PaymentModeID"]);
                        cmbIsPaid.SelectedValue = Convert.ToString(dtInventory.Rows[0]["IsPaid"]);
                        cmbIsOnCredit.SelectedValue = Convert.ToString(dtInventory.Rows[0]["IsOnCredit"]);
                        dtpInventoryDate.Text = Convert.ToString(dtInventory.Rows[0]["InventoryDate"]);
                        dtPaymentDate.Text = Convert.ToString(dtInventory.Rows[0]["PaymentDate"]);
                        dtReceivedDate.Text = Convert.ToString(dtInventory.Rows[0]["ItemsReceivedOn"]);
                        txtTaxAmount.Text = Convert.ToString(dtInventory.Rows[0]["TotalTaxAmount"]);
                        txtTotalAmount.Text = Convert.ToString(dtInventory.Rows[0]["TotalAmount"]);
                        txtRemark.Text = Convert.ToString(dtInventory.Rows[0]["Remarks"]);
                       txtInvoiceNo.Text = Convert.ToString(dtInventory.Rows[0]["InvoiceNo"]);
                        txtBillingAddress.Text = Convert.ToString(dtInventory.Rows[0]["BillingAddress"]);
                        txtShippingAddress.Text = Convert.ToString(dtInventory.Rows[0]["ShippingAddress"]);                    
                        txtGSTIN.Text = Convert.ToString(dtInventory.Rows[0]["GSTIN"]);
                    }

                    dtInventoryItem = new DataTable();
                    dtInventoryItem = inventoryBLL.GetAllInventoryItem(InventoryID, 0);
                    dataGridView2.AutoGenerateColumns = false;

                  //  dtInventoryItem.Columns.Add["NewRow"];
                    dataGridView2.DataSource = dtInventoryItem;
                    if (dtInventoryItem.Rows.Count > 0)
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
                    this.gvInventory.AllowUserToAddRows = false;
                    GetProduct();
                    GetUnit();

                }
            }
            else if ((senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (senderGrid.Columns[e.ColumnIndex].DisplayIndex == 17 && e.RowIndex >= 0) && (!(String.IsNullOrEmpty(gvInventory.Rows[aa].Cells[0].Value.ToString()))))
            {
                InventoryID = Convert.ToInt32(gvInventory.Rows[aa].Cells[0].Value);
                bool check = inventoryBLL.DeleteInventory(InventoryID, 1);
                if (check == true)
                    MessageBox.Show("Selected Inventory Deleted Successfully.");
                else
                    MessageBox.Show("Selected Inventory cannot be deleted due to maaping with other masters.");

                ClearFields();
            }
            FillGrid();
        }
      
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            //if (validation() == true)
            {
                DataTable dtInventory = new DataTable();
                dtInventory.TableName = "Inventory";
                dtInventory.Columns.Add("CompanyName", typeof(String));
                dtInventory.Columns.Add("ContactPersonName", typeof(String));
                dtInventory.Columns.Add("ContactNumber", typeof(String));
                dtInventory.Columns.Add("Email", typeof(String));
                dtInventory.Columns.Add("Website", typeof(String));
                dtInventory.Columns.Add("PaymentModeID", typeof(Int32));
                dtInventory.Columns.Add("PaymentMode", typeof(String));
                dtInventory.Columns.Add("IsPaid", typeof(Int32));
                dtInventory.Columns.Add("IsOnCredit", typeof(Int32));
                dtInventory.Columns.Add("InventoryDate", typeof(String));
                dtInventory.Columns.Add("PaymentDate", typeof(String));
                dtInventory.Columns.Add("ItemsReceivedOn", typeof(String));                
                dtInventory.Columns.Add("TotalTaxAmount", typeof(Int32));
                dtInventory.Columns.Add("TotalAmount", typeof(Int32));
                dtInventory.Columns.Add("Remarks", typeof(String));
                dtInventory.Columns.Add("InvoiceNo", typeof(String));
                dtInventory.Columns.Add("BillingAddress", typeof(String));
                dtInventory.Columns.Add("ShippingAddress", typeof(String));

                dtInventory.Columns.Add("GSTIN", typeof(String));

                DataRow rowInventory = dtInventory.NewRow();
                rowInventory["CompanyName"] = txtCompany.Text.Trim();
                rowInventory["ContactPersonName"] = txtContactPerson.Text.Trim();
                rowInventory["ContactNumber"] = txtContactNumber.Text.Trim();
                rowInventory["Email"] = txtEmail.Text.Trim();
                rowInventory["Website"] = txtWebsite.Text.Trim();
                rowInventory["PaymentModeID"] = 1;
                rowInventory["PaymentMode"] = "Cash";
                rowInventory["IsPaid"] = 1;
                rowInventory["IsOnCredit"] = 1;
                rowInventory["InventoryDate"] = Convert.ToDateTime(dtpInventoryDate.Value).ToString("MM-dd-yy"); 
                rowInventory["PaymentDate"] = Convert.ToDateTime(dtPaymentDate.Value).ToString("MM-dd-yy");
                rowInventory["ItemsReceivedOn"] = Convert.ToDateTime(dtReceivedDate.Value).ToString("MM-dd-yy");
                rowInventory["TotalTaxAmount"] = Convert.ToInt32(txtTaxAmount.Text);
                rowInventory["TotalAmount"] = Convert.ToInt32(txtTotalAmount.Text);
                rowInventory["Remarks"] = txtRemark.Text.Trim();
                rowInventory["InvoiceNo"] = txtInvoiceNo.Text.Trim();
                rowInventory["BillingAddress"] = txtBillingAddress.Text.Trim();
                rowInventory["ShippingAddress"] = txtShippingAddress.Text.Trim();              
                rowInventory["GSTIN"] = txtGSTIN.Text.Trim();
                dtInventory.Rows.Add(rowInventory);

                DataTable dtInventoryItems = new DataTable();
                dtInventoryItems.TableName = "InventoryDetails";
                dtInventoryItems.Columns.Add("InventoryItemID", typeof(Int32));
                dtInventoryItems.Columns.Add("InventoryID", typeof(Int32));
                dtInventoryItems.Columns.Add("ProductID", typeof(Int32));
                dtInventoryItems.Columns.Add("Product", typeof(String));
                dtInventoryItems.Columns.Add("Make", typeof(String));
                dtInventoryItems.Columns.Add("Quantity", typeof(Decimal));
                dtInventoryItems.Columns.Add("UnitID", typeof(String));
                dtInventoryItems.Columns.Add("SizeId", typeof(Int32));
                dtInventoryItems.Columns.Add("Size", typeof(String));
                dtInventoryItems.Columns.Add("RatePerUnit", typeof(Decimal));
                dtInventoryItems.Columns.Add("TaxID", typeof(Int32));
                dtInventoryItems.Columns.Add("HSN_SAC", typeof(Int32));
                dtInventoryItems.Columns.Add("TaxPercentage", typeof(Int32));
                dtInventoryItems.Columns.Add("TaxAmount", typeof(Decimal));
                dtInventoryItems.Columns.Add("Discount", typeof(Decimal));
                dtInventoryItems.Columns.Add("DiscountAmount", typeof(Decimal));
                dtInventoryItems.Columns.Add("TotalAmount", typeof(Decimal));
                dtInventoryItems.Columns.Add("Remark", typeof(String));
                dtInventoryItems.Columns.Add("Delete", typeof(Boolean));
                //Added 
              //  dtInventoryItems.Columns.Add("IsNew", typeof(Boolean));

                DataRow row;
                for (int i = 0; i <dataGridView2.Rows.Count; i++)
                {
                    row = dtInventoryItems.NewRow();

                    row["InventoryItemID"] = dataGridView2.Rows[i].Cells["InventoryItemID"].Value.ToString() == "" ? "0" : dataGridView2.Rows[i].Cells["InventoryItemID"].Value;

                    row["InventoryID"] = InventoryID;
                    row["ProductID"] = dataGridView2.Rows[i].Cells["Product"].Value;
                    row["Product"] = Convert.ToString(dataGridView2.Rows[i].Cells["Product"].Value);
                    row["Make"] = Convert.ToString(dataGridView2.Rows[i].Cells["Make"].Value);
                    row["Quantity"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Quantity"].Value);

                    row["UnitID"] = Convert.ToString(dataGridView2.Rows[i].Cells["Unit"].Value);
                    row["TaxID"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["TaxID"].Value);
                    if (Convert.ToInt32(dataGridView2.Rows[i].Cells["Size"].Value) > 0)
                    {
                        row["SizeId"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["Size"].Value);
                    }
                    else
                    {
                        MessageBox.Show("Please select size for the selected product.");
                        isValid = false;
                    }
                    row["RatePerUnit"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["RateperUnit"].Value);
                    if (Convert.ToString(dataGridView2.Rows[i].Cells["HSN_SAC"].Value) != "")
                        row["HSN_SAC"] = Convert.ToInt32(dataGridView2.Rows[i].Cells["HSN_SAC"].Value);
                    else
                    {
                        row["HSN_SAC"] = DBNull.Value;
                    }
                    row["TaxPercentage"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Tax"].Value);
                    row["Discount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Discount"].Value);

                    if (Convert.ToString(dataGridView2.Rows[i].Cells["TaxAmt"].Value) != "")
                    {
                        row["TaxAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["TaxAmt"].Value);
                    }
                    else
                    {
                        row["TaxAmount"] = 0;
                    }
                    if (Convert.ToString(dataGridView2.Rows[i].Cells["DiscountAmt"].Value) != "")
                    {
                        row["DiscountAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DiscountAmt"].Value);
                    }
                    else
                    {
                        row["DiscountAmount"] = 0;
                    }
                    if (Convert.ToString(dataGridView2.Rows[i].Cells["TotalAmt"].Value) != "")
                    {
                        row["TotalAmount"] = Convert.ToDecimal(dataGridView2.Rows[i].Cells["TotalAmt"].Value);
                    }
                    else
                    {
                        row["TotalAmount"] = 0;
                    }

                    row["Remark"] = Convert.ToString(dataGridView2.Rows[i].Cells["Remark"].Value);
                    DataGridViewCheckBoxCell ch1 = new DataGridViewCheckBoxCell();
                    ch1 = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                    row["Delete"] = ch1.Value == null ? false : true;
                  //  row["IsNew"] = true;
                    dtInventoryItems.Rows.Add(row);
                }

                System.IO.StringWriter swSQL;                
                StringBuilder sbSQL1 = new StringBuilder();
                swSQL = new System.IO.StringWriter(sbSQL1);
                dtInventoryItems.WriteXml(swSQL);
                swSQL.Dispose();

                System.IO.StringWriter swSQL2;
                StringBuilder sbSQL2 = new StringBuilder();
                swSQL2 = new System.IO.StringWriter(sbSQL2);
                dtInventory.WriteXml(swSQL2);
                swSQL2.Dispose();

                if (isValid)
                {
                    inventoryBLL.InventorySave(InventoryID, sbSQL2.ToString(), sbSQL1.ToString(), UserId);

                    InventoryID = 0;
                    ClearFields();
                    FillGrid();
                    this.dataGridView2.DataSource = null;
                }

            }
        }
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            DataRow newRow;
            DataTable dtItem = new DataTable();
            DataTable tdExistRows = (DataTable)dataGridView2.DataSource;
            if (dtInventoryItem == null)
            {
                dtItem = new DataTable();
                dtItem.TableName = "InvoiceDetails";
                dtItem.Columns.Add("InventoryItemID", typeof(Int32));
                dtItem.Columns.Add("InventoryID", typeof(Int32));
                dtItem.Columns.Add("ProductID", typeof(Int32));
                dtItem.Columns.Add("Product", typeof(String));
                dtItem.Columns.Add("Make", typeof(String));
                dtItem.Columns.Add("Quantity", typeof(Decimal));
                dtItem.Columns.Add("UnitID", typeof(String));
                dtItem.Columns.Add("SizeId", typeof(Int32));
                dtItem.Columns.Add("Size", typeof(String));
                dtItem.Columns.Add("RatePerUnit", typeof(Decimal));
                dtItem.Columns.Add("TaxID", typeof(Int32));
                dtItem.Columns.Add("HSN_SAC", typeof(Int32));                
                dtItem.Columns.Add("TaxPercentage", typeof(Int32));
                dtItem.Columns.Add("TaxAmount", typeof(Decimal));
                dtItem.Columns.Add("Discount", typeof(String));
                dtItem.Columns.Add("DiscountAmount", typeof(Decimal));
                dtItem.Columns.Add("TotalAmount", typeof(Decimal));
                dtItem.Columns.Add("Remark", typeof(String));
                dtItem.Columns.Add("Delete", typeof(Boolean));
                dtItem.Columns.Add("IsNew", typeof(Boolean));

                newRow = dtItem.NewRow();
                dtItem.Rows.Add(newRow);

                dataGridView2.AutoGenerateColumns = false;
              

                if (tdExistRows != null)
                {
                    newRow = tdExistRows.NewRow();
                    newRow["IsNew"] = true;
                    tdExistRows.Rows.Add(newRow);
                }
                else
                {
                    tdExistRows = dtItem;
                }
                dataGridView2.DataSource = tdExistRows;

                this.gvInventory.AllowUserToAddRows = false;
                GetSingleProduct(dataGridView2.Rows.Count - 1);
                GetSingleUnit(dataGridView2.Rows.Count - 1);
            }
            else
            {
                newRow = tdExistRows.NewRow();
                tdExistRows.Rows.Add(newRow);
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.DataSource = tdExistRows;
                this.gvInventory.AllowUserToAddRows = false;
                GetSingleProduct(dataGridView2.Rows.Count - 1);
                GetSingleUnit(dataGridView2.Rows.Count - 1);
            }


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

                DataTable dtZise = new DataTable();
                dtZise = sizeBLL.GetAllSize(null, selectValue);
                dtZise.Rows.Add(new object[] { 0, "Select" });
                dtZise.DefaultView.Sort = "SizeId ASC";

                DataGridViewComboBoxCell objSize = new DataGridViewComboBoxCell();
                objSize = (DataGridViewComboBoxCell)dataGridView2.Rows[i].Cells["Size"];
                var dataSize = dataGridView2.Rows[i].Cells["SizeId"].Value;
                selectValue = 0;
                if (data != null)
                {
                    if (dataSize.ToString() != string.Empty)
                        selectValue = Convert.ToInt32(dataSize);
                }
                objSize.DataSource = dtZise;
                objSize.ValueMember = "SizeID";
                objSize.DisplayMember = "SizeName";
                objSize.Value = selectValue;

                // GetSize(selectValue);

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

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Sorry we are unable to procceed dut to this " + e.Exception.Message.ToString());
        }


        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)e.Control).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)e.Control).AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == ConstProduct && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += LastColumnComboSelectionChanged;
            }
            if ((dataGridView2.CurrentCell.ColumnIndex == ConstQuantity) && e.Control is TextBox)
            {
                TextBox txtQuantity = e.Control as TextBox;
                txtQuantity.TextChanged += txtQuantity_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == ConstRate && e.Control is TextBox)
            {
                TextBox txtRate = e.Control as TextBox;
                //txtRate.TextChanged += txtRate_TextChanged;
                txtRate.Leave += txtRate_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == ConstDiscount && e.Control is TextBox)
            {
                TextBox txtDiscount = e.Control as TextBox;
                txtDiscount.TextChanged += txtDiscount_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == ConstTax && e.Control is TextBox)
            {
                TextBox txtTax = e.Control as TextBox;
                txtTax.TextChanged += txtTax_TextChanged;
            }
            if (dataGridView2.CurrentCell.ColumnIndex == ConstUnit && e.Control is ComboBox)
            {
                ComboBox comboBoxUnit = e.Control as ComboBox;
                comboBoxUnit.SelectedIndexChanged += LastColumnComboUnitSelectionChanged;
            }

        }
        private void LastColumnComboUnitSelectionChanged(object sender, EventArgs e)
        {
            var currentcell = dataGridView2.CurrentCellAddress;
            var sendingCB = sender as DataGridViewComboBoxEditingControl;

            DataGridViewComboBoxCell sizedll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstSize];
            if (currentcell.X == ConstUnit)
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
            if (dataGridView2.CurrentCell.ColumnIndex == ConstTax)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTax];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscount];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscountAmount];
                DataGridViewTextBoxCell Quantity = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstQuantity];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTaxAmount];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTotal];
                decimal outValue = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);

                decimal QuantityRate=0;
                decimal dicountPer = 0;
                
                if (Quantity.Value!= System.DBNull.Value && Rate.Value!= System.DBNull.Value)
                { 
                 QuantityRate = Convert.ToDecimal(Quantity.Value) * Convert.ToDecimal(Rate.Value);
                }
                if(QuantityRate!=0 && discount.Value!=null)
                { 
                    dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                }
                if(dicountPer!=0)
                { 
                discountAmt.Value = Convert.ToDecimal(dicountPer);
                }

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
            if (dataGridView2.CurrentCell.ColumnIndex == ConstDiscount)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstRate];
                DataGridViewTextBoxCell Quantity = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstQuantity];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscount];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTax];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTaxAmount];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTotal];

                decimal outValue = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);

                decimal QuantityRate = 0;
        
                if (Quantity.Value!= System.DBNull.Value && Rate.Value!= System.DBNull.Value)
                {
                 QuantityRate = Convert.ToDecimal(Quantity.Value) * Convert.ToDecimal(Rate.Value);
                }

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
            if (dataGridView2.CurrentCell.ColumnIndex == ConstQuantity)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var sendingTB = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell Rate = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstRate];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscount];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscountAmount];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTax];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTaxAmount];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTotal];
                decimal outValue = 0;
                decimal QuantityRate=0;
                decimal dicountPer = 0;
                Decimal.TryParse(sendingTB.Text, out outValue);
                if(Rate.Value!= System.DBNull.Value)
                { 
               QuantityRate = outValue * Convert.ToDecimal(Rate.Value);
                }

                if(discount.Value!=System.DBNull.Value)
                { 
                dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                }
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;
                decimal totalTax = 0;
                if (Tax.Value !=System.DBNull.Value)
                {
                    totalTax = totalafterDiscount / 100 * Convert.ToDecimal(Tax.Value);
                }
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
            if (dataGridView2.CurrentCell.ColumnIndex == ConstRate)
            {
                var currentcell = dataGridView2.CurrentCellAddress;
                var Rate = sender as DataGridViewTextBoxEditingControl;
                DataGridViewTextBoxCell sendingTB = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstRate];
                DataGridViewTextBoxCell discount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscount];
                DataGridViewTextBoxCell discountAmt = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstDiscountAmount];
                DataGridViewTextBoxCell Tax = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTax];
                DataGridViewTextBoxCell TaxAmount = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTaxAmount];
                DataGridViewTextBoxCell Total = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTotal];
                DataGridViewTextBoxCell quantity = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstQuantity];
                decimal outValue = 0;
                Decimal.TryParse(Rate.Text, out outValue);
                //   Decimal.TryParse(quantity.Text, out outValue);


                // decimal QuantityRate = Convert.ToDecimal(sendingTB.Value) * outValue;

                decimal QuantityRate = 0;
                decimal dicountPer = 0;
                if (quantity.Value!=System.DBNull.Value)
                { 
                QuantityRate= Convert.ToDecimal(sendingTB.EditedFormattedValue) * (Decimal)quantity.Value;
                }
                if(discount.Value!=System.DBNull.Value)
                {
                 dicountPer = QuantityRate / 100 * Convert.ToDecimal(discount.Value);
                }
                discountAmt.Value = Convert.ToDecimal(dicountPer);

                decimal totalafterDiscount = QuantityRate - dicountPer;

                decimal totalTax = 0;
                if(Tax.Value !=System.DBNull.Value)
                { 
                 totalTax = totalafterDiscount / 100 * Convert.ToDecimal(Tax.Value);
                }
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
            DataGridViewTextBoxCell cel = (DataGridViewTextBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstTaxID];
            DataGridViewComboBoxCell unitdll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstUnit];
            DataGridViewComboBoxCell sizedll = (DataGridViewComboBoxCell)dataGridView2.Rows[currentcell.Y].Cells[ConstSize];
            if (currentcell.X == ConstProduct)
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
                                row.Cells["HSN_SAC"].Value = dtProduct.Rows[0]["HSN_SAC"];
                                row.Cells[ConstQuantity].Value = 0;
                                row.Cells[10].Value = 0;
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
                                row.Cells[ConstTax].Value = dtProduct.Rows[0]["TaxPercentage"];
                                row.Cells[15].Value = 0;
                                row.Cells[ConstDiscount].Value = dtProduct.Rows[0]["Discount"];
                                row.Cells[17].Value = 0;
                                row.Cells[18].Value = 0;
                                row.Cells[ConstRemark].Value = dtProduct.Rows[0]["Remark"];

                            }
                        }

                    }
                }
            }
        }


        private void chkSameAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSameAddress.Checked)
            {
                txtShippingAddress.Text = txtBillingAddress.Text;
                txtShippingAddress.Enabled = false;
            }
            else
            {
                txtShippingAddress.Enabled = true;
                txtShippingAddress.Text = "";
            }
        }

       

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        #endregion [Events] 

        #region [FillGrid] 

        private void FillGrid()
        {
            DataTable dt = new DataTable();
            dt = inventoryBLL.GetAllInventory(0);
            GridBind(dt);
        }

        private void GridBind(DataTable dt)
        {
            gvInventory.AutoGenerateColumns = false;
            dt.Columns.Add("Edit");
            gvInventory.DataSource = dt;            
            this.gvInventory.AllowUserToAddRows = false;
        }

        #endregion [FillGrid]

        #region [Combo Fill] 
        private void GetAllPaymentMode()
        {
            DataTable dt = new DataTable();
            dt = inventoryBLL.GetAllPaymentMode();
            cmbPaymentMode.DataSource = dt;
            dt.Rows.Add(new object[] { 0, "Select" });
            dt.DefaultView.Sort = "PaymentModeID";
            cmbPaymentMode.ValueMember = "PaymentModeID";
            cmbPaymentMode.DisplayMember = "PaymentMode";
            cmbPaymentMode.SelectedValue = 0;
        }

        private void GetAllIsPaid()
        {            
            cmbIsPaid.Items.Insert(0, "Yes");
            cmbIsPaid.Items.Insert(1, "No");            
            cmbIsPaid.ValueMember = "IsPaidID";
            cmbIsPaid.DisplayMember = "IsPaid";
            cmbIsPaid.SelectedIndex = 0;
            cmbIsPaid.SelectedValue = 0;
        }

        private void GetAllIsOnCredit()
        {
           
            cmbIsOnCredit.ValueMember = "IsOnCreditID";
            cmbIsOnCredit.DisplayMember = "IsOnCredit";
            cmbIsOnCredit.SelectedIndex = 0;
            cmbIsOnCredit.SelectedValue = 0;
        }

        private void GetProduct()
        {
            DataTable dt = new DataTable();
            dt = inventoryBLL.GetProduct();
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
        private void GetSingleProduct(int index)
        {
            DataTable dt = new DataTable();
            dt = inventoryBLL.GetProduct();
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
        private void FillCombo()
        {
            GetAllPaymentMode();
            GetAllIsOnCredit();
            GetAllIsPaid();
            GetProduct();
            GetUnit();

        }

        #endregion [Combo Fill] 

        #region [ClearFields] 

        private void ClearFields()
        {
            string invPrefix = ConfigurationManager.AppSettings["InvoicePrefix"];

            string maxinvoiceID;

            DataTable dt = inventoryBLL.GetMax_Inventory();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    int maxID = 0;
                    int.TryParse(Convert.ToString(dt.Rows[0]["InvoiceID"]), out maxID);
                    maxinvoiceID = Convert.ToString(maxID + 1);
                    if (maxinvoiceID.Length == 1)
                    {
                        maxinvoiceID = invPrefix + "00" + maxinvoiceID;
                    }
                    else if (maxinvoiceID.Length == 2)
                    {
                        maxinvoiceID = invPrefix + "0" + maxinvoiceID;
                    }
                    else
                    {
                        maxinvoiceID = invPrefix + maxinvoiceID;
                    }
                }
                else
                {
                    maxinvoiceID = invPrefix + "001";
                }
                txtInvoiceNo.Text = maxinvoiceID;
            }
            txtCompany.Text = string.Empty;
            txtContactPerson.Text = string.Empty;
            txtContactNumber.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtWebsite.Text = string.Empty;
            cmbPaymentMode.SelectedValue = 0;
            cmbIsOnCredit.SelectedValue = 0;
            cmbIsPaid.SelectedValue = 0;
            txtRemark.Text = string.Empty;
            dtReceivedDate.Value = DateTime.Now;
            dtpInventoryDate.Value = DateTime.Now;
            dtPaymentDate.Value = DateTime.Now;
            txtTotalAmount.Text = "0";
            txtTaxAmount.Text = "0";
            InventoryID = 0;
        }

        #endregion [ClearFields] 

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
