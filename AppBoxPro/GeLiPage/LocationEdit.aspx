<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LocationEdit.aspx.cs"
    Inherits="GeLiPage_WMS.GeLiPage.LocationEdit" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style>
        .btn-in-form {
            margin-bottom: 5px;
            display: table !important;
        }
    </style>
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
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose"
                            OnClick="btnSaveClose_Click" runat="server" Text="保存后关闭">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:SimpleForm ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server"
                    BodyPadding="10px" Title="SimpleForm">
                    <Items>
                        <f:Panel ID="Panel2" Layout="Column" ShowHeader="false" ShowBorder="false" runat="server">
                            <Items>
                                <f:Label runat="server" ID="lab_WMSPosition" Label="WMS库位"
                                    ColumnWidth="50%" LabelWidth="110px">
                                </f:Label>
                                <f:Label runat="server" ID="lab_WMSLie" Label="WMS列名" LabelWidth="120px"
                                    ColumnWidth="50%">
                                </f:Label>
                            </Items>
                        </f:Panel>

                        <f:Panel ID="Panel3" Layout="Column" ShowHeader="false" ShowBorder="false" runat="server">
                            <Items>
                                <f:Label runat="server" ID="lab_AGVPosition" Label="AGV库位"
                                    ColumnWidth="50%" LabelWidth="110px">
                                </f:Label>

                                <f:Label runat="server" ID="lab_WMSLieBatchNo" Label="WMS列名-批号" LabelWidth="120px"
                                    ColumnWidth="50%">
                                </f:Label>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel4" Layout="Column" ShowHeader="false" ShowBorder="false" runat="server">
                            <Items>
                                <%--<f:TextBox ID="tbxTrayNo" runat="server" Label="托盘标签" ColumnWidth="60%"
                                    LabelWidth="110px">
                                </f:TextBox>--%>

                                <f:DropDownBox runat="server" ID="ddlTrayNo" Label="托盘标签"  ColumnWidth="85%"
                                    EmptyText="请从下拉列表选择" OnTextChanged="ddlTrayNo_TextChanged"  LabelWidth="110px">
                                    <PopPanel>
                                        <f:Grid runat="server" ID="Grid1" ShowBorder="true" ShowHeader="false" Hidden="true"
                                            DataIDField="ID"  DataTextField="TrayNO"
                                            AllowSorting="true" SortField="TrayNO" SortDirection="ASC"
                                             PageSize="10" IsFluid="true"
                                            Width="1000px" Height="500px" 
                                            OnRowSelect="Grid1_RowSelect" EnableRowClickEvent="true">
                                             <Toolbars>
                                                <f:Toolbar runat="server" Position="Top">
                                                    <Items>
                                                        <f:TwinTriggerBox Width="300px" runat="server" EmptyText="标签查找" 
                                                            ShowLabel="false" ID="ttbSearch" Label="标签查找"
                                                            ShowTrigger1="false"   
                                                            Trigger1Icon="Clear" OnTrigger1Click="ttbSearch_Trigger1Click"
                                                            Trigger2Icon="Search" OnTrigger2Click="ttbSearch_Trigger2Click">
                                                        </f:TwinTriggerBox>
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>

                                            <Columns>
                                                <f:RowNumberField />
                                                <f:BoundField Width="180px" DataField="TrayNO" SortField="TrayNO"
                                                     HeaderText="标签">
                                                </f:BoundField>
                                                 <f:BoundField Width="100px" DataField="batchNo" SortField="batchNo"
                                                     HeaderText="批号">
                                                </f:BoundField>
                                                 <f:BoundField Width="200px" DataField="proname" SortField="proname"
                                                     HeaderText="产品名称">
                                                </f:BoundField>
                                            </Columns>


                                        </f:Grid>


                                    </PopPanel>

                                </f:DropDownBox>

                                <f:Label runat="server" ColumnWidth="2%"></f:Label>
                                <f:Button ID="btnClearTray" CssClass="btn-in-form" runat="server"
                                    Text="清空仓位" ColumnWidth="10%" OnClick="btnClearTray_Click"
                                    ConfirmText="确定清空仓位？" ConfirmTarget="Top">
                                </f:Button>
                            </Items>
                        </f:Panel>

                        <f:Label runat="server" ID="labProName" Label="产品名称" LabelWidth="110px"></f:Label>
                        <f:Label runat="server" ID="labBatchNo" Label="产品批号" LabelWidth="110px"></f:Label>
                        <f:DropDownList ID="ddlState" Label="库位占用状态" Required="true" ShowRedStar="true"
                            runat="server" LabelWidth="110px">
                            <f:ListItem Text="空" Value="空" />
                            <f:ListItem Text="占用" Value="占用" />
                            <f:ListItem Text="预进" Value="预进" />
                            <f:ListItem Text="预出" Value="预出" />
                        </f:DropDownList>

                    </Items>
                </f:SimpleForm>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
