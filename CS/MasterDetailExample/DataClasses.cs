using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.Xpf.Collections;


namespace MasterDetailExample
{

    public class Supplier {
        static int nextID = 0;
        int id;
        string name;

        public int SupplierID { get { return id; } }
        public string CompanyName { get { return name; } }

        public Supplier(string name) {
            this.name = name;

            this.id = nextID;
            nextID++;
        }

    }

    public class Product {
        static int nextID = 0;

        int suppID;
        int prodID;
        string name;

        public int SupplierID { get { return suppID; } }
        public int ProductID { get { return prodID; } }
        public string ProductName { get { return name; } }

        public Product(int suppID, string name) {
            this.suppID = suppID;
            this.name = name;

            this.prodID = nextID;
            nextID++;
        }
    }

    public class OrderDetail {
        int supplierID;
        int prodID;
        string orderID;
        short quantity;
        public int SupplierID { get { return supplierID; } }
        public int ProductID { get { return prodID; } }
        public string OrderID { get { return orderID; } }
        public short Quantity { get { return quantity; } }

        public OrderDetail(int supplierID, int prodID, string orderID, int quantity) {
            this.supplierID = supplierID;
            this.prodID = prodID;
            this.orderID = orderID;
            this.quantity = Convert.ToInt16(quantity);
        }
    }
}
