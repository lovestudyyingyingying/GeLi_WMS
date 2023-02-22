<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WarehouseManage.aspx.cs" Inherits="GeLiPage_WMS.GeLiPage.WarehouseManage" %>
<%@ Register Src="~/GeLiPage/ImageLabel_UC.ascx"
    TagName="ImageLabel_UC" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="RegionPanel1"></f:PageManager>

        <f:RegionPanel ID="RegionPanel1" ShowBorder="false" Margin="10px" runat="server">
            <Regions>
                <f:Region ID="Region1" ShowBorder="false" ShowHeader="false" RegionPosition="Left" BodyPadding="0 5 0 0"
                    Width="550px" Layout="VBox" runat="server">
                    <Items>
                        <%--CssStyle="border-bottom:none;"--%>
                        <f:Panel ShowHeader="false" BodyPadding="5px" ShowBorder="true"
                            runat="server"
                            Height="120px" Layout="HBox" Hidden="false">
                            <Items>
                                <f:Panel runat="server" BoxFlex="1" Title="格力一楼"
                                    CssStyle="border-bottom:none;border-top:none;border-left:none;"
                                    Layout="HBox" ShowHeader="true">
                                    <Items>
                                        <f:UserControlConnector runat="server">
                                            <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC1"
                                                ImagePath="~/res/images/WareInfo/仓库库存.png"
                                                Title="已用库位" TitleColor="#C00000" TitleSize="15px"
                                                Value="3333" ValueColor="#C00000" ValueSize="18px" />
                                        </f:UserControlConnector>

                                       <%-- <f:Panel runat="server" BoxFlex="1" ShowHeader="false" ShowBorder="false"
                                            Layout="HBox">--%>
                                        <f:UserControlConnector runat="server">
                                            <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC2"
                                                ImagePath="~/res/images/WareInfo/仓库库存 (1).png"
                                                Title="空闲库位" TitleColor="#2b9464" TitleSize="15px"
                                                Value="3333" ValueColor="#2b9464" ValueSize="18px" />
                                        </f:UserControlConnector>
                                    </Items>
                                </f:Panel>
                                <f:Panel runat="server" BoxFlex="1" Title="格力二楼" ShowBorder="false"
                                    Layout="HBox" ShowHeader="true">

                                    <Items>
                                        <f:UserControlConnector runat="server">
                                            <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC3"
                                                ImagePath="~/res/images/WareInfo/仓库库存.png"
                                                Title="已用库位" TitleColor="#C00000" TitleSize="15px"
                                                Value=""  ValueColor="#C00000" ValueSize="18px" />
                                        </f:UserControlConnector>

                                       <%-- <f:Panel runat="server" BoxFlex="1" ShowHeader="false" ShowBorder="false"
                                            Layout="HBox">--%>
                                        <f:UserControlConnector runat="server">
                                            <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC4"
                                                ImagePath="~/res/images/WareInfo/仓库库存 (1).png" 
                                                Title="空闲库位" TitleColor="#2b9464" TitleSize="15px"
                                                Value=""  ValueColor="#2b9464" ValueSize="18px" />
                                        </f:UserControlConnector>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>

                        <f:Panel ShowHeader="false" BodyPadding="0px" ShowBorder="false"
                            runat="server" CssStyle="border-bottom:none;" Layout="Fit" BoxFlex="1">
                            <Items>
                                <f:RegionPanel ID="RegionPanel2" ShowBorder="false" Margin="0px" runat="server" BoxFlex="1">
                                    <Regions>
                                        <f:Region ID="Region3" ShowBorder="false" ShowHeader="false" RegionPosition="Left" BodyPadding="0 5 0 0"
                                            Width="170px" Layout="Fit" runat="server" BoxFlex="1">
                                            <Items>
                                                <f:Grid ID="Grid2" ShowBorder="true" ShowHeader="true" Title="楼层" runat="server"
                                                    DataKeyNames="ID,WHName" EnableMultiSelect="false" EnableRowSelectEvent="true"
                                                    ShowGridHeader="false" OnRowSelect="Grid2_RowSelect">
                                                    <Columns>

                                                        <f:TemplateField Width="60px">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label2" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </f:TemplateField>
                                                        <f:BoundField ExpandUnusedSpace="true" MinWidth="100px" ColumnID="WHName" DataField="WHName" DataFormatString="{0}"
                                                            HeaderText="仓库" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Region>
                                        <f:Region ID="Region4" ShowBorder="false" ShowHeader="false" RegionPosition="Left" BodyPadding="0 0 0 0"
                                            Width="375px" Layout="Fit" runat="server" MarginLeft="10px">
                                            <Items>

                                                <f:Grid ID="Grid3" ShowBorder="true" ShowHeader="true" Title="缓存区" runat="server"
                                                    DataKeyNames="ID,WareAreaState" EnableMultiSelect="false" EnableRowSelectEvent="true"
                                                    ShowGridHeader="false" SortField="WareNo" SortDirection="ASC" AllowSorting="true"
                                                    OnRowSelect="Grid3_RowSelect" OnPreRowDataBound="Grid3_PreRowDataBound" IsFluid="true"
                                                    OnRowCommand="Grid3_RowCommand">

                                                    <Columns>
                                                        <f:TemplateField Width="40px">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label3" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </f:TemplateField>
                                                        <f:BoundField MinWidth="80px" ColumnID="WareNo" DataField="WareNo" DataFormatString="{0}"
                                                            HeaderText="姓名" />
                                                        <f:BoundField MinWidth="80px" ColumnID="AreaClass"
                                                            DataField="AreaClassName" DataFormatString="{0}"
                                                            HeaderText="库区类型" />
                                                        <f:BoundField MinWidth="80px" ColumnID="WareAreaState"
                                                            DataField="WareAreaState" DataFormatString="{0}"
                                                            HeaderText="启用状态" Hidden="true" />
                                                        <f:LinkButtonField HeaderText="库位启用状态"
                                                            Width="80px" TextAlign="Center" ColumnID="ChangeOpenArea"
                                                            ConfirmText="确定修改整个库区的库位状态？" ConfirmTarget="Top"
                                                            CommandName="ChangeOpenArea" Icon="Accept" Text="已启用"
                                                            ExpandUnusedSpace="true" />

                                                    </Columns>
                                                </f:Grid>


                                            </Items>
                                        </f:Region>
                                    </Regions>
                                </f:RegionPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Region>

                <f:Region ID="Region2" ShowBorder="false" ShowHeader="false" Position="Center"
                    Layout="VBox" BoxConfigAlign="Stretch" runat="server">
                    <Items>

                        <f:Grid ID="Grid1" runat="server" BoxFlex="1" ShowBorder="true" ShowHeader="true"
                            Title="库位" DataKeyNames="ID,IsOpen" AllowPaging="true" IsDatabasePaging="true"
                            OnSort="Grid1_Sort" SortField="WareLocaNo" SortFieldArray="WareLocaNo,WareLocaNo" SortDirection="ASC" AllowSorting="true"
                            OnPageIndexChange="Grid1_PageIndexChange" IsFluid="true"
                            OnRowDataBound="Grid1_RowDataBound" OnPreRowDataBound="Grid1_PreRowDataBound"
                            OnRowCommand="Grid1_RowCommand">

                            <Toolbars>
                                <f:Toolbar runat="server">
                                    <Items>
                                        <f:TextBox runat="server" ID="tbx_WlNo" Label="WMS库位"
                                            LabelWidth="100px" Width="250px">
                                        </f:TextBox>
                                        <f:TextBox runat="server" ID="tbx_TrayNo" Label="托盘标签"
                                            LabelWidth="100px" Width="250px">
                                        </f:TextBox>
                                        <f:TextBox runat="server" ID="tbx_GuanJianZi" Label="关键字"
                                            LabelWidth="100px" Width="250px">
                                        </f:TextBox>

                                        <f:DropDownList runat="server" ID="ddlIsOpen" Label="启用状态" Width="160px" LabelWidth="80px">
                                            <f:ListItem Text="全部" Value="" />
                                            <f:ListItem Text="已开启" Value="1" />
                                            <f:ListItem Text="未启用" Value="0" />

                                        </f:DropDownList>

                                        <f:Button runat="server" ID="btn_Search" Text="搜索" Icon="SystemSearch"
                                            OnClick="btn_Search_Click">
                                        </f:Button>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:TemplateField Width="60px">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                    </ItemTemplate>
                                </f:TemplateField>
                                <f:BoundField Width="100px" ColumnID="AGVPosition" DataField="AGVPosition"
                                    HeaderText="AGV库位号" />

                                <f:BoundField Width="180px" ColumnID="WareLocaNo" DataField="WareLocaNo"
                                    HeaderText="WMS库位号"  />

                 
                                <f:RenderField Width="200px" DataField="TrayStateNo"
                                    HeaderText="托盘标签" />

                                <f:BoundField Width="120px" DataField="TrayStateBatchNo"
                                    HeaderText="产品批号" />
                                <f:BoundField Width="120px" ColumnID="WareLocaState" DataField="WareLocaState"
                                    HeaderText="库位占用状态" />

                                <f:BoundField Width="80px" ColumnID="IsOpen" DataField="IsOpen"
                                    HeaderText="库位状态" Hidden="true" />
                                <f:LinkButtonField HeaderText="修改" Width="60px" Icon="ApplicationEdit"
                                    CommandName="ChangeTrayNo" ColumnID="ChangeTrayNo" TextAlign="Center"
                                    Hidden="true">
                                </f:LinkButtonField>
                                <f:WindowField ColumnID="editField" TextAlign="Center" Icon="ApplicationEdit"
                                    ToolTip="修改" WindowID="Window1" Title="修改"
                                    DataIFrameUrlFields="ID"
                                    DataIFrameUrlFormatString="~/GeLiPage/LocationEdit.aspx?id={0}"
                                    Width="50px" />

                                <f:LinkButtonField HeaderText="库位启用状态"
                                    Width="120px" TextAlign="Center" ColumnID="ChangeOpen"
                                    ConfirmText="确定修改库位状态？" ConfirmTarget="Top"
                                    CommandName="ChangeOpen" Icon="Accept" Text="已启用" />

                                <%-- <f:TemplateField Width="180px" ColumnID="Actions">
                                    <ItemTemplate>
                                        <div class="action btncontainer"></div>
                                    </ItemTemplate>
                                </f:TemplateField>--%>
                            </Columns>
                            <Listeners>
                                <f:Listener Event="dataload" Handler="onGridDataLoad" />
                            </Listeners>
                        </f:Grid>
                    </Items>
                </f:Region>



            </Regions>

        </f:RegionPanel>



        <f:Window runat="server" ID="Window1" EnableIFrame="true" Height="600px" Width="800px" Title="" Hidden="true"
            OnClose="Window1_Close" EnableMaximize="false" EnableMinimize="false" EnableResize="false">
        </f:Window>

        <f:Window runat="server" ID="Window2" EnableIFrame="true" Height="900px" Width="1500px" Title="" Hidden="true"
            EnableMaximize="true" EnableMinimize="true" OnClose="Window2_Close" EnableResize="true">
        </f:Window>

        <script>
            // 表格数据加载完毕
            function onGridDataLoad() {
                var grid = this;

                grid.bodyEl.find('.f-grid-cell-Actions .btncontainer').each(function () {
                    var btncontainer = $(this);
                    F.create({
                        type: 'button',
                        renderTo: btncontainer,
                        iconFont: 'user',
                        text: '动态按钮',
                        EnablePress: 'true'
                    });
                });

                // 动态创建的按钮会导致表格内内容错位，需要重新布局
                grid.doLayout();
            }

            function RenderPrice(value) {
                return F.addCommas(value.toFixed(2));
            }

            function RenderClose(value) {
                if (value == 0)
                    return '未完结';
                else
                    return '已完结';
            }


        </script>

    </form>
</body>
</html>
