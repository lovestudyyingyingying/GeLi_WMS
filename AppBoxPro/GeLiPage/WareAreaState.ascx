<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WareAreaState.ascx.cs" Inherits="GeLiPage_WMS.GeLiPage.WareAreaState" %>
<%@ Register Src="~/GeLiPage/ImageLabel_UC.ascx"
    TagName="ImageLabel_UC" TagPrefix="uc1" %>
<f:Panel runat="server" BoxFlex="1" ShowHeader="false" ShowBorder="true"
    Layout="VBox" Block="3" BoxConfigAlign="Center" BoxConfigPosition="End" Margin="30 10 10 10" BoxConfigChildMargin="0 0 5 0">
    <Items>
        <f:Label runat="server" Text="空托区" ID="labWareArea"
            CssStyle="font-size:15px;color:black"
            MarginLeft="7px" MarginTop="10px" Height="10px">
        </f:Label>
        <f:Panel runat="server" BoxFlex="1" 
            CssStyle="border-bottom:none;border-top:none;border-left:none;"
            Layout="HBox" ShowHeader="false"  Width="250px" ShowBorder="false">
            <Items>
                <f:UserControlConnector runat="server">
                    <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC1Used"
                        ImagePath="~/res/images/WareInfo/仓库库存.png"
                        Title="已用库位" TitleColor="#C00000" TitleSize="15px"
                        Value="3333" ValueColor="#C00000" ValueSize="18px" />
                </f:UserControlConnector>

              
                <f:UserControlConnector runat="server">
                    <uc1:ImageLabel_UC runat="server" ID="ImageLabel_UC2NoUsed"
                        ImagePath="~/res/images/WareInfo/仓库库存 (1).png"
                        Title="空闲库位" TitleColor="#2b9464" TitleSize="15px"
                        Value="3333" ValueColor="#2b9464" ValueSize="18px" />
                </f:UserControlConnector>
            </Items>
        </f:Panel>
    </Items>
</f:Panel>


