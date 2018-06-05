using System;

namespace DesktopModules.Icatt.Geeltjes.UI.Display
{
    public partial class Display_Settings : global::Icatt.DotNetNuke.Modules.Geeltjes.Business.Views.Display_Settings
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected override void loadSettingsInUI()
        {
            ctlURL.Url = settingsAdapter.ImageUrl;
            ctlURL.UrlType = settingsAdapter.ImageUrlType;
            this.chkEditMode.Checked = settingsAdapter.AllowEdit;
            this.chkRateThumb.Checked = settingsAdapter.RateThumb;
        }

        protected override void readSettingsFromUI()
        {
            settingsAdapter.ImageUrl = ctlURL.Url ;
            settingsAdapter.ImageUrlType =ctlURL.UrlType ;
            settingsAdapter.AllowEdit = chkEditMode.Checked;
            settingsAdapter.RateThumb = chkRateThumb.Checked;
        }

    }
}