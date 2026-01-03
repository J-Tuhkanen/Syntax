namespace Syntax.API.Controllers
{
    public record UserDetailsDto(
        string? Email, 
        DateTime JoinedDate, 
        UserSettingsDto Settings, 
        UserActivityDto Activity);
}
