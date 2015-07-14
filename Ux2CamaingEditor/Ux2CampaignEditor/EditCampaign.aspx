<%@ Page Title="" Language="C#" MasterPageFile="~/GanoInternal.Master" AutoEventWireup="true" CodeBehind="EditCampaign.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.EditCampaign" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlaceHolder" runat="server">

    <link rel="stylesheet" type="text/css" href="CampaignEditor.css"/>

    <asp:HiddenField runat="server" ID="hiddenTarget" />

    <asp:ToolkitScriptManager runat="server" ID="campaignScriptManager">
    </asp:ToolkitScriptManager> 

    <asp:ModalPopupExtender ID="savedMPE" runat="server" PopupControlID="savedPanel" OkControlID="hiddenTarget" DropShadow="False" TargetControlID="hiddenTarget">
    </asp:ModalPopupExtender>
    
    <asp:ModalPopupExtender ID="saveFirstMPE" runat="server" PopupControlID="saveFirstPanel" OkControlID="noButton" DropShadow="False" TargetControlID="hiddenTarget">
    </asp:ModalPopupExtender>
    
    <div id="editCampaignHeaderDiv">

        <table class="CampaignIdTable">
            <tr>
                <td>
                    <label class="CampaignIdField" ID="CampaignIdTitle" for="campaignIdLabel">Campaign Id:</label>
                </td>
                <td>
                    <asp:Label runat="server" id="campaignIdLabel" class="CampaignId"></asp:Label>
                </td>
            </tr>
        
        </table>


        <div id="saveButtonDiv">
            <asp:LinkButton runat="server" ID="saveButton" onclick="saveButton_Click" CssClass="SaveButtons">Save Campaign</asp:LinkButton>
        </div>  
        <div id="applyButtonDiv">
            <asp:LinkButton runat="server" ID="applyButton" onclick="applyButton_Click" CssClass="SaveButtons">Apply Changes</asp:LinkButton>
        </div> 
                    
        <div id="editCampaignRightTopDiv"></div>
        <div id="editCampaignRightMiddleDiv"></div>
        <div id="editCampaignRightBottomDiv"></div>    
    </div>
    
    <div id="campaignInfoDiv"> 
        <p>
            <asp:Label runat="server" Text="Campaign Schedule Description:" ID="descriptionLabel" AssociatedControlID="campaignDescriptionTextBox" CssClass="CampaignInfo"></asp:Label>        
        </p>
        <p>
            <asp:TextBox runat="server" ID="campaignDescriptionTextBox" MaxLength="127" TextMode="MultiLine" CssClass="CampaignInfo CampaignDescription"></asp:TextBox>   
        </p>
        <p>
            <asp:CheckBox runat="server" ID="campaignStatusCheckBox" TextAlign="Left" Text="Active Campaign:"  CssClass="CampaignInfo" />    
        </p>
    </div>  
    
    <div id="emailsDiv">
        <div id="emailsHeaderDiv">
            <asp:Label runat="server" Text="Campaign Emails Found: " ID="emailsCountLabel" CssClass="EmailsCount"></asp:Label>
            <asp:LinkButton runat="server" ID="newEmailButton" 
                onclick="newEmailButton_Click" CssClass="EmailsNew">Add New Email</asp:LinkButton>
        
            <div id="emailsRightTopDiv"></div>
            <div id="emailsRightMiddleDiv"></div>
            <div id="emailsRightBottomDiv"></div>                 
        </div>
        <div class="EditorTablesDiv">
            <asp:Table runat="server" ID="emailsTable">
            </asp:Table>  
            <div id="emailsHeaderLeftDiv" class="EditorTablesHeaderLeftDiv"></div>
            <div id="emailsTableHeaderRightDiv" class="EditorTablesHeaderRightDiv"></div>      
        </div>
 
    </div>

    <asp:Panel ID="savedPanel" runat="server" Width="500px"  CssClass="PopupPanel">
         <div id="savedDiv" class="PopupPanelText">
             <asp:Label runat="server" Text="The tpye has been saved." ID="savedLabel"></asp:Label>
         </div>
         <div id="savedOKButtonDiv">
            <asp:Button ID="savedOKButton" runat="server" Text="OK" 
                 onclick="savedOKButton_Click" />
         </div>
    </asp:Panel> 
    
    <asp:Panel ID="saveFirstPanel" runat="server" Width="500px"  CssClass="PopupPanel">
         <div id="saveFirstDiv" class="PopupPanelText">
             <asp:Label runat="server" Text="The Campaign must be first saved before Emails can be added to it. Do you want to save now?" ID="saveFirstLabel"></asp:Label>
         </div>
         <div id="yesNoButtonsDiv">
            <asp:Button ID="yesButton" runat="server" Text="Yes" 
                 onclick="yesButton_Click" />
            <asp:Button ID="noButton" runat="server" Text="No" 
                 />
         </div>
    </asp:Panel> 
    
</asp:Content>

