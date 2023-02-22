<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TouLiaoNew.aspx.cs" Inherits="NanXingGuoRen_WMS.ProductReport.TouLiao1.TouLiaoNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        .f-grid-row-summary .f-grid-cell-inner {
            font-weight: bold;
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="Panel1"></f:PageManager>

        <f:Panel runat="server" ID="Panel1" BodyPadding="5px" AutoScroll="true" ShowBorder="false" ShowHeader="false" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start">
            <Items>
                <f:Form runat="server" ID="form2" ShowBorder="true" ShowHeader="true" Title="排产信息" BodyPadding="5 5 0 5">
                    <Rows>

                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddl_Class" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddl_Class_SelectedIndexChanged"
                                    Width="300px" Label="车间类型" />
                                <%--<f:DatePicker runat="server" Label="生产日期" Required="true" ID="datepicker1" ShowRedStar="true" />--%>
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxBatchNo" runat="server" Label="原料批号" Required="true" />
                                <%--<f:DropDownList runat="server" ID="ddlname" EnableEdit="true" Label="产品品名" Required="true"></f:DropDownList>--%>
                            
                                <f:NumberBox ID="tbxWeight" runat="server" Label="物料重量(kg)" Text="50" />
                                <f:DropDownList runat="server" ID="ddlgrade"  Label="原料等级" Required="true">
                                    <f:ListItem  Text="A" Value="A"/>
                                    <f:ListItem  Text="A+" Value="A+"/>
                                </f:DropDownList>

                                <f:TextBox ID="tbxUserID" runat="server" Label="投料人" Required="true" Text="001" />

                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxProPosition" runat="server" Label="生产机台" Required="true" />
                                <f:NumberBox ID="tbxCount" runat="server" Label="数量" Required="true"  />
                                 <f:TextBox ID="tbxProUID" runat="server" Label="生产人员" Required="true" Text="030"  />

                                <f:ToolbarFill runat="server" />

                            </Items>
                        </f:FormRow>
                        
                        <f:FormRow>
                            <Items>
                                <f:DatePicker ID="dpProDate" runat="server" Label="投料时间" DateFormatString="yyyy-MM-dd HH:mm"
                                    ShowTime="true" ShowSecond="false" Required="true" />
                                <f:NumberBox ID="tbxJGSecond" runat="server" Label="投料间隔(秒)" Required="true" />
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>

                <f:Grid runat="server" BoxFlex="1" Title="产品明细" ShowHeader="false" ID="Grid2" ClicksToEdit="1"
                    AllowCellEditing="true" EnableCheckBoxSelect="true" PageSize="20" AutoSelectEditor="true"
                    SummaryPosition="Bottom" EnableSummary="true">

                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:Button ID="btnNew" runat="server" Icon="DatabaseAdd" EnablePostBack="false" Text="生成"
                                    OnClientClick="onAddClick()" ValidateForms="form2" />
                                <%--OnClientClick="if(!isValid()){return false;}"--%>
                                <f:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="保存"
                                    Icon="SystemSave">
                                </f:Button>
                                <f:Button ID="btnDelete" Text="删除选中行" Icon="Delete" EnablePostBack="false" runat="server" Hidden="true">
                                </f:Button>

                                <f:Button ID="btnReset" Text="重置表格数据" EnablePostBack="false" runat="server">
                                </f:Button>

                                <f:ToolbarFill runat="server"></f:ToolbarFill>

                                <f:DropDownBox runat="server" ID="DropDownBox1" DataControlID="CheckBoxList1" EnableMultiSelect="true" Values="" Width="500px"
                                    Label="合并单元格" Hidden="true">
                                    <PopPanel>
                                        <f:SimpleForm ID="SimpleForm2" BodyPadding="10px" runat="server" AutoScroll="true"
                                            ShowBorder="true" ShowHeader="false" Hidden="true">
                                            <Items>
                                                <f:Label ID="Label1" runat="server" Text="请选择编程语言：" Hidden="true"></f:Label>
                                                <f:CheckBoxList ID="CheckBoxList1" ColumnNumber="3" runat="server">
                                                    <f:CheckItem Text="品名" Value="name" />
                                                    <f:CheckItem Text="规格" Value="spec" />
                                                    <f:CheckItem Text="单位" Value="unit" />
                                                    <f:CheckItem Text="数量" Value="count" />
                                                    <f:CheckItem Text="批号" Value="batchNo" />
                                                    <f:CheckItem Text="箱号" Value="boxNo" />
                                                    <f:CheckItem Text="纸箱" Value="boxName" />
                                                    <f:CheckItem Text="状态/配料号" Value="ingredients" />
                                                    <f:CheckItem Text="备注" Value="remark1" Selected="true" />
                                                </f:CheckBoxList>
                                            </Items>
                                            <Toolbars>
                                                <f:Toolbar runat="server" Position="Top">
                                                    <Items>
                                                        <f:Button runat="server" ID="btnSelectAll" EnablePostBack="false" Text="全选">
                                                            <Listeners>
                                                                <f:Listener Event="click" Handler="onSelectAllClick" />
                                                            </Listeners>
                                                        </f:Button>
                                                        <f:Button runat="server" ID="btnClearAll" EnablePostBack="false" Text="清空">
                                                            <Listeners>
                                                                <f:Listener Event="click" Handler="onClearAllClick" />
                                                            </Listeners>
                                                        </f:Button>
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                        </f:SimpleForm>
                                    </PopPanel>
                                </f:DropDownBox>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>

                    <Columns>
                        <f:RowNumberField EnablePagingNumber="true"></f:RowNumberField>
                         <f:RenderField DataField="lotno" SortField="lotno" Width="150px" HeaderText="原料批号" ColumnID="lotno"></f:RenderField>
                      
                       
                        <f:RenderField DataField="RecTime" HeaderText="投料时间" Width="200px"  RendererArgument="yyyy-MM-dd HH:mm:ss" Renderer="Date" 
                            SortField="RecTime" ColumnID="RecTime"></f:RenderField>
                        <f:RenderField DataField="prosn" SortField="prosn" Width="180px" HeaderText="条码" ColumnID="prosn"></f:RenderField>

                          
                        <f:RenderField DataField="weight" SortField="weight" Width="150px" HeaderText="物料重量(kg)" ColumnID="weight"></f:RenderField>
                        <f:RenderField DataField="grade" SortField="grade" Width="80px" HeaderText="等级" ColumnID="grade" />
                        <f:RenderField DataField="userID" SortField="userID" Width="100px" HeaderText="投料人" ColumnID="userID"></f:RenderField>

                        <f:RenderField DataField="reserve1" SortField="reserve1" Width="120px" HeaderText="生产线别" ColumnID="reserve1" ></f:RenderField>
                       
                        <f:RenderField DataField="reserve2" SortField="reserve2" Width="120px" HeaderText="生产机台" ColumnID="reserve2"></f:RenderField>
                        <f:RenderField DataField="operator" SortField="operator" Width="120px" HeaderText="生产人员" ColumnID="operator"></f:RenderField>

                    </Columns>

                    <Listeners>
                        <%--<f:Listener Event="afteredit" Handler="onGridAfterEdit" />--%>
                        <f:Listener Event="afteredit" Handler="onGridAfterEdit1" />
                    </Listeners>

                </f:Grid>
            </Items>
        </f:Panel>


        <script>
            var dropDownBox1ClientID = '<%= DropDownBox1.ClientID %>';
            var checkBoxList1ClientID = '<%= CheckBoxList1.ClientID %>';

            function onSelectAllClick() {
                var checkBoxList1 = F(checkBoxList1ClientID);
                $.each(checkBoxList1.items, function (index, item) {
                    item.setValue(true);
                });

                // 将数据控件中的值同步到输入框
                F(dropDownBox1ClientID).syncToBox();
            }
            var grid1ClientID = '<%= Grid2.ClientID %>';

            function updateSummary() {
                var me = F(grid1ClientID), mathTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    mathTotal += me.getCellValue(tr, 'count');
                    //mathTotal += me.getCellValue(tr, 'MathScore');
                });

                // 第三个参数 true，强制更新，不显示左上角的更改标识
                me.updateSummaryCellValue('count', mathTotal, true);
                //me.updateSummaryCellValue('MathScore', mathTotal, true);
            }

            function onGridAfterEdit1() {
                updateSummary();
            }

            function onClearAllClick() {
                var checkBoxList1 = F(checkBoxList1ClientID);

                $.each(checkBoxList1.i++ + tems, function (index, item) {
                    item.setValue(false);
                });
                //将数据控件中的值同步到输入框
                F(dropDownBox1ClientID).syncToBox();
            }



            function onGridAfterEdit(event, value, params) {
                if (params.columnId === 'count') {
                    if (params.rowValue['count'] > params.rowValue['库存卷数']) {
                        //params.rowValue['count']
                        this.updateCellValue(params.rowId, 'count', params.rowValue['库存卷数'], true);
                        alert("不能大于库存卷数");
                    }
                }
                //改变了入库卷数，则改变面积
                if (params.columnId === 'count' || params.columnId === 'length' || params.columnId === 'width') {
                    this.updateCellValue(params.rowId, 'area', calculateCountValue(params.rowValue), true);
                }
            }

            <%--function editGetterGrid2(editor, columnId, rowId) {
                console.log(2783);
                var tbxSearch = '<%=tbxSearch.ClientID%>';
                F(tbxSearch).focus();
            }--%>

            //复制按钮
            //复制按钮
            function onCopyClick(event) {
                try {

                    // 选中行数据
                    var rowData = F(grid1ClientID).getSelectedRow(true);

                    var rowValue = rowData.values;
                    console.log(rowValue);
                    F(grid1ClientID).addNewRecord(rowValue, true);
                    //updateSummary();
                }
                catch (error) {
                    F.alert("请选中要复制的行")
                }
            }
            //新增按钮

            function onAddClick() {
                F(grid1ClientID).clearData();
                var count = parseInt(F('<%= tbxCount.ClientID %>').o0o10);
                var jg = parseInt(F('<%= tbxJGSecond.ClientID %>').o0o10);
                //console.log(F('<%= dpProDate.ClientID %>'));
                var sdtime = getDate24Hours();
                var time=new Date(F('<%= dpProDate.ClientID %>').o0o10);
               
                for (i = 1; i <= count; i++) {
                    var round = Math.floor(Math.random() * (999999 + 1)).toString();
                    var liushui = NumToString(round, 6);
                    var round2 = Math.floor(Math.random() * (99 + 1)).toString();
                    var liushui2 = NumToString(round2, 2);

                    var prosn = "1" + sdtime + liushui + liushui2;
                    F(grid1ClientID).addNewRecord(
                        {
                            "prosn": prosn,
                            "RecTime": AddSeconds(time, Math.floor(Math.random() * jg) + 3),
                            "lotno": F('<%= tbxBatchNo.ClientID %>').o0o10,
                            "weight": F('<%=tbxWeight.ClientID %>').o0o10,
                            "grade": F('<%=ddlgrade.ClientID %>').o0o10,
                            "userID": F('<%=tbxUserID.ClientID %>').o0o10,
                            "reserve1": F('<%=ddl_Class.ClientID %>').o0o10,
                            "reserve2": F('<%=tbxProPosition.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxProPosition.ClientID %>').o0o10,
                            "operator": F('<%=tbxProUID.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxProUID.ClientID %>').o0o10,
                         }
                        , true);

                }
                //F.alert(F(form2ID));
            }
            //将数字转化为固定长度的字符串<br data-filtered="filtered"> 
            //num为要转化的数字 len为要转化的长度
            function NumToString(num, len) {
                var numlen = num.toString().length; //得到num的长度
                var strChar = "0";  //空缺位填充字符
                var str = num;
                for (var i = 0; i < len - numlen; i++) {
                    str = strChar + str;
                }
                return str;
            }

            function getDate24Hours() {
                let myDate = new Date();
                let years = myDate.getFullYear();
                let month = myDate.getMonth();
                let day = myDate.getDate();
                
                month = month + 1;
                if (month < 10) {
                    month = "0" + month;
                }
                if (day < 10) {
                    day = "0" + day;
                }
                let time = years.toString().substring(2, 4) +'' + month
                //console.log("24-hours:" + time);
                return time;
            }
           

            function AddSeconds(d, value) {
                console.log(d.getSeconds());
                d.setSeconds(d.getSeconds() + value);
                var datetime = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + ':' + d.getSeconds();
                return datetime;
            }

            function calculateCountValue(rowValue) {
                var count = 0;
                count = rowValue['width'] / 1000 * rowValue['length'] * rowValue['count'];
                if (isNaN(count)) {
                    count = 0;
                }
                return count;
            }

            function RenderProvider(value) {
                return F(ddlProvider).getTextByValue(value);
            }
            function RenderClient(value) {
                return F(ddlClient).getTextByValue(value);
            }
            function renderItemno(value) {
                if (!value) {
                    return '';
                }
                var grid2 = F(grid2ClientID);

                return grid2.getRowData(value).text;
            }

            function onGrid2RowClick(event, grid2RowId) {
                var grid1 = F(grid1ClientID);
                var grid1RowId = grid1.getSelectedCell()[0];
                var rowValue = this.getRowValue(grid2RowId);

                grid1.updateCellValue(grid1RowId, {
                    //'Code2': grid2RowId,
                    //'Code': grid2RowId,
                    //'itemno': rowValue.itemno,
                    'name': rowValue.name,
                    'spec': rowValue.spec,
                    'model': rowValue.model,
                    //'inName': rowValue.inName,
                    //'color': rowValue.color,
                    //'material': rowValue.material,
                    //'Class': rowValue.Class,
                    'unit': '件',
                    'count': '0',
                    //'price': rowValue.price,
                    //'priceOut': '',
                    //'length': rowValue.length,
                    //'width': rowValue.width,
                    //'库存数量': rowValue.库存数量,
                    //'库存平方': rowValue.库存平方,
                    'remark': '',
                    //'PO_Item_ID': rowValue.PO_Item_ID,
                    'ingredients': '',
                    'boxName': '',
                    'boxNo': '',
                    'batchNo': '',

                });

            }

            function isValid() {
                var grid1 = F(grid1ClientID);
                var valid = true, modifiedData = grid1.getModifiedData();

                console.log(datepicker1.value);


                $.each(modifiedData, function (index, rowData) {

                    // rowData.id: 行ID
                    // rowData.status: 行状态（newadded, modified, deleted）
                    // rowData.values: 行中修改单元格对象，比如 { "Name": "刘国2", "Gender": 0, "EntranceYear": 2003 }
                    if (rowData.status === 'deleted') {
                        return true; // continue
                    }

                    //var itemno = rowData.values['itemno'];
                    var count = rowData.values['count'];
                    //var price = rowData.values['price'];
                    //var priceOut = rowData.values['priceOut'];
                    //// 更改了姓名列，并且为空字符串
                    //// 如果typeof(name)=='undefined'，则表示姓名没有更改，需要排除在外！！
                    //if (typeof (itemno) != 'undefined' && $.trim(itemno) == '') {
                    //    F.alert({
                    //        message: '料号不能为空！',
                    //        ok: function () {
                    //            grid1.startEdit(rowData.id, 'itemno');
                    //        }
                    //    });

                    //    valid = false;

                    //    return false; // break
                    //}

                    //count
                    if (typeof (count) != 'undefined' && $.trim(count) == '') {
                        F.alert({
                            message: '数量不能为空！',
                            ok: function () {
                                grid1.startEdit(rowData.id, 'count');
                            }
                        });

                        valid = false;

                        return false; // break
                    }

                });
                return valid;
            }
        </script>
    </form>
</body>
</html>
