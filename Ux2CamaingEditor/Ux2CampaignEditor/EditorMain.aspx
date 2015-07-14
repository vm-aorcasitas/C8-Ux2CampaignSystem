<%@ Page Title="" Language="C#" MasterPageFile="~/GanoInternal.Master" AutoEventWireup="true" CodeBehind="EditorMain.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.EditorMain" %>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlaceHolder" runat="server">
    <%--<link rel="stylesheet" type="text/css" href="CampaignEditor.css"/>--%>
    <link rel="stylesheet" type="text/css" href="GanoInternal.css"/>
    <div id="campaignsHeaderDiv">
        <asp:Label runat="server" Text="Campaign Schedules Found: 0" ID="countLabel" CssClass="EditorMainHeaderInfo EditorMainCount"></asp:Label>

        <asp:LinkButton runat="server" class="NewCampaign" Text="Add A New Campaign Schedule" 
            ID="newCampaignButton" CssClass="EditorMainHeaderInfo EditorMainNew" onclick="newCampaignButton_Click"/>

        <div id="mainRightTopDiv"></div>
        <div id="mainRightMiddleDiv"></div>
        <div id="mainRightBottomDiv"></div>
    </div>
    
    <div class="EditorTablesDiv" >
        <div id="campaignTableDiv">
            <asp:Table runat="server" ID="campaignsTable" class="EditorTables" >
            </asp:Table>
            <div id="campaignTableHeaderLeftDiv" class="EditorTablesHeaderLeftDiv"></div>
            <div id="campaignTableHeaderRightDiv" class="EditorTablesHeaderRightDiv"></div>
        </div> 
    </div>

</asp:Content>

