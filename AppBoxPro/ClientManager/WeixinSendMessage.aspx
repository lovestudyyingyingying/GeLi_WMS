<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeixinSendMessage.aspx.cs" Inherits="NanXingGuoRen_WMS.ClientManager.WeixinSendMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server">
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button runat="server" Text="发送" ID="btnsend" OnClick="btnsend_Click"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"
                    Title="SimpleForm">
                    <Rows>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox ID="tbxfirst" runat="server" Label="标题" Required="true" ShowRedStar="true">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox ID="keyword1" runat="server" Label="订单编号" Required="true" ShowRedStar="true">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow1" runat="server">
                            <Items>
                                <f:RadioButtonList runat="server" Label="出入方向" ID="keyword2">
                                    <f:RadioItem  Selected="true" Text="入库" Value="入库" />
                                    <f:RadioItem   Text="出库" Value="出库" />
                                </f:RadioButtonList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow2" runat="server">
                            <Items>
                                <f:NumberBox runat="server" NoDecimal="true" NoNegative="true" ID="keyword3" Label="件数" Required="true"></f:NumberBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow4" runat="server">
                            <Items>
                                <f:NumberBox runat="server" NoDecimal="true" NoNegative="true" ID="keyword4" Label="重量" Required="true"></f:NumberBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow5" runat="server">
                            <Items>
                                <f:TextBox runat="server" Label="备注" ID="tbxremark"></f:TextBox>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
