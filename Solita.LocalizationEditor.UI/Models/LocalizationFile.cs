using System;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace Solita.LocalizationEditor.UI.Models
{
    [ContentType(
        GUID = "0E99DF44-8473-45AB-989B-6EF54ADF1D10",
        DisplayName = "LocalizationResult file"
        )]
    [Obsolete("To be removed")]
    [MediaDescriptor(ExtensionString = "localizationFile")]
    public class LocalizationFile : MediaData
    {
        
    }
}