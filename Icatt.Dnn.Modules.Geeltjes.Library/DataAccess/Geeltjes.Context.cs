//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.EntityClient;

namespace Icatt.DotNetNuke.Modules.Geeltjes.DataAccess
{
    public partial class GeeltjesEntities : ObjectContext
    {
        public const string ConnectionString = "name=GeeltjesEntities";
        public const string ContainerName = "GeeltjesEntities";
    
        #region Constructors
    
        private GeeltjesEntities()
            : base(ConnectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = false;
        }
    
        private GeeltjesEntities(string connectionString)
            : base(connectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = false;
        }
    
        private GeeltjesEntities(EntityConnection connection)
            : base(connection, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = false;
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<Icatt_Geeltjes_Geeltje> Icatt_Geeltjes_Geeltje
        {
            get { return _icatt_Geeltjes_Geeltje  ?? (_icatt_Geeltjes_Geeltje = CreateObjectSet<Icatt_Geeltjes_Geeltje>("Icatt_Geeltjes_Geeltje")); }
        }
        private ObjectSet<Icatt_Geeltjes_Geeltje> _icatt_Geeltjes_Geeltje;
    
        public ObjectSet<Icatt_Geeltjes_UserGeeltje> Icatt_Geeltjes_UserGeeltje
        {
            get { return _icatt_Geeltjes_UserGeeltje  ?? (_icatt_Geeltjes_UserGeeltje = CreateObjectSet<Icatt_Geeltjes_UserGeeltje>("Icatt_Geeltjes_UserGeeltje")); }
        }
        private ObjectSet<Icatt_Geeltjes_UserGeeltje> _icatt_Geeltjes_UserGeeltje;
    
        public ObjectSet<Users> Users
        {
            get { return _users  ?? (_users = CreateObjectSet<Users>("Users")); }
        }
        private ObjectSet<Users> _users;
    
        public ObjectSet<Modules> Modules
        {
            get { return _modules  ?? (_modules = CreateObjectSet<Modules>("Modules")); }
        }
        private ObjectSet<Modules> _modules;

        #endregion

    }
}
