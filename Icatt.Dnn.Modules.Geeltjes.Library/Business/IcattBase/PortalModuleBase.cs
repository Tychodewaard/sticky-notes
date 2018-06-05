// Decompiled with JetBrains decompiler
// Type: Icatt.DotNetNuke.Entities.Modules.PortalModuleBase
// Assembly: Icatt.DotNetNuke.Entities, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 151AD93D-7EAC-4DE7-86C1-7684C01B65CF
// Assembly location: D:\Websites\dev\dev-734\bin\Icatt.DotNetNuke.Entities.dll

//using Microsoft.VisualBasic.CompilerServices;
using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Icatt.DotNetNuke.Entities.Modules
{
  public abstract class IcattPortalModuleBase : PortalModuleBase
  {
    protected SettingsAdapterBase _settingsAdapter;

    protected IcattPortalModuleBase()
    {
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
            string Left = this.get_localizedString(key.ToString("G"));
            if (!string.IsNullOrEmpty(Left))
                return Left;
            throw new Exception(string.Format("Localization Error: No entry found for key '{0:G}' in resourcefile '{1}'", (object)key, (object)this.LocalResourceFile));
        }

        protected string get_localizedString(Enum key, params object[] args)
        {
            return string.Format(this.get_localizedString(key), args);
        }
    }
}
