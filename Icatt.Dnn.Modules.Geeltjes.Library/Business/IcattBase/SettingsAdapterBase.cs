// Decompiled with JetBrains decompiler
// Type: Icatt.DotNetNuke.Entities.Modules.SettingsAdapterBase
// Assembly: Icatt.DotNetNuke.Entities, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 151AD93D-7EAC-4DE7-86C1-7684C01B65CF
// Assembly location: D:\Websites\dev\dev-734\bin\Icatt.DotNetNuke.Entities.dll

using DotNetNuke.Entities.Modules;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Icatt.DotNetNuke.Entities.Modules
{
  public abstract class SettingsAdapterBase
  {
    private readonly Hashtable _moduleSettingsTable;
    private readonly Hashtable _tabModuleSettingsTable;
    private ModuleController _moduleControllerInstance;
    private readonly char QUALIFIER_SEPERATOR_CHAR;

    private ModuleController moduleControllerInstance
    {
      get
      {
        if (this._moduleControllerInstance == null)
          this._moduleControllerInstance = new ModuleController();
        return this._moduleControllerInstance;
      }
    }

    protected virtual string get_moduleSettings(Enum key)
    {
      return this.get_moduleSettings(key, (string) null);
    }

    protected string get_moduleSettings(Enum key, string qualifier)
    {
      object objectValue = RuntimeHelpers.GetObjectValue(this._moduleSettingsTable[string.IsNullOrEmpty(qualifier) ? (object) key.ToString("G") : (object) (key.ToString("G") + QUALIFIER_SEPERATOR_CHAR.ToString() + qualifier)]);
      return objectValue != null ? objectValue.ToString() : (string) null;
    }

    protected virtual string get_tabModuleSettings(Enum key)
    {
      return this.get_tabModuleSettings(key, (string) null);
    }

    protected string get_tabModuleSettings(Enum key, string qualifier)
    {
      if (this._tabModuleSettingsTable == null)
        throw new InvalidOperationException("The ModuleConfiguration property must be set before any operations on the object. The parameterless constructor is only avaliable to facilitate use of the base type in genereric code constructs (which do not allow for paramterized constructors).");
      object objectValue = RuntimeHelpers.GetObjectValue(this._tabModuleSettingsTable[string.IsNullOrEmpty(qualifier) ? (object) key.ToString("G") : (object) (key.ToString("G") + QUALIFIER_SEPERATOR_CHAR.ToString() + qualifier)]);
      return objectValue != null ? objectValue.ToString() : (string) null;
    }

    public SettingsAdapterBase(ModuleInfo info)
    {
      this._moduleSettingsTable = (Hashtable) null;
      this._tabModuleSettingsTable = (Hashtable) null;
      this.QUALIFIER_SEPERATOR_CHAR = ':';
      this._moduleSettingsTable = info.ModuleSettings;
      this._tabModuleSettingsTable = info.TabModuleSettings;
    }

    public virtual void Save(int moduleId, int tabModuleId)
    {
      throw new NotImplementedException("Overload SettingsAdapterBase.Save(moduleId,tabModuleId) must be overridden in deriving class. Will be made MustOverride in future");
    }

    [Obsolete("Use overload Save(moduleId,tabModuleId)")]
    public virtual void Save(int moduleId)
    {
      throw new NotImplementedException("Obsolete method. Only present for backwards compatibilty. You should override the Save(moduleId,tabModuleId) overload and refrain from using the Save(moduleId) overload");
    }

    protected virtual void updateModuleSetting(int moduleId, Enum settingName, string value)
    {
      this.updateModuleSetting(moduleId, string.Empty, settingName, value);
    }

    protected void updateModuleSetting(int moduleId, string settingQualifier, Enum settingName, string value)
    {
      string settingName1 = string.IsNullOrEmpty(settingQualifier) ? settingName.ToString("G") : settingQualifier + QUALIFIER_SEPERATOR_CHAR.ToString() + settingName.ToString("G");
      if (value == null)
        this.moduleControllerInstance.DeleteModuleSetting(moduleId, settingName1);
      else
        this.moduleControllerInstance.UpdateModuleSetting(moduleId, settingName1, value);
    }

    protected void updateTabModuleSetting(int tabModuleId, Enum settingName, string value)
    {
      this.updateTabModuleSetting(tabModuleId, string.Empty, settingName, value);
    }

    protected void updateTabModuleSetting(int tabModuleId, string settingQualifier, Enum settingName, string value)
    {
      string settingName1 = string.IsNullOrEmpty(settingQualifier) ? settingName.ToString("G") : settingQualifier + QUALIFIER_SEPERATOR_CHAR.ToString() + settingName.ToString("G");
      if (value == null)
        this.moduleControllerInstance.DeleteTabModuleSetting(tabModuleId, settingName1);
      else
        this.moduleControllerInstance.UpdateTabModuleSetting(tabModuleId, settingName1, value);
    }
  }
}
