
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Data.Models;

public class UserRegistrationForm
{
    [DataType(DataType.Text)]
    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^(?i)[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;
    [Required]
    [Display(Name = "Password", Prompt = "Enter your password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be between 8 and 15 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.")]
    public string Password { get; set; } = null!;
    [Required]
    [Display(Name = "Confirm Password", Prompt = "Confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
    [Required]
    [Display(Name = "Terms and Conditions", Prompt = "I agree to the terms and conditions")]
    public bool TermsAndConditions { get; set; }  

}
