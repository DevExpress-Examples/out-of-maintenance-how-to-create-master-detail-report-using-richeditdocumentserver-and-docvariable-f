Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports DevExpress.Xpf.RichEdit
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports System.Collections

Namespace MasterDetailExample
	Partial Public Class MainPage
		Inherits UserControl
		Private Shared dataSuppliers As List(Of Supplier)
		Private Shared dataDetailedForProducts As List(Of Product)
		Private Shared dataDetailedForOrders As List(Of OrderDetail)
		Private Shared fakeDataSource As List(Of Integer)
		Private supplierID As Integer = -1
		Private productID As Integer = -1

		Public Sub New()
			InitializeComponent()

			AddHandler tabControl.SelectionChanged, AddressOf tabControl_SelectionChanged

			' Subscribe to the CalculateDocumentVariable event that triggers the master-detail report generation
			AddHandler resultRichEdit.CalculateDocumentVariable, AddressOf resultRichEdit_CalculateDocumentVariable

			' Load main template
			RtfLoadHelper.Load("main.rtf", mainRichEdit)

			' Create data sources for the project
			fakeDataSource = DataHelper.CreateFakeDataSource()
			DataHelper.CreateData()
			dataSuppliers = DataHelper.Suppliers
			dataDetailedForProducts = DataHelper.Products
			dataDetailedForOrders = DataHelper.OrderDetails

		End Sub

		#Region "#startmailmerge"
		Private Sub biMergeToNewDocument_ItemClick(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
			' Main template contains no merge fields, so we provide a fake data source.
			' Otherwise mail merge will not start.
			mainRichEdit.Options.MailMerge.DataSource = fakeDataSource

			' Trigger the multistage process. After the first mailmerge the CalculateDocumentVariable event
			'for the resultRichEdit control fires.
			mainRichEdit.MailMerge(resultRichEdit.Document)
			tabControl.SelectedTabItem = resultingDocumentTabItem
		End Sub
		#End Region ' #startmailmerge

		#Region "#secondstage"
		' Second stage. For each Supplier ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
		Private Sub resultRichEdit_CalculateDocumentVariable(ByVal sender As Object, ByVal e As CalculateDocumentVariableEventArgs)
			If e.VariableName = "Supplier" Then

				' Create a document container and load a document containing the Supplier header section (master section)
				Dim richServerSupplierTemplate As New RichEditDocumentServer()
				RtfLoadHelper.Load("supplier.rtf", richServerSupplierTemplate)
				' Create a text engine to process a document after the mail merge.
				Dim richServerMasterProcessor As New RichEditDocumentServer()
				' Provide a procedure for further processing
				AddHandler richServerMasterProcessor.CalculateDocumentVariable, AddressOf richServerMasterProcessor_CalculateDocumentVariable
				' Create a merged document using the Supplier template. The document will contain DOCVARIABLE fields with ProductID arguments. 
				' The CalculateDocumentVariable event for the richServerMasterProcessor fires.
				' Note that the data source for mail merge should be specified via the RichEditDocumentServer.Options.MailMerge.DataSource property.
				richServerSupplierTemplate.Options.MailMerge.DataSource = dataSuppliers
				richServerSupplierTemplate.MailMerge(richServerMasterProcessor.Document)
				RemoveHandler richServerMasterProcessor.CalculateDocumentVariable, AddressOf richServerMasterProcessor_CalculateDocumentVariable
				' Return the document to insert in place of the DOCVARIABLE field.
				e.Value = richServerMasterProcessor
				' Required. Otherwise e.Value will be ignored.
				e.Handled = True
			End If
		End Sub
		#End Region ' #secondstage
		#Region "#thirdstage"
		' Third stage. For each Product ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
		Private Sub richServerMasterProcessor_CalculateDocumentVariable(ByVal sender As Object, ByVal e As CalculateDocumentVariableEventArgs)
			Dim currentSupplierID As Integer = GetID(e.Arguments(0).Value)
			If currentSupplierID = -1 Then
				Return
			End If

			If supplierID <> currentSupplierID Then
				' Get data source that contains products for the specified supplier.
				dataDetailedForProducts = CType(GetProductsDataFilteredbySupplier(currentSupplierID), List(Of Product))
				supplierID = currentSupplierID
			End If

			If e.VariableName = "Product" Then
				' Create a document container and load a document containing the Product header section (detail section)
				Dim richServerProductsTemplate As New RichEditDocumentServer()
				RtfLoadHelper.Load("detail.rtf", richServerProductsTemplate)
				' Create a text engine to process a document after the mail merge.
				Dim richServerDetailProcessor As New RichEditDocumentServer()

				' Specify data source for mail merge.
				richServerProductsTemplate.Options.MailMerge.DataSource = dataDetailedForProducts
				' Specify that the resulting table should be joined with the header table.
				' Do not specify this option if calculated fields are not within table cells.
				Dim options As MailMergeOptions = richServerProductsTemplate.CreateMailMergeOptions()
				options.MergeMode = MergeMode.JoinTables
				' Provide a procedure for further processing.
				AddHandler richServerDetailProcessor.CalculateDocumentVariable, AddressOf richServerDetailProcessor_CalculateDocumentVariable
				' Create a merged document using the Product template. The document will contain DOCVARIABLE fields with OrderID arguments. 
				' The CalculateDocumentVariable event for the richServerDetail fires.
				richServerProductsTemplate.MailMerge(options, richServerDetailProcessor)
				RemoveHandler richServerDetailProcessor.CalculateDocumentVariable, AddressOf richServerDetailProcessor_CalculateDocumentVariable
				' Return the document to insert.
				e.Value = richServerDetailProcessor
				' This setting is required for inserting e.Value into the source document. Otherwise it will be ignored.
				e.Handled = True
			End If
		End Sub
		#End Region ' #thirdstage
		#Region "#fourthstage"
		' Fourth stage. For each Order ID create a detailed document that will be inserted in place of the DOCVARIABLE field.
		' This is the final stage and the Product.Orders template does not contain DOCVARIABLE fields. So, further processing is not required.
		Private Sub richServerDetailProcessor_CalculateDocumentVariable(ByVal sender As Object, ByVal e As CalculateDocumentVariableEventArgs)
			Dim currentProductID As Integer = GetID(e.Arguments(0).Value)
			If currentProductID = -1 Then
				Return
			End If

			If productID <> currentProductID Then
				' Get data source that contains orders for the specified product and supplier.
				dataDetailedForOrders = CType(GetOrderDataFilteredbyProductAndSupplier(supplierID, currentProductID), List(Of OrderDetail))
				productID = currentProductID
			End If

			If e.VariableName = "OrderDetails" Then

				Dim richServerOrdersTemplate As New RichEditDocumentServer()
				RtfLoadHelper.Load("detaildetail.rtf", richServerOrdersTemplate)
				richServerOrdersTemplate.Options.MailMerge.DataSource = dataDetailedForOrders

				Dim richServerDetailDetailProcessor As New RichEditDocumentServer()

				Dim options As MailMergeOptions = richServerOrdersTemplate.CreateMailMergeOptions()
				options.MergeMode = MergeMode.JoinTables

				richServerOrdersTemplate.MailMerge(options, richServerDetailDetailProcessor)

				e.Value = richServerDetailDetailProcessor
				e.Handled = True
			End If
		End Sub
		#End Region ' #fourthstage

		Protected Friend Overridable Function GetProductsDataFilteredbySupplier(ByVal supplierID As Integer) As Object
			Dim items As IList(Of Product) = DataHelper.Products
			Dim query = _
				From item In items _
				Where item.SupplierID = supplierID _
				Select item

			Return query.ToList()
		End Function

		Protected Friend Overridable Function GetOrderDataFilteredbyProductAndSupplier(ByVal supplierID As Integer, ByVal productID As Integer) As Object
			Dim query = _
				From item In DataHelper.OrderDetails _
				Where (item.ProductID = productID AndAlso item.SupplierID = supplierID) _
				Select item

			Return query.ToList()

		End Function

		Protected Friend Overridable Function GetID(ByVal value As String) As Integer
			Dim result As Integer
			If Int32.TryParse(value, result) Then
				Return result
			End If
			Return -1
		End Function

		#Region "#TabSwitchHelpers"
		Private Sub tabControl_SelectionChanged(ByVal sender As Object, ByVal e As DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs)
			Dim selectedRichEditControl As RichEditControl = GetSelectedRichEditControl()
			If selectedRichEditControl Is Nothing Then
				Return
			End If
			ResetBarManagers()
			selectedRichEditControl.BarManager = barManager1
			selectedRichEditControl.Ribbon = ribbon
		End Sub
		Protected Friend Overridable Function GetSelectedRichEditControl() As RichEditControl
			Dim selectedTabItem As Object = tabControl.SelectedItem
			If selectedTabItem Is templateTabItem Then
				Return mainRichEdit
			End If
			If selectedTabItem Is resultingDocumentTabItem Then
				Return resultRichEdit
			End If
			Return Nothing
		End Function
		Protected Friend Overridable Sub ResetBarManagers()
			mainRichEdit.BarManager = Nothing
			resultRichEdit.BarManager = Nothing
			mainRichEdit.Ribbon = Nothing
			resultRichEdit.Ribbon = Nothing
		End Sub
		#End Region ' #TabSwitchHelpers
	End Class
End Namespace
