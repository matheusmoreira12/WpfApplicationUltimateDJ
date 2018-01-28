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
using System.Threading;
using System.Diagnostics;

namespace UltimateDJ.CompressedResources
{
    static class Utils
    {
        public static void EmptyUpTempDirectory(string directory)
        {
            string[] allFiles = Directory.GetFiles(directory);

            foreach (var file in allFiles)
                File.Delete($@"{directory}{file}");
        }

        public static string ExtractBufferToTempDirectory(byte[] buffer, ResourceExtractor extractor)
        {
            var outTempDirectory = $@"{Path.GetTempPath()}{Guid.NewGuid().ToString()}\";
            Directory.CreateDirectory(outTempDirectory);

            using (var zipstream = new MemoryStream(buffer))
            {
                using (ZipFile zip = ZipFile.Read(zipstream))
                {
                    zip.ExtractProgress += extractor.OnExtractProgress;
                    zip.ExtractAll(outTempDirectory);
                }
            }

            return outTempDirectory;
        }
    }


    public class AwaitingHook : IDisposable
    {
        public Action Callback { get; private set; }
        public int TimeoutMsecs { get; private set; }
        public Func<object, bool> Validator;
        public object Parameter;

        private Thread awaitThread;

        private void awaitCondition()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            awaitThread = new Thread(() =>
            {
                bool validated;

                while (!(validated = Validator(Parameter)))
                    if (watch.ElapsedMilliseconds > TimeoutMsecs)
                        break;

                if (validated)
                    Callback();
            });

            awaitThread.Start();
        }

        void IDisposable.Dispose()
        {
            if (awaitThread.IsAlive)
                awaitThread.Abort();
        }

        public AwaitingHook(Func<object, bool> validator, object param, Action callback, int timeoutMsecs)
        {
            Callback = callback;
            Parameter = param;
            Validator = validator;
            TimeoutMsecs = timeoutMsecs;

            awaitCondition();
        }
    }


    public class ResourceExtractorEventArgs
    {
        ResourceExtractor Extractor;

        public ResourceExtractorEventArgs(ResourceExtractor extractor)
        {
            Extractor = extractor;
        }
    }

    public delegate void ResourceExtractorEventHandler(object obj, ResourceExtractorEventArgs args);

    public sealed class ResourceExtractorErrorEventArgs : ResourceExtractorEventArgs
    {
        ResourceExtractor Extractor;
        Exception Exception;

        public ResourceExtractorErrorEventArgs(ResourceExtractor extractor, Exception exception) : base(extractor)
        {
            Extractor = extractor;
            Exception = exception;
        }
    }

    public delegate void ResourceExtractorErrorEventHandler(object obj, ResourceExtractorErrorEventArgs args);

    public sealed class ResourceExtractorProgressEventArgs : ResourceExtractorEventArgs
    {
        public static string GetMessage(ResourceExtractorProgressEventArgs args)
        {
            return String.Format("Extracting entry \"{0}\" from archive\"{1}\" to \"{2}\"... {5:0.0}% completed ({3} of {4} entries" +
                "extracted, {5} of {6} bytes transferred).", args.CurrentEntryFileName, args.ArchiveName, args.ExtractLocation,
                args.Progress, args.EntriesExtracted, args.EntriesTotal, args.BytesTransferred, args.BytesTotal);
        }

        public double Progress { get { return (EntriesExtracted + BytesTransferred / BytesTotal) / EntriesTotal * 100; } }

        public string ArchiveName { get; private set; }
        public long BytesTransferred { get; private set; }
        public long BytesTotal { get; private set; }
        public int EntriesExtracted { get; private set; }
        public int EntriesTotal { get; private set; }
        public string ExtractLocation { get; private set; }
        public string CurrentEntryFileName { get; private set; }

        public ResourceExtractor Extractor { get; private set; }
        public string Message { get { return GetMessage(this); } }

        public ResourceExtractorProgressEventArgs(ResourceExtractor extractor, ExtractProgressEventArgs baseEvt) : base(extractor)
        {
            ArchiveName = baseEvt.ArchiveName;
            BytesTransferred = baseEvt.BytesTransferred;
            BytesTotal = baseEvt.TotalBytesToTransfer;
            EntriesExtracted = baseEvt.EntriesExtracted;
            EntriesTotal = baseEvt.EntriesTotal;
            ExtractLocation = baseEvt.ExtractLocation;
            CurrentEntryFileName = baseEvt.CurrentEntry.FileName;
            Extractor = extractor;
        }
    }

    public delegate void ResourceExtractorProgressEventHandler(object obj, ResourceExtractorProgressEventArgs args);



    /// <summary>
    /// Extracts a zipped resource
    /// </summary>
    public sealed class ResourceExtractor : IDisposable
    {
        public static Uri GetRelativeUri(ResourceExtractor extractor, string filePath)
        {
            return new Uri($@"{extractor.OutputTemporaryDirectory}{filePath}");
        }

        /// <summary>
        /// Gets the name of the resource used by this extractor.
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Gets the generated temporary output directory
        /// </summary>
        public string OutputTemporaryDirectory { get; private set; }

        /// <summary>
        /// Indicates whether the extractor has finished loading.
        /// </summary>
        public bool HasFinished { get; private set; } = false;

        /// <summary>
        /// Indicates whether the extractor has completed extracting with no errors.
        /// </summary>
        public bool ExtractionSucceeded { get; private set; } = false;

        #region Resource Extractor Events
        public event ResourceExtractorErrorEventHandler Error;

        internal void OnError(ResourceExtractorErrorEventArgs args)
        {
            Error?.Invoke(this, args);
        }

        public event ResourceExtractorEventHandler Started;

        internal void OnStarted(ResourceExtractorEventArgs args)
        {
            Started?.Invoke(this, args);
        }

        public event ResourceExtractorEventHandler Finished;

        internal void OnFinished(ResourceExtractorEventArgs args)
        {
            Finished?.Invoke(this, args);
        }

        public event ResourceExtractorProgressEventHandler Progress;

        private void OnProgress(ResourceExtractorProgressEventArgs args)
        {
            Progress?.Invoke(this, args);
        }
        #endregion

        #region Resource Validation and Loading
        internal void OnExtractProgress(object obj, ExtractProgressEventArgs args)
        {
            OnProgress(new ResourceExtractorProgressEventArgs(this, args));
        }

        public void loadAndExtractResource()
        {
            if (ResourceName == null)
                throw new InvalidOperationException($"Property IconResource is required and cannot be null.");

            byte[] resBuffer = (byte[])WpfApplicationUltimateDJ.Properties.Resources.ResourceManager.GetObject(ResourceName);

            OutputTemporaryDirectory = Utils.ExtractBufferToTempDirectory(resBuffer, this);
        }

        internal void Execute()
        {
            OnStarted(new ResourceExtractorEventArgs(this));

            try
            {
                loadAndExtractResource();

                ExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                HasFinished = true;
                OnError(new ResourceExtractorErrorEventArgs(this, e));
            }

            HasFinished = true;
            OnFinished(new ResourceExtractorEventArgs(this));
        }
        #endregion

        void IDisposable.Dispose()
        {
            Utils.EmptyUpTempDirectory(OutputTemporaryDirectory);
            Directory.Delete(OutputTemporaryDirectory);
        }

        public ResourceExtractor(string resourceName)
        {
            ResourceName = resourceName;
        }
    }

    /*    public class IconGlyph : Image
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
        }*/
}