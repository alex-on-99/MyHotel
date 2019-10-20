using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class User
    {
        [Key] public int Id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле \"Имя\" не заполнено")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Имя может содержать от 3х до 15ти символов")]
        [RegularExpression("^([A-Za-z]+||[А-ЯЁа-яё]+)$",
            ErrorMessage = "Имя должно состоять из символов русского или английского алфавита")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле \"Фамилия\" не заполнено")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Фамилия может содержать от 3х до 15ти символов")]
        [RegularExpression("^([A-Za-z]+||[А-ЯЁа-яё]+)$",
            ErrorMessage = "Фамилия должна состоять из символов русского или английского алфавита")]
        public string SecondName { get; set; }

        [Display(Name = "Серия и номер паспорта")]
        [RegularExpression("^[А-Яа-я]{2}[0-9]{6}$", ErrorMessage = "Некорректные паспортные данные")]
        public string Passport { get; set; }

        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Дата не указана")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Поле \"Логин\" не заполнено")]
        [StringLength(12, MinimumLength = 5, ErrorMessage = "Логин может содержать от 5ти до 12ти символов")]
        [RegularExpression("^[a-zA-Z0-9_\\.\\-]+$",
            ErrorMessage =
                "Некорректный логин. Логин может содержать символы английского алфавита, цифры, символы '_', '-' и '.'")]
        public string Login { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Поле \"Email\" не заполнено")]
        [RegularExpression("^[a-zA-Z0-9_\\.\\+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-\\.]+$",
            ErrorMessage = "Некорректный Email")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Поле \"Пароль\" не заполнено")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required] public int ViolatingPower { get; set; }

        [Display(Name = "Дата конца блокировки")]
        [DataType(DataType.Date)]
        public DateTime BlockDate { get; set; }

        [Required] public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}