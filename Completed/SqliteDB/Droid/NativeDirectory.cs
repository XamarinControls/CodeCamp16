using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using sqlitetest1;
using sqlitetest1.Interfaces;
namespace sqlitetest1.Droid
{
	public class NativeDirectory : IDirectory
	{
		private DirectoryInfo _DirInfo;
		private IEnumerable<IDirectory> _Directories;
		private IEnumerable<IFile> _Files;
		private IDirectory _Root;
		private IDirectory _ParentDirectory;

		public NativeDirectory()
		{
			
		}

		public Folders RootDirectory { get; set; } = Folders.Undefined;

		protected internal static Environment.SpecialFolder SpecialFolderTranslation(Folders folder)
		{
			switch (folder)
			{
				case Folders.ApplicationData:
					return Environment.SpecialFolder.ApplicationData;
				case Folders.CommonApplicationData:
					return Environment.SpecialFolder.CommonApplicationData;
				case Folders.CommonTemplates:
					return Environment.SpecialFolder.CommonTemplates;
				case Folders.Desktop:
					return Environment.SpecialFolder.Desktop;
				case Folders.DesktopDirectory:
					return Environment.SpecialFolder.DesktopDirectory;
				case Folders.Fonts:
					return Environment.SpecialFolder.Fonts;
				case Folders.LocalApplicationData:
					return Environment.SpecialFolder.LocalApplicationData;
				case Folders.MyDocuments:
					return Environment.SpecialFolder.MyDocuments;
				case Folders.MyMusic:
					return Environment.SpecialFolder.MyMusic;
				case Folders.MyPictures:
					return Environment.SpecialFolder.MyPictures;
				case Folders.MyVideos:
					return Environment.SpecialFolder.MyVideos;
				case Folders.Templates:
					return Environment.SpecialFolder.Templates;
				case Folders.UserProfile:
					return Environment.SpecialFolder.UserProfile;
				default:
					return Environment.SpecialFolder.MyDocuments;
			}
		}

		protected internal static string ParseFolders(Folders folder, string inPath)
		{
			string path1 = "";

			if (folder == Folders.External)
			{
				path1 = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			}
			else if (folder == Folders.CacheFolder)
			{
				path1 = GlobalSettings.GetContext.CacheDir.Path;
			}
			else
			{
				path1 = System.Environment.GetFolderPath(SpecialFolderTranslation(folder), Environment.SpecialFolderOption.Create);
			}

			if (String.IsNullOrEmpty(inPath) == false)
			{
				return Path.Combine(path1, inPath);
			}

			return path1;
		}

		public NativeDirectory(string path)
		{
			this._DirInfo = new DirectoryInfo(ParseFolders(RootDirectory, path));
			//System.Diagnostics.Debug.WriteLine($"NativeDirectory:= {this._DirInfo.FullName}");
		}

		public NativeDirectory(DirectoryInfo info)
		{
			this._DirInfo = info;
		}

		public void SetPath(string path)
		{
			this._DirInfo = new DirectoryInfo(ParseFolders(RootDirectory, path));
			//System.Diagnostics.Debug.WriteLine($"NativeDirectory:= {this._DirInfo.FullName}");
		}

		#region IDirectory Members

		public string Name
		{
			get { return this._DirInfo.Name; }
		}


		public string FullName
		{
			get { return this._DirInfo.FullName; }
		}


		public bool Exists
		{
			get { return this._DirInfo.Exists; }
		}


		public IDirectory Root
		{
			get
			{
				this._Root = this._Root ?? new NativeDirectory(this._DirInfo.Root);
				return this._Root;
			}
		}

		public IDirectory Parent
		{
			get
			{
				this._ParentDirectory = this._ParentDirectory ?? new NativeDirectory(this._DirInfo.Parent);
				return this._ParentDirectory;
			}
		}


		public DateTime CreationTime
		{
			get { return this._DirInfo.CreationTime; }
		}


		public DateTime LastAccessTime
		{
			get { return this._DirInfo.LastAccessTime; }
		}


		public DateTime LastWriteTime
		{
			get { return this._DirInfo.LastWriteTime; }
		}


		public void Create()
		{
			if (this._DirInfo.Exists == false)
			{
				this._DirInfo.Create();
			}
		}


		public void MoveTo(string path)
		{
			this._DirInfo.MoveTo(path);
		}

		public List<string> CopyTo(Folders folder, string path)
		{
			var copyPath = ParseFolders(folder, path);
			var obj = new List<string>();
			foreach (var file in this._DirInfo.GetFiles())
			{
				var temp = new FileInfo(Path.Combine(copyPath, file.Name));
				if (temp.Directory.Exists == false)
				{
					temp.Directory.Create();
				}
				file.CopyTo(temp.FullName, true);
				obj.Add(temp.FullName);
			}
			return obj;
		}

		public bool FileExists(string fileName)
		{
			var path = Path.Combine(this.FullName, fileName);
			return System.IO.File.Exists(path);
		}


		public IFile CreateFile(string fileName)
		{
			var path = Path.Combine(this.FullName, fileName);
			return new NativeFile(new FileInfo(path));
		}


		public IDirectory CreateSubdirectory(string path)
		{
			var dir = this._DirInfo.CreateSubdirectory(path);
			return new NativeDirectory(dir);
		}


		public void Delete(bool recursive = false)
		{
			if (this._DirInfo.Exists)
			{
				this._DirInfo.Delete(recursive);
			}
		}


		public IEnumerable<IDirectory> Directories
		{
			get
			{
				this._Directories = this._Directories ?? this._DirInfo.GetDirectories().Select(x => new NativeDirectory(x)).ToList();
				return this._Directories;
			}
		}

		public IEnumerable<IFile> Files
		{
			get
			{
				this._Files = this._Files ?? this._DirInfo.GetFiles().Select(x => new NativeFile(x)).ToList();
				return this._Files;
			}
		}

		#endregion
	}
}
	