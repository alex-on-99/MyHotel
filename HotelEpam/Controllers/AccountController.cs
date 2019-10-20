using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HotelEpam.Models;

namespace HotelEpam.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(UserRegistration user)
        {
            if (user.Password != null)
            {
                if (user.Password.Length < 6 || user.Password.Length > 15)
                {
                    ModelState.AddModelError(nameof(user.Password), "Пароль должен содержать от 6 до 15 символов");
                }

                if (!Regex.IsMatch(user.Password, "^[a-zA-ZА-Яа-я0-9_\\.\\-]+$"))
                {
                    ModelState.AddModelError(nameof(user.Password), "Пароль содержит некорректные символы");
                }

                if (user.ConfirmPassword != user.Password)
                {
                    ModelState.AddModelError(nameof(user.ConfirmPassword), "Пароли не совпадают");
                }
            }

            if (!(user.DateOfBirth > DateTime.Now.AddYears(-110)
                  && user.DateOfBirth < DateTime.Now.AddYears(-16)))
            {
                ModelState.AddModelError(nameof(user.DateOfBirth), 
                    "Некорректная дата рождения. Наши пользователи должны быть старше 16 и моложе 110 лет");
            }

            if (user.Login != null)
            {
                if (!Regex.IsMatch(user.Login, "^[A-Za-z]+"))
                {
                    ModelState.AddModelError(nameof(user.Login),
                        "В логине должны присутствовать буквы английского алфавита");
                }

                if (CommunicationWithDataBase.GetUserByLogin(user.Login.ToLower()) != null)
                {
                    ModelState.AddModelError(nameof(user.Login), "Пользователь с таким логином уже существует");
                }
            }

            if (user.Email != null)
            {
                if (CommunicationWithDataBase.GetUserByEmail(user.Email) != null)
                {
                    ModelState.AddModelError(nameof(user.Email), "Email используется в системе");
                }
            }

            if (ModelState.IsValid)
            {
                    CommunicationWithDataBase.CreateUser(user.FirstName.ToLower(), user.SecondName.ToLower(),
                        user.Login.ToLower(), user.Email,user.Password,user.DateOfBirth);

                    FormsAuthentication.SetAuthCookie(user.Login.ToLower(),true);

                    return RedirectToAction("Index", "Home");
            }

            return View(user);
        }
        public ActionResult Authorization()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorization(UserAuthorization model)
        {
            if (CommunicationWithDataBase.CheckUserAuthorization(model.Login?.ToLower(), model?.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Login.ToLower(), true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("AuthorizationValidationError","Логин или пароль некорректен");
            }

            return View(model);
        }
    }
}