using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class AssetStorageProvider : StorageProvider
{
	// Token: 0x060004C4 RID: 1220 RVA: 0x0001E81E File Offset: 0x0001CA1E
	public AssetStorageProvider(string directory)
	{
		this.directory = directory;
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00003296 File Offset: 0x00001496
	public void Initialize()
	{
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool IsInitialized()
	{
		return true;
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0001E82D File Offset: 0x0001CA2D
	public List<string> GetAvailableGames()
	{
		return (from r in Resources.LoadAll<TextAsset>(this.directory)
		select r.name).ToList<string>();
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001E863 File Offset: 0x0001CA63
	public bool HasGameData(string name)
	{
		return this.LoadAsset(name) != null;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001E874 File Offset: 0x0001CA74
	public void GetGameData(string name, MemoryStream stream)
	{
		TextAsset textAsset = this.LoadAsset(name);
		stream.Write(textAsset.bytes, 0, textAsset.bytes.Length);
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001E89E File Offset: 0x0001CA9E
	public string GetGameId(string name)
	{
		return string.Empty;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreGameData(string gameId, string gameName, string name, MemoryStream stream)
	{
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00003296 File Offset: 0x00001496
	public void DeleteGameData(string name)
	{
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00003296 File Offset: 0x00001496
	public void DeleteGamesData(List<string> name)
	{
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool HasProfile()
	{
		return false;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00003296 File Offset: 0x00001496
	public void GetProfileData(MemoryStream stream)
	{
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreProfileData(MemoryStream stream)
	{
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool HasSettings()
	{
		return false;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00003296 File Offset: 0x00001496
	public void GetSettingsData(MemoryStream stream)
	{
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreSettingsData(MemoryStream stream)
	{
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00003296 File Offset: 0x00001496
	public void Flush()
	{
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0001E8A8 File Offset: 0x0001CAA8
	private TextAsset LoadAsset(string name)
	{
		return Resources.Load<TextAsset>(string.Format("{0}/{1}", this.directory, name));
	}

	// Token: 0x04000505 RID: 1285
	private string directory;
}
