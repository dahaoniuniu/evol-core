﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Evol.Util.Configuration
{
    public abstract class FileStrongConfigurationSource : IStrongConfigurationSource
    {
        public abstract Type StrongType { get; }

        public abstract IStrongConfigurationProvider Build(IStrongConfigurationBuilder builder);

		public IFileProvider FileProvider
        {
            get;
            set;
        }

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if loading the file is optional.
        /// </summary>
        public bool Optional
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the source will be loaded if the underlying file changes.
        /// </summary>
        public bool ReloadOnChange
        {
            get;
            set;
        }

        /// <summary>
        /// Number of milliseconds that reload will wait before calling Load.  This helps
        /// avoid triggering reload before a file is completely written. Default is 250.
        /// </summary>
        public int ReloadDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Will be called if an uncaught exception occurs in FileConfigurationProvider.Load.
        /// </summary>
        public Action<FileLoadExceptionContext> OnLoadException
        {
            get;
            set;
        }

        /// <summary>
        /// Builds the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        /// <returns>A <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /></returns>
        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);

        /// <summary>
        /// Called to use any default settings on the builder like the FileProvider or FileLoadExceptionHandler.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        public void EnsureDefaults(IStrongConfigurationBuilder builder)
        {
            this.FileProvider = (FileProvider ?? builder.GetFileProvider());
            this.OnLoadException = (OnLoadException ?? builder.GetFileLoadExceptionHandler());
        }

        /// <summary>
        /// If no file provider has been set, for absolute Path, this will creates a physical file provider 
        /// for the nearest existing directory.
        /// </summary>
        public void ResolveFileProvider()
        {
            if (this.FileProvider == null && !string.IsNullOrEmpty(this.Path) && System.IO.Path.IsPathRooted(this.Path))
            {
                string directoryName = System.IO.Path.GetDirectoryName(this.Path);
                string text = System.IO.Path.GetFileName(this.Path);
                while (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    text = System.IO.Path.Combine(System.IO.Path.GetFileName(directoryName), text);
                    directoryName = System.IO.Path.GetDirectoryName(directoryName);
                }
                if (Directory.Exists(directoryName))
                {
                    this.FileProvider = new PhysicalFileProvider(directoryName);
                    this.Path = text;
                }
            }
        }

        protected FileStrongConfigurationSource():base()
        {
            ReloadDelay = 250;
        }
    }
}