<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="GeLiPage_WMS.admin._default" %>

<%@ Register Src="~/GeLiPage/ImageLabelMain_UC .ascx"  TagName="ImageLabelMain_UC" TagPrefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>

    <style type="text/css">
        .center{
          
            background:#deedf7 !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" Layout="Region" ShowBorder="false" ShowHeader="false" AutoScroll="true"
            runat="server">
            <Items>
               <f:Panel runat="server" RegionPosition="Top" RegionSplit="true" Layout="HBox"  BodyPadding="10px"
            BoxConfigChildMargin="0 5 0 0"  RegionPercent="15%" ShowHeader="false" ShowBorder="true" >
                   <Items>
                        <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UCTodayMission"
                                                ImagePath="~/res/images/WareInfo/任务.png"
                                                Title="今日任务量" TitleColor="black" TitleSize="15px"
                                                Value="11" ValueColor="black" ValueSize="20px" />
                                        </f:UserControlConnector>
                       <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UCTodayComplte"
                                                ImagePath="~/res/images/WareInfo/已完成.png"
                                                Title="已完成" TitleColor="green" TitleSize="15px"
                                                Value="3333" ValueColor="green" ValueSize="20px" />
                                        </f:UserControlConnector>
                       <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UCNoComplte"
                                                ImagePath="~/res/images/WareInfo/未完成.png"
                                                Title="未完成" TitleColor="#C00000" TitleSize="15px"
                                                Value="3333" ValueColor="#C00000" ValueSize="20px" />
                                        </f:UserControlConnector>
                       <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UCCancel"
                                                ImagePath="~/res/images/WareInfo/取消任务.png"
                                                Title="取消" TitleColor="gray" TitleSize="15px"
                                                Value="3333" ValueColor="gray" ValueSize="20px" />
                                        </f:UserControlConnector>
                       <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UC5"
                                                ImagePath="~/res/images/WareInfo/进行中的任务.png"
                                                Title="进行中" TitleColor="blue" TitleSize="15px"
                                                Value="3333" ValueColor="blue" ValueSize="20px" />
                                        </f:UserControlConnector>
                          <f:UserControlConnector runat="server">
                                            <uc1:ImageLabelMain_UC runat="server" ID="ImageLabelMain_UCWarn"
                                                ImagePath="~/res/images/WareInfo/报警.png"
                                                Title="报警" TitleColor="red" TitleSize="15px"
                                                Value="3333" ValueColor="red" ValueSize="20px" />
                                        </f:UserControlConnector>
                   </Items>
               </f:Panel>
                  <f:Panel runat="server"  IsFluid="true" ID="mainPanel"   RegionPosition="Center"  RegionPercent="75%" ShowHeader="false" AutoScroll="true"  Layout="Block" CssClass="center" >
                      <Items>

                      </Items>
                  </f:Panel>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
