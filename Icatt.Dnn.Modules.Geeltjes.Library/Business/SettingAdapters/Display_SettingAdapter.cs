using System;
using System.Globalization;
using DotNetNuke.Entities.Modules;
using Icatt.DotNetNuke.Entities.Modules;

namespace Icatt.DotNetNuke.Modules.Geeltjes.Business.SettingAdapters {
    public class Display_SettingAdapter: SettingsAdapterBase {

        #region fields

		string _imageUrl;
        private string _imageUrlType;
        bool? _rateThumb = null;
        bool? _allowEdit = null;

        #endregion

        #region props

        /// <remarks>
        /// Copy paste this block as template for string properties for which Nothing is not a valid value
        /// </remarks>
        public string ImageUrl
        {
            get
            {
                //NB. Do NOT test for Empty here! 
				return _imageUrl ?? (_imageUrl = this.get_tabModuleSettings(SettingName.ImageUrl));
            }
            set { _imageUrl = value; }
        }

        public string ImageUrlType
        {
            get
            {
                //NB. Do NOT test for Empty here! 
				return _imageUrlType ?? (_imageUrlType = this.get_tabModuleSettings(SettingName.ImageUrlType));
            }
            set { _imageUrlType = value; }
        }

        public bool AllowEdit {
           get
           {
			   if (!_allowEdit.HasValue){

				   string value = this.get_tabModuleSettings(SettingName.AllowEdit);
				   bool flag;
				   _allowEdit = !Boolean.TryParse(value,out flag) || flag;
			   	
			   }

                return _allowEdit.Value;
            }

            set { _allowEdit = value; }

        }

        public bool RateThumb
        {
            get
            {
				if (!_rateThumb.HasValue){

					string value = this.get_tabModuleSettings(SettingName.RateThumb);
					bool flag;
					_rateThumb = !bool.TryParse(value, out flag) || flag;
				}


                return _rateThumb.Value;
            }

            set { _rateThumb = value; }
        }
        #endregion

        
        public Display_SettingAdapter(ModuleInfo info) : base(info) {
            
        }

        public override void Save(int moduleId,int tabModuleId) {
			updateTabModuleSetting(tabModuleId, SettingName.AllowEdit, AllowEdit.ToString(CultureInfo.InvariantCulture));

			updateTabModuleSetting(tabModuleId, SettingName.RateThumb, RateThumb.ToString(CultureInfo.InvariantCulture));

			updateTabModuleSetting(tabModuleId, SettingName.ImageUrl,
                                !string.IsNullOrEmpty(_imageUrl) ? ImageUrl : string.Empty);

			updateTabModuleSetting(tabModuleId, SettingName.ImageUrlType,
                                !string.IsNullOrEmpty(_imageUrlType) ? ImageUrlType : string.Empty);
        }

        public enum SettingName {
            ImageUrl,
            ImageUrlType,
            AllowEdit,
            RateThumb
        }
    }
}
