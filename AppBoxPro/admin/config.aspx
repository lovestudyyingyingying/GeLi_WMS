<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="config.aspx.cs" Inherits="GeLiPage_WMS.admin.config" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" ShowHeader="false"  ShowBorder="false"
        BodyPadding="5px" AutoScroll="true" runat="server">
        <Items>
            <f:SimpleForm ID="SimpleForm1" runat="server" LabelWidth="120px" BodyPadding="5px"
                Width="600px" LabelAlign="Top"  ShowBorder="false"
                ShowHeader="false">
                <Items>
                    <f:TextBox ID="tbxTitle" runat="server" Label="网站标题" Required="true" ShowRedStar="true">
                    </f:TextBox>
                    <f:DropDownList ID="ddlTheme" Label="网站主题" runat="server" Required="true" ShowRedStar="true">
                        <f:ListItem Text="默认（Default）" Value="Default" />
                        <f:ListItem Text="Metro_Blue" Value="Metro_Blue" />
                        <f:ListItem Text="Metro_Dark_Blue" Value="Metro_Dark_Blue" />
                        <f:ListItem Text="Metro_Gray" Value="Metro_Gray" />
                        <f:ListItem Text="Metro_Green" Value="Metro_Green" />
                        <f:ListItem Text="Metro_Orange" Value="Metro_Orange" />
                         <f:ListItem Text="Pure_Black" Value="Pure_Black" />
                        <f:ListItem Text="Pure_Green" Value="Pure_Green" />
                        <f:ListItem Text="Pure_Blue" Value="Pure_Blue" />
                        <f:ListItem Text="Pure_Purple" Value="Pure_Purple" />
                        <f:ListItem Text="Pure_Orange" Value="Pure_Orange" />
                        <f:ListItem Text="Pure_Red" Value="Pure_Red" />
                        <f:ListItem Text="Black_Tie" Value="Black_Tie" />
                        <f:ListItem Text="Blitzer" Value="Blitzer" />
                        <f:ListItem Text="Cupertino" Value="Cupertino" />
                        <f:ListItem Text="Dark_Hive" Value="Dark_Hive" />
                        <f:ListItem Text="Dot_Luv" Value="Dot_Luv" />
                        <f:ListItem Text="Eggplant" Value="Eggplant" />
                        <f:ListItem Text="Excite_Bike" Value="Excite_Bike" />
                        <f:ListItem Text="Flick" Value="Flick" />
                        <f:ListItem Text="Hot_Sneaks" Value="Hot_Sneaks" />
                        <f:ListItem Text="Humanity" Value="Humanity" />
                        <f:ListItem Text="Le_Frog" Value="Le_Frog" />
                        <f:ListItem Text="Humanity" Value="Humanity" />
                        <f:ListItem Text="Mint_Choc" Value="Mint_Choc" />
                        <f:ListItem Text="Overcast" Value="Overcast" />
                        <f:ListItem Text="Pepper_Grinder" Value="Pepper_Grinder" />
                        <f:ListItem Text="Redmond" Value="Redmond" />
                        <f:ListItem Text="Smoothness" Value="Smoothness" />
                        <f:ListItem Text="South_Street" Value="South_Street" />
                        <f:ListItem Text="Start" Value="Start" />
                        <f:ListItem Text="Sunny" Value="Sunny" />
                        <f:ListItem Text="Swanky_Purse" Value="Swanky_Purse" />
                        <f:ListItem Text="Trontastic" Value="Trontastic" />
                        <f:ListItem Text="UI_Darkness" Value="UI_Darkness" />
                        <f:ListItem Text="UI_Lightness" Value="UI_Lightness" />
                        <f:ListItem Text="Vader" Value="Vader" />
                       
                    </f:DropDownList>
                    <f:DropDownList ID="ddlMenuType" Label="菜单样式" runat="server" Required="true" ShowRedStar="true">
                        <f:ListItem Text="树型菜单" Selected="true" Value="tree" />
                        <f:ListItem Text="手风琴菜单" Value="accordion" />
                    </f:DropDownList>
                    <f:NumberBox ID="nbxPageSize" runat="server" Label="表格默认记录数" Required="true" ShowRedStar="true">
                    </f:NumberBox>
                    <f:TextArea runat="server" ID="tbxHelpList" Height="320" Label="帮助下拉列表" Required="true"
                        ShowRedStar="true">
                    </f:TextArea>
                    <f:Button ID="btnSave" runat="server" Icon="SystemSave" OnClick="btnSave_OnClick"
                        ValidateForms="SimpleForm1" ValidateTarget="Top" Text="保存设置">
                    </f:Button>
                </Items>
            </f:SimpleForm>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
