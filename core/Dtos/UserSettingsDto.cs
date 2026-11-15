namespace Syntax.API.Controllers
{
    public record UserSettingsDto(
        string DisplayName, 
        bool ShowTopics, 
        bool ShowComments, 
        string? ProfilePicture);
}
