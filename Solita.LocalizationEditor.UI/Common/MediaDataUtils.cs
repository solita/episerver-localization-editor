using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Solita.LocalizationEditor.UI.Common
{
    public static class MediaDataUtils
    {
        public static ContentAssetFolder GetContentAssetFolder(ContentReference contentLink)
        {
            var helper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            return helper.GetOrCreateAssetFolder(contentLink);
        }

        public static MediaData CreateFile(ContentReference parent, string filename)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var mediaDataResolver = ServiceLocator.Current.GetInstance<ContentMediaResolver>();

            // Get a suitable MediaData type from extension
            var extension = "." + filename.Split('.').Last();
            var mediaType = mediaDataResolver.GetFirstMatching(extension);
            var contentType = contentTypeRepository.Load(mediaType);

            //Get a new empty file data
            var media = contentRepository.GetDefault<MediaData>(parent, contentType.ID);
            media.Name = filename;
            var reference = contentRepository.Save(media, SaveAction.Publish, AccessLevel.NoAccess);

            return contentRepository.Get<MediaData>(reference);
        }

        

        public static IEnumerable<T> GetContentAssetFiles<T>(Guid contentAssetsID) where T : MediaData
        {
            if (contentAssetsID == Guid.Empty)
            {
                return Enumerable.Empty<T>();
            }

            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentAssets = contentRepository.Get<ContentAssetFolder>(contentAssetsID);
            return contentRepository.GetChildren<T>(contentAssets.ContentLink, LanguageSelector.AutoDetect(true));
        }

        public static ContentFolder GetOrCreateFolder(ContentReference parentFolder, string folderName)
        {
            //var languageSelector = new LanguageSelector(languageId);
            var contentRepo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var storedFolder = contentRepo.GetBySegment(parentFolder, folderName, CultureInfo.InvariantCulture) as ContentFolder;
            if (storedFolder != null)
            {
                return storedFolder;
            }
            storedFolder = contentRepo.GetDefault<ContentFolder>(parentFolder, CultureInfo.InvariantCulture);
            storedFolder.Name = folderName;
            contentRepo.Save(storedFolder, SaveAction.Publish, AccessLevel.NoAccess);

            return storedFolder;
        }

        //public static T GetOrCreateFile<T>(string filepath) where T : MediaData
        //{
        //    var file = FindFile<T>(filepath);

        //    if (file != null)
        //    {
        //        return file;
        //    }

        //    var folderPath = ParseParentFolderPath(filepath);
        //    var folder = GetOrCreateFolder(folderPath);

        //    var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        //    file = contentRepository.GetDefault<T>(folder.ContentLink);
        //    file.Name = ParseFilename(filepath);
        //    var reference = contentRepository.Save(file, SaveAction.Publish, AccessLevel.NoAccess);

        //    return contentRepository.Get<T>(reference);
        //}

        //public static T FindFile<T>(string filepath) where T : MediaData
        //{
        //    var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
        //    return urlResolver.Route(new UrlBuilder(filepath)) as T;
        //}

        //public static ContentFolder GetOrCreateFolder(string directoryPath)
        //{
        //    if (string.IsNullOrWhiteSpace(directoryPath))
        //    {
        //        throw new ArgumentException("Invalid directoryPath");
        //    }

        //    var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
        //    var folder = urlResolver.Route(new UrlBuilder(directoryPath)) as ContentFolder;

        //    if (folder != null)
        //    {
        //        return folder;
        //    }

        //    var parentFolderPath = ParseParentFolderPath(directoryPath);
        //    var parentFolder = GetOrCreateFolder(parentFolderPath);

        //    var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        //    folder = contentRepository.GetDefault<ContentFolder>(parentFolder.ContentLink);
        //    folder.Name = ParseFolderName(directoryPath);
        //    var reference = contentRepository.Save(folder, SaveAction.Publish, AccessLevel.NoAccess);

        //    return contentRepository.Get<ContentFolder>(reference);
        //}

        private static string ParseParentFolderPath(string directoryPath)
        {
            var parentFolderIndex = directoryPath.TrimEnd(new[] { '/' }).LastIndexOf('/');
            return (parentFolderIndex < 0) ? string.Empty : directoryPath.Substring(0, parentFolderIndex);
        }

        private static string ParseFolderName(string directoryPath)
        {
            return directoryPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string ParseFilename(string filevirtualpath)
        {
            var segments = filevirtualpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return segments.Last();
        }
    }
}