<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoveInventory.aspx.cs" Inherits="NanXingGuoRen_WMS.InventoryReport.MoveInventory" %>

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
                <f:Grid runat="server" IsDatabasePaging="true" DataKeyNames="ID,prosn" AllowPaging="true" ID="Grid1" OnPageIndexChange="Grid1_PageIndexChange"
                    AllowSorting="true" OnSort="Grid1_Sort" EnableTextSelection="true" SummaryPosition="Bottom" EnableSummary="true" SortField="OrderTime" Title="调拨明细"
                    SortDirection="DESC" EnableRowDoubleClickEvent="true" OnRowDoubleClick="Grid1_RowDoubleClick" OnRowCommand="Grid1_RowCommand"
                    BoxFlex="1" EnableCheckBoxSelect="true">
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:DatePicker runat="server" ID="dp1" Label="调拨日期" Width="200px" LabelWidth="80px"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dp2" Label="结束日期" ShowLabel="false" Width="120px"></f:DatePicker>
                                <f:DropDownList runat="server" Label="工序分类" ID="ddlPositionClass"
                                    AutoSelectFirstItem="false" EnableEdit="true" Width="200px" LabelWidth="80px"
                                     Hidden="true">
                                    <f:ListItem Text="原料车间" Value="原料车间" />
                                    <f:ListItem Text="烘烤车间" Value="烘烤车间" />
                                    <f:ListItem Text="大包装车间" Value="大包装车间" />
                                    <f:ListItem Text="小包装车间" Value="小包装车间" />
                                </f:DropDownList>
                                <f:Label Width="10px" Label=""></f:Label>
                                <f:TextBox runat="server" Label="产品名称" ID="tbxProname" Width="200px" LabelWidth="80px"></f:TextBox>
                               
                                 <f:TextBox runat="server" Label="批号" ID="tbxBatchNo" Width="200px" LabelWidth="50px"></f:TextBox>
                               
                                
                                <f:TextBox runat="server" Label="规格" ID="tbxSpec" Width="180px" LabelWidth="50px"></f:TextBox>
                                <f:TextBox runat="server" Label="产品标准" ID="tbxBiaoZhun" Width="200px" LabelWidth="80px"></f:TextBox>

                                <f:TextBox runat="server" Label="颜色" ID="tbxColor" Width="180px" LabelWidth="50px"></f:TextBox>
                                <f:ToolbarFill></f:ToolbarFill>
                                
                                <f:TextBox runat="server" EmptyText="关键词搜索" ID="tbxSearch" Label="关键词搜索" Hidden="true"></f:TextBox>

                            </Items>
                        </f:Toolbar>
                       
                    </Toolbars>


                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <%--  <f:CheckBox runat="server" Text="显示(收货数量)" ID="cbx2" OnCheckedChanged="cbx2_CheckedChanged" AutoPostBack="true" Hidden="true"></f:CheckBox>--%>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button runat="server" ID="btnSearch" Text="搜索" Icon="SystemSearch" OnClick="btnSearch_Click" Type="Submit"></f:Button>
                                <f:Button ID="btnBack" Text="后退" runat="server" Icon="PageBack" OnClick="btnBack_Click" Hidden="true"></f:Button>
                                <f:Button Type="Reset" Text="重置" runat="server" Icon="ArrowRefresh" ID="btnReset" OnClick="btnReset_Click" Hidden="true"></f:Button>
                                <f:Button runat="server" ID="btnExcel" Icon="PageExcel" Text="导出" OnClick="btnExcel_Click" DisableControlBeforePostBack="false" EnableAjax="false"></f:Button>
                                
                                
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField HeaderText="序号" Width="50px" HeaderTextAlign="Center" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField DataField="MissionNo" SortField="MissionNo" Width="150px" HeaderText="调拨单号" ColumnID="MissionNo" />
                       
                        <f:BoundField DataField="OrderTime" HeaderText="调拨时间" Width="180px" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}"
                            SortField="OrderTime" ColumnID="OrderTime"></f:BoundField>
                      <f:BoundField DataField="EndLocation" SortField="EndLocation" Width="120px" HeaderText="调拨仓位" ColumnID="EndLocation"></f:BoundField>
                       
                     <%--   <f:RenderField HeaderText="生产车间" DataField="positionClass" SortField="positionClass" ColumnID="positionClass">
                        </f:RenderField>--%>
                       <f:BoundField DataField="proname" SortField="proname" Width="300px" HeaderText="产品名称" ColumnID="proname"></f:BoundField>
                        <f:BoundField DataField="batchNo" SortField="batchNo" Width="150px" HeaderText="批号" ColumnID="batchNo"></f:BoundField>

                        <f:BoundField DataField="spec" SortField="spec" Width="90px" HeaderText="规格" ColumnID="spec"></f:BoundField>
                        <f:BoundField DataField="unit" SortField="unit" Width="80px" HeaderText="单位" ColumnID="unit">
                        </f:BoundField>
                        <%--<f:BoundField DataField="inName" SortField="inName" Width="150px" HeaderText="销售型号" ColumnID="inName"></f:BoundField>--%>

                        <f:RenderField DataField="procount" SortField="procount" Width="100px" HeaderText="调拨数量" ColumnID="procount" FieldType="Float" RendererFunction="RenderPrice" SummaryType="Sum"></f:RenderField>
                          
                        <f:BoundField DataField="color" SortField="color" Width="70px" HeaderText="颜色" ColumnID="color"></f:BoundField>
                        <f:BoundField DataField="boxName" SortField="boxName" Width="90px" HeaderText="箱名" ColumnID="boxName" Hidden="true"></f:BoundField>
                      
                        <f:BoundField DataField="probiaozhun" SortField="probiaozhun" Width="120px" HeaderText="产品标准" ColumnID="probiaozhun"></f:BoundField>

                        <f:BoundField DataField="remark" SortField="remark" Width="150px" HeaderText="备注" ColumnID="remark"></f:BoundField>
                       
                        <%--<f:LinkButtonField HeaderText="打印" Width="100px" Icon="Printer" CommandName="Print" ColumnID="print" TextAlign="Center"></f:LinkButtonField>--%>

                        <%-- <f:RenderField DataField="isClose" SortField="isClose" Width="100px" HeaderText="完结状态" ColumnID="isClose" RendererFunction="RenderClose"></f:RenderField>--%>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

        <f:Window runat="server" ID="Window1" EnableIFrame="true" Height="600px" Width="500px" Title="" Hidden="true"
            OnClose="Window1_Close"  EnableMaximize="true" EnableMinimize="true" EnableResize="true"></f:Window>

        <f:Window runat="server" ID="Window2" EnableIFrame="true" Height="900px" Width="1500px" Title="" Hidden="true" 
            EnableMaximize="true" EnableMinimize="true" OnClose="Window2_Close"  EnableResize="true"></f:Window>

        <%--<script src="../js/LodopFuncs.js"></script>--%>
        <script>
            function RenderPrice(value) {
                //return F.addCommas(value.toFixed(2));
            }

            function RenderClose(value) {
                if (value == 0)
                    return '未完结';
                else
                    return '已完结';
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
                            if (i < 3)
                            {
                                singlePageCount++;
                                let currentTop = top + i % pageSize * height;
                                LODOP.ADD_PRINT_RECT(currentTop, 31,804, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 83, 100, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 330, 197, 65, 0, 1);
                                LODOP.ADD_PRINT_RECT(currentTop, 592, 59, 65, 0, 1);

                            }else
                                break;
                        }
                        else
                        {
                            singlePageCount++;

                            let currentTop = top + i % pageSize * height;

                            LODOP.ADD_PRINT_RECT(currentTop, 31,804, 65, 0, 1);
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
