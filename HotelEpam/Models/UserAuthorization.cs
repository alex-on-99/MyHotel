using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelEpam.Models
{
    public class UserAuthorization
    {
        [Required(ErrorMessage = "Поле \"Логин\" не заполнено")]
        public string Login { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле \"Пароль\" не заполнено")]
        public string Password { get; set; }
    }
}