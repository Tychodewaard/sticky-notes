// Decompiled with JetBrains decompiler
// Type: Icatt.DotNetNuke.Entities.Modules.ModuleSettingsBase
// Assembly: Icatt.DotNetNuke.Entities, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 151AD93D-7EAC-4DE7-86C1-7684C01B65CF
// Assembly location: D:\Websites\dev\dev-734\bin\Icatt.DotNetNuke.Entities.dll

using DotNetNuke.Entities.Modules;
using System;
using DotNetNuke.Services.Localization;

namespace Icatt.DotNetNuke.Entities.Modules
{
  public abstract class IcattModuleSettingsBase : ModuleSettingsBase
  {
    protected SettingsAdapterBase _settingsAdapter;

    protected IcattModuleSettingsBase()
    {
      this.Load += new EventHandler(this.Page_Load);
      this._settingsAdapter = (SettingsAdapterBase) null;
    }

    protected T getModuleSettingsAdapter<T>() where T : SettingsAdapterBase
    {
      if (this._settingsAdapter == null)
        this._settingsAdapter = this.createModuleSettingsAdapter();
      return (T) this._settingsAdapter;
    }

    protected abstract SettingsAdapterBase createModuleSettingsAdapter();

    private string get_localizedString(string key)
    {
      return Localization.GetString(key, this.LocalResourceFile);
    }

    protected string get_localizedString(Enum key)
    {
      return this.get_localizedString(key.ToString("G"));
    }

    protected abstract void loadSettingsInUI();

    protected abstract void readSettingsFromUI();

    public override sealed void LoadSettings()
    {
      base.LoadSettings();
    }

    public override sealed void UpdateSettings()
    {
      if (!this.Page.IsValid)
        return;
      this.readSettingsFromUI();
      this.getModuleSettingsAdapter<SettingsAdapterBase>().Save(this.ModuleId, this.TabModuleId);
      ModuleController.SynchronizeModule(this.ModuleId);
    }

    private void Page_Load(object sender, EventArgs e)
    {
      if (this.IsPostBack)
        return;
      this.loadSettingsInUI();
    }
  }
}
