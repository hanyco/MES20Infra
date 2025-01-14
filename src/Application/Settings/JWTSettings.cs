namespace Application.Settings;
public class JWTSettings
{
    public string Audience { get; set; }
    public double DurationInMinutes { get; set; }
    public string Issuer { get; set; }
    public string Key { get; set; }
}