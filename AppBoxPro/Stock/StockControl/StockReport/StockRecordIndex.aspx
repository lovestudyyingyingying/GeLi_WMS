<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockRecordIndex.aspx.cs"
    Inherits="GeLiPage_WMS.ProductionOrder.PlanOrderControl.StockRecordIndex" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <style>
       /* .f-grid-cell.f-grid-cell-ProName
        .f-grid-cell-inner {
            white-space: normal;
            word-break: break-all;
        }

        .f-grid-cell.f-grid-cell-Workshops
        .f-grid-cell-inner {
            white-space: normal;
            word-break: break-all;
        }*/
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="Panel1"></f:PageManager>
        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" ID="Panel1" Layout="HBox">
            <Items>
                <f:Grid runat="server" IsDatabasePaging="true" DataKeyNames="ID,PlanOrderNo" AllowPaging="true" ID="Grid1"
                    OnPageIndexChange="Grid1_PageIndexChange" Title="进出仓记录"
                    AllowSorting="true" OnSort="Grid1_Sort" EnableTextSelection="true" SummaryPosition="Bottom" EnableSummary="true"
                    SortField="OrderTime" SortDirection="DESC" EnableRowDoubleClickEvent="true" OnRowCommand="Grid1_RowCommand"
                    BoxFlex="1" EnableCheckBoxSelect="false">
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:DatePicker runat="server" ID="dp1" Label="任务日期" Width="200px" LabelWidth="80px" />
                                <f:DatePicker runat="server" ID="dp2" Label="结束日期" ShowLabel="false" Width="120px" />
                                <f:DropDownList runat="server" Label="任务类型" ID="ddlStockType"
                                    AutoSelectFirstItem="false" EnableEdit="true" Width="200px" LabelWidth="80px">
                                    <f:ListItem Text="全部" Value="全部" />
                                    <f:ListItem Text="进仓" Value="进仓" />
                                    <f:ListItem Text="出仓" Value="出仓" />
                                    <f:ListItem Text="调拨" Value="调拨" />
                                </f:DropDownList>
                                <f:TextBox runat="server" Label="产品名称" ID="tbxSearch"  Width="200px" LabelWidth="80px"/>

                                 <f:TextBox runat="server" Label="托盘条码" ID="tbxTrayNo" Width="200px" LabelWidth="80px"/>

                                <f:TextBox runat="server" Label="产品批次" ID="tbxBatchNo" Width="200px" LabelWidth="80px"/>

                                <f:Button runat="server" ID="btnSearch" Text="搜索" Icon="SystemSearch" OnClick="btnSearch_Click"
                                    Type="Submit"></f:Button>
                                
                                <f:Button runat="server" ID="btnExcel" Icon="PageExcel" Text="导出" OnClick="btnExcel_Click"
                                    DisableControlBeforePostBack="false" EnableAjax="false"></f:Button>

                            </Items>
                        </f:Toolbar>

                        <f:Toolbar runat="server" hidden="true">
                            <Items>
                               
                                <%--<f:TextBox runat="server" Label="规格" ID="tbxspec" Width="200px" LabelWidth="80px"></f:TextBox>--%>
                                <%--<f:TextBox runat="server" Label="批号" ID="tbxprovidername" Width="200px" LabelWidth="80px"></f:TextBox>--%>
                                <%--<f:TextBox runat="server" Label="厂家型号" ID="tbxmodel" Width="190px" LabelWidth="70px"></f:TextBox>
                                <f:TextBox runat="server" Label="销售型号" ID="tbxinName" Width="190px" LabelWidth="70px"></f:TextBox>--%>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>


                    <Toolbars>
                        <f:Toolbar runat="server" hidden="true">
                            <Items>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                              
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField HeaderText="序号" Width="50px" HeaderTextAlign="Center" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField DataField="StockType" SortField="StockType" ColumnID="StockType"
                            HeaderText="进出仓类型" Width="100px" />
                        <f:BoundField DataField="TrayNo" SortField="TrayNo" ColumnID="TrayNo"
                            HeaderText="托盘条码" Width="180px" />
                        <f:RenderField DataField="BatchNo" SortField="BatchNo" ColumnID="BatchNo"
                            HeaderText="产品批次" Width="100px" RendererFunction="renderMajor" />
                        <f:RenderField DataField="ProName" SortField="ProName" ColumnID="ProName"
                            HeaderText="产品名称" Width="200px" RendererFunction="renderMajor"/>
                        <f:BoundField DataField="ProCount" SortField="ProCount" ColumnID="ProCount"
                            HeaderText="箱数" Width="80px" />

                        <f:BoundField DataField="OrderTime" SortField="OrderTime" ColumnID="OrderTime"
                            HeaderText="执行时间" Width="180px" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <f:BoundField DataField="FinishTime" SortField="FinishTime" ColumnID="FinishTime"
                            HeaderText="完成时间" Width="180px" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <f:BoundField DataField="StartLocation" SortField="StartLocation" ColumnID="StartLocation"
                            HeaderText="起点仓位" Width="100px" />
                        <f:BoundField DataField="EndLocation" SortField="EndLocation" ColumnID="EndLocation"
                            HeaderText="终点仓位" Width="100px" />
                        <f:BoundField DataField="MissionNo" SortField="MissionNo" ColumnID="MissionNo"
                            HeaderText="任务单号" Width="150px" />
                        <f:BoundField DataField="StockTypeDesc" SortField="StockTypeDesc" ColumnID="StockTypeDesc"
                            HeaderText="调度类型" Width="100px" />
                        <f:BoundField DataField="OrderUser" SortField="OrderUser" ColumnID="OrderUser"
                            HeaderText="下发人员" Width="80px" />
                        <f:BoundField DataField="OrderAGV" SortField="OrderAGV" ColumnID="OrderAGV"
                            HeaderText="执行车辆" Width="80px" />
                        
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

        <f:Window runat="server" ID="Window1" EnableIFrame="true" Height="600px" Width="500px" Title="" Hidden="true"
            OnClose="Window1_Close" EnableMaximize="false" EnableMinimize="false" EnableResize="false">
        </f:Window>

        <f:Window runat="server" ID="Window2" EnableIFrame="true" Height="900px" Width="1500px" Title="" Hidden="true"
            EnableMaximize="true" EnableMinimize="true" OnClose="Window2_Close" EnableResize="true">
        </f:Window>

        <script src="../js/LodopFuncs.js"></script>
        <script>
            function RenderPrice(value) {
                return F.addCommas(value.toFixed(2));
            }

            function RenderClose(value) {
                if (value == 0)
                    return '未完结';
                else
                    return '已完结';
            }

            function renderMajor(value) {
                return F.formatString('<span data-qtip="{0}">{0}</span>', F.htmlEncode(value));
            }
        </script>

        <script type="text/javascript">
            function Print(PurOrderDto) {
                console.log(PurOrderDto);
                var LODOP = getLodop();
                LODOP.PRINT_INITA(0, 0, 2100, 2970, "自定义");
                LODOP.SET_PRINT_PAGESIZE(0, 2100, 2970, 'A4');
                LODOP.SET_PRINT_MODE("PRINT_PAGE_PERCENT", "85%");
                var header = PurOrderDto.title;
                var printList = PurOrderDto.pol;
                //for (var z = 0; z < PurOrderDto.length; z++) {


                //每页记录数
                var pageSize = 10;
                //表格中字体大小
                var fontSize = 12;
                //需要多少页
                var pageCount = Math.ceil(printList.length / pageSize);
                LODOP.NewPage();
                //表头
                LODOP.ADD_PRINT_TEXT(12, 290, 377, 45, PurOrderDto.title);
                LODOP.SET_PRINT_STYLEA(0, "FontSize", 23);
                LODOP.SET_PRINT_STYLEA(0, "Alignment", 2);
                for (var j = 0; j < pageCount; j++) {
                    var maxPage = (j + 1) * pageSize;//最后一页的最后索引
                    var minPage = j * pageSize;//开始索引
                    var top = 363;
                    var height = 64;
                    //本页的明细数
                    var singlePageCount = 0;
                    for (var i = minPage; i < maxPage; i++) {

                        if (i == printList.length) {
                            if (i < 3) {
                                singlePageCount++;
                                let currentTop = top + i % pageSize * height;
                                LODOP.ADD_PRINT_RECT(currentTop, 31, 804, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 83, 100, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 330, 197, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 592, 59, 65, 0, 1);

                            } else
                                break;
                        }
                        else {
                            singlePageCount++;

                            let currentTop = top + i % pageSize * height;

                            LODOP.ADD_PRINT_RECT(currentTop, 31, 804, 65, 0, 1);
                            LODOP.ADD_PRINT_RECT(currentTop, 83, 100, 65, 0, 1);
                            LODOP.ADD_PRINT_RECT(currentTop, 330, 197, 65, 0, 1);
                            LODOP.ADD_PRINT_RECT(currentTop, 592, 59, 65, 0, 1);

                            LODOP.ADD_PRINT_TEXT(currentTop + 20, 31, 52, 29, i + 1);
                            LODOP.SET_PRINT_STYLEA(0, "FontSize", fontSize);
                            LODOP.SET_PRINT_STYLEA(0, "Alignment", 2);
                        }
                    }
                }

                //}
                LODOP.PREVIEW();
            }

        </script>
    </form>
</body>
</html>
