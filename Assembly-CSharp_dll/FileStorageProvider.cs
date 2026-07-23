using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class FileStorageProvider : StorageProvider
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x0002C4C0 File Offset: 0x0002A6C0
	public void Initialize()
	{
		Directory.CreateDirectory(this.SavePath());
		try
		{
			this.MaybeMoveOldData();
		}
		catch (Exception ex)
		{
			Log.Debug("Attempted to move old data, failed.", new object[]
			{
				"Exception",
				ex
			});
		}
		this.isInitialized = true;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0002C518 File Offset: 0x0002A718
	public bool IsInitialized()
	{
		return this.isInitialized;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00003296 File Offset: 0x00001496
	private void MaybeMoveOldData()
	{
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0002C520 File Offset: 0x0002A720
	public void GetGameData(string name, MemoryStream dataStream)
	{
		string fullFilePath = this.GetFullFilePath(name);
		this.Load(fullFilePath, name, dataStream);
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0002C540 File Offset: 0x0002A740
	private void Load(string path, string name, MemoryStream loadInto)
	{
		if (File.Exists(path))
		{
			using (FileStream fileStream = File.Open(path, FileMode.Open))
			{
				this.CopyStream(fileStream, loadInto);
				return;
			}
		}
		throw new FileNotFoundException("No save file found", path);
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0002C590 File Offset: 0x0002A790
	private void CopyStream(Stream from, Stream to)
	{
		byte[] array = new byte[1024];
		int num;
		do
		{
			num = from.Read(array, 0, array.Length);
			to.Write(array, 0, num);
		}
		while (num >= array.Length);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0002C5C8 File Offset: 0x0002A7C8
	public List<string> GetAvailableGames()
	{
		string path = this.SavePath();
		if (!Directory.Exists(path))
		{
			return new List<string>();
		}
		return (from f in Directory.GetFiles(path, "*.sav")
		select Path.GetFileNameWithoutExtension(f)).ToList<string>();
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0001E89E File Offset: 0x0001CA9E
	public string GetGameId(string name)
	{
		return string.Empty;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0002C620 File Offset: 0x0002A820
	public void StoreGameData(string gameId, string gameName, string name, MemoryStream stream)
	{
		string fullFilePath = this.GetFullFilePath(name);
		string text = string.Format("{0}{1}", fullFilePath, ".tmp");
		using (FileStream fileStream = File.Create(text))
		{
			this.CopyStream(stream, fileStream);
		}
		File.Copy(text, fullFilePath, true);
		try
		{
			File.Delete(text);
		}
		catch (Exception ex)
		{
			Log.Warning("Failed to delete temporary save file.", new object[]
			{
				"temp file",
				text,
				"Exception",
				ex.Message
			});
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0002C6C0 File Offset: 0x0002A8C0
	public void DeleteGameData(string name)
	{
		string fullFilePath = this.GetFullFilePath(name);
		if (File.Exists(fullFilePath))
		{
			File.Delete(fullFilePath);
			return;
		}
		throw new FileNotFoundException("No file found to delete", fullFilePath);
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0002C6F0 File Offset: 0x0002A8F0
	public void DeleteGamesData(List<string> names)
	{
		foreach (string name in names)
		{
			this.DeleteGameData(name);
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0002C740 File Offset: 0x0002A940
	public bool HasGameData(string name)
	{
		return File.Exists(this.GetFullFilePath(name));
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0002C74E File Offset: 0x0002A94E
	private string GetFullFilePath(string name)
	{
		return Path.Combine(this.SavePath(), string.Format("{0}{1}", name, ".sav"));
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0002C76C File Offset: 0x0002A96C
	private string SavePath()
	{
		string text = Application.persistentDataPath;
		string text2 = "unity.Monomi Park.Slime Rancher";
		if (text.EndsWith(text2))
		{
			text = text.Replace(text2, Path.Combine("Monomi Park", "Slime Rancher"));
		}
		return text;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0002C7A6 File Offset: 0x0002A9A6
	public bool HasProfile()
	{
		return this.FileExists("slimerancher.prf");
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0002C7B3 File Offset: 0x0002A9B3
	public void GetProfileData(MemoryStream dataStream)
	{
		this.LoadDataStream("slimerancher.prf", dataStream);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0002C7C1 File Offset: 0x0002A9C1
	public void StoreProfileData(MemoryStream profileDataStream)
	{
		this.StoreDataStream("slimerancher.prf", profileDataStream);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0002C7CF File Offset: 0x0002A9CF
	public bool HasSettings()
	{
		return this.FileExists("slimerancher.cfg");
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0002C7DC File Offset: 0x0002A9DC
	public void GetSettingsData(MemoryStream dataStream)
	{
		this.LoadDataStream("slimerancher.cfg", dataStream);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0002C7EA File Offset: 0x0002A9EA
	public void StoreSettingsData(MemoryStream dataStream)
	{
		this.StoreDataStream("slimerancher.cfg", dataStream);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00003296 File Offset: 0x00001496
	public void Flush()
	{
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0002C7F8 File Offset: 0x0002A9F8
	private void LoadDataStream(string fileName, MemoryStream stream)
	{
		string text = this.ToPath(fileName);
		Log.Debug("Loading data from file.", new object[]
		{
			text
		});
		if (File.Exists(text))
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				this.CopyStream(fileStream, stream);
				return;
			}
		}
		Log.Warning("File not found", new object[]
		{
			"Path",
			text
		});
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0002C874 File Offset: 0x0002AA74
	private void StoreDataStream(string fileName, MemoryStream stream)
	{
		string text = this.ToPath(fileName);
		Log.Debug("Saving file.", new object[]
		{
			fileName
		});
		using (FileStream fileStream = File.Create(text))
		{
			try
			{
				this.CopyStream(stream, fileStream);
			}
			catch (Exception ex)
			{
				Log.Warning("Failed to save file.", new object[]
				{
					"Path",
					text,
					"Exception",
					ex.Message,
					"Stack Trace",
					ex.StackTrace
				});
			}
		}
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0002C918 File Offset: 0x0002AB18
	private bool FileExists(string fileName)
	{
		return File.Exists(this.ToPath(fileName));
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0002C926 File Offset: 0x0002AB26
	private string ToPath(string fileName)
	{
		return Path.Combine(this.SavePath(), fileName);
	}

	// Token: 0x04000858 RID: 2136
	private const string EXTENSION = ".sav";

	// Token: 0x04000859 RID: 2137
	private const string TEMP_EXTENSION = ".tmp";

	// Token: 0x0400085A RID: 2138
	private const string PROFILE_FILENAME = "slimerancher.prf";

	// Token: 0x0400085B RID: 2139
	private const string SETTINGS_FILENAME = "slimerancher.cfg";

	// Token: 0x0400085C RID: 2140
	private bool isInitialized;
}
