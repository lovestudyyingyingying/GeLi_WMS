<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientManager_New.aspx.cs" Inherits="NanXingGuoRen_WMS.ClientManager.ClientManager_New" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="Panel1"></f:PageManager>
        <f:Panel  ID="Panel1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnClose" Icon="SystemClose" EnablePostBack="false" runat="server"
                            Text="关闭">
                        </f:Button>
                        <f:ToolbarSeparator ID="ToolbarSeparator2" runat="server">
                        </f:ToolbarSeparator>
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnSaveClose_Click"
                            runat="server" Text="保存后关闭">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"
                    Title="SimpleForm">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxNumber" Label="客户编号" Required="true"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxName" Label="名称" Required="true"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlCodeLevel" Label="分级代码" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlkehujingli" Label="客户经理"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlshishirenyuan" Label="实施人员"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlgendan" Label="跟单员"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DatePicker runat="server" ID="dpDate" Label="服务有效日期"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlcondiction" Label="服务条件" ForceSelection="true" EnableEdit="true">
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxremark" Label="备注"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxfax" Label="传真"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxaddress" Label="地址"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxbank" Label="开户银行"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbxemail" Label="EMAIL"></f:TextBox>
                            </Items>
                        </f:FormRow>
                    </Rows>

                </f:Form>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
