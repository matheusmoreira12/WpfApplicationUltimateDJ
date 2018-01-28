using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UltimateDJ.CompressedResources.Markup
{
    public sealed class CompressedResource : DependencyObject
    {
        #region Markup Properties
        public DependencyProperty ResourceNameProperty = DependencyProperty.RegisterAttached("ResourceName", typeof(string),
            typeof(CompressedResource));

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public string ResourceName
        {
            get { return (string)GetValue(ResourceNameProperty); }
            set { SetValue(ResourceNameProperty, value); }
        }
        #endregion

        internal ResourceExtractor AssociatedExtractor { get; private set; }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ResourceNameProperty)
            {
                AssociatedExtractor = new ResourceExtractor(ResourceName);
                AssociatedExtractor.Execute();
            }
        }
    }

    public sealed class ExtractedImage : Image
    {
        #region Markup Properties
        public DependencyProperty CompressedResourceProperty = DependencyProperty.RegisterAttached("ResourceName", 
            typeof(CompressedResource), typeof(ExtractedImage));

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public CompressedResource CompressedResource
        {
            get { return (CompressedResource)GetValue(CompressedResourceProperty); }
            set { SetValue(CompressedResourceProperty, value); }
        }

        public DependencyProperty ExtractedFilePathProperty = DependencyProperty.RegisterAttached("ExtractedFilePath", typeof(string),
            typeof(ResourceExtractor));

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public string ExtractedFilePath
        {
            get { return (string)GetValue(ExtractedFilePathProperty); }
            set { SetValue(ExtractedFilePathProperty, value); }
        }
        #endregion

        internal ResourceExtractor AssociatedExtractor { get; private set; }

        internal AwaitingHook ExtractorHook { get; private set; }

        private void updateSource()
        {
            BitmapImage imgSrc = new BitmapImage(ResourceExtractor.GetRelativeUri(AssociatedExtractor, ExtractedFilePath));
            Source = imgSrc;

            InvalidateVisual();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (ExtractedFilePath == null)
                throw new InvalidOperationException("ExtractedFilePath is a required property and cannot be null.");
            if (CompressedResource == null)
                throw new InvalidOperationException("CompressedResource is a required property and cannot be null.");

            AssociatedExtractor = CompressedResource.AssociatedExtractor;

            updateSource();
        }
    }
}