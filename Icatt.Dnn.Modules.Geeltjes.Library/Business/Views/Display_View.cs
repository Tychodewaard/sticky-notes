using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.FileSystem;
using Icatt.DotNetNuke.Modules.Geeltjes.Business.SettingAdapters;
using Icatt.DotNetNuke.Modules.Geeltjes.DataAccess;
using System.Linq;

namespace Icatt.DotNetNuke.Modules.Geeltjes.Business.Views
{
    public abstract class Display_View : Base.PortalModuleBase
    {


#region fields

        private IEnumerable<Tuple<Icatt_Geeltjes_Geeltje, Icatt_Geeltjes_UserGeeltje>> _dataSource;
        private Display_SettingAdapter _settingAdapter ;
		private UserInfo _currentUser;

#endregion

		protected PortalSettings portalSettings {
			get {
				return PortalController.GetCurrentPortalSettings();
			}
		}

		protected UserInfo currentUser {
			get {
				if (_currentUser == null)
					_currentUser = UserController.GetCurrentUserInfo();

				return _currentUser;
			}
		}

        #region Page Event Handlers

        // Override the OnLoad method to set _text to
        // a default value if it is null.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _settingAdapter = getModuleSettingsAdapter<Display_SettingAdapter>();

            using (var entities = GeeltjesEntities.GetContext()){

            	_dataSource = entities.Icatt_Geeltjes_Geeltje
            		.Include("Icatt_Geeltjes_UserGeeltje")
            		.Where(g => g.ModuleId == this.ModuleId)
					.ToList()
            		.Select(g => new Tuple<Icatt_Geeltjes_Geeltje, Icatt_Geeltjes_UserGeeltje>(
            		             	g,
            		             	g.Icatt_Geeltjes_UserGeeltje.SingleOrDefault(ug => ug.UserId == currentUser.UserID)
            		             	))
					.ToList();
            }

             DataBind();


        }

        #endregion

        protected IEnumerable<Tuple<Icatt_Geeltjes_Geeltje, Icatt_Geeltjes_UserGeeltje>> DataSource
        {
            get { return _dataSource; }
        }

        protected Display_SettingAdapter SettingAdapter
        {
            get { return _settingAdapter; }
        }

        protected override Entities.Modules.SettingsAdapterBase createModuleSettingsAdapter()
        {
            return new Display_SettingAdapter(ModuleConfiguration);
        }

        protected string backgroundNotesContainer
        {
            get
            {
            	string strUrl = _settingAdapter.ImageUrl;
            	if (!string.IsNullOrEmpty(strUrl))
                {
                    try
                    {
                        Int32 portalId = PortalSettings.Current.PortalId;
                        string imgfileId;

                        if (!String.IsNullOrWhiteSpace(strUrl))
                        {
                            imgfileId = strUrl.Substring(7);

                            Int32 fileId;
                            if (Int32.TryParse(imgfileId, out fileId))
                            {
                                var objFileController = new FileController();
                                FileInfo objFileInfo = objFileController.GetFileById(fileId, portalId);
                                string fileName = objFileInfo.FileName;
                                string dirPath = Server.MapPath(System.Web.VirtualPathUtility.ToAbsolute(portalSettings.HomeDirectory + objFileInfo.Folder));

                                return dirPath;
                            }
                        }

     
                    }
                    catch (Exception ex)
                    {

                        throw ex.InnerException;
                    }

                }
                return string.Empty;
            }
        }
    }
}
