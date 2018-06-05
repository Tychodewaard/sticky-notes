<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" CodeFile="Display_View.ascx.cs" Inherits="DesktopModules.Icatt.Geeltjes.UI.Display.Display_View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude ID="StickyNotesStyles" runat="server" FilePath="~/DesktopModules/ICATT/Geeltjes/UI.Display/css/stickynotes.css" />
<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/fancybox/3.2.5/jquery.fancybox.min.css" />

<dnn:DnnJsInclude ID="customJS" runat="server" FilePath="https://cdnjs.cloudflare.com/ajax/libs/fancybox/3.2.5/jquery.fancybox.min.js" />

<div id="<%=ClientID %>" class="geeltjesOuterContainer StickyNotes StickyNotes-canvas">
	<div id="<%=ClientID %>_notesContainer" class="geeltjesNotesContainer StickyNotes" style='<%# containerStyle("") %>' >
		<a class="add-button StickyNotes-button" href="#<%=ClientID %>_noteEditor" style='<%# canAdd() ? "":"display:none;" %>'><%# get_localizedString(LocalResourceKeys.AddNoteButtonTitle) %></a>
		<asp:Repeater ID="rptGeeltjesItem" runat="server" DataSource='<%# DataSource %>'>
			<ItemTemplate>
				<div class="<%# getMyClassNames((string) DataBinder.Eval(Container.DataItem, "Item1.Color")) %>"
					style="<%# getMyStyle((string) DataBinder.Eval(Container.DataItem, "Item1.Xyz")) %>">
					<div class="body"><%# GetMyText((string)DataBinder.Eval(Container.DataItem, "Item1.Text"))%></div>
					<div class="author">
						<%# DataBinder.Eval(Container.DataItem, "Item1.Name")%></div>
					<span class="data" style="display: none;"><%# DataBinder.Eval(Container.DataItem, "Item1.Id")%></span>
					<span class="voted" style="display: none;"><%# currentUserVote(Container.DataItem) %></span>
					<div class="button-container">
						<div class="control-container" style="<%# displayAllowEditNotes((int)DataBinder.Eval(Container.DataItem, "Item1.CreatedByUserId")) %>">
							<a class="control edit ir"  href="#<%=ClientID %>_noteEditor">Deze notitie aanpassen</a>
							<a class="control delete ir" title="Deze notitie verwijderen">Deze notitie verwijderen</a>
						</div>
						<div class="thumbs-container" style="<%# displayRateThumbs()%>">
							<a class='<%# thumbClasses(Container.DataItem,"thumbs up ir",1) %>' title="Deze notitie leuk vinden"   >Deze notitie leuk vinden</a><span
								class="total-up"><%# myThumbUpCount(Container.DataItem)%></span>
							<a class='<%# thumbClasses(Container.DataItem,"thumbs down ir",-1) %>' title="Deze notitie niet leuk vinden" >Deze notitie niet leuk
								vinden</a><span class="total-down"><%# myThumbDownCount(Container.DataItem)%></span>
						</div>
					</div>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
	<div style="display: none;">
		<div id="<%= ClientID %>_noteEditor" class="geeltjesNoteEditor StickyNotes StickyNotes-editor">
			<h3 class="popupTitle">Notitie wijzigen</h3>
			<div class="note yellow" style="left: 0px; top: 37px; z-index: 1">
				<div class="body">
				</div>
				<div class="author">
				</div>
				<span class="data" style="display: none;"></span>
				<span class="voted" style="display: none;">0</span>
                <div class="button-container">
					<div class="control-container">
						<a class="control edit ir"  href="#<%=ClientID %>_noteEditor">Deze notitie aanpassen</a>
						<a class="control delete ir" title="Deze notitie verwijderen">Deze notitie verwijderen</a>
					</div>
					<div class="thumbs-container" style="<%= displayRateThumbs()%>">
						<a class="thumbs up ir" title="Deze notitie leuk vinden">&nbsp;</a><span
							class="total-up">0</span> <a class="thumbs down ir" title="Deze notitie niet leuk vinden">
								&nbsp;</a><span class="total-down">0</span>
					</div>
				</div>
			</div>
			<div class="noteData">
				<!-- Holds the form -->
				<label for="note-body">
					Tekst voor uw notitie</label>
				<textarea name="note-body" class="pr-body" cols="30" rows="6"></textarea>
				<label for="note-name">
					Uw naam</label>
				<input type="text" name="note-name" class="pr-author" value="" />
				<label>
					Kleur van de notitie</label>
				<!-- Clicking one of the divs changes the color of the preview -->
                <div>
				    <div class="color yellow"></div>
				    <div class="color blue"></div>
				    <div class="color green"></div>
                </div>
				<!-- The green submit button: -->
				<a class="save-button StickyNotes-button">Wijziging opslaan</a>
			</div>
		</div>
	</div>
	<script type="text/javascript" language="javascript">

	(function ($) {

		$(document).ready(function() {
			$.icatt.stickynotes({
				 ajaxPath: "<%= ResolveClientUrl(this.AppRelativeTemplateSourceDirectory + "AjaxHandler.ashx?portalId=" + this.PortalId ) %>"
				 ,portalId: <%= this.PortalId %>
				 ,editEnabled: <%= canAdd().ToString(CultureInfo.InvariantCulture).ToLower() %>
				,modulePath: "<%= ResolveClientUrl(this.AppRelativeTemplateSourceDirectory) %>"
				,moduleId: <%= this.ModuleId %>
				,userFullName: "<%= currentUser.DisplayName %>"
				,clientId: "<%= ClientID %>"
				,editTitle: '<%= tryGetLocalizedString(LocalResourceKeys.EditTitle) %>'
				,addTitle: '<%= tryGetLocalizedString(LocalResourceKeys.AddTitle) %>'
				,addButtonTitle: '<%= tryGetLocalizedString(LocalResourceKeys.AddButtonTitle) %>'
				,editButtonTitle: '<%= tryGetLocalizedString(LocalResourceKeys.EditButtonTitle) %>'
			    ,confirmDeleteText: '<%= tryGetLocalizedString(LocalResourceKeys.ConfirmDeleteText) %>'
			    ,tooShortText:  '<%= tryGetLocalizedString(LocalResourceKeys.NoteTooShortText) %>'
			    ,noAuthorText:  '<%= tryGetLocalizedString(LocalResourceKeys.NoAuthorText) %>'
				,classNames :{ addButton: 'add-button', saveButton: 'save-button' }
			});
		});

	})(jQuery)
	</script>
</div>
