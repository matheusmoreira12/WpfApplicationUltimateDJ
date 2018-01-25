using System;
using System.IO;
using System.Windows;

using Ionic.Zip;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UltimateDJ.IconPackManagement
{
    static class Decompression
    {
        public static string DecompressStreamToTempDirectory(byte[] buffer)
        {
            using (var zipstream = new MemoryStream(buffer))
            {
                string outTempDirectoryPath = $@"{Path.GetTempPath()}{Guid.NewGuid().ToString()}\";
                Directory.CreateDirectory(outTempDirectoryPath);

                using (ZipFile zip = ZipFile.Read(zipstream))
                {
                    zip.ExtractAll(outTempDirectoryPath);

                    return outTempDirectoryPath;
                }
            }
        }
    }

    public class IconGlyph : Image
    {
        public static readonly DependencyProperty PackResourceNameProperty = DependencyProperty.RegisterAttached("PackResourceName",
            typeof(string), typeof(IconResourceManager));

        /// <summary>
        /// Gets or sets the icon package resource name.
        /// </summary>
        public string PackResourceName
        {
            get { return (string)GetValue(PackResourceNameProperty); }
            set { SetValue(PackResourceNameProperty, value); }
        }

        public static readonly DependencyProperty IconNameProperty = DependencyProperty.RegisterAttached("IconName",
            typeof(string), typeof(IconResourceManager));

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (PackResourceName == null)
                throw new Exception("PackResourceName is a required property and cannot be null.");
            if (IconName == null)
                throw new Exception("IconName is a required property and cannot be null.");

            string sourcePath = $"{IconResourceManager.GetFileDirectoryPath(IconName, PackResourceName)}{IconName}";

            BitmapImage image = new BitmapImage(new Uri(sourcePath));

            Source = image;
        }
    }

    public class IconPackResourceExtractor : IDisposable
    {
        public string ResourceName;
        public string OutputTemporaryDirectory;

        private void extractToTempDirectory()
        {
            if (ResourceName == null)
                return;

            byte[] resourceBuffer = null;

            try
            {
                resourceBuffer = (byte[])WpfApplicationUltimateDJ.Properties.Resources.ResourceManager.GetObject(ResourceName);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                    throw new Exception($"Icon resource \"{ResourceName}\" could not be found.", e);
                else if (e is FileNotFoundException)
                    throw new Exception($"Icon resource \"{ResourceName}\" could not be loaded.", e);
                else
                    throw e;
            }

            try
            {
                OutputTemporaryDirectory = Decompression.DecompressStreamToTempDirectory(resourceBuffer);
            }
            catch (Exception e)
            {
                throw new Exception($"Icon resource \"{ResourceName}\" could not be extracted because it does not" +
                    " represent a valid zip file.", e);
            }
        }

        private void emptyTempDirectory(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);

            foreach (var file in allFiles)
                File.Delete($@"{directory}\\{file}");
        }

        public void Dispose()
        {
            emptyTempDirectory(OutputTemporaryDirectory);
            Directory.Delete(OutputTemporaryDirectory);
        }

        public IconPackResourceExtractor(string resourceName)
        {
            ResourceName = resourceName;
            extractToTempDirectory();
        }
    }

    public class IconPackResourceExtractorCollection : List<IconPackResourceExtractor>
    {
        public IconPackResourceExtractor GetByResourceName(string resourceName)
        {
            foreach (var item in this)
                if (item.ResourceName == resourceName)
                    return item;

            return null;
        }
    }

    public static class IconResourceManager
    {
        private static IconPackResourceExtractorCollection extractors = new IconPackResourceExtractorCollection();

        internal static string GetFileDirectoryPath(string iconName, string resourceName)
        {
            var extractor = extractors.GetByResourceName(resourceName);

            if (extractor == null)
                extractors.Add(extractor = new IconPackResourceExtractor(resourceName));

            return extractor.OutputTemporaryDirectory;
        }
    }
}