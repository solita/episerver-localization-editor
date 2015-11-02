using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using System;

namespace Solita.LocalizationEditor.UI.Models
{
    [ContentType(GUID = "C50E6345-22FA-4EFF-A909-3FFFF7F3E199")]
    [MediaDescriptor(ExtensionString = "localizationdata")]
    public class LocalizationXml : ContentBase, IBinaryStorable
    {
        private Blob _blob;

        public virtual Blob BinaryData
        {
            get
            {
                return this._blob;
            }
            set
            {
                this._blob = value;
            }
        }

        public virtual Uri BinaryDataContainer { get; }
    }
}