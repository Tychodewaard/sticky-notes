using Icatt.DotNetNuke.Entities.Modules;
using Icatt.DotNetNuke.Modules.Geeltjes.Business.SettingAdapters;

namespace Icatt.DotNetNuke.Modules.Geeltjes.Business.Views {

    public abstract class Display_Settings : IcattModuleSettingsBase
    {
        

#region Private Properties
        protected Display_SettingAdapter settingsAdapter
        {
            get { return this.getModuleSettingsAdapter<Display_SettingAdapter>(); }
        }

        #endregion

        protected override SettingsAdapterBase createModuleSettingsAdapter()
        {
            return new Display_SettingAdapter(ModuleConfiguration);
        }


    }
}
