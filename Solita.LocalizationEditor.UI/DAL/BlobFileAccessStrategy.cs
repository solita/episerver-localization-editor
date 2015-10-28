using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Solita.LocalizationEditor.UI.Models;
using EPiServer.Web;
using EPiServer.Logging.Compatibility;
using System.Xml.Linq;
using EPiServer.Security;
using Solita.LocalizationEditor.UI.Common;
using System;

namespace Solita.LocalizationEditor.UI.DAL
{
    public class BlobFileAccessStrategy : FileAccessStrategy
    {
        private const string LocalizationFileName = "Localizations.xml";
        private IContentRepository _contentRepository;
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlobFileAccessStrategy));
        private static ContentReference LocalizationsParent = ContentReference.RootPage;

        IContentRepository ContentRepo
        {
            get
            {
                if (_contentRepository == null)
                {
                    _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                }
                return _contentRepository;
            }
        }

        public ContentReference LocalizationStorageReference
        {
            get
            {
                var assetsRoot = SiteDefinition.Current.GlobalAssetsRoot;
                var contentFolder = MediaDataUtils.GetOrCreateFolder(assetsRoot, "Localizations");

                return contentFolder.ContentLink;
            }
        }
        public BlobFileAccessStrategy()
        {

        }
        public BlobFileAccessStrategy(IContentRepository contentRepo)
        {
            _contentRepository = contentRepo;
        }

        public override IList<XmlVersionInfo> GetTranslationFileVersions()
        {
            return (from mediaFile in GetLocalizationFiles()
                    select new XmlVersionInfo(mediaFile.BinaryData.ID.ToString(), mediaFile.Name, mediaFile.Created, mediaFile.CreatedBy)
                    ).ToList();
        }

        public override XmlDocument LoadXml()
        {
            var file = GetLocalizationFile();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = LoadBinaryDataToXml(file.BinaryData);

            return xmlDoc;
        }

        public override XmlDocument LoadVersion(string version)
        {
            var localizationFile = GetLocalizationFiles()
                .Where(f => f.BinaryData != null &&
                            f.BinaryData.ID != null &&
                            f.BinaryData.ID.AbsoluteUri.Contains(version))
                            .SingleOrDefault();

            if (localizationFile == null)
            {
                throw new FileNotFoundException($"Could not find file with url: '{version}'");
            }

            return LoadBinaryDataToXml(localizationFile.BinaryData);
        }

        public IEnumerable<MediaData> GetLocalizationFiles()
        {
            var localizationFileType = typeof(LocalizationFile).Name;

            var file = GetLocalizationContentInstance();
            var files = ContentRepo.GetChildren<LocalizationFile>(LocalizationStorageReference)
                            .Where(f => f.BinaryData != null)
                            .OrderByDescending(f => f.Created);
            return files;
        }

        public MediaData GetLocalizationFile()
        {
            var file = GetLocalizationFiles().FirstOrDefault();

            return file ?? ContentRepo.GetDefault<LocalizationFile>(LocalizationStorageReference);
        }

        public override void SaveXml(XmlDocument xml)
        {
            var blobFactory = ServiceLocator.Current.GetInstance<BlobFactory>();
            var file = GetLocalizationContentInstance();
            var blob = blobFactory.CreateBlob(file.BinaryDataContainer, ".xml");
            using (var blobStream = blob.OpenWrite())
            {
                StreamWriter streamWriter = new StreamWriter(blobStream);
                XDocument xDoc = XDocument.Parse(xml.OuterXml);
                streamWriter.Write(xDoc.ToString());
                streamWriter.Flush();
            }
            file.BinaryData = blob;
            ContentRepo.Save(file, SaveAction.Publish);
        }

        private LocalizationFile GetLocalizationContentInstance()
        {
            var file = ContentRepo.GetDefault<LocalizationFile>(LocalizationStorageReference);
            file.Name = "Localizations";

            return file;
        }

        private XmlDocument LoadBinaryDataToXml(Blob blob)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (blob == null)
            {
                return xmlDoc;
            }
            try
            {
                using (var stream = blob.OpenRead())
                {
                    xmlDoc.Load(stream);
                    stream.Flush();
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                //physical xml translations file is not present anymore, so return blank one instead.
                Log.Error(ex.Message);
            }
            return xmlDoc;
        }

    }
}