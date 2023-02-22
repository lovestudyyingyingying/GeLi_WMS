<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WareAreaIndex.aspx.cs" Inherits="NanXingGuoRen_WMS.Stock.WareAreaIndex" %>

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
            ShowBorder="false" ShowHeader="false" Layout="Fit">
            <Items>
                <f:Grid ID="Grid1" runat="server" ShowBorder="true" ShowHeader="false" EnableCheckBoxSelect="false"
                    DataKeyNames="ID" OnPreDataBound="Grid1_PreDataBound"
                    OnRowCommand="Grid1_RowCommand">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" Position="Top" runat="server">
                            <Items>
                                <f:Button ID="btnNew" runat="server" Icon="Add" EnablePostBack="false" Text="新增库区信息">
                                </f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField></f:RowNumberField>
                        <%--<f:BoundField DataField="SortIndex" HeaderText="编号" Width="80px" TextAlign="Right" />--%>
                        <f:BoundField DataField="WareNo" HeaderText="库区编号"  Width="150px" />

                       <%-- <f:BoundField DataField="BigClass" HeaderText="分类"
                            Width="150px"  />--%>

                        <f:RenderField DataField="AreaClass" HeaderText="库区类型"  Width="80px"   />
                        <f:RenderField DataField="WHName" HeaderText="所属仓库"  Width="80px"  />
                        <f:RenderField DataField="WareAreaState" HeaderText="库区状态" Width="80px"  RendererFunction="RenderHeji"  />
                        <f:BoundField DataField="Remark" HeaderText="备注" ExpandUnusedSpace="true" />
                       
                        <f:WindowField ColumnID="editField" TextAlign="Center" Icon="Pencil" ToolTip="编辑"
                            WindowID="Window1" Title="编辑" DataIFrameUrlFields="ID" Width="50px" 
                            DataIFrameUrlFormatString="~/stock/WareAreaEdit.aspx?id={0}" />

                        <f:LinkButtonField ColumnID="deleteField" TextAlign="Center" Icon="Delete" ToolTip="删除"
                            ConfirmText="确定删除此记录？" ConfirmTarget="Top" CommandName="Delete" Width="50px" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

        <f:Window ID="Window1" CloseAction="Hide" runat="server" IsModal="true" Hidden="true"
            Target="Top" EnableResize="true" EnableMaximize="true" EnableIFrame="true" IFrameUrl="about:blank"
            Width="900px" Height="500px" OnClose="Window1_Close">
        </f:Window>
    </form>
      <script>
          function RenderAreaClass(value) {
           
             if (value=="True")
                return "启用";
             else {
                return "停用";
             }
          }

          function RenderHeji(value) {
              if (value == "True")
                  return "启用";
              else {
                  return "停用";
              }
          }
      </script>
</body>
</html>
