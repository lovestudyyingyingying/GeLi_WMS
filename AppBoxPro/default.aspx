<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="GeLiPage_WMS._default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>系统登陆</title>
    <script type="text/javascript">

        // 本页面一定是顶层窗口，不会嵌在IFrame中
        if (top.window != window) {
            top.window.location.href = "./default.aspx";
        }

        // 将 localhost 转换为 localhost/default.aspx
        if (window.location.href.indexOf('/default.aspx') < 0) {
            window.location.href = "./default.aspx";
        }

    </script>
    <style>
        .login-image {
            background-color: #efefef;
            border-right: solid 1px #ddd;
        }

            .login-image img {
                width: 160px;
                height: 120px;
                padding: 10px;
                margin-top:40px
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server"></f:PageManager>
        <f:Window ID="Window1" runat="server" IsModal="true" Hidden="false" EnableClose="false"
            EnableMaximize="false" WindowPosition="GoldenSection" Icon="Key"   Title="系统登陆"
            Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" Width="500px">
            <Items>
                <f:Image ID="imageLogin" ImageUrl="~/res/images/login/微信图片_20210513153613.jpg" runat="server"
                    CssClass="login-image">
                </f:Image>
                <f:SimpleForm ID="SimpleForm1" LabelAlign="Top" BoxFlex="1" runat="server"
                    BodyPadding="30px 20px" ShowBorder="false" ShowHeader="false">
                    <Items>
                        <f:TextBox ID="tbxUserName" FocusOnPageLoad="true" runat="server" Label="帐号" Required="true"
                            ShowRedStar="true" Text="admin">
                        </f:TextBox>
                        <f:TextBox ID="tbxPassword" TextMode="Password" runat="server" Required="true" ShowRedStar="true"  Text="telenadmin99"
                            Label="密码">
                        </f:TextBox>
                    </Items>
                </f:SimpleForm>
            </Items>
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRegister" Icon="UserAdd" runat="server" EnablePostBack="false" Text="注册" hidden="true"></f:Button>
                        <f:Button ID="btnSubmit" Icon="LockOpen" Type="Submit" runat="server" ValidateForms="SimpleForm1"
                            OnClick="btnSubmit_Click" Text="登陆">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>

        <f:Window runat="server" ID="window2" Hidden="true" Height="400px" Width="700px" IsFluid="true" EnableIFrame="true"></f:Window>
    </form>
</body>
</html>
