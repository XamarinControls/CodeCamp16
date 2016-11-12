using System;
using System.IO;

namespace sqlitetest1.Interfaces
{
	public interface IFile
	{
		void SetFile(string fileName);
		string Name { get; }
		string FullName { get; }
		string Extension { get; }
		long Length { get; }
		bool Exists { get; }
		string MimeType { get; }

		StreamWriter Create();
		StreamReader OpenRead();
		StreamWriter OpenWrite();
		void MoveTo(string path);
		IFile CopyTo(string path);
		void Delete();

		IDirectory Directory { get; }
		DateTime LastAccessTime { get; }
		DateTime LastWriteTime { get; }
		DateTime CreationTime { get; }
	}
}
