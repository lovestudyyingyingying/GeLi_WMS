<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MissionRecordWebForm.aspx.cs" Inherits="GeLiPage_WMS.MissionRecordWebForm.MissionRecordWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="Panel1"></f:PageManager>
        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" ID="Panel1" Layout="HBox">
            <Items>

                <f:Grid runat="server" Title="任务管理" IsDatabasePaging="true"
                    DataKeyNames="ID" BoxFlex="1"
                    AllowPaging="true" ID="Grid1"
                    EnableCheckBoxSelect="true" EnableTextSelection="true" OnPageIndexChange="Grid1_PageIndexChange"
                    SummaryPosition="Bottom" EnableSummary="true" OnSort="Grid1_Sort"
                    AllowSorting="true" SortingCancel="true" SortingToolTip="true" SortingMulti="true"
                    AllowColumnLocking="true" EnableColumnLines="true" EnableRowLines="true">
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:DatePicker runat="server" ID="dp1" Label="下发日期" Width="200px" ShowLabel="true" LabelWidth="80px"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dp2" Label="结束日期" ShowLabel="false" Width="120px"></f:DatePicker>
                                <f:TextBox runat="server" Label="任务号" ID="tbxMissionNo" Width="300px" LabelWidth="80px"></f:TextBox>
                             
   
                                <f:Button runat="server" ID="btnSearch" Text="搜索" Icon="SystemSearch" OnClick="btnSearch_Click" EnableAjax="false" DisableControlBeforePostBack="true"></f:Button>

                            </Items>
                        </f:Toolbar>
                   <f:Toolbar runat="server">

                            <Items>
                                   <f:DropDownList runat="server" ID="DropDownListName" Label="工序名称" Width="220px" LabelWidth="75px">
                                    <f:ListItem Text="全部" Value="全部" Selected="true" />
                                    <f:ListItem Text="胀管空托上线" Value="胀管空托上线" />
                                    <f:ListItem Text="胀管物料下线" Value="胀管物料下线" />
                                    <f:ListItem Text="烘干物料上线" Value="烘干物料上线" />
                                    <f:ListItem Text="烘干空托下线" Value="烘干空托下线" />
                                    <f:ListItem Text="焊接空托上线" Value="焊接空托上线" />
                                    <f:ListItem Text="焊接物料下线" Value="焊接物料下线" />
                                    <f:ListItem Text="喷氧物料上线" Value="喷氧物料上线" />
                                    <f:ListItem Text="喷氧空托下线" Value="喷氧空托下线" />
                                    <f:ListItem Text="切割空托上线" Value="切割空托上线" />
                                    <f:ListItem Text="切割物料下线" Value="切割物料下线" />
                                    <f:ListItem Text="氮检物料上线" Value="氮检物料上线" />
                                    <f:ListItem Text="氮检空托下线" Value="氮检空托下线" />

                                </f:DropDownList>
                                                             <f:DropDownList runat="server" ID="DropDownSendState" Label="任务状态" Width="200px" LabelWidth="75px">
                                    <f:ListItem Text="全部" Value="全部" Selected="true" />

                                    <f:ListItem Text="已分类" Value="已分类" />
                                    <f:ListItem Text="成功" Value="成功" />
                                    <f:ListItem Text="失败" Value="失败" />
                                    <f:ListItem Text="步骤一" Value="步骤一" />
                                    <f:ListItem Text="步骤二" Value="步骤二" />


                                </f:DropDownList>
                                <f:DropDownList runat="server" ID="DropDownListRunState" Label="运行状态" Width="200px" LabelWidth="75px">
                                    <f:ListItem Text="全部" Value="全部" Selected="true" />

                                    <f:ListItem Text="已完成" Value="已完成" />
                                    <f:ListItem Text="已取货" Value="已取货" />
                                    <f:ListItem Text="已离开" Value="已离开" />
                                    <f:ListItem Text="已取消" Value="已取消" />
                                </f:DropDownList>
                                <f:Button runat="server" ID="btnCancel" Text="取消任务" Icon="Cancel" OnClick="btnCancel_Click" ConfirmText="确定要取消该任务吗？" EnableAjax="false" DisableControlBeforePostBack="true"></f:Button>
                                <f:Button runat="server" ID="btnFinish" Text="手动完成" Icon="Overlays" OnClick="btnFinish_Click" ConfirmText="确定要手动完成该任务吗？" EnableAjax="false" DisableControlBeforePostBack="true"></f:Button>

                            </Items>
                        </f:Toolbar>
                    </Toolbars>


                    <Columns>
                        <f:RowNumberField HeaderText="序号" Width="50px" HeaderTextAlign="Center" TextAlign="Center"></f:RowNumberField>
                        <f:RenderField ColumnID="MissionNo" DataField="MissionNo" SortField="MissionNo" Width="140px" HeaderText="任务编号"
                            Locked="true" />

                        <f:BoundField ColumnID="OrderTime" DataField="OrderTime" SortField="OrderTime" Width="180px" HeaderText="下发时间"
                            DataFormatString="{0:yyyy-MM-dd hh:mm:ss}" />
                        <f:BoundField ColumnID="NodeTime" DataField="NodeTime" SortField="NodeTime" Width="180px" HeaderText="节点时间"
                            DataFormatString="{0:yyyy-MM-dd hh:mm:ss}" />
                        <f:RenderField ColumnID="TrayNo" DataField="TrayNo" SortField="TrayNo" HeaderText="货物条码" Width="160px" />

                        <f:RenderField HeaderText="任务类型" ColumnID="Mark" DataField="Mark" SortField="Mark" HeaderTextAlign="Center" TextAlign="Center" Width="140px"></f:RenderField>
                        <f:RenderField HeaderText="起始AGV点位" ColumnID="StartLocation" DataField="StartLocation" SortField="StartLocation" HeaderTextAlign="Center" TextAlign="Center" Width="120px"></f:RenderField>

                        <f:RenderField ColumnID="StartPosition" DataField="StartPosition" SortField="StartPosition" Width="120px" HeaderText="起始库位点位" />

                        <f:RenderField ColumnID="EndLocation" DataField="EndLocation" SortField="EndLocation" Width="120px" HeaderText="结束AGV点位" />
                          <f:RenderField ColumnID="EndPosition" DataField="EndPosition" SortField="EndPosition" Width="120px" HeaderText="起始库位点位" />



                        <f:RenderField ColumnID="SendState" DataField="SendState" SortField="SendState" Width="120px" HeaderText="任务状态" />
                        <f:RenderField ColumnID="RunState" DataField="RunState" SortField="RunState" Width="80px" HeaderText="运行状态" />
                        <f:RenderField ColumnID="AGVCarId" DataField="AGVCarId" SortField="AGVCarId" Width="80px" HeaderText="AGV车号" />
                        <f:RenderField ColumnID="userId" DataField="userId" SortField="userId" Width="80px" HeaderText="下发用户" />
                        <f:RenderField ColumnID="Reserve1" DataField="Reserve1" SortField="Reserve1" Width="120px" HeaderText="工序名称" />



                        <%--  
                        <f:RenderField ColumnID="BoxNo" DataField="BoxNo" SortField="BoxNo" Width="120px" HeaderText="箱号"
                            />--%>
                    </Columns>
                    <%--      <PageItems>
                        <f:ToolbarSeparator ID="ToolbarSeparator2" runat="server">
                        </f:ToolbarSeparator>
                        <f:TextBox runat="server" Label="筛选条件" ID="grid1Tbxname" Width="900px" LabelWidth="78px"></f:TextBox>
                        <f:Button ID="btnBackFind" Text="后退" runat="server" OnClick="btnBackFind_Click" />
                        <f:Button ID="btnSort" Text="高级排序" runat="server" OnClick="btnSort_Click" />
                    </PageItems>--%>
                </f:Grid>
            </Items>
        </f:Panel>

    </form>
</body>
</html>
