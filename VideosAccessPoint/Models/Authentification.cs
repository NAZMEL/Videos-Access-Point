using System.ComponentModel.DataAnnotations;

namespace VideosAccessPoint.Models
{
    /// <summary>
    /// these classes are for authentification
    /// </summary>

        public class Login
        {
            [Required(ErrorMessage = "The Name field must not be empty")]
            public string Name { set; get; }

            [Required(ErrorMessage = "The Password field must not be empty")]
            [DataType(DataType.Password)]
            public string Password { set; get; }
        }

        public class Register
        {
            [Required(ErrorMessage = "The Name field must not be empty")]
            [StringLength(30)]
            public string Name { set; get; }

            [Required(ErrorMessage = "The Password field must not be empty")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
            [DataType(DataType.Password)]
            public string Password { set; get; }

            [Required(ErrorMessage = "The Confirmation password field must not be empty")]
            [DataType(DataType.Password)] 
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { set; get; }
        }

    
}