using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Collections;

namespace MasterDetailExample
{
    public partial class MainPage : UserControl
    {
        static List<Supplier> dataSuppliers;
        static List<Product> dataDetailedForProducts;
        static List<OrderDetail> dataDetailedForOrders;
        static List<int> fakeDataSource;
        int supplierID = -1;
        int productID = -1; 

        public MainPage()
        {
            InitializeComponent();

            tabControl.SelectionChanged += new DevExpress.Xpf.Core.TabControlSelectionChangedEventHandler(tabControl_SelectionChanged);

            // Subscribe to the CalculateDocumentVariable event that triggers the master-detail report generation
            resultRichEdit.CalculateDocumentVariable += new CalculateDocumentVariableEventHandler(resultRichEdit_CalculateDocumentVariable);

            // Load main template
            RtfLoadHelper.Load("main.rtf", mainRichEdit);

            // Create data sources for the project
            fakeDataSource = DataHelper.CreateFakeDataSource();
            DataHelper.CreateData();
            dataSuppliers = DataHelper.Suppliers;
            dataDetailedForProducts = DataHelper.Products;
            dataDetailedForOrders = DataHelper.OrderDetails;
            
        }

        #region #startmailmerge
        private void biMergeToNewDocument_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            // Main template contains no merge fields, so we provide a fake data source.
            // Otherwise mail merge will not start.
            mainRichEdit.Options.MailMerge.DataSource = fakeDataSource;

            // Trigger the multistage process. After the first mailmerge the CalculateDocumentVariable event
            //for the resultRichEdit control fires.
            mainRichEdit.MailMerge(resultRichEdit.Document);
            tabControl.SelectedTabItem = resultingDocumentTabItem;
        }
        #endregion #startmailmerge

        #region #secondstage
        // Second stage. For each Supplier ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
        void resultRichEdit_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
        {
            if (e.VariableName == "Supplier") {
                // Create a document container and load a document containing the Supplier header section (master section)
                IRichEditDocumentServer richServerSupplierTemplate = mainRichEdit.CreateDocumentServer();
                RtfLoadHelper.Load("supplier.rtf", richServerSupplierTemplate);
                // Create a text engine to process a document after the mail merge.
                IRichEditDocumentServer richServerMasterProcessor = mainRichEdit.CreateDocumentServer();
                // Provide a procedure for further processing
                richServerMasterProcessor.CalculateDocumentVariable+=new CalculateDocumentVariableEventHandler(richServerMasterProcessor_CalculateDocumentVariable);
                // Create a merged document using the Supplier template. The document will contain DOCVARIABLE fields with ProductID arguments. 
                // The CalculateDocumentVariable event for the richServerMasterProcessor fires.
                // Note that the data source for mail merge should be specified via the RichEditDocumentServer.Options.MailMerge.DataSource property.
                richServerSupplierTemplate.Options.MailMerge.DataSource = dataSuppliers;
                richServerSupplierTemplate.MailMerge(richServerMasterProcessor.Document);
                richServerMasterProcessor.CalculateDocumentVariable -= richServerMasterProcessor_CalculateDocumentVariable;
                // Return the document to insert in place of the DOCVARIABLE field.
                e.Value = richServerMasterProcessor;
                // Required. Otherwise e.Value will be ignored.
                e.Handled = true;
            }
        }
        #endregion #secondstage
        #region #thirdstage
        // Third stage. For each Product ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
        void richServerMasterProcessor_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
        {
            int currentSupplierID = GetID(e.Arguments[0].Value);
            if (currentSupplierID == -1)
                return;

            if (supplierID != currentSupplierID) {
                // Get data source that contains products for the specified supplier.
                dataDetailedForProducts = (List<Product>)GetProductsDataFilteredbySupplier(currentSupplierID);
                supplierID = currentSupplierID;
            }

            if (e.VariableName == "Product") {
                // Create a document container and load a document containing the Product header section (detail section)
                IRichEditDocumentServer richServerProductsTemplate = mainRichEdit.CreateDocumentServer();
                RtfLoadHelper.Load("detail.rtf", richServerProductsTemplate);
                // Create a text engine to process a document after the mail merge.
                IRichEditDocumentServer richServerDetailProcessor = mainRichEdit.CreateDocumentServer();

                // Specify data source for mail merge.
                richServerProductsTemplate.Options.MailMerge.DataSource = dataDetailedForProducts;
                // Specify that the resulting table should be joined with the header table.
                // Do not specify this option if calculated fields are not within table cells.
                MailMergeOptions options = richServerProductsTemplate.CreateMailMergeOptions();
                options.MergeMode = MergeMode.JoinTables;
                // Provide a procedure for further processing.
                richServerDetailProcessor.CalculateDocumentVariable+=new CalculateDocumentVariableEventHandler(richServerDetailProcessor_CalculateDocumentVariable);
                // Create a merged document using the Product template. The document will contain DOCVARIABLE fields with OrderID arguments. 
                // The CalculateDocumentVariable event for the richServerDetail fires.
                richServerProductsTemplate.MailMerge(options, richServerDetailProcessor);
                richServerDetailProcessor.CalculateDocumentVariable -= richServerDetailProcessor_CalculateDocumentVariable;
                // Return the document to insert.
                e.Value = richServerDetailProcessor;
                // This setting is required for inserting e.Value into the source document. Otherwise it will be ignored.
                e.Handled = true;
            }
        }
        #endregion #thirdstage
        #region #fourthstage
        // Fourth stage. For each Order ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
        // This is the final stage and the Product.Orders template does not contain DOCVARIABLE fields. So, further processing is not required.
        void richServerDetailProcessor_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
        {
            int currentProductID = GetID(e.Arguments[0].Value);
            if (currentProductID == -1)
                return;

            if (productID != currentProductID) {
                // Get data source that contains orders for the specified product and supplier.
                dataDetailedForOrders = (List<OrderDetail>)GetOrderDataFilteredbyProductAndSupplier(supplierID, currentProductID);
                productID = currentProductID;
            }

            if (e.VariableName == "OrderDetails") {

                IRichEditDocumentServer richServerOrdersTemplate = mainRichEdit.CreateDocumentServer();
                RtfLoadHelper.Load("detaildetail.rtf", richServerOrdersTemplate);
                richServerOrdersTemplate.Options.MailMerge.DataSource = dataDetailedForOrders;

                IRichEditDocumentServer richServerDetailDetailProcessor = mainRichEdit.CreateDocumentServer();

                MailMergeOptions options = richServerOrdersTemplate.CreateMailMergeOptions();
                options.MergeMode = MergeMode.JoinTables;

                richServerOrdersTemplate.MailMerge(options, richServerDetailDetailProcessor);

                e.Value = richServerDetailDetailProcessor;
                e.Handled = true;
            }
        }
        #endregion #fourthstage

        protected internal virtual object GetProductsDataFilteredbySupplier(int supplierID)
        {
            IList<Product> items = DataHelper.Products;
            var query = from item in items
                        where item.SupplierID == supplierID
                        select item;

            return query.ToList();
        }

        protected internal virtual object GetOrderDataFilteredbyProductAndSupplier(int supplierID, int productID)
        {
            var query = from item in DataHelper.OrderDetails
                        where (item.ProductID == productID && item.SupplierID == supplierID)
                        select item;

            return query.ToList();

        }

        protected internal virtual int GetID(string value)
        {
            int result;
            if (Int32.TryParse(value, out result))
                return result;
            return -1;
        }

        #region #TabSwitchHelpers
        void tabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            RichEditControl selectedRichEditControl = GetSelectedRichEditControl();
            if (selectedRichEditControl == null) return;
            ResetBarManagers();
            selectedRichEditControl.BarManager = barManager1;
            selectedRichEditControl.Ribbon = ribbon;
        }
        protected internal virtual RichEditControl GetSelectedRichEditControl()
        {
            object selectedTabItem = tabControl.SelectedItem;
            if (selectedTabItem == templateTabItem)
                return mainRichEdit;
            if (selectedTabItem == resultingDocumentTabItem)
                return resultRichEdit;
            return null;
        }
        protected internal virtual void ResetBarManagers()
        {
            mainRichEdit.BarManager = null;
            resultRichEdit.BarManager = null;
            mainRichEdit.Ribbon = null;
            resultRichEdit.Ribbon = null;
        }
        #endregion #TabSwitchHelpers
    }
}
