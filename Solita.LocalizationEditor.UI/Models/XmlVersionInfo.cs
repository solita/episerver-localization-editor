using System;

namespace Solita.LocalizationEditor.UI.Models
{
    public class XmlVersionInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public XmlVersionInfo(string id, string name, DateTime created, string createdBy)
        {
            Id = id;
            Name = name;
            Created = created;
            CreatedBy = createdBy;
        }
    }
}