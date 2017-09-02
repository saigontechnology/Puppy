#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileWatcher.cs </Name>
//         <Created> 02/09/17 10:33:32 PM </Created>
//         <Key> f5134db0-9a76-4588-966f-401105e3a490 </Key>
//     </File>
//     <Summary>
//         FileWatcher.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.IO;

namespace Puppy.Core.FileUtils
{
    public class FileWatcher
    {
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        ///     Create new instance of file system watcher 
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="path" /> parameter contains invalid characters, is empty, or
        ///     contains only white spaces.
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     NoteIn the .NET for Windows Store apps or the Portable Class Library, catch the base
        ///     class exception, <see cref="T:System.IO.IOException" />, instead.The
        ///     <paramref name="path" /> parameter is longer than the system-defined maximum length.
        /// </exception>
        public FileWatcher(string path)
        {
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(path),
                IncludeSubdirectories = false
            };

            // Add Event
            _watcher.Changed += OnFileChanged;
        }

        public FileWatcher(string path, OnChangedEvent onChangedEvent)
        {
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(path),
                IncludeSubdirectories = false
            };

            // Add Event
            OnChanged = onChangedEvent;
            _watcher.Changed += OnFileChanged;
        }

        /// <summary>
        ///     Delegate for event on change 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">     </param>
        public delegate void OnChangedEvent(object source, FileSystemEventArgs e);

        /// <summary>
        ///     On file changed event 
        /// </summary>
        public OnChangedEvent OnChanged { get; set; }

        /// <summary>
        ///     Start watcher 
        /// </summary>
        /// <exception cref="FileNotFoundException">
        ///     The directory specified in <see cref="P:System.IO.FileSystemWatcher.Path" /> could
        ///     not be found.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="T:System.IO.FileSystemWatcher" /> object has been disposed.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     The current operating system is not Microsoft Windows NT or later.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <see cref="P:System.IO.FileSystemWatcher.Path" /> has not been set or is invalid.
        /// </exception>
        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     Stop watcher 
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="T:System.IO.FileSystemWatcher" /> object has been disposed.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     The directory specified in <see cref="P:System.IO.FileSystemWatcher.Path" /> could
        ///     not be found.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        ///     The current operating system is not Microsoft Windows NT or later.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <see cref="P:System.IO.FileSystemWatcher.Path" /> has not been set or is invalid.
        /// </exception>
        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        ///     On file changed event 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">     </param>
        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                _watcher.EnableRaisingEvents = false;

                // do delegate
                OnChanged?.Invoke(source, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}