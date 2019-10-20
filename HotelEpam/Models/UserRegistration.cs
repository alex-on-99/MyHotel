using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelEpam.Models
{
    public class UserRegistration : User
    {
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public bool PrivacyPolicy { get; set; }
    }
}