<!-- default file list -->
*Files to look at*:

* [DataClasses.cs](./CS/MasterDetailExample/DataClasses.cs) (VB: [DataClasses.vb](./VB/MasterDetailExample/DataClasses.vb))
* [DataHelper.cs](./CS/MasterDetailExample/DataHelper.cs) (VB: [DataHelper.vb](./VB/MasterDetailExample/DataHelper.vb))
* [MainPage.xaml](./CS/MasterDetailExample/MainPage.xaml) (VB: [MainPage.xaml.vb](./VB/MasterDetailExample/MainPage.xaml.vb))
* [MainPage.xaml.cs](./CS/MasterDetailExample/MainPage.xaml.cs) (VB: [MainPage.xaml.vb](./VB/MasterDetailExample/MainPage.xaml.vb))
<!-- default file list end -->
# How to create Master-Detail report using RichEditDocumentServer and DOCVARIABLE fields


<p>This example shows how the <a href="http://documentation.devexpress.com/#Silverlight/CustomDocument5658"><u>MailMerge</u></a> feature enhanced with the <a href="http://documentation.devexpress.com/#Silverlight/CustomDocument5648"><u>DOCVARIABLE field</u></a> specifics with the help of the <a href="http://documentation.devexpress.com/#CoreLibraries/clsDevExpressXtraRichEditRichEditDocumentServertopic"><u>RichEditDocumentServer</u></a> text processing engine empowers the users to accomplish complex reporting tasks, such as creating Master-Detail reports. <br />
Template for each level is loaded in a document container that is the RIchEditDocumentServer instance and then a mail merge operation is performed. Mail merge destination is another RichEditDocumentServer instance playing the role of a text processor.<br />
Data are obtained from three objects of List type, which contain Supplier, Product and OrderDetail class instances respectively. Master-detail relationships are formed by using ID fields within those objects.</p><p>Click <strong>Merge to New Document</strong> at the <strong>Mailings</strong> Ribbon group to create a report.</p><p>The application screenshot is shown below.</p><p><img src="https://raw.githubusercontent.com/DevExpress-Examples/how-to-create-master-detail-report-using-richeditdocumentserver-and-docvariable-fields-e3377/11.1.4+/media/70877dbb-d7e1-47dd-a569-12b915bdfe1a.png"></p>

<br/>


