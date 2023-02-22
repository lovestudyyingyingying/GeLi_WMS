<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WareHouse_edit.aspx.cs"
    Inherits="GeLiPage_WMS.Stock.WareHouseControl.WareHouse_edit" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnClose" Icon="SystemClose" EnablePostBack="false" runat="server"
                            Text="关闭">
                        </f:Button>
                        <f:ToolbarSeparator ID="ToolbarSeparator1" runat="server">
                        </f:ToolbarSeparator>
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose"
                            OnClick="btnSaveClose_Click" runat="server" Text="保存后关闭">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:SimpleForm ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server"
                    BodyPadding="10px"  Title="SimpleForm">
                    <Items>
                        <f:TextBox ID="tbxName" runat="server" Label="名称" Required="true" ShowRedStar="true">
                        </f:TextBox>

                         <f:TextBox ID="tbxPosition" runat="server" Label="位置" Required="true" ShowRedStar="true">
                        </f:TextBox>

                        <f:CheckBox ID="cbStates"  runat="server" Label="状态" ShowRedStar="true" Text="启用" Checked="true">
                        </f:CheckBox>

                        <f:TextArea ID="tbxRemark" Label="备注" runat="server">
                        </f:TextArea>
                    </Items>
                </f:SimpleForm>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
