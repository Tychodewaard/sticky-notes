using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using global::Icatt.DotNetNuke.Modules.Geeltjes.DataAccess;

namespace DesktopModules.Icatt.Geeltjes.UI.Display
{
    
    public partial class Display_View : global::Icatt.DotNetNuke.Modules.Geeltjes.Business.Views.Display_View

    {
		private static readonly Regex htmlTag =  new System.Text.RegularExpressions.Regex("<(?!br)[^>]+>",RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled) ;
		private bool? _canEditModule;


		protected override void OnLoad(EventArgs e) {
			
			base.OnLoad(e);

            //Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jquery.fancybox", ResolveClientUrl("~/js/fancybox/jquery.fancybox-1.3.4.js?p2"));
            //Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jquery.easing", ResolveClientUrl("~/js/fancybox/jquery.easing-1.3.pack.js"));

            var url = ResolveClientUrl("js/scripts.js");
			Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "jquery.stickynotes", url);


		}



        #region UIhelper

        protected string getMyClassNames(string myColor) 
        {
            if (! String.IsNullOrEmpty(myColor))
            {
                var strClr = "note " + myColor;
                return strClr;
            }

            return string.Empty;
        }

        protected string getMyStyle(string myXyZ)
        {
            if (!String.IsNullOrEmpty(myXyZ))
            {
                string[] strXyz = myXyZ.Split(new string[] { "x" }, StringSplitOptions.None);

                if (strXyz.Length != 3) return string.Empty;

                string strStyle = String.Format("left: {0}px; top: {1}px; z-index: {2};border:1px solid black", strXyz[0], strXyz[1], strXyz[2]);

                return strStyle;
            }

            return string.Empty;
        }

        protected string GetMyText(string myText)
        {
			//remove all non <br /> tags
        	htmlTag.Replace(myText, "");
            var strText = myText;
            return strText;
        }

        protected string backgroundImgNotes
        {
            get
            {
                var myUrl = backgroundNotesContainer;
                //var myUrl = "";
                if (String.IsNullOrEmpty(myUrl))
                {
                    return String.Empty;
                }
                string style = "background: url(" + myUrl + ") no-repeat left top;";
                return myUrl;
            }
        }


    	protected bool canEditModule{
    		get{
    			if (!_canEditModule.HasValue){
    				_canEditModule = ModulePermissionController.CanEditModuleContent(this.ModuleConfiguration);
    			}
    			return _canEditModule.Value;
    		}
    	}

		/// <summary>
		/// Returns &lt; 0 if user voted down, &gt; 0 if user voted up and 0 if current user did not vote
		/// </summary>
		/// <param name="dataitem"></param>
		/// <returns></returns>
		protected int currentUserVote(object dataitem){

			var userVote = DataBinder.Eval(dataitem,"Item2") as Icatt_Geeltjes_UserGeeltje;

			if (userVote == null) return 0;

			if (userVote.ThumbUp.HasValue && userVote.ThumbUp.Value != 0) return 1;

			if (userVote.ThumbDown.HasValue && userVote.ThumbDown.Value != 0) return -1;

			return 0;

		}




		protected  string containerStyle(string fixedStyle){

			var backgroundStyle = "";
			if (backgroundImageUrl != null){
				var y = SettingAdapter.ImageUrl;
				backgroundStyle = "background: url("+backgroundImageUrl+");";
				

			}
			return fixedStyle + backgroundStyle;
		}


    	protected string backgroundImageUrl{
			get{
				if (string.IsNullOrEmpty(SettingAdapter.ImageUrlType) || SettingAdapter.ImageUrlType != "F"){
					return null;
				}

				var imgUrl = SettingAdapter.ImageUrl;

			    string appRelative = null;
			    if (imgUrl.StartsWith("fileid=", StringComparison.OrdinalIgnoreCase))
			    {
                    //URL stored as 'fileid=xxxx' string
			        var imgfileId = imgUrl.Substring(7); //remove "fileid=" from string

			        Int32 fileId;
			        if (!Int32.TryParse(imgfileId, out fileId)) return null;

			        var objFileInfo = FileManager.Instance.GetFile(fileId);

			        if (objFileInfo == null) return null;

			        appRelative = VirtualPathUtility.Combine(portalSettings.HomeDirectory, objFileInfo.RelativePath);
			    }
			    else
			    {
                    //URL stored as path relative to HomeDirectory 
			        //get portal folder root url
			        appRelative= VirtualPathUtility.Combine(portalSettings.HomeDirectory, imgUrl);
			    }

			    return appRelative == null ? null : ResolveClientUrl(appRelative);

			}

		}

		protected Boolean canAdd(){
			//user must be logged on
			if (currentUser == null || currentUser.UserID < 0) return false;

			if (canEditModule) return true;

			return SettingAdapter.AllowEdit;
		}

    	protected string displayAllowEditNotes(Int32 notesUserId){

    		bool canEditNote = canEditModule;

    		canEditNote = canEditNote || (SettingAdapter.AllowEdit && notesUserId == currentUser.UserID);

			if (canEditNote)
            {
                return "";
            }
            return "display:none;";
        }

        protected string displayRateThumbs(){
        	bool canRateThumbs = currentUser != null && currentUser.UserID >= 0;

        	canRateThumbs = canRateThumbs && SettingAdapter.RateThumb;

			return canRateThumbs ?  "display:block;" :"display:none;" ;
        }


		protected string thumbClasses(object dataitem,string fixedClasses, int upordown){

			var usergeeltje = DataBinder.Eval(dataitem, "Item2") as Icatt_Geeltjes_UserGeeltje;
			bool enabled = false;
			if (usergeeltje == null)
				enabled = true;
			else
				enabled = (upordown > 0 && usergeeltje.ThumbUp == 0) || (upordown < 0 && usergeeltje.ThumbDown == 0);

			return enabled ? fixedClasses : fixedClasses + " disabled";

		}

		protected string displayThumb(object dataitem, int value){

			var display = false;

			var usergeeltje = DataBinder.Eval(dataitem, "Item2") as Icatt_Geeltjes_UserGeeltje ;

			if (usergeeltje == null) 
				display = true;
			else 
				display = (value > 0 && usergeeltje.ThumbUp == 0) || (value < 0 && usergeeltje.ThumbDown == 0);

			return display ? "" : "display:none;";
		}

		protected string myThumbUpCount(object dataItem){

			var thumbUpCount = DataBinder.Eval(dataItem, "Item1.ThumbUpCount");

			if (thumbUpCount == null) { return "0"; }

			return thumbUpCount.ToString();
		}

		protected string myThumbDownCount(object dataItem)
        {
			var thumbDownCount = DataBinder.Eval(dataItem, "Item1.ThumbDownCount");

			if (thumbDownCount == null) { return "0"; }

			return thumbDownCount.ToString();
		}


        #endregion


		#region nested types

		public enum LocalResourceKeys{
			ConfirmDeleteText,
			NoteTooShortText,
			NoAuthorText,
			EditTitle,
			AddTitle,
			AddButtonTitle,
			EditButtonTitle,
			AddNoteButtonTitle
		}
		#endregion

    	protected string tryGetLocalizedString(LocalResourceKeys key){

    		string localizedString;
			try{

				localizedString = get_localizedString(key);
				
			} catch(Exception){
				localizedString = String.Format("Missing resource key '{0}' in local resource file '{1}'", key, LocalResourceFile);
			}

    		return HttpUtility.JavaScriptStringEncode(localizedString);
    	}


    } //class

} //namespace