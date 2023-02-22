<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user_select_title.aspx.cs"
    Inherits="GeLiPage_WMS.admin.user_select_title" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnClose" Icon="SystemClose" EnablePostBack="false" runat="server"
                            Text="关闭">
                        </f:Button>
                        <f:ToolbarSeparator ID="ToolbarSeparator1" runat="server">
                        </f:ToolbarSeparator>
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" EnablePostBack="false"
                            runat="server" Text="选择后关闭">
                            <Listeners>
                                <f:Listener Event="click" Handler="onSaveCloseClick" />
                            </Listeners>
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"
                    Title="SimpleForm">
                    <Rows>
                        <f:FormRow runat="server">
                            <Items>
                                <f:CheckBoxList ID="cblJobTitle" ColumnNumber="4" Label="所属角色" ShowLabel="false" runat="server">
                                </f:CheckBoxList>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
        </f:Panel>
    </form>
    <script>
        var cblJobTitleClientID = '<%= cblJobTitle.ClientID %>';

        function onSaveCloseClick() {
            // 数据源 - 复选框列表
            var cblJobTitle = F(cblJobTitleClientID);

            var titleNames = [], titleIds = [];
            //cblJobTitle.items.each(function (item) {
            //    // 是否选中
            //    if (item.getValue()) {
            //        titleNames.push(item.boxLabel);
            //        titleIds.push(item.inputValue);
            //    }
            //});
            var selectedValues = cblJobTitle.getValue();
            $.each(selectedValues, function (index, value) {
                titleIds.push(value);
                titleNames.push(cblJobTitle.getTextByValue(value));
            })

            // 返回当前活动Window对象（浏览器窗口对象通过F.getActiveWindow().window获取）
            var activeWindow = F.getActiveWindow();
            activeWindow.window.updateSelectedTitle(titleNames.join(','), titleIds.join(','));
            activeWindow.hide();
        }

    </script>
</body>
</html>
