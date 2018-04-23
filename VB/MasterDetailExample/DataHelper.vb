Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.Xpf.RichEdit
Imports System.IO
Imports System.Reflection
Imports DevExpress.XtraRichEdit

Namespace MasterDetailExample
	Friend Class DataHelper
		Private Shared _Suppliers As List(Of Supplier)
		Public Shared Property Suppliers() As List(Of Supplier)
			Get
				Return _Suppliers
			End Get
			Set(ByVal value As List(Of Supplier))
				_Suppliers = value
			End Set
		End Property

		Private Shared _Products As List(Of Product)
		Public Shared Property Products() As List(Of Product)
			Get
				Return _Products
			End Get
			Set(ByVal value As List(Of Product))
				_Products = value
			End Set
		End Property

		Private Shared _OrderDetails As List(Of OrderDetail)
		Public Shared Property OrderDetails() As List(Of OrderDetail)
			Get
				Return _OrderDetails
			End Get
			Set(ByVal value As List(Of OrderDetail))
				_OrderDetails = value
			End Set
		End Property

		Private Shared random As New Random(5)

		Public Shared Sub CreateData()
			_Suppliers = New List(Of Supplier)()
			_Products = New List(Of Product)()
			_OrderDetails = New List(Of OrderDetail)()

			Dim supplier As New Supplier("Exotic Liquids")
			_Suppliers.Add(supplier)
			_Products.Add(CreateProduct(supplier.SupplierID, "Chai"))
			_Products.Add(CreateProduct(supplier.SupplierID, "Chang"))
			_Products.Add(CreateProduct(supplier.SupplierID, "Aniseed Syrup"))


			supplier = New Supplier("New Orleans Cajun Delights")
			_Suppliers.Add(supplier)
			_Products.Add(CreateProduct(supplier.SupplierID, "Chef Anton's Cajun Seasoning"))
			_Products.Add(CreateProduct(supplier.SupplierID, "Chef Anton's Gumbo Mix"))


			supplier = New Supplier("Grandma Kelly's Homestead")
			_Suppliers.Add(supplier)
			_Products.Add(CreateProduct(supplier.SupplierID, "Grandma's Boysenberry Spread"))
			_Products.Add(CreateProduct(supplier.SupplierID, "Uncle Bob's Organic Dried Pears"))
			_Products.Add(CreateProduct(supplier.SupplierID, "Northwoods Cranberry Sauce"))
		End Sub


	   Public Shared Function CreateProduct(ByVal supplierID As Integer, ByVal productName As String) As Product
			Dim product As New Product(supplierID, productName)

			_OrderDetails.Add(New OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100)))
			_OrderDetails.Add(New OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100)))
			_OrderDetails.Add(New OrderDetail(supplierID, product.ProductID, GetRandomString(6), random.Next(0, 100)))

			Return product
	   End Function

		Public Shared Function CreateFakeDataSource() As List(Of Integer)
			Dim result As New List(Of Integer)()
			result.Add(0)
			Return result
		End Function

		Public Shared Function GetRandomString(ByVal size As Integer) As String
			Dim sb As New StringBuilder()
			Dim ch As Char
			For i As Integer = 0 To size - 1
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)))
				sb.Append(ch)
			Next i
			Return sb.ToString()
		End Function
	End Class

	Public Class RtfLoadHelper
		Public Shared Sub Load(ByVal fileName As String, ByVal richEditControl As IRichEditDocumentServer)
				Dim stream As Stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MasterDetailExample" & "." & fileName)
				richEditControl.LoadDocument(stream, DocumentFormat.Rtf)
		End Sub
	End Class
End Namespace
