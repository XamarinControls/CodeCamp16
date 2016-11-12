using System;
using System.IO;
using sqlitetest1.Interfaces;

namespace sqlitetest1.iOS
{
	public class NativeFile : IFile
	{
		private FileInfo _FileInfo;
		private NativeDirectory _NativeDirectory;
		private string _MimeType;

		public NativeFile()
		{

		}

		public Folders RootDirectory { get; set; }

		public NativeFile(string fileName) : this(new FileInfo(fileName)) { }

		public NativeFile(FileInfo info)
		{
			this._FileInfo = info;
		}

		#region IFile Members

		public void SetFile(string fileName)
		{
			var path1 = "";
			if (RootDirectory != Folders.Undefined)
			{
				path1 = System.Environment.GetFolderPath(NativeDirectory.SpecialFolderTranslation(RootDirectory), Environment.SpecialFolderOption.Create);
			}

			this._FileInfo = new FileInfo(Path.Combine(path1, fileName));
			//System.Diagnostics.Debug.WriteLine($"SetFile Path:= {this._FileInfo.FullName}");
		}

		public string Name
		{
			get { return this._FileInfo.Name; }
		}


		public string FullName
		{
			get { return this._FileInfo.FullName; }
		}


		public string Extension
		{
			get { return this._FileInfo.Extension; }
		}

		public string MimeType
		{
			get
			{
				this._MimeType = this._MimeType ?? GetMimeType();
				return this._MimeType;
			}
		}


		public long Length
		{
			get { return this._FileInfo.Length; }
		}


		public bool Exists
		{
			get { return this._FileInfo.Exists; }
		}


		public StreamWriter Create()
		{
			Stream stream = this._FileInfo.Create();
			stream.Seek(0, SeekOrigin.End);
			//System.Diagnostics.Debug.WriteLine($"File Path:= {this._FileInfo.FullName}");
			return new StreamWriter(stream);
		}


		public StreamReader OpenRead()
		{
			if (this.Exists)
			{
				return new StreamReader(this._FileInfo.OpenRead());
			}
			else {
				throw new FileNotFoundException(this._FileInfo.Name);
			}
		}


		public StreamWriter OpenWrite()
		{
			if (this.Exists == true)
			{
				//System.Diagnostics.Debug.WriteLine($"File Path:= {this._FileInfo.FullName}");
				Stream stream = _FileInfo.OpenWrite();
				stream.Seek(0, SeekOrigin.End);
				return new StreamWriter(stream);
			}
			else {
				return this.Create();
			}
		}


		public void MoveTo(string path)
		{
			if (this.Exists)
			{
				this._FileInfo.MoveTo(path);
			}
		}


		public IFile CopyTo(string path)
		{
			if (this.Exists)
			{
				var file = this._FileInfo.CopyTo(path);
				return new NativeFile(file);
			}
			return null;
		}


		public void Delete()
		{
			if (this.Exists)
			{
				this._FileInfo.Delete();
			}
		}

		public IDirectory Directory
		{
			get
			{
				this._NativeDirectory = this._NativeDirectory ?? new NativeDirectory(this._FileInfo.Directory);
				return this._NativeDirectory;
			}
		}


		public DateTime LastAccessTime
		{
			get
			{
				if (this.Exists)
				{
					return this._FileInfo.LastAccessTime;
				}
				return DateTime.MinValue;
			}
		}


		public DateTime LastWriteTime
		{
			get
			{
				if (this.Exists)
				{
					return this._FileInfo.LastWriteTime;
				}
				return DateTime.MinValue;
			}
		}


		public DateTime CreationTime
		{
			get
			{
				if (this.Exists)
				{
					return this._FileInfo.CreationTime;
				}
				return DateTime.MinValue;
			}
		}

		#endregion


		private string GetMimeType()
		{
			return String.Empty;
		}
	}
}