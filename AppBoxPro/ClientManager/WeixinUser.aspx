<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeixinUser.aspx.cs" Inherits="NanXingGuoRen_WMS.ClientManager.WeixinUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Grid1" runat="server" />
        <f:Grid ID="Grid1"  ShowBorder="false" ShowHeader="false" Title="已星标的微信用户" runat="server" EnableCollapse="false"
            DataKeyNames="nickname,openid" EnableCheckBoxSelect="true" EnableMultiSelect="false"
            EnableRowDoubleClickEvent="true" OnRowDoubleClick="Grid1_RowDoubleClick" >
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbxsearch" EmptyText="在昵称中搜索"></f:TextBox>
                        <f:Button runat="server" ID="btnsearche" Text="搜索" OnClick="btnsearche_Click"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Columns>
                <f:RowNumberField />

                <f:BoundField Width="100px" ColumnID="nickname" DataField="nickname"  HeaderText="昵称"  />
                <f:BoundField ExpandUnusedSpace="true" ColumnID="openid" DataField="openid" HeaderText="openid" />
            </Columns>
            <Toolbars>
                <f:Toolbar runat="server" Position="Top">
                    <Items>
                        <f:Button ID="btnClose" EnablePostBack="false" Text="关闭" runat="server" Icon="SystemClose">
                        </f:Button>
                        <f:Button ID="btnSaveClose" Text="选择后关闭" runat="server" Icon="SystemSaveClose" OnClick="btnSaveClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Grid>
    </form>
</body>
</html>
