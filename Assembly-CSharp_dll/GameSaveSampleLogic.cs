using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xbox;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000037 RID: 55
public class GameSaveSampleLogic : MonoBehaviour
{
	// Token: 0x060000DF RID: 223 RVA: 0x000098F9 File Offset: 0x00007AF9
	private void Start()
	{
		this.playerSaveData = new GameSaveSampleLogic.PlayerSaveData();
		this.playerSaveData.name = "Jane Doe";
		this.playerSaveData.level = 2;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00009924 File Offset: 0x00007B24
	public void Save()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, this.playerSaveData);
			Gdk.Helpers.Save(memoryStream.ToArray());
			this.output.text = string.Concat(new object[]
			{
				"\n Saved game data:\n Name: ",
				this.playerSaveData.name,
				"\n Level: ",
				this.playerSaveData.level
			});
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x000099C0 File Offset: 0x00007BC0
	public void Load()
	{
		Gdk.Helpers.OnGameSaveLoaded += this.OnGameSaveLoaded;
		Gdk.Helpers.LoadSaveData();
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000099E4 File Offset: 0x00007BE4
	private void OnGameSaveLoaded(object sender, GameSaveLoadedArgs saveData)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (MemoryStream memoryStream = new MemoryStream(saveData.Data))
		{
			object obj = binaryFormatter.Deserialize(memoryStream);
			this.playerSaveData = (obj as GameSaveSampleLogic.PlayerSaveData);
			this.output.text = string.Concat(new object[]
			{
				"\n Loaded save game:\n Name: ",
				this.playerSaveData.name,
				"\n Level: ",
				this.playerSaveData.level
			});
		}
	}

	// Token: 0x0400014A RID: 330
	public Text output;

	// Token: 0x0400014B RID: 331
	private GameSaveSampleLogic.PlayerSaveData playerSaveData;

	// Token: 0x02000038 RID: 56
	[Serializable]
	private class PlayerSaveData
	{
		// Token: 0x0400014C RID: 332
		public string name;

		// Token: 0x0400014D RID: 333
		public int level;
	}
}
