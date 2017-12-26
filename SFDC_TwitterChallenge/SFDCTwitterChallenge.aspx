<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SFDCTwitterChallenge.aspx.cs" Inherits="SFDC_TwitterChallenge.SFDCTwitterChallenge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <style>
    label { position: absolute; text-align:right; width:130px; }
    input, textarea {
        }
    label.check, label.radio { position:relative; text-align:left; }

    img[src=""] {
    display: none;
}

</style>
    <script>
    function button_click(objTextBox,objBtnID)
    {
        if(window.event.keyCode==13)
        {
            document.getElementById(objBtnID).focus();
            document.getElementById(objBtnID).click();
        }
    }
        function txtSearch_OnFocusOut(objTextBox, objBtnID) {
            document.getElementById(objBtnID).focus();
            document.getElementById(objBtnID).click();
        }
        
</script>
    <form id="form1" runat="server" >
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:Timer ID="TimeRefresh" OnTick="TimeRefresh_Tick" runat="server" Interval="60000" />
        <div id="d">
            <br />
            <b>User Profile Image</b><br />
            <asp:Image ID="imgUserProfile" runat="server" Height="118px" Width="157px" />
            <br />
            <br />
            <br />
            <br />
            <table>
                <tr>
                    <td><b>User Name </b></td>
                    <td><b> : </b></td>
                    <td><asp:Label ID="lblUserName" runat="server" /></td>
                </tr>
                <tr>
                    <td><b>User Screen Name</b></td>
                    <td><b> : </b></td>
                    <td><asp:Label ID="lblUserScreenName" runat="server" /></td>
                </tr>
            </table>
            <br />
            <br />
            <table>
                <tr>
                    <td><b>Search Text</b></td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:TextBox ID="txtSearch" runat="server" ></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="search" />
                    </td>
                </tr>
            </table>


        </div>

        <table width="100%">
            <tr>
                <td width="100%">
               <asp:DataGrid id="grdTwitter" Width="100%" Visible="true" runat="server" CssClass="grid" AutoGenerateColumns="False">
                   <Columns>
                      <asp:TemplateColumn HeaderText="Tweet Data" ItemStyle-Width="60%"  >
                            <ItemTemplate>
                                <asp:Table runat="server" width="100%">
                                    <asp:TableRow>
                                        <asp:TableCell Width="150">
                                            <asp:Label Text='<%# DataBinder.Eval(Container.DataItem, "Text") %>' runat="server" />
                                        </asp:TableCell></asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <img width="200" height="160" src='<%# DataBinder.Eval(Container.DataItem, "Media_url_https") %>'>
                                        </asp:TableCell></asp:TableRow></asp:Table></ItemTemplate></asp:TemplateColumn>
                       <asp:BoundColumn ItemStyle-Width="20%" DataField="CreatedAt" HeaderText="Created At" ReadOnly="True" />
                      <asp:BoundColumn ItemStyle-Width="20%" DataField="Retweet_count" HeaderText="ReTweet Count" ReadOnly="True" />
                   </Columns>
                </asp:DataGrid>
                    </td>
                </tr>
        </table>
    </form>
</body>
</html>