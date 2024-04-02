using System.ComponentModel.DataAnnotations;

namespace StackOverflow.Infrastructure.Authentication;

public class RegisterVM
{
    [Display(Name = "User name")]
    [Required(ErrorMessage = "User name is required")]
    public string UserName { get; set; }

    [Display(Name = "Email address")]
    [Required(ErrorMessage = "Email address is required")]
    public string EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}
