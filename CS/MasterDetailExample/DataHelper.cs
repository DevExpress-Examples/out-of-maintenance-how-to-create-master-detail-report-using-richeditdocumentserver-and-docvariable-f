using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.RichEdit;
using System.IO;
using System.Reflection;
using DevExpress.XtraRichEdit;

namespace MasterDetailExample
{
    class DataHelper
    {
        private static List<Supplier> _Suppliers;
        public static List<Supplier> Suppliers
        {
        	get {	return _Suppliers; }
        	set { _Suppliers = value;	}
        }

        private static List<Product> _Products;
        public static List<Product> Products
        {
        	get {	return _Products; }
        	set { _Products = value;	}
        }

        private static List<OrderDetail> _OrderDetails;
        public static List<OrderDetail> OrderDetails
        {
        	get {	return _OrderDetails; }
        	set { _OrderDetails = value;	}
        }

        static Random random = new Random(5);

        public static void CreateData()
        {
            _Suppliers = new List<Supplier>();
            _Products = new List<Product>();
            _OrderDetails = new List<OrderDetail>();

            Supplier supplier = new Supplier("Exotic Liquids");
            _Suppliers.Add(supplier);
            _Products.Add(CreateProduct(supplier.SupplierID, "Chai"));
            _Products.Add(CreateProduct(supplier.SupplierID, "Chang"));
            _Products.Add(CreateProduct(supplier.SupplierID, "Aniseed Syrup"));


            supplier = new Supplier("New Orleans Cajun Delights");
            _Suppliers.Add(supplier);
            _Products.Add(CreateProduct(supplier.SupplierID, "Chef Anton's Cajun Seasoning"));
            _Products.Add(CreateProduct(supplier.SupplierID, "Chef Anton's Gumbo Mix"));


            supplier = new Supplier("Grandma Kelly's Homestead");
            _Suppliers.Add(supplier);
            _Products.Add(CreateProduct(supplier.SupplierID, "Grandma's Boysenberry Spread"));
            _Products.Add(CreateProduct(supplier.SupplierID, "Uncle Bob's Organic Dried Pears"));
            _Products.Add(CreateProduct(supplier.SupplierID, "Northwoods Cranberry Sauce"));
        }


       public static Product CreateProduct(int supplierID, string productName)
        {
            Product product = new Product(supplierID, productName);

            _OrderDetails.Add(new OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100)));
            _OrderDetails.Add(new OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100)));
            _OrderDetails.Add(new OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100))); 

            return product;
        }

        public static List<int> CreateFakeDataSource()
        {
            List<int> result = new List<int>();
            result.Add(0);
            return result;
        }

        public static string GetRandomString(int size)
        {
            StringBuilder sb = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }

    public class RtfLoadHelper
    {
        public static void Load(String fileName, IRichEditDocumentServer richEditControl)
        {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MasterDetailExample"+"."+ fileName);
                richEditControl.LoadDocument(stream, DocumentFormat.Rtf);
        }
    }
}
