using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class GameData : Persistable
{
	// Token: 0x06000CD9 RID: 3289 RVA: 0x00034E48 File Offset: 0x00033048
	public GameData()
	{
		this.gameName = "";
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00034ED4 File Offset: 0x000330D4
	public GameData(string gameName)
	{
		if (gameName == null || gameName.Length <= 0)
		{
			throw new ArgumentException();
		}
		this.gameName = gameName;
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00034F6C File Offset: 0x0003316C
	private static BinaryFormatter CreateFormatter()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		SurrogateSelector surrogateSelector = new SurrogateSelector();
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
		binaryFormatter.SurrogateSelector = surrogateSelector;
		return binaryFormatter;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00034FAC File Offset: 0x000331AC
	public void Load(Stream stream)
	{
		BinaryFormatter binaryFormatter = GameData.CreateFormatter();
		int num = (int)binaryFormatter.Deserialize(stream);
		if (num > 3)
		{
			Debug.Log(string.Concat(new object[]
			{
				"File format newer than current version type=GameData fileVer=",
				num,
				" currVer=",
				3
			}));
			throw new VersionMismatchException("File format newer than current version.");
		}
		if (num < 3)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Unhandled version type=GameData fileVer=",
				num,
				" currVer=",
				3
			}));
			throw new VersionMismatchException("File format unhandled.");
		}
		this.world = DataModule<WorldData>.Deserialize(binaryFormatter, stream, 6);
		this.player = DataModule<PlayerData>.Deserialize(binaryFormatter, stream, 3);
		this.ranch = DataModule<RanchData>.Deserialize(binaryFormatter, stream, 3);
		this.actors = DataModule<ActorsData>.Deserialize(binaryFormatter, stream, 1);
		this.pedia = DataModule<PediaData>.Deserialize(binaryFormatter, stream, 1);
		this.achieve = DataModule<GameAchieveData>.Deserialize(binaryFormatter, stream, 1);
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x000350A2 File Offset: 0x000332A2
	public long Write(Stream stream)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x000350AC File Offset: 0x000332AC
	public static void AssertEquals(GameData dataA, GameData dataB)
	{
		WorldData.AssertEquals(dataA.world, dataB.world);
		PlayerData.AssertEquals(dataA.player, dataB.player);
		ActorsData.AssertEquals(dataA.actors, dataB.actors);
		RanchData.AssertEquals(dataA.ranch, dataB.ranch);
		PediaData.AssertEquals(dataA.pedia, dataB.pedia);
		GameAchieveData.AssertEquals(dataA.achieve, dataB.achieve);
	}

	// Token: 0x04000BB0 RID: 2992
	public string gameName;

	// Token: 0x04000BB1 RID: 2993
	public int worldFormatID = 6;

	// Token: 0x04000BB2 RID: 2994
	public int playerFormatID = 3;

	// Token: 0x04000BB3 RID: 2995
	public int ranchFormatID = 3;

	// Token: 0x04000BB4 RID: 2996
	public int actorsFormatID = 1;

	// Token: 0x04000BB5 RID: 2997
	public int pediaFormatID = 1;

	// Token: 0x04000BB6 RID: 2998
	public int achieveFormatID = 1;

	// Token: 0x04000BB7 RID: 2999
	public WorldData world = new WorldData();

	// Token: 0x04000BB8 RID: 3000
	public PlayerData player = new PlayerData();

	// Token: 0x04000BB9 RID: 3001
	public RanchData ranch = new RanchData();

	// Token: 0x04000BBA RID: 3002
	public ActorsData actors = new ActorsData();

	// Token: 0x04000BBB RID: 3003
	public PediaData pedia = new PediaData();

	// Token: 0x04000BBC RID: 3004
	public GameAchieveData achieve = new GameAchieveData();

	// Token: 0x04000BBD RID: 3005
	private const string EXTENSION = ".sav";

	// Token: 0x04000BBE RID: 3006
	private const int CURR_FORMAT_ID = 3;

	// Token: 0x0200025C RID: 604
	public class Summary
	{
		// Token: 0x06000CDF RID: 3295 RVA: 0x00035120 File Offset: 0x00033320
		public Summary(string name, string displayName, Identifiable.Id iconId, PlayerState.GameMode gameMode, string version, int day, int currency, int pediaCount, bool gameOver, DateTimeOffset saveTimestamp, string saveName, ulong saveNumber)
		{
			this.name = name;
			this.displayName = displayName;
			this.iconId = iconId;
			this.gameMode = gameMode;
			this.version = version;
			this.day = day;
			this.currency = currency;
			this.pediaCount = pediaCount;
			this.gameOver = gameOver;
			this.isInvalid = false;
			this.saveTimestamp = saveTimestamp;
			this.saveName = saveName;
			this.saveNumber = saveNumber;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x00035198 File Offset: 0x00033398
		public Summary(string name)
		{
			this.name = name;
			this.displayName = name;
			this.iconId = Identifiable.Id.BOOM_PLORT;
			this.gameMode = PlayerState.GameMode.CLASSIC;
			this.version = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get("l.unknown");
			this.day = 0;
			this.currency = 0;
			this.pediaCount = 0;
			this.gameOver = false;
			this.isInvalid = true;
			this.saveTimestamp = DateTimeOffset.MinValue;
			this.autosaveCount = 0;
			this.saveName = name;
			this.saveNumber = 0UL;
		}

		// Token: 0x04000BBF RID: 3007
		public string name;

		// Token: 0x04000BC0 RID: 3008
		public string displayName;

		// Token: 0x04000BC1 RID: 3009
		public Identifiable.Id iconId;

		// Token: 0x04000BC2 RID: 3010
		public PlayerState.GameMode gameMode;

		// Token: 0x04000BC3 RID: 3011
		public string version;

		// Token: 0x04000BC4 RID: 3012
		public int day;

		// Token: 0x04000BC5 RID: 3013
		public int currency;

		// Token: 0x04000BC6 RID: 3014
		public int pediaCount;

		// Token: 0x04000BC7 RID: 3015
		public bool isInvalid;

		// Token: 0x04000BC8 RID: 3016
		public bool gameOver;

		// Token: 0x04000BC9 RID: 3017
		public DateTimeOffset saveTimestamp;

		// Token: 0x04000BCA RID: 3018
		public int autosaveCount;

		// Token: 0x04000BCB RID: 3019
		public string saveName;

		// Token: 0x04000BCC RID: 3020
		public ulong saveNumber;
	}
}
