<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageLabel_UC.ascx.cs"
    Inherits="GeLiPage_WMS.GeLiPage.ImageLabel_UC" %>

<f:Panel runat="server" BoxFlex="1" ShowHeader="false" ShowBorder="false"
    Layout="HBox">
    <Items>
        <f:Image runat="server" ImageHeight="40px" MarginTop="10px"
            ImageUrl="" BoxFlex="1" ID="Image1" MarginLeft="5px">
        </f:Image>
        <f:Panel runat="server" Layout="VBox" ShowHeader="false" BoxFlex="2"
            ShowBorder="false">
            <Items>
                <f:Label runat="server" Text="空闲仓位" ID="labTitle"
                    CssStyle="font-size:12px;color:green"
                    MarginLeft="7px" MarginTop="10px" Height="10px">
                </f:Label>
                <f:Label runat="server" MarginTop="-15px" MarginLeft="7px" Text="222" ID="labValue">
                </f:Label>
            </Items>
        </f:Panel>

    </Items>
</f:Panel>
