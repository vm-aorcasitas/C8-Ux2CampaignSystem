<%@ Page Title="" Language="C#" MasterPageFile="~/GanoInternal.Master" AutoEventWireup="true" CodeBehind="EditEmail.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.EditEmail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="CampaignEditor.css"/>
    
    <asp:HiddenField runat="server" ID="hiddenTarget" />

    <asp:ToolkitScriptManager runat="server" ID="emailScriptManager">
    </asp:ToolkitScriptManager> 
 
    <asp:ModalPopupExtender ID="savedMPE" runat="server" PopupControlID="savedPanel" OkControlID="hiddenTarget" DropShadow="False" TargetControlID="hiddenTarget">
    </asp:ModalPopupExtender>

    <asp:ModalPopupExtender ID="previewMPE" runat="server" PopupControlID="previewPanel" OkControlID="previewOKButton" DropShadow="False" TargetControlID="hiddenTarget">
    </asp:ModalPopupExtender>
    

    
    <script type="text/javascript">

        var popupWindow;



        function ShowSQLPopup() {

            $find('ctl00_bodyContentPlaceHolder_previewMPE').show();
            popupWindow = window.open("SQLPreview.aspx", "Preview", "toolbar=0,width=600,height=600,scrollbars=1");

            return false;
        }

        function ClosePopup() {

            popupWindow.close();

            return false;
        
        }

        function TemplateDropDownChanged() {
            var dropDown = document.getElementById('ctl00_bodyContentPlaceHolder_emailInfoAccordionPane_content_emailTemplatesDropDown');

            document.getElementById('ctl00_bodyContentPlaceHolder_emailInfoAccordionPane_content_templateGroupLabel').innerHTML = dropDown.options[dropDown.selectedIndex].title;

            return true;
        }

        function getCaret(el) {
            if (el.selectionStart) {
                return el.selectionStart;
            } else if (document.selection) {
                el.focus();

                var r = document.selection.createRange();
                if (r == null) {
                    return 0;
                }

                var re = el.createTextRange(),
                rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);

                return rc.text.length;
            }
            return 0;
        }

        function setSelectionRange(input, selectionStart, selectionEnd) {
            if (input.setSelectionRange) {
                input.focus();
                input.setSelectionRange(selectionStart, selectionEnd);
            }
            else if (input.createTextRange) {
                var range = input.createTextRange();
                range.collapse(true);
                range.moveEnd('character', selectionEnd);
                range.moveStart('character', selectionStart);
                range.select();
            }
        }

        function insertTab(el) {
            var curPos;

            curPos = getCaret(el);
            el.value = el.value.substr(0, curPos) + String.fromCharCode(9) + el.value.substr(curPos);
            setSelectionRange(el, curPos + 1, curPos + 1);

        }
                   
    </script>
    
    <div id="editEmailDiv" runat="server"> 
    
        <div id="editEmailHeaderDiv">
            <table class="EmailIdTable" id="emailHeaderTable">
                <tr>
                    <td>
                        <label class="EmailIdField" ID="EmailIdTitle" for="emailIdLabel">Email Id:</label>
                    </td>
                    <td>
                        <asp:Label runat="server" id="emailIdLabel" class="emailId"></asp:Label>
                    </td>
                </tr>
            </table>

            <div id="saveButtonDiv" class="SaveButtons">
                <asp:LinkButton runat="server" ID="saveButton" onclick="saveButton_Click" CssClass="SaveButtons">Save Email</asp:LinkButton>
            </div>  
            <div id="applyButtonDiv" class="SaveButtons">
                <asp:LinkButton runat="server" ID="applyButton" onclick="applyButton_Click" CssClass="SaveButtons">Apply Changes</asp:LinkButton>
            </div>  
                    
            <div id="editEmailRightTopDiv"></div>
            <div id="editEmailRightMiddleDiv"></div>
            <div id="editEmailRightBottomDiv"></div>  
        </div>
        
        <asp:Accordion runat="server" ID="editEmailAccordion" HeaderCssClass="EmailEditAccordionHeader" HeaderSelectedCssClass="EmailEditAccordionHeaderSelected" ContentCssClass="AccordionContent" CssClass="EmailAccordion" AutoSize="Limit" TransitionDuration="250">
            <Panes>
                <asp:AccordionPane runat="server" ID="emailInfoAccordionPane" >
                    <Header>
                        Email Settings
                        <div class="headerRight"></div>
                    </Header>
                    <Content>
                        <div class="HelpButtonDiv">
                            <asp:Label runat="server" ID="infoHelpLabel" CssClass="HelpLink">Help</asp:Label>
                        </div>
                        <div class="InnerContentDiv">
                            <asp:DropDownExtender runat="server" ID="infoHelpDropDown" TargetControlID="infoHelpLabel" DropDownControlID="infoHelpPanel" DropArrowWidth="0">
                            </asp:DropDownExtender>  
                            <p>
                                <asp:Label runat="server" Text="Campaign Email Description" ID="descriptionLabel" AssociatedControlID="descriptionTextBox"></asp:Label>  
                            </p>  
                            <p>    
                                <asp:TextBox runat="server" ID="descriptionTextBox" MaxLength="127" TextMode="MultiLine" CssClass="EmailDescriptionTextBox"></asp:TextBox>   
                            </p>
                            <p>
                                <asp:Label runat="server" Text="Email Template:" ID="emailTemplateLabel"></asp:Label>                        
                                <asp:HyperLink runat="server" Target="_blank"  ID="editEmailTemplatesLink">Edit Email Templates</asp:HyperLink>
                                <asp:DropDownList runat="server" ID="emailTemplatesDropDown">
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label runat="server" Text="Email Template Group:" ID="templateGroupLabel"></asp:Label>                        
                            </p>
                            <p>
                                <asp:Label runat="server" Text="Offset (dd:hh):" ID="offsetLabel" AssociatedControlID="offsetTextBox"></asp:Label>
                                <asp:TextBox runat="server" ID="offsetTextBox"></asp:TextBox>       
                            </p>
                            <p>
                                <asp:CheckBox runat="server" ID="statusCheckBox" TextAlign="Left" Text="Active Email:" /> 
                            </p>
                        </div>
                    </Content>
                </asp:AccordionPane> 
                  
                <asp:AccordionPane runat="server"  ID="queryAccordionPane"  >
                    <Header>
                        Email SQL Query
                        <div class="headerRight"></div>
                    </Header>
                    <Content>    

                        <div class="HelpButtonDiv">            
                            <asp:Label runat="server" ID="sqlQueryHelpLabel" CssClass="HelpLink">Help</asp:Label>
                        </div>
                        <div class="InnerContentDiv">
                            <asp:DropDownExtender runat="server" ID="sqlQueryHelpDropDown" TargetControlID="sqlQueryHelpLabel" DropDownControlID="sqlHelpPanel" DropArrowWidth="0">
                            </asp:DropDownExtender>
                            
                            <p>                            
                                <asp:Label runat="server" Text="SQL Qyery (optional):" ID="queryLabel" AssociatedControlID="queryTextBox"></asp:Label>
                                <asp:LinkButton runat="server" ID="previewSQLLink">Preview</asp:LinkButton> 
                            </p>
                            <p>
                                <asp:TextBox runat="server" ID="queryTextBox" Wrap="False" TextMode="MultiLine" CssClass="SQLPreviewDescriptionTextBox"></asp:TextBox>   
                            </p>
                        </div>
                    </Content>
                </asp:AccordionPane> 
                                    
 
                <asp:AccordionPane runat="server"  ID="parametersAccordionPane"  >
                    <Header>
                        Email Template Parameter Names
                        <div class="headerRight"></div>
                    </Header>
                    <Content> 
                    
                        <div class="HelpButtonDiv"> 
                            <asp:Label runat="server" ID="templateHelpLabel" CssClass="HelpLink">Help</asp:Label>
                        </div>
                        <div class="InnerContentDiv">
                            <asp:DropDownExtender runat="server" ID="templatesHelpDropDown" TargetControlID="templateHelpLabel" DropDownControlID="templateHelpPanel" DropArrowWidth="0">
                            </asp:DropDownExtender>
                            
                            <p>
                                <asp:Label runat="server" Text="Primary Source Field Name (optional):" ID="param1NameLable" AssociatedControlID="param1NameTextBox" CssClass="ParamLabels"></asp:Label>
                                <asp:TextBox runat="server" ID="param1NameTextBox" CssClass="ParamTestBoxes"></asp:TextBox>                  
                            </p>
                            <p>
                                <asp:Label runat="server" Text="Secondary Source Field Name (optional):" ID="param2NameLable" AssociatedControlID="param2NameTextBox" CssClass="ParamLabels"></asp:Label>
                                <asp:TextBox runat="server" ID="param2NameTextBox" CssClass="ParamTestBoxes"></asp:TextBox>    
                            </p>
                            <p>                            
                                <asp:Label runat="server" Text="Tertiary Source Field Name (optional):" ID="param3NameLable" AssociatedControlID="param3NameTextBox" CssClass="ParamLabels"></asp:Label>
                                <asp:TextBox runat="server" ID="param3NameTextBox" CssClass="ParamTestBoxes"></asp:TextBox>    
                            </p>
                            <p>                           
                                <asp:Label runat="server" Text="Event Data Field Name (optional):" ID="eventDataParamNameLable" AssociatedControlID="eventDataParamNameTextBox" CssClass="ParamLabels"></asp:Label>
                                <asp:TextBox runat="server" ID="eventDataParamNameTextBox" CssClass="ParamTestBoxes"></asp:TextBox>    
                            </p>
                            <p>                                                     
                                <asp:Label runat="server" Text="Distributor Id Field Name (optional):" ID="distIdParamNameLable" AssociatedControlID="distIdParamNameTextBox" CssClass="ParamLabels"></asp:Label>
                                <asp:TextBox runat="server" ID="distIdParamNameTextBox" CssClass="ParamTestBoxes"></asp:TextBox>
                            </p>
                    
                        </div>
                    </Content>
                </asp:AccordionPane> 
                                        
            </Panes>
        </asp:Accordion>                                

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
    
    <asp:Panel ID="previewPanel" runat="server" Width="500px"  CssClass="PopupPanel">
         <div id="previewDiv" class="PopupPanelText">
            Click OK to close the Preview window.
         </div>
         <div id="previewOKButtonDiv">
            <asp:Button ID="previewOKButton" runat="server" Text="Close" />
         </div>
    </asp:Panel>   

    <asp:Panel ID="infoHelpPanel" runat="server" Width="500px"  CssClass="DropDownHelpPanel">
         <div id="infoHelpDiv" class="DropDownHelpPanelText">
             <asp:TextBox runat="server" Text="Here a desciption for this campaign email can be entered. The Email Template is the type of email that will be created. The templates can be edited
             but the page must be refreshed for those changes to show up in the dropdown list. The Offset is also set here. The offset may be set as days:hours or just a number of hours. This
             is the number of hours that the system will wait before sending out this email from the time the campaing event was triggered. This campaign email may also be deactivated which means
             it will not be evaluated and no email will be sent." ID="infoHelpTextBox" CssClass="HelpText" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
         </div>
    </asp:Panel>   

    <asp:Panel ID="sqlHelpPanel" runat="server" Width="500px"  CssClass="DropDownHelpPanel">
         <div id="sqlHelpDiv" class="DropDownHelpPanelText">
             <asp:TextBox runat="server" Text="This SQL query is optional. If a query is provided here, when the campaing email is evaluated, an attempt will be made to replace the parameter 
             names (names in [{...}]) with values in the Parameters table. These values are generated when the instance of the campaing is entered in the queue. The parameters in the Email 
             Template will then be filled by values returned from this query. If no query is provided, then the values passed to the Email Template will be pulled from the Parameters table in 
             the database." ID="sqlHelpTextBox"  CssClass="HelpText" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
         </div>
    </asp:Panel>
    
        <asp:Panel ID="templateHelpPanel" runat="server" Width="500px"  CssClass="DropDownHelpPanel">
         <div id="templateHelpDiv" class="DropDownHelpPanelText">
             <asp:TextBox runat="server" Text="The names for these parameters indicate returned fields from the query for this campaign email or parameters from the Parameters table to be used
             by the selected Email Template. A blank parameter name means that no data will be passed for that parameter. For the Primary, Secondary, Tertiary and DistId parameters, the field they 
             reference should be an integer type." ID="helpTextBox" TextMode="MultiLine"  CssClass="HelpText" ReadOnly="True"></asp:TextBox>
         </div>
    </asp:Panel>
            
</asp:Content>

