using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class UpdateRequest
{
    public string AccountNumber { get; set; }

    public string Address { get; set; }

    public string BankName { get; set; }

    public string BirthPlace { get; set; }

    public DateTime? BrithDate { get; set; }

    public string CardNumber { get; set; }

    public string ChildCount { get; set; }

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

    [Required]
    public string Id { get; set; }

    public string IdNumber { get; set; }
    public bool IsActive { get; set; }
    public string Issued { get; set; }
    public string JobCode { get; set; }
    public int? JobPositionId { get; set; }
    public string NationalCode { get; set; }
    public string PersonnelCode { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfilePicture { get; set; }
    public Roles? Role { get; set; }

    public int? ServiceLocationId { get; set; }

    public DateTime? SettlementDate { get; set; }

    public DateTime? SmartCardFromDate { get; set; }

    public string SmartCardNumber { get; set; }

    public DateTime? SmartCardToDate { get; set; }

    public string TelephoneNumber { get; set; }

    public Guid? UserGroupId { get; set; }

    [Required]
    public int UserLevelId { get; set; }

    [Required]
    [MinLength(4)]
    public string UserName { get; set; }
}