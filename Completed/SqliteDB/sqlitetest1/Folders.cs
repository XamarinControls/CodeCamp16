using System;

namespace sqlitetest1
{
	public enum Folders
	{
		/// <summary>
		/// default. none selected
		/// </summary>
		Undefined,
		/// <summary>
		/// <para>iOS: /Documents/Desktop</para>
		/// <para>Android: /data/data/{package name}/files/Desktop</para>
		/// </summary>
		Desktop,
		/// <summary>
		/// <para>iOS: /Documents</para>
		/// <para>Android: /data/data/{package name}/files</para>
		/// </summary>
		MyDocuments,
		/// <summary>
		/// <para>iOS: /Library/Favorites</para>
		/// </summary>
		Favorites,
		/// <summary>
		/// <para>iOS: /Documents/Music</para>
		/// <para>Android: /data/data/{package name}/files/Music</para>
		/// </summary>
		MyMusic,
		/// <summary>
		/// <para>iOS: /Documents/Videos</para>
		/// <para>Android: /data/data/{package name}/files/Videos</para>
		/// </summary>
		MyVideos,
		/// <summary>
		/// <para>iOS: /Documents/Desktop</para>
		/// <para>Android: /data/data/{package name}/files/Desktop</para>
		/// </summary>
		DesktopDirectory,
		/// <summary>
		/// <para>iOS: /Documents/.fonts</para>
		/// <para>Android: /data/data/{package name}/files/.fonts</para>
		/// </summary>
		Fonts,
		/// <summary>
		/// <para>iOS: Documents/Templates</para>
		/// <para>Android: /data/data/{package name}/files/Templates</para> 
		/// </summary>            
		Templates,
		/// <summary>
		/// <para>iOS: /Documents/.config</para>
		/// <para>Android: /data/data/{package name}/files/.config</para>
		/// </summary>
		ApplicationData,
		/// <summary>
		/// <para>iOS: /Documents</para>
		/// <para>Android: /data/data/{package name}/files/.local/share</para>
		/// </summary>
		LocalApplicationData,
		/// <summary>
		/// <para>iOS: /Library/Caches</para>
		/// </summary>
		InternetCache,
		/// <summary>
		/// <para>iOS: /usr/share</para>
		/// <para>Android: /usr/share</para>
		/// </summary>
		CommonApplicationData,
		/// <summary>
		/// <para>iOS: /Applications</para>
		/// </summary>
		ProgramFiles,
		/// <summary>
		/// <para>iOS: Documents/Pictures</para>
		/// <para>Android: /data/data/{package name}/files/Pictures</para>
		/// </summary>
		MyPictures,
		/// <summary>
		/// <para>iOS: /usr/share/templates</para>
		/// <para>Android: /usr/share/templates</para>
		/// </summary>
		CommonTemplates,
		/// <summary>
		/// <para>iOS: /Library</para>
		/// </summary>
		Resources,
		/// <summary>
		/// <para>Android: /data/data/{package name}/files</para>
		/// </summary>
		UserProfile,
		/// <summary>
		/// <para>Android: external storage folder</para>
		/// </summary>
		External,
		/// <summary>
		/// <para>Android: Cache folder</para>
		/// </summary>
		CacheFolder,
	}
}

