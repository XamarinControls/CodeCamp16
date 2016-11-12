using System;
using System.Collections.Generic;

namespace sqlitetest1.Interfaces
{
	public interface IDirectory
	{
		Folders RootDirectory { get; set; }

		void SetPath(string path);

		string Name { get; }
		string FullName { get; }
		bool Exists { get; }

		IDirectory Root { get; }
		IDirectory Parent { get; }

		DateTime CreationTime { get; }
		DateTime LastAccessTime { get; }
		DateTime LastWriteTime { get; }

		void Create();
		void MoveTo(string path);
		void Delete(bool recursive = false);

		bool FileExists(string fileName);
		IFile CreateFile(string name);
		IDirectory CreateSubdirectory(string name);
		IEnumerable<IDirectory> Directories { get; }
		IEnumerable<IFile> Files { get; }

		/// <summary>
		/// Copy the file to the path
		/// </summary>
		/// <returns>file copy paths</returns>
		/// <param name="folder">Folder</param>
		/// <param name="path">Path</param>
		List<string> CopyTo(Folders folder, string path);
	}
}
