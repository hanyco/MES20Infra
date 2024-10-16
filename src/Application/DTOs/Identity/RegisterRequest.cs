using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class RegisterRequest
{
    public string AccountNumber { get; set; }
    public string Address { get; set; }
    public string BankName { get; set; }
    public string BirthPlace { get; set; }
    public DateTime? BrithDate { get; set; }
    public string CardNumber { get; set; }
    public string ChildCount { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string DisplayName { get; set; }

    public DateTime? DrivingLicenseDate { get; set; }
    public DateTime? DrivingLicenseEXPDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public int? DrivingLicenseTypeId { get; set; }
    public string Email { get; set; }
    public DateTime? EmployeementDate { get; set; }
    public string FatherName { get; set; }
    public int? GenderId { get; set; }
    public string IdNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsDriver { get; set; }
    public string Issued { get; set; }
    public string JobCode { get; set; }
    public int? JobPositionId { get; set; }
    public string NationalCode { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The password field must contain at least 4 characters")]
    public string Password { get; set; }

    public string PersonnelCode { get; set; }

    //public Roles? Role { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }

    public string ProfilePicture { get; set; }
    public int? ServiceLocationId { get; set; }

    public DateTime? SettlementDate { get; set; }

    public DateTime? SmartCardFromDate { get; set; }

    public string SmartCardNumber { get; set; }

    public DateTime? SmartCardToDate { get; set; }

    public string TelephoneNumber { get; set; }

    public Guid? UserGroupId { get; set; }

    public int? UserLevelId { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The username field must contain at least 4 characters")]
    public string UserName { get; set; }
}
