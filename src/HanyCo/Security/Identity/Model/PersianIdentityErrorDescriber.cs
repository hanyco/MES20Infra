namespace HanyCo.Infra.Security.Identity.Model;

public class PersianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) => new()
    {
        Code = nameof(DuplicateEmail),
        Description = $"ایمیل {email} تکراری می‌باشد."
    };

    public override IdentityError DuplicateUserName(string userName) => new()
    {
        Code = nameof(DuplicateEmail),
        Description = $"نام کاربری {userName} تکراری می‌باشد."
    };
}
