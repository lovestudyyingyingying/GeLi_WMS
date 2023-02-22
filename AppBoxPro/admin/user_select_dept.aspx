<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user_select_dept.aspx.cs"
    Inherits="GeLiPage_WMS.admin.user_select_dept" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:pagemanager id="PageManager1" autosizepanelid="Grid1" runat="server" />
        <f:grid id="Grid1" runat="server" showborder="false" showheader="false" enablecheckboxselect="true"
            datakeynames="ID,Name" enablemultiselect="false" onrowdatabound="Grid1_RowDataBound" 
            DataIDField="ID">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" Position="Top" runat="server">
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
            <Columns>
                <f:RowNumberField />
                <f:BoundField ColumnID="Name" DataField="Name" HeaderText="部门名称" DataSimulateTreeLevelField="TreeLevel"
                    Width="150px" />
                <f:BoundField DataField="Remark" HeaderText="部门描述" ExpandUnusedSpace="true" />
            </Columns>
        </f:grid>
    </form>
    <script>
        var grid1ClientID = '<%= Grid1.ClientID %>';

        // 去掉字符串中的html标签
        function stripHtmlTags(str) {
            if (str) {
                return str.replace(/<[^>]*>/g, '');
            }
            return '';
        }

        function onSaveCloseClick() {
            // 数据源 - 表格控件
            var grid1 = F(grid1ClientID);

            var deptName = '', deptId = '';
            //var selection = grid1.getSelectionModel().getSelection();
            //Ext.Array.each(selection, function (record, index) {
            //    deptId = record.getId()
            //    deptName = stripHtmlTags(record.data['Name']);
            //    // 单选，找到第一个选中行即可
            //    return false; // break
            //});

            var selectedRowData = grid1.getSelectedRow(true);
            deptId = selectedRowData.id;
            deptName = stripHtmlTags(selectedRowData.values.Name);


            

            // 返回当前活动Window对象（浏览器窗口对象通过F.getActiveWindow().window获取）
            var activeWindow = F.getActiveWindow();
            activeWindow.window.updateSelectedDept(deptName, deptId);
            activeWindow.hide();
        }

    </script>
</body>
</html>
