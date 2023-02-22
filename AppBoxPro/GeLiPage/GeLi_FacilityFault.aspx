<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeLi_FacilityFault.aspx.cs" Inherits="GeLiPage_WMS.GeLiPage.GeLi_FacilityFault" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="Panel1"></f:PageManager>
        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" ID="Panel1" Layout="HBox">
            <Items>
                <f:Grid runat="server" IsDatabasePaging="true" DataKeyNames="ID" AllowPaging="true" ID="Grid1"
                     Title="格力设备故障表" OnPageIndexChange="Grid1_PageIndexChange" OnRowDataBound="Grid1_RowDataBound"
                    AllowSorting="true" OnSort="Grid1_Sort" EnableTextSelection="true" SummaryPosition="Bottom" EnableSummary="true"
                    SortField="alarmDate" SortDirection="DESC" EnableRowDoubleClickEvent="true" OnRowCommand="Grid1_RowCommand"
                    BoxFlex="1" EnableCheckBoxSelect="false">
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:DropDownList runat="server" Label="日期类型" ID="DDL_DateType" ShowLabel="false" 
                                    AutoSelectFirstItem="true" EnableEdit="false" Width="90px" LabelWidth="80px">
                                    <f:ListItem Text="报警日期" Value="报警日期" Selected="true" />
                                    <f:ListItem Text="接收日期" Value="接收日期" />
                                </f:DropDownList>
                                <f:DatePicker runat="server" ID="dp1" Label="开始日期" Width="120px"  ShowLabel="false"/>
                                <f:DatePicker runat="server" ID="dp2" Label="结束日期" ShowLabel="false" Width="120px"/>
                                <f:DropDownList runat="server" Label="故障设备类型" ID="deviceFault" 
                                    AutoSelectFirstItem="true" EnableEdit="false" Width="200px" LabelWidth="120px">
                                    <f:ListItem Text="全部" Value="全部"  Selected="true"/>
                                    <f:ListItem Text="提升机" Value="提升机" />
                                    <f:ListItem Text="AGV" Value="AGV" />
                                    <f:ListItem Text="码盘机" Value="码盘机" />
                                </f:DropDownList>
                              


                                <f:TextBox runat="server" Label="故障详细描述" ID="faultDesc" Width="300px" LabelWidth="120px"/>

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
                        <f:RenderField DataField="deviceNum" SortField="deviceNum" ColumnID="deviceNum"
                            HeaderText="设备号" Width="100px" RendererFunction="renderMajor" />
                        <f:BoundField DataField="deviceName" SortField="deviceName" ColumnID="deviceName"
                            HeaderText="设备名称" Width="100px" />
                        <f:BoundField DataField="alarmDesc" SortField="alarmDesc" ColumnID="alarmDesc"
                            HeaderText="报警内容" Width="180px" />
                        <f:RenderField DataField="alarmGrade" SortField="alarmGrade" ColumnID="alarmGrade"
                            HeaderText="报警等级" Width="100px" RendererFunction="renderMajor" />
                        
                        <f:RenderField DataField="alarmDate" SortField="alarmDate" ColumnID="alarmDate"
                            HeaderText="设备报警时间" Width="200px" RendererFunction="renderMajor"/>
                        <f:BoundField DataField="recTime" SortField="recTime" ColumnID="recTime"
                            HeaderText="接收报警时间" Width="180px" />
                        

                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

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
                LODOP.PREVIEW();
            }

        </script>
    </form>
</body>
</html>
