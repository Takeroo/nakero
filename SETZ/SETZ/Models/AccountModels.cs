using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace SETZ.Models
{

    public class UniquePhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();
            if (value != null)
            {
                string PhoneNumber = value.ToString();
                int count = 0;
                if (setzDB.Users.ToList().Count() > 0)
                {
                    count = setzDB.Users.Where(x => x.PhoneNumber == PhoneNumber).ToList().Count();
                }
                if (count != 0)
                    return new ValidationResult("Этот номер телефона уже зарегистрирован");
                return ValidationResult.Success;
            }
            return new ValidationResult("Просьба предоставить другой номер телефона");
        }
    }
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Требуется номер телефона", AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходим пароль", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        [Required(ErrorMessage = "Укажите ваше полное имя", AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [UniquePhoneNumber]
        [Required(ErrorMessage = "Просьба предоставить номер телефона")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Неправильный номер телефона")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Подтверждение номера телефона")]
        [Compare("PhoneNumber", ErrorMessage = "Номер телефона и подтверждение телефона не совпадают")]
        public string ConfirmPhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходим пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной не менее {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля не совпадают")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Terms and Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Вы должны согласиться!")]
        public bool TermsAndConditions { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
