Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Collections
Imports System.ComponentModel
Imports System.Linq
Imports DevExpress.Xpf.Collections


Namespace MasterDetailExample

	Public Class Supplier
		Private Shared nextID As Integer = 0
		Private id As Integer
		Private name As String

		Public ReadOnly Property SupplierID() As Integer
			Get
				Return id
			End Get
		End Property
		Public ReadOnly Property CompanyName() As String
			Get
				Return name
			End Get
		End Property

		Public Sub New(ByVal name As String)
			Me.name = name

			Me.id = nextID
			nextID += 1
		End Sub

	End Class

	Public Class Product
		Private Shared nextID As Integer = 0

		Private suppID As Integer
		Private prodID As Integer
		Private name As String

		Public ReadOnly Property SupplierID() As Integer
			Get
				Return suppID
			End Get
		End Property
		Public ReadOnly Property ProductID() As Integer
			Get
				Return prodID
			End Get
		End Property
		Public ReadOnly Property ProductName() As String
			Get
				Return name
			End Get
		End Property

		Public Sub New(ByVal suppID As Integer, ByVal name As String)
			Me.suppID = suppID
			Me.name = name

			Me.prodID = nextID
			nextID += 1
		End Sub
	End Class

	Public Class OrderDetail
		Private supplierID_Renamed As Integer
		Private prodID As Integer
		Private orderID_Renamed As String
		Private quantity_Renamed As Short
		Public ReadOnly Property SupplierID() As Integer
			Get
				Return supplierID_Renamed
			End Get
		End Property
		Public ReadOnly Property ProductID() As Integer
			Get
				Return prodID
			End Get
		End Property
		Public ReadOnly Property OrderID() As String
			Get
				Return orderID_Renamed
			End Get
		End Property
		Public ReadOnly Property Quantity() As Short
			Get
				Return quantity_Renamed
			End Get
		End Property

		Public Sub New(ByVal supplierID As Integer, ByVal prodID As Integer, ByVal orderID As String, ByVal quantity As Integer)
			Me.supplierID_Renamed = supplierID
			Me.prodID = prodID
			Me.orderID_Renamed = orderID
			Me.quantity_Renamed = Convert.ToInt16(quantity)
		End Sub
	End Class
End Namespace
