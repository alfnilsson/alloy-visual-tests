using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Toders.VisualTests.StaticContent.Models
{
    [ContentType(
        DisplayName = "Visual Tests Assets Root Folder",
        GUID = "afd4d4a8-8dbf-4ff6-a20d-94bc949e4307",
        Description = "")]
    public class StaticAssetsRootFolder : ContentFolder
    {
    }
}