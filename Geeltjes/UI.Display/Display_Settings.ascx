<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Display_Settings.ascx.cs" Inherits="DesktopModules.Icatt.Geeltjes.UI.Display.Display_Settings" %>
<%@ Register tagPrefix="dnn" tagName="URL" src="~/controls/urlcontrol.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>


<table cellspacing="0" cellpadding="2" border="0">
	<tr id="rowPolls" runat="server">
		<td>
			<span>
				<dnn:label id="lblEditMode" runat="server" Suffix=":" controlname="chkEditMode" MaxLength="245"></dnn:label>
			</span>
		</td>
		<td>
			<asp:checkbox id="chkEditMode" runat="server" />
		</td>
	</tr>
	<tr id="Tr1" runat="server">
		<td>
			<span>
				<dnn:label id="lblRateThumb" runat="server" Suffix=":" controlname="chkRateThumb" MaxLength="245"></dnn:label>
			</span>
		</td>
		<td>
			<asp:checkbox id="chkRateThumb" runat="server" />
           </td>
	</tr>
    <tr>
        <td class="SubHead" valign="top" width="200">
            <dnn:Label ID="lblURL" runat="server" Suffix=":" ControlName="ctlURL"></dnn:Label>
        </td>
        <td class="NormalTextBox" width="325">
            <dnn:URL ID="ctlURL" ShowImages="False" ShowUpLoad="True" ShowFiles="True" ShowUrls="False" ShowTabs="false"  runat="server" Width="300" ShowLog="False" ShowNone="True" ShowTrack="False"/>
        </td>
    </tr>
</table>
