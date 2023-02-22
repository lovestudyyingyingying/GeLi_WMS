<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BanChengPinReport.aspx.cs" Inherits="NanXingGuoRen_WMS.ProductReport.BanChengPinReport" %>

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
                    AllowSorting="true" OnSort="Grid1_Sort" EnableTextSelection="true" SummaryPosition="Bottom" EnableSummary="true" 
                    SortField="prodate" Title="半成品产量明细"
                    SortDirection="DESC" EnableRowDoubleClickEvent="true"  OnRowCommand="Grid1_RowCommand"
                    BoxFlex="1" EnableCheckBoxSelect="true">
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:DatePicker runat="server" ID="dp1" Label="生产日期" Width="200px" LabelWidth="80px"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dp2" Label="结束日期" ShowLabel="false" Width="120px"></f:DatePicker>
                                <%--<f:DropDownList runat="server" Label="工序分类" ID="ddlPositionClass"
                                    AutoSelectFirstItem="false" EnableEdit="true" Width="200px" LabelWidth="80px"
                                     Hidden="true">
                                    <f:ListItem Text="原料车间" Value="原料车间" />
                                    <f:ListItem Text="烘烤车间" Value="烘烤车间" />
                                    <f:ListItem Text="大包装车间" Value="大包装车间" />
                                    <f:ListItem Text="小包装车间" Value="小包装车间" />
                                </f:DropDownList>--%>
                                <f:Label Width="10px" Label=""></f:Label>
                                
                                
                                
                                 <f:TextBox runat="server" Label="原料批号" ID="tbxBatchNo" Width="250px" LabelWidth="80px"></f:TextBox>
                               
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
                                <f:Button runat="server" ID="btnNew" Text="新增" Icon="DatabaseAdd" OnClick="btnNew_Click" Hidden="true"></f:Button>
                                
                                
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField HeaderText="序号" Width="50px" HeaderTextAlign="Center" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField DataField="prosn" SortField="prosn" Width="180px" HeaderText="条码" ColumnID="prosn"></f:BoundField>
                        
                        <f:BoundField DataField="lotno" SortField="lotno" Width="150px" HeaderText="原料批号" ColumnID="lotno"></f:BoundField>
                      
                        <f:BoundField DataField="prodate" HeaderText="生产时间" Width="200px" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                            SortField="prodate" ColumnID="prodate"></f:BoundField>

                          
                        <f:BoundField DataField="weight" SortField="weight" Width="150px" HeaderText="物料重量(kg)" ColumnID="weight"></f:BoundField>
                        <f:BoundField DataField="grade" SortField="grade" Width="80px" HeaderText="等级" ColumnID="grade" />
                       
                        <%--<f:BoundField DataField="reserve1" SortField="reserve1" Width="120px" HeaderText="生产线别" ColumnID="reserve1" ></f:BoundField>--%>
                       
                        <f:BoundField DataField="reserve2" SortField="reserve2" Width="120px" HeaderText="生产机台" ColumnID="reserve2"></f:BoundField>
                        <f:BoundField DataField="operator" SortField="operator" Width="120px" HeaderText="生产人员" ColumnID="operator"></f:BoundField>

                      
                     
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
