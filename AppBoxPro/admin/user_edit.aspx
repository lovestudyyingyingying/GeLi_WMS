<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user_edit.aspx.cs" Inherits="GeLiPage_WMS.admin.user_edit" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
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
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnSaveClose_Click"
                            runat="server" Text="保存后关闭">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"
                    Title="SimpleForm">
                    <Rows>
                        <f:FormRow runat="server">
                            <Items>
                                <f:Label ID="labName" runat="server" Label="用户名">
                                </f:Label>
                                <f:TextBox ID="tbxRealName" runat="server" Label="中文名" Required="true" ShowRedStar="true">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:RadioButtonList ID="ddlGender" Label="性别" Required="true" ShowRedStar="true" runat="server">
                                    <f:RadioItem Text="男" Value="男" />
                                    <f:RadioItem Text="女" Value="女" />
                                </f:RadioButtonList>
                                <f:CheckBox ID="cbxEnabled" runat="server" Label="是否启用">
                                </f:CheckBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow1" runat="server">
                            <Items>
                                <f:TextBox ID="tbxEmail" runat="server" Label="邮箱" Required="true" ShowRedStar="true"
                                    RegexPattern="EMAIL">
                                </f:TextBox>
                                <f:TextBox ID="tbxCompanyEmail" runat="server" Label="公司邮箱" RegexPattern="EMAIL">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow2" runat="server">
                            <Items>
                                <f:TextBox ID="tbxOfficePhone" runat="server" Label="工作电话">
                                </f:TextBox>
                                <f:TextBox ID="tbxOfficePhoneExt" runat="server" Label="分机号">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ID="FormRow4" runat="server">
                            <Items>
                                <f:TextBox ID="tbxHomePhone" runat="server" Label="家庭电话">
                                </f:TextBox>
                                <f:TextBox ID="tbxCellPhone" runat="server" Label="手机号">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TwinTriggerBox ID="tbSelectedRole" EnableEdit="false" EnableTrigger1PostBack="false" EnableTrigger2PostBack="false"
                                    Trigger1Icon="Clear" Trigger2Icon="Search" ShowTrigger1="false" ShowTrigger2="true"
                                    OnClientTrigger1Click="onSelectedRoleTrigger1Click();" OnClientTrigger2Click="onSelectedRoleTrigger2Click();"
                                    Label="所属角色" runat="server">
                                </f:TwinTriggerBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TwinTriggerBox ID="tbSelectedDept" EnableEdit="false" EnableTrigger1PostBack="false" EnableTrigger2PostBack="false"
                                    Trigger1Icon="Clear" Trigger2Icon="Search" ShowTrigger1="false" ShowTrigger2="true"
                                    OnClientTrigger1Click="onSelectedDeptTrigger1Click();" OnClientTrigger2Click="onSelectedDeptTrigger2Click();"
                                    Label="所属部门" runat="server">
                                </f:TwinTriggerBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TwinTriggerBox ID="tbSelectedTitle" EnableEdit="false" EnableTrigger1PostBack="false" EnableTrigger2PostBack="false"
                                    Trigger1Icon="Clear" Trigger2Icon="Search" ShowTrigger1="false" ShowTrigger2="true"
                                    OnClientTrigger1Click="onSelectedTitleTrigger1Click();" OnClientTrigger2Click="onSelectedTitleTrigger2Click();"
                                    Label="拥有职称" runat="server">
                                </f:TwinTriggerBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextArea ID="tbxRemark" runat="server" Label="备注">
                                </f:TextArea>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfSelectedRole" runat="server">
        </f:HiddenField>
        <f:HiddenField ID="hfSelectedDept" runat="server">
        </f:HiddenField>
        <f:HiddenField ID="hfSelectedTitle" runat="server">
        </f:HiddenField>
        <f:Window ID="Window1" Title="编辑" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Top" IsModal="True" Width="550px"
            Height="350px">
        </f:Window>
    </form>
    <script>

        /////////////////////////////////////////////////////
        var tbSelectedRoleClientID = '<%= tbSelectedRole.ClientID %>';
        var hfSelectedRoleClientID = '<%= hfSelectedRole.ClientID %>';


        function checkSelectedRoleTriggerStatus() {
            if (F(tbSelectedRoleClientID).getValue()) {
                F(tbSelectedRoleClientID).showTrigger1();
            } else {
                F(tbSelectedRoleClientID).hideTrigger1();
            }
        }

        function onSelectedRoleTrigger1Click() {
            F(tbSelectedRoleClientID).setValue('');
            F(hfSelectedRoleClientID).setValue('');
            checkSelectedRoleTriggerStatus();
        }

        function onSelectedRoleTrigger2Click() {
            F('Window1').show(F.baseUrl + 'admin/user_select_role.aspx?ids=' + F(hfSelectedRoleClientID).getValue() + '', '选择用户所属的角色');
        }

        function updateSelectedRole(roleNames, roleIds) {
            F(tbSelectedRoleClientID).setValue(roleNames);
            F(hfSelectedRoleClientID).setValue(roleIds);
            checkSelectedRoleTriggerStatus();
        }
        /////////////////////////////////////////////////////


        /////////////////////////////////////////////////////
        var tbSelectedDeptClientID = '<%= tbSelectedDept.ClientID %>';
        var hfSelectedDeptClientID = '<%= hfSelectedDept.ClientID %>';

        function checkSelectedDeptTriggerStatus() {
            if (F(tbSelectedDeptClientID).getValue()) {
                F(tbSelectedDeptClientID).showTrigger1();
            } else {
                F(tbSelectedDeptClientID).hideTrigger1();
            }
        }

        function onSelectedDeptTrigger1Click() {
            F(tbSelectedDeptClientID).setValue('');
            F(hfSelectedDeptClientID).setValue('');
            checkSelectedDeptTriggerStatus();
        }

        function onSelectedDeptTrigger2Click() {
            F('Window1').show(F.baseUrl + 'admin/user_select_dept.aspx?ids=' + F(hfSelectedDeptClientID).getValue() + '', '选择用户所属的部门');
        }

        function updateSelectedDept(deptName, deptId) {
            F(tbSelectedDeptClientID).setValue(deptName);
            F(hfSelectedDeptClientID).setValue(deptId);
            checkSelectedDeptTriggerStatus();
        }
        /////////////////////////////////////////////////////

        /////////////////////////////////////////////////////
        var tbSelectedTitleClientID = '<%= tbSelectedTitle.ClientID %>';
        var hfSelectedTitleClientID = '<%= hfSelectedTitle.ClientID %>';

        function checkSelectedTitleTriggerStatus() {
            if (F(tbSelectedTitleClientID).getValue()) {
                F(tbSelectedTitleClientID).showTrigger1();
            } else {
                F(tbSelectedTitleClientID).hideTrigger1();
            }
        }

        function onSelectedTitleTrigger1Click() {
            F(tbSelectedTitleClientID).setValue('');
            F(hfSelectedTitleClientID).setValue('');
            checkSelectedTitleTriggerStatus();
        }

        function onSelectedTitleTrigger2Click() {
            F('Window1').show(F.baseUrl + 'admin/user_select_title.aspx?ids=' + F(hfSelectedTitleClientID).getValue() + '', '选择用户拥有的职称');
        }

        function updateSelectedTitle(titleNames, titleIds) {
            F(tbSelectedTitleClientID).setValue(titleNames);
            F(hfSelectedTitleClientID).setValue(titleIds);
            checkSelectedTitleTriggerStatus();
        }
        /////////////////////////////////////////////////////

        F.ready(function () {
            checkSelectedRoleTriggerStatus();
            checkSelectedDeptTriggerStatus();
            checkSelectedTitleTriggerStatus();
        });

    </script>
</body>
</html>
