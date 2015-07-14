<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Preview.aspx.cs" Inherits="GanoExcel.Ux2CampaignEditor.ExecQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="AETEPopups.css"/>

    <title></title>

    <script type="text/javascript">

        var skipUnload = false;

        function Page_Unload() {
            if (!skipUnload)
                window.opener.$find('ctl00_bodyContentPlaceHolder_previewMPE').hide();
        }

        function Back_Link_Click() {
            skipUnload = true;
        }

        function Window_Resize() {
            document.getElementById('resultsPanel').height = window.innerHeight - 85;

        }

        function Page_Load() {
            document.getElementById('resultsPanel').height = window.innerHeight - 85;

        }

    </script>   
</head>
<body onunload="Page_Unload();" onresize="Window_Resize();" onload="Page_Load();" class="PreviewBody PreviewFullHeight">
    <form id="execQueryForm" runat="server" class="PreviewFullHeight">
        <div class="PreviewHeader">
            <div class="ResultsDiv">
                <asp:Label runat="server" Text="Label" ID="resultsLabel"></asp:Label>
            </div>
            <div class="BackDiv">
                <asp:LinkButton runat="server" ID="backLabel" OnClientClick="javascript:Back_Link_Click();" >&lt;&lt; Back</asp:LinkButton>
            </div>
        </div>
<%--        <asp:Panel runat="server" ID="resultsPanel" style="margin-top:50px;">
        </asp:Panel>--%>
        <iframe  class="PreviewFrame " id="resultsPanel" runat="server" style="width:100%;">
        </iframe>
    </form>
</body>



</html>
