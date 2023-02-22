<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="GeLiPage_WMS.register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server"></f:PageManager>
        <f:Form runat="server" ShowBorder="false" ShowHeader="false" ID="form2" Margin="10px">
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button runat="server" ID="btnSubmit" Text="提交" OnClick="btnSubmit_Click" ValidateForms="form2">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Rows>
                <f:FormRow>
                    <Items>
                        <f:TextBox runat="server" Label="用户名" ID="tbxname" Required="true">
                        </f:TextBox>
                    </Items>
                </f:FormRow>
                <f:FormRow>
                    <Items>
                        <f:TextBox runat="server" Label="密码" ID="tbxpassword" TextMode="Password" Required="true">
                        </f:TextBox>
                    </Items>
                </f:FormRow>
                <f:FormRow>
                    <Items>
                        <f:TextBox runat="server" Label="中文名" ID="tbxchinesename" Required="true">
                        </f:TextBox>
                    </Items>
                </f:FormRow>
                <f:FormRow>
                    <Items>
                        <f:DropDownList runat="server" ForceSelection="true" ID="ddlDept" Required="true" Label="部门">
                        </f:DropDownList>
                    </Items>
                </f:FormRow>
                <f:FormRow>
                    <Items>
                        <f:RadioButtonList runat="server" ID="rblGender" Required="true" Label="性别">
                            <f:RadioItem Text="男" Value="男" Selected="true"  />
                            <f:RadioItem Text="女" Value="女" />
                        </f:RadioButtonList>
                    </Items>
                </f:FormRow>
                <f:FormRow>
                    <Items>
                        <f:TextBox runat="server" ID="tbxEmail" Label="邮箱" RegexPattern="EMAIL" Required="true"></f:TextBox>
                    </Items>
                </f:FormRow>
            </Rows>
        </f:Form>
    </form>
    <script>
        function okscript() {
            window.location.href='./default.aspx'
        }
    </script>
</body>
</html>
