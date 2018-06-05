using System;
using DotNetNuke.Services.Localization;
using Icatt.DotNetNuke.Entities.Modules;

namespace Icatt.DotNetNuke.Modules.Geeltjes.Business.Base {
    //public abstract class PortalModuleBase : DotNetNuke.Entities.Modules.PortalModuleBase {
    public abstract class PortalModuleBase : IcattPortalModuleBase {

        public virtual string LocalizedString(string key) {
            return Localization.GetString(key, LocalResourceFile);
        }

    }
}
