<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientManager.aspx.cs" Inherits="NanXingGuoRen_WMS.ClientManager.ClientManager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="5px"
            ShowBorder="false" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            ShowHeader="false" Title="客户管理">
            <Items>
                <f:Grid ID="Grid1" runat="server" BoxFlex="1" ShowBorder="true" ShowHeader="false"
                    EnableCheckBoxSelect="true"
                    DataKeyNames="ID" AllowSorting="true" OnSort="Grid1_Sort" SortField="ID"
                    SortDirection="ASC" AllowPaging="true" IsDatabasePaging="true"
                    OnPageIndexChange="Grid1_PageIndexChange" EnableTextSelection="true" AllowCellEditing="true" ClicksToEdit="1">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:Button ID="btnNew" runat="server" Icon="Add" EnablePostBack="false" Text="新增客户">
                                </f:Button>

                                <f:Button ID="btnSave" runat="server" Icon="PageSave" Text="保存修改" OnClick="btnSave_Click"></f:Button>
                                <f:TextBox runat="server" EmptyText="客户编码" ID="tbxnumber"></f:TextBox>
                                <f:TextBox runat="server" EmptyText="名称" ID="tbxname"></f:TextBox>
                                <f:Button runat="server" ID="btnsearch" OnClick="btnsearch_Click" Text="搜索"></f:Button>
                                <f:ToolbarSeparator runat="server">
                                </f:ToolbarSeparator>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server">
                                </f:ToolbarFill>
                                <f:Button ID="btnDeleteSelected" Icon="Delete" runat="server" Text="删除选中记录" OnClick="btnDeleteSelected_Click">
                                </f:Button>

                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <PageItems>
                        <f:ToolbarSeparator ID="ToolbarSeparator1" runat="server">
                        </f:ToolbarSeparator>
                        <f:ToolbarText ID="ToolbarText1" runat="server" Text="每页记录数：">
                        </f:ToolbarText>
                        <f:DropDownList ID="ddlGridPageSize" Width="80px" AutoPostBack="true" OnSelectedIndexChanged="ddlGridPageSize_SelectedIndexChanged"
                            runat="server">
                            <f:ListItem Text="10" Value="10" />
                            <f:ListItem Text="20" Value="20" />
                            <f:ListItem Text="50" Value="50" />
                            <f:ListItem Text="100" Value="100" />
                        </f:DropDownList>
                    </PageItems>
                    <Columns>
                        <f:RowNumberField EnablePagingNumber="true" Hidden="true"></f:RowNumberField>
                        <f:RenderField DataField="Number" SortField="Number" Width="100px" HeaderText="客户编码" ColumnID="Number" >
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="Name" SortField="Name" Width="250px" HeaderText="名称" ColumnID="Name" >
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:GroupField HeaderText="微信信息" HeaderTextAlign="Center" Width="400px" Hidden="true">
                            <Columns>
                                <f:RenderField ColumnID="nickname" DataField="nickname"
                                    HeaderText="昵称" Width="100px">
                                    <Editor>
                                        <f:TriggerBox ID="tbxEditorName" TriggerIcon="Search" OnTriggerClick="tbxEditorName_TriggerClick" runat="server">
                                        </f:TriggerBox>
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField HeaderText="openid" ColumnID="openid" DataField="openid" Width="300px">
                                    <Editor>
                                        <f:TextBox runat="server"></f:TextBox>
                                    </Editor>
                                </f:RenderField>
                                <f:WindowField DataIFrameUrlFields="openid" DataIFrameUrlFormatString="~/clientmanager/Weixinsendmessage.aspx?openid={0}" IconFont="Send" HeaderText="发送" WindowID="Window1"></f:WindowField>
                            </Columns>
                        </f:GroupField>
                        <f:RenderField DataField="levelCode" SortField="levelCode" Width="100px" HeaderText="分级代码" ColumnID="levelCode">
                            <Editor>
                                  <f:DropDownList runat="server" ID="ddlCodeLevel"  EnableEdit="true" ForceSelection="true"></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="clientManager" SortField="clientManager" Width="100px" HeaderText="客户经理"  ColumnID="clientManager">
                            <Editor>
                                <f:DropDownList runat="server" ID="ddlkehujingli" ></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="implementer" SortField="implementer" HeaderText="实施人员" Width="100px" ColumnID="implementer">
                            <Editor>
                               <f:DropDownList runat="server" ID="ddlshishirenyuan" ></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="gendan" SortField="gendan" Width="100px" HeaderText="跟单员" ColumnID="gendan">
                            <Editor>
                                <f:DropDownList runat="server" ID="ddlgendan" ></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="serverTime" SortField="serverTime" Width="120px" HeaderText="服务有效期" ColumnID="serverTime" Renderer="Date" FieldType="Date" RendererArgument="yyyy-MM-dd">
                            <Editor>
                                <f:DatePicker runat="server"></f:DatePicker>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="condiction" HeaderText="服务条件" Width="120px" ColumnID="condiction">
                            <Editor>
                                <f:DropDownList runat="server" ID="ddlcondiction"  ForceSelection="true" EnableEdit="true">
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="remark" HeaderText="备注" ColumnID="remark">
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="fax" HeaderText="传真" ColumnID="fax">
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="address" HeaderText="地址" Width="300px" ColumnID="address">
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="bank" HeaderText="开户银行" ColumnID="bank">
                            <Editor>
                                <f:TextBox runat="server"></f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField DataField="Eamil" HeaderText="邮箱" ColumnID="Eamil">
                            <Editor>
                                <f:TextBox runat="server" ></f:TextBox>
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:Window ID="Window1" runat="server" IsModal="true" Hidden="true" Target="Top" EnableResize="true"
            EnableMaximize="true" EnableIFrame="true" IFrameUrl="about:blank" Width="800px"
            Height="500px" OnClose="Window1_Close">
        </f:Window>
    </form>
</body>
</html>
