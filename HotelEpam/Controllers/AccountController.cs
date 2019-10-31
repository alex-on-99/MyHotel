using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HotelEpam.Models;
using NLog;
using NLog.Fluent;

namespace HotelEpam.Controllers
{
    public class AccountController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(UserRegistration user)
        {
            logger.Debug("Попытка создания нового пользователя");

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

                logger.Debug($"Обращение к базе данных для проврки уникальности логина {user.Login}");
                if (CommunicationWithDataBase.GetUserByLogin(user.Login.ToLower()) != null)
                {
                    ModelState.AddModelError(nameof(user.Login), "Пользователь с таким логином уже существует");
                }
            }

            if (user.Email != null)
            {
                logger.Debug($"Обращение к базе данных для проврки уникальности email {user.Email}");
                if (CommunicationWithDataBase.GetUserByEmail(user.Email) != null)
                {
                    ModelState.AddModelError(nameof(user.Email), "Email используется в системе");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CommunicationWithDataBase.CreateUser(user.FirstName.ToLower(), user.SecondName.ToLower(),
                        user.Login.ToLower(), user.Email, user.Password, user.DateOfBirth);
                }
                catch
                {
                    logger.Error("Ошибка. пользователь не был создан");
                }

                logger.Info("В систему добавлен новый пользователь");

                FormsAuthentication.SetAuthCookie(user.Login.ToLower(), true);

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
            logger.Debug("Обращение к базе данных для получения данных о пользователе");
            User user = CommunicationWithDataBase.GetUserByLogin(model.Login?.ToLower());

            if (user?.BlockDate > DateTime.Now)
            {
                logger.Debug($"Ошибка входа, пользователь {user.Login} заблокирован в системе");
                ModelState.AddModelError("AuthorizationValidationError",
                    $"Вы заблокированы в системе до {user.BlockDate.ToString("d")}.");
            }
            else if (model.Password == null)
            {
                ModelState.AddModelError("AuthorizationValidationError", "Логин или пароль некорректен");
            }
            else if (CommunicationWithDataBase.CheckUserAuthorization(model.Login?.ToLower(), model?.Password))
            {
                logger.Debug($"Обращение к базе данных для проверки подлиности данных авторизации");
                if (model.Login == null)
                {
                    logger.Error("Ошибка. Логин пользователя был null");
                }

                FormsAuthentication.SetAuthCookie(model.Login.ToLower(), true);
                logger.Info($"Пользователь {user.Login} выполнил вход на сайт");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("AuthorizationValidationError", "Логин или пароль некорректен");
            }

            logger.Info($"Отказ входа. некорректные данные");
            return View(model);
        }
    }
}