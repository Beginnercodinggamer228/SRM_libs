using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020001E3 RID: 483
public class EmptyStorageProvider : StorageProvider, IDisposable
{
	// Token: 0x06000A0C RID: 2572 RVA: 0x00003296 File Offset: 0x00001496
	public void DeleteGameData(string name)
	{
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00003296 File Offset: 0x00001496
	public void DeleteGamesData(List<string> names)
	{
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00003296 File Offset: 0x00001496
	public void Dispose()
	{
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0002C2F6 File Offset: 0x0002A4F6
	public List<string> GetAvailableGames()
	{
		return new List<string>();
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0001E89E File Offset: 0x0001CA9E
	public string GetGameId(string name)
	{
		return string.Empty;
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00003296 File Offset: 0x00001496
	public void GetGameData(string name, MemoryStream dataStream)
	{
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00003296 File Offset: 0x00001496
	public void GetProfileData(MemoryStream dataStream)
	{
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00003296 File Offset: 0x00001496
	public void GetSettingsData(MemoryStream dataStream)
	{
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool HasGameData(string name)
	{
		return false;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool HasProfile()
	{
		return false;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool HasSettings()
	{
		return false;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00003296 File Offset: 0x00001496
	public void Initialize()
	{
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool IsInitialized()
	{
		return true;
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreGameData(string gameId, string gameName, string name, MemoryStream dataStream)
	{
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreProfileData(MemoryStream dataStream)
	{
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00003296 File Offset: 0x00001496
	public void StoreSettingsData(MemoryStream dataStream)
	{
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00003296 File Offset: 0x00001496
	public void Flush()
	{
	}
}
