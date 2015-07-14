<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SQLPreview.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.SQLPreview" validateRequest=false%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">





<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="CampaignEditorPopups.css"/>

    <title></title>
    <script type="text/javascript">

        var skipUnload = false;

        function Page_Load() {


        }
        


        function Page_Unload() {
            if (skipUnload) {
                document.write("<div id=\"executeMessageDiv\">Executing Query. Please Wait...</div>");
            }
            else {
                window.opener.$find('ctl00_bodyContentPlaceHolder_previewMPE').hide();
            }

        }

        function Run_Button_Click() {
            skipUnload = true;
        }

    </script>
</head>



<body onLoad="javascript:Page_Load();" onUnload="javascript:Page_Unload();">

   
    
    <form id="sqlPreviewForm" runat="server">

    
        <div class="HeaderDiv">
            <label id="sqlHeaderLabel" class="SQLHeader">
                Enter values for the following parameters:
            </label>
        </div>
        
        <div id="runButtonDiv" class="RunDiv">
            <asp:LinkButton runat="server" Text="Run Query" ID="runButton" 
                onclick="runButton_Click" OnClientClick="javascript:Run_Button_Click();"/>
        </div>
                
        <div id="paramsDiv" class="Parameters">

        </div>

        
    </form>
</body>
</html>
