using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Hinata.Data.Commands;
using Hinata.Filters;
using Hinata.Models;
using Hinata.Web.Mvc;

namespace Hinata.Controllers
{
    [Authorize]
    public class UserController : WindowsAuthenticationContoller
    {
        private readonly UserDbCommand _userDbCommand = new UserDbCommand(GlobalSettings.DefaultConnectionString);
        private readonly ItemDbCommand _itemDbCommand = new ItemDbCommand(GlobalSettings.DefaultConnectionString);

        [Route("user/{name}")]
        [HttpGet]
        public async Task<ActionResult> Index(string name)
        {
            var user = await _userDbCommand.FindByNameAsync(name);
            if (user == null) return HttpNotFound();

            var items = await _itemDbCommand.GetByAuthorAsync(user);
            var itemModels = Mapper.Map<IEnumerable<ItemIndexModel>>(items).ToArray();

            ViewBag.Title = user.DisplayName;

            if (LogonUser == user)
            {
                var model = new MyPageModel
                {
                    Name = user.Name,
                    DisplayName = user.DisplayName
                };
                model.PublicItems.AddRange(itemModels.Where(x => x.IsPublic));
                model.PrivateItems.AddRange(itemModels.Where(x => x.IsPublic == false));
                return View("MyPage", model);
            }
            else
            {
                var model = new UserIndexModel()
                {
                    Name = user.Name,
                    DisplayName = user.DisplayName
                };
                model.Items.AddRange(itemModels.Where(x => x.IsPublic));
                return View("Index", model);
            }
        }

        [Route("register")]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> Register(UserCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User(User.Identity.Name);
            Mapper.Map(model, user);

            await _userDbCommand.SaveAsync(user);

            return RedirectToAction("Index", "Item");
        }

        [Route("userupdate")]
        [HttpGet]
        public async Task<ActionResult> Update(string id)
        {
            if (id != LogonUser.Id) return new HttpUnauthorizedResult();

            var user = await _userDbCommand.FindAsync(id);
            if (user == null) return HttpNotFound();
            var model = Mapper.Map<UserUpdateModel>(user);
            return View(model);
        }

        [Route("userupdate")]
        [HttpPost]
        public async Task<ActionResult> Update(UserUpdateModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (model.Id != LogonUser.Id) return new HttpUnauthorizedResult();

            var user = await _userDbCommand.FindAsync(model.Id);
            if (user == null) return HttpNotFound();

            Mapper.Map(model, user);

            await _userDbCommand.SaveAsync(user);

            return RedirectToAction("Index", "User", new {name = user.Name});
        }

        [NoCache]
        [Route("checkname")]
        [HttpGet]
        public async Task<ActionResult> CheckName(string name)
        {
            if (Hinata.User.IsReservedName(name))
            {
                return Json(@"この名前は使用できません。", JsonRequestBehavior.AllowGet);
            }

            if (LogonUser != null && LogonUser.Name == name)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            if (await _userDbCommand.ExistAsync(name))
            {
                return Json(@"この名前はすでに使用されています。", JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}