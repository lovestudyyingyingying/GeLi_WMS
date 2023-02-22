<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchNoSCNew.aspx.cs" Inherits="NanXingGuoRen_WMS.ProductReport.BatchNoSC.BatchNoSCNew" %>

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
                                <f:DropDownList runat="server" ID="ddl_Class" AutoPostBack="true" OnSelectedIndexChanged="ddl_Class_SelectedIndexChanged"
                                    Width="300px" Label="车间类型" />
                                <%--<f:DatePicker runat="server" Label="生产日期" Required="true" ID="datepicker1" ShowRedStar="true" />--%>
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />

                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxOrderNo" runat="server" Label="排产单号" Required="true" />
                                <f:DropDownList runat="server" ID="ddlname" EnableEdit="true" Label="产品品名" Required="true"></f:DropDownList>
                                <f:TextBox ID="tbxSpec" runat="server" Label="产品规格" />
                                <f:DropDownList runat="server" Label="单位" Required="true" ID="ddlUnit">
                                    <f:ListItem Text="箱" Value="箱" Selected="true" />
                                    <f:ListItem Text="kg" Value="kg" />
                                    <f:ListItem Text="g" Value="g" />
                                    <f:ListItem Text="吨" Value="吨" />
                                    <f:ListItem Text="柜" Value="柜" />
                                    <f:ListItem Text="包" Value="包" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxBatchNo" runat="server" Label="批号" Required="true" />
                                <f:NumberBox ID="tbxCount" runat="server" Label="数量" Required="true" />
                                <f:TextBox ID="tbxBoxNo" runat="server" Label="纸箱号" />
                                <f:TextBox ID="tbxBoxName" runat="server" Label="纸箱名" />

                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <%--<f:TextBox ID="tbxERPNo" runat="server" Label="ERP订单"  Hidden="true"/>--%>
                                <%--<f:TextBox ID="tbxERPItem" runat="server" Label="ERP料号"  Hidden="true" />--%>
                                <f:TextArea ID="tbxRemark" runat="server" Label="备注" />
                                <f:TextBox ID="tbxColor" runat="server" Label="颜色"  />
                                <f:TextBox ID="tbxPosition" runat="server" Label="机台"  />

                                <f:ToolbarFill runat="server" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DatePicker ID="dpProDate" runat="server" Label="生产时间" DateFormatString="yyyy-MM-dd HH:mm"
                                    ShowTime="true" ShowSecond="false" Required="true" />
                                <f:NumberBox ID="tbxJGSecond" runat="server" Label="生产间隔(秒)" Required="true" />
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarFill runat="server" />
                            </Items>
                        </f:FormRow>

                       
                    </Rows>

                </f:Form>

                <f:Grid runat="server" BoxFlex="1" Title="产品明细" ShowHeader="false" ID="Grid1" ClicksToEdit="1"
                    AllowCellEditing="true" EnableCheckBoxSelect="true" PageSize="20" AutoSelectEditor="true"
                    SummaryPosition="Bottom" EnableSummary="true">

                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                               <f:Button ID="btnNew" runat="server" Icon="DatabaseAdd" EnablePostBack="false" Text="生成"
                                    OnClientClick="onAddClick()" ValidateForms="form2" />
                               <%--OnClientClick="if(!isValid()){return false;}"--%>
                                <f:Button runat="server"  ID="btnSave" OnClick="btnSave_Click" Text="保存"
                                    Icon="SystemSave">
                                </f:Button>
                                <f:Button ID="btnDelete" Text="删除选中行" Icon="Delete" EnablePostBack="false" runat="server" Hidden="true">
                                </f:Button>

                                <f:Button ID="btnReset" Text="重置表格数据" EnablePostBack="false" runat="server">
                                </f:Button>

                                <f:ToolbarFill runat="server"></f:ToolbarFill>

                                <f:DropDownBox runat="server" ID="DropDownBox1" DataControlID="CheckBoxList1" EnableMultiSelect="true" Values="" Width="500px"
                                    Label="合并单元格">
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
                        <f:RenderField HeaderText="条码" DataField="prosn" SortField="prosn" ColumnID="prosn" />

                        <f:RenderField HeaderText="生产日期" DataField="prodate" ColumnID="prodate" Width="180px"
                            RendererArgument="yyyy-MM-dd HH:mm:ss" Renderer="Date" />
                        <f:RenderField HeaderText="品名" DataField="name" SortField="name" ColumnID="name" Width="250px" />

                        <f:RenderField HeaderText="规格" DataField="spec" SortField="spec" ColumnID="spec" Width="120px" />

                        <f:RenderField HeaderText="单位" DataField="unit" SortField="unit" ColumnID="unit" />


                        <f:RenderField HeaderText="批号" ID="batchNo" DataField="batchNo" ColumnID="batchNo" />
                        <f:RenderField HeaderText="颜色" ID="color" DataField="color" ColumnID="color" />


                        <f:RenderField HeaderText="箱号" ID="boxNo" DataField="boxNo" ColumnID="boxNo" />


                        <f:RenderField HeaderText="箱名" ID="boxName" DataField="boxName" ColumnID="boxName" />

                        <f:RenderField HeaderText="ERP订单" DataField="ERPOrderNo" ColumnID="ERPOrderNo" Width="150px" />

                        <f:RenderField HeaderText="ERP料号" DataField="itemno" ColumnID="itemno" Width="120px" />




                        <f:RenderField HeaderText="备注（换行用；分号分隔）" DataField="remark1" ColumnID="remark1"
                            ExpandUnusedSpace="true" MinWidth="200px" />


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
            var grid1ClientID = '<%= Grid1.ClientID %>';

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
                var sdtime = new Date( F('<%= dpProDate.ClientID %>').o0o10);
                
                for (i = 1; i <= count; i++) {
                    var liushui = NumToString(i, 5);
                    
                    var prosn = "2" + F('<%= tbxOrderNo.ClientID %>').o0o10 + liushui;
                    F(grid1ClientID).addNewRecord(
                        {
                            "prosn": prosn,
                            "prodate": AddSeconds(sdtime, Math.floor(Math.random() * jg) + 3),
                            "name": F('<%= ddlname.ClientID %>').o0o10,
                            "spec": F('<%=tbxSpec.ClientID %>').o0o10,
                            "unit": F('<%=ddlUnit.ClientID %>').o0o10,
                            "batchNo": F('<%=tbxBatchNo.ClientID %>').o0o10,
                            "class": F('<%=ddl_Class.ClientID %>').o0o10,
                            "boxNo": F('<%=tbxBoxNo.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxBoxNo.ClientID %>').o0o10,
                            "boxName": F('<%=tbxBoxName.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxBoxName.ClientID %>').o0o10,
                          
                            
                            "remark1": F('<%=tbxRemark.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxRemark.ClientID %>').o0o10,
                            "color": F('<%=tbxColor.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxColor.ClientID %>').o0o10,
                            "position": F('<%=tbxPosition.ClientID %>').o0o10 == undefined ? "" : F('<%= tbxPosition.ClientID %>').o0o10,

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

            function AddSeconds(d, value) {
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
