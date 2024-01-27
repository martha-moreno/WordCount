<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="A8_WordCount._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <h1>Web Application Performing Automated MapReduce</h1>  
         <br /><br />
        <asp:FileUpload ID="FileUploadControl" runat="server" />
        <br />
        <asp:Button runat="server" Text="Upload" ID="btn_Upload" OnClick="btn_Upload_Click"></asp:Button>
        <br />
    </div>

    <div>
        <asp:Label ID="lbl_status" runat="server" Text="Upload Status:"  Font-Bold="true"> </asp:Label>
    </div>
    <br />
    <asp:Label ID="lbl_warning" runat="server" Text="Please upload a file" Visible="false" BackColor="Red" Font-Bold="true"></asp:Label>

    <div>
        <asp:Label ID="Label1" runat="server" Text="Choose N, the number of parallel threads. N>=1"></asp:Label>
        <br />
        <asp:TextBox ID="txt_N" runat="server"></asp:TextBox>
    </div>
    <div> 
        <asp:Label ID="Label2" runat="server" Text="Provide the Web service address for Map function"></asp:Label>
        <br />
        <asp:TextBox ID="txt_WebServiceMapFunction" runat="server"></asp:TextBox>
        <br /> <br />
        <asp:Label ID="Label3" runat="server" Text="Provide the Web service address for Reduce function"></asp:Label>
        <br />
        <asp:TextBox ID="txt_WebServiceReduceFunction" runat="server"></asp:TextBox>
        <br /><br />
        <asp:Label ID="Label4" runat="server" Text="Provide the Web service address for Combiner function"></asp:Label>
        <br />
        <asp:TextBox ID="txt_WebServiceCombinerFunction" runat="server"></asp:TextBox>
        <br /><br />
        <asp:Button ID="btn_PerformMapReduce" runat="server" Text="Perform Map Reduce Computation" OnClick="btn_PerformMapReduce_Click" />
        <br />
        <asp:Label ID="lbl_results" runat="server" Text="Display Results"></asp:Label>
       

        <asp:Label id="LengthLabel"
           runat="server">
        </asp:Label>  
        
        <br /><br />
       
        <asp:Label id="Contents"
           runat="server">
        </asp:Label>  
        
        <br /><br />

    </div>

</asp:Content>
