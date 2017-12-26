<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SFDCTwitterChallenge.aspx.cs" Inherits="SFDC_TwitterChallenge.SFDCTwitterChallenge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css" integrity="sha384-PsH8R72JQ3SOdhVi3uxftmaW6Vc51MKb0q5P2rRUpPvrszuE4W1povHYgTpBfshb" crossorigin="anonymous">

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
        function button_click(objTextBox, objBtnID) {
            if (window.event.keyCode == 13) {
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
        <div id="d" class="container">
            <br />
            <b>User Profile Image</b><br />
            <asp:Image ID="imgUserProfile" runat="server" Height="118px" Width="157px" />
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
                <tr >
                    <td >
                         <div class="form-group">
                        <b>Search Text</b>
                            </div> 
                             </td>

                    <td>&nbsp;</td>
                    <td >
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-group" ></asp:TextBox>
                    </td>
                    <td>
                        <div class="form-group">
                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="btn btn-default" Text="search" />
                            </div>
                    </td>
                </tr>
            </table>


        </div>
        <div class="container">
        <table width="100%">
            <tr>
                <td width="100%">
                    <!--table table-hover table-striped -->
                    
               <asp:DataGrid id="grdTwitter" Width="100%" Visible="true" runat="server" CssClass="table table-hover" AutoGenerateColumns="False" UseAccessibleHeader="True">
                     
                   <Columns>
                       
                      <asp:TemplateColumn HeaderText="Tweet Data" ItemStyle-Width="60%"  >
                          <HeaderTemplate>
                               <asp:Label Text="Tweet Data" runat="server"></asp:Label>
                          </HeaderTemplate>
                            <ItemTemplate>

                                <asp:Table runat="server" width="100%">
                                    <asp:TableRow>
                                        <asp:TableCell Width="150">
                                            <div class="container">
                                                <asp:Label Text='<%# ( (string) Eval( "Text" ) ) %>' runat="server"  />
                                            </div>
                                            
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>

                                            <img width="200" CssClass="img-circle" height="160" src='<%# DataBinder.Eval(Container.DataItem, "Media_url_https") %>'>
                                        </asp:TableCell>

                                                                       </asp:TableRow>

                                </asp:Table></ItemTemplate>

                      </asp:TemplateColumn>
                       <asp:BoundColumn ItemStyle-Width="20%" DataField="CreatedAt" HeaderText="Created At" ReadOnly="True" />
                           
                      <asp:BoundColumn ItemStyle-Width="20%" DataField="Retweet_count" HeaderText="ReTweet Count" ReadOnly="True" />
                   </Columns>
                </asp:DataGrid>
                    </td>
                </tr>
        </table>
            </div>
    </form>
</body>
</html>
