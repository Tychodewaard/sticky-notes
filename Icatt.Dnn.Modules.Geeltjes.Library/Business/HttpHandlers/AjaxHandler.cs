using System;
using System.Globalization;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using Icatt.DotNetNuke.Modules.Geeltjes.DataAccess;

namespace Icatt.DotNetNuke.Modules.Geeltjes.Business.HttpHandlers {
	public class AjaxHandler : IHttpHandler {
		private string _userName;
		private Int32 _userId = -1;
		private readonly UserInfo _currentUser = UserController.GetCurrentUserInfo(); //returns empty UserInfo object with UserID === -1 when not logged in..
		private HttpContext _context;
		public void ProcessRequest(HttpContext context) {
			context.Response.Expires = 0;
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.CacheControl = "no-cache";
			context.Response.ContentType = "text/plain";

			try {

				//TODO handler does not check whether EditMode is enabled in the module settings. 
				//Enforcing the edit mode is not considered a security issue
				if (_currentUser == null || _currentUser.UserID < 0) {
					context.Response.Write("Error: Anonymous access not allowed.");
					return;
				}

				_userName = _currentUser.FirstName;
				_userId = _currentUser.UserID;
				_context = context;


				string myTask = context.Request.Params["edit"];
				Int32 geeltjeId = -1, intValue;
				Int32? moduleId = null;
				if (Int32.TryParse(context.Request.Params["moduleId"], out intValue))
					moduleId = intValue;

				if (Int32.TryParse(context.Request.Params["id"], out geeltjeId)) {


					switch (myTask) {
						case "delete":

							if (!moduleId.HasValue) {
								context.Response.Write("Error: Missing moduleId parameter. Required for 'delete' action.");
								return;
							}

							deleteNotes(geeltjeId, moduleId.Value);
							break;
						case "edit":
							if (!moduleId.HasValue) {
								context.Response.Write("Error: Missing moduleId parameter. Required for 'edit' action.");
								return;
							}
							updateGeeltje(geeltjeId, moduleId.Value);
							break;
						case "thumbsup":
						case "thumbsdown":
							rateNotes(geeltjeId, myTask);
							break;
						default:
							updatePosition(geeltjeId);
							break;
					}


				} else {
					geeltjeId = addGeeltje();
				}

				context.Response.Write(geeltjeId.ToString(CultureInfo.InvariantCulture));

			} catch (Exception ex) {
				context.Response.Write("Error: " + ex.Message);
			}
		}

		public bool IsReusable {
			get { return false; }
		}

		protected void updatePosition(Int32 myId) {

			string myX = _context.Request.Params["x"];
			string myY = _context.Request.Params["y"];
			string myZ = _context.Request.Params["z"];
			if (!string.IsNullOrEmpty(myX) && !string.IsNullOrEmpty(myY) && !string.IsNullOrEmpty(myZ)) {
				string[] myNewPos = { myX, myY, myZ };
				String myStrNewPos = String.Join("x", myNewPos);

				using (var ctx = GeeltjesEntities.GetContext()) {
					try {
						var posToUpdate = ctx.Icatt_Geeltjes_Geeltje.FirstOrDefault(geel => geel.Id == myId);
						if (posToUpdate != null) {
							posToUpdate.Xyz = myStrNewPos;
							ctx.SaveChanges();
						}
					} catch (Exception ex) {

						throw ex.InnerException;
					}
				}
			}
		}

		protected void updateGeeltje(Int32 myId, int moduleId) {

			string text = _context.Request.Params["body"];
			string name = _context.Request.Params["author"];
			string color = _context.Request.Params["color"];

			using (var ctx = GeeltjesEntities.GetContext()) {
				try {
					var posToUpdate = ctx.Icatt_Geeltjes_Geeltje.FirstOrDefault(geel => geel.Id == myId && geel.ModuleId == moduleId);

					if (posToUpdate != null && canEditNote(posToUpdate, moduleId)) {

						posToUpdate.Name = name;
						posToUpdate.Text = text;
						posToUpdate.Color = color;
						ctx.SaveChanges();
					}
				} catch (Exception ex) {

					throw ex.InnerException;
				}
			}
		}

		protected int addGeeltje() {
			using (var ctx = GeeltjesEntities.GetContext()) {
				try {

					string myUserName = _context.Request.Params["author"];
					string myText = _context.Request.Params["body"];
					string myColor = _context.Request.Params["color"];
					string moduleIdValue = _context.Request.Params["moduleId"];

					string myX = _context.Request.Params["x"];
					string myY = _context.Request.Params["y"];
					string myZ = _context.Request.Params["zIndex"];
					string[] myNewPos = { myX, myY, myZ };
					String myStrNewPos = String.Join("x", myNewPos);

					int moduleId = -1;

					if (!int.TryParse(moduleIdValue, out moduleId))
						throw new ArgumentException("Missing or invalid 'moduleId' parameter in request to add geeltje for AjaxHandler");

					var newNotes = new Icatt_Geeltjes_Geeltje {
						Color = myColor
						,
						Name = myUserName
						,
						Text = myText
						,
						Xyz = myStrNewPos
						,
						CreatedAt = DateTime.Now
						,
						CreatedByUserId = _userId
						,
						ModuleId = moduleId
					};


					ctx.Icatt_Geeltjes_Geeltje.AddObject(newNotes);
					ctx.SaveChanges();

					return newNotes.Id;

				} catch (Exception ex) {

					throw ex.InnerException;
				}
			}
		}

		private bool canEditNote(Icatt_Geeltjes_Geeltje geeltje, int moduleId) {

			if (geeltje.CreatedByUserId == _currentUser.UserID)
				return true;

			var mc = new ModuleController();
			var modInfo = mc.GetModule(moduleId);

			return ModulePermissionController.CanEditModuleContent(modInfo);

		}

		protected void deleteNotes(Int32 geeltjeId, int moduleId) {

			using (var ctx = GeeltjesEntities.GetContext()) {
				try {
					Icatt_Geeltjes_Geeltje notesToDelete;

					notesToDelete = ctx.Icatt_Geeltjes_Geeltje.FirstOrDefault(geel => geel.Id == geeltjeId && geel.ModuleId == moduleId);


					if (notesToDelete != null && canEditNote(notesToDelete, moduleId)) {

						var userNotesToDelete = notesToDelete.Icatt_Geeltjes_UserGeeltje;

						//Delete votes
						foreach (var icattGeeltjesUserGeeltje in userNotesToDelete) {
							ctx.Icatt_Geeltjes_UserGeeltje.DeleteObject(icattGeeltjesUserGeeltje);
						}

						ctx.Icatt_Geeltjes_Geeltje.DeleteObject(notesToDelete);
						ctx.SaveChanges();
					}
				} catch (Exception ex) {

					throw ex.InnerException;
				}
			}
		}

		protected void rateNotes(Int32 geeltjeId, string noteRate) {
			using (var ctx = GeeltjesEntities.GetContext()) {
				try {

					bool isChange;
					if (crudUsersGeeltjes(geeltjeId, noteRate, out isChange)) {
						//only allow new rating or change of rating
						rateUpDownNotes(geeltjeId, noteRate, isChange);
					}

				} catch (Exception ex) {

					throw ex.InnerException;
				}
			}
		}

		protected bool crudUsersGeeltjes(Int32 geeltjeId, string noteRate, out bool isChange) {
			var validCount = false;
			isChange = false;
			using (var ctx = GeeltjesEntities.GetContext()) {
				try {
					Int32 thumbUp = 0;
					Int32 thumbDown = 0;

					if (noteRate == "thumbsup")
						thumbUp = 1;
					else
						thumbDown = 1;

					Icatt_Geeltjes_UserGeeltje userGeeltje =
						ctx.Icatt_Geeltjes_UserGeeltje.FirstOrDefault(ug => ug.GeeltjeId == geeltjeId && ug.UserId == _currentUser.UserID);


					if (userGeeltje == null) {
						//add
						validCount = true;
						userGeeltje = new Icatt_Geeltjes_UserGeeltje {
							GeeltjeId = geeltjeId,
							UserId = _userId,
							ThumbDown = thumbDown,
							ThumbUp = thumbUp
						};
						// save UserGeeltje object
						ctx.Icatt_Geeltjes_UserGeeltje.AddObject(userGeeltje);
					} else {
						if (
								((!userGeeltje.ThumbDown.HasValue || userGeeltje.ThumbDown.Value == 0) && thumbDown == 1) ||
								((!userGeeltje.ThumbUp.HasValue || userGeeltje.ThumbUp.Value == 0) && thumbUp == 1)
							) {
							//update if count is valid
							validCount = true;
							isChange = true;
							userGeeltje.ThumbDown = thumbDown;
							userGeeltje.ThumbUp = thumbUp;

						}


					}


					ctx.SaveChanges();

				} catch (Exception ex) {

					throw ex.InnerException;
				}

				return validCount;

			}
		}

		protected void rateUpDownNotes(Int32 geeltjeId, string noteRate, bool isChange) {

			using (var ctx = GeeltjesEntities.GetContext()) {
				try {
					Icatt_Geeltjes_Geeltje notesToRate = ctx.Icatt_Geeltjes_Geeltje.FirstOrDefault(geel => geel.Id == geeltjeId);

					if (notesToRate != null) {
						Int32 noteThumbUpCount = notesToRate.ThumbUpCount.HasValue ? notesToRate.ThumbUpCount.Value : 0;
						Int32 noteThumbDownCount = notesToRate.ThumbDownCount.HasValue ? notesToRate.ThumbDownCount.Value : 0;

						if (noteRate == "thumbsup") {
							//increase 1 the ThumbUpCount of the notes
							noteThumbUpCount++;
							notesToRate.ThumbUpCount = noteThumbUpCount;

							if (isChange) {
								//change of thumbs down to thumbs up. decrease thumb down
								noteThumbDownCount--;
								notesToRate.ThumbDownCount = noteThumbDownCount;
							}

						} else {
							//increase 1 the ThumbDownCount of the notes
							noteThumbDownCount++;
							notesToRate.ThumbDownCount = noteThumbDownCount;

							if (isChange) {
								//change of thumbsup to thubsdown. decrease thumb up
								noteThumbUpCount--;
								notesToRate.ThumbUpCount = noteThumbUpCount;
							}
						}

						// save Geeltje object
						ctx.SaveChanges();

					}
				} catch (Exception ex) {

					throw new InvalidOperationException(string.Format("There was an error rating this notes : {0}", ex.Message), ex);
				}
			}
		}

	}
}
