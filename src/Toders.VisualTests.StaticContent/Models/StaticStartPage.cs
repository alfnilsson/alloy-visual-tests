using AlloyTemplates.Models.Pages;
using EPiServer.DataAnnotations;

namespace Toders.VisualTests.StaticContent.Models
{
    [ContentType(
        DisplayName = "Visual Tests Start Page",
        GUID = "c9d2704d-ea9c-44a6-802e-e9ec739faa77",
        Description = "")]
    public class StaticStartPage : StartPage
    {
    }
}
