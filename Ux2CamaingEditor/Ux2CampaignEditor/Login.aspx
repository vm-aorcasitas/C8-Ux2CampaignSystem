<%@ Page Language="C#" MasterPageFile="~/GanoInternal.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.Login" %>

<asp:Content ID="content1" ContentPlaceHolderID="bodyContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="CampaignEditor.css"/>
    
    <div id="loginDiv">
    
        <asp:Login ID="loginControl" runat="server" class="loginClass"
            onauthenticate="loginControl_Authenticate" 
            DestinationPageUrl="" TitleText="" LoginButtonStyle-CssClass="LoginButton">
        </asp:Login>
    
    </div>
   
</asp:Content>