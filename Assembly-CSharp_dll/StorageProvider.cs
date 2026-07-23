using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200051E RID: 1310
public interface StorageProvider
{
	// Token: 0x06001B4F RID: 6991
	void Initialize();

	// Token: 0x06001B50 RID: 6992
	bool IsInitialized();

	// Token: 0x06001B51 RID: 6993
	void StoreGameData(string gameId, string gameName, string name, MemoryStream dataStream);

	// Token: 0x06001B52 RID: 6994
	string GetGameId(string name);

	// Token: 0x06001B53 RID: 6995
	void GetGameData(string name, MemoryStream dataStream);

	// Token: 0x06001B54 RID: 6996
	List<string> GetAvailableGames();

	// Token: 0x06001B55 RID: 6997
	bool HasGameData(string name);

	// Token: 0x06001B56 RID: 6998
	void DeleteGameData(string name);

	// Token: 0x06001B57 RID: 6999
	void DeleteGamesData(List<string> name);

	// Token: 0x06001B58 RID: 7000
	void Flush();

	// Token: 0x06001B59 RID: 7001
	bool HasProfile();

	// Token: 0x06001B5A RID: 7002
	void GetProfileData(MemoryStream dataStream);

	// Token: 0x06001B5B RID: 7003
	void StoreProfileData(MemoryStream dataStream);

	// Token: 0x06001B5C RID: 7004
	bool HasSettings();

	// Token: 0x06001B5D RID: 7005
	void GetSettingsData(MemoryStream dataStream);

	// Token: 0x06001B5E RID: 7006
	void StoreSettingsData(MemoryStream dataStream);
}
