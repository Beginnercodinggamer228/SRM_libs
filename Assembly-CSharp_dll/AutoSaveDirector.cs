using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000D7 RID: 215
public class AutoSaveDirector : SRBehaviour
{
	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0001E8D4 File Offset: 0x0001CAD4
	// (set) Token: 0x060004DA RID: 1242 RVA: 0x0001E8DC File Offset: 0x0001CADC
	public SavedGame SavedGame { get; private set; }

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060004DB RID: 1243 RVA: 0x0001E8E5 File Offset: 0x0001CAE5
	// (set) Token: 0x060004DC RID: 1244 RVA: 0x0001E8ED File Offset: 0x0001CAED
	public SavedProfile ProfileManager { get; private set; } = new SavedProfile();

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060004DD RID: 1245 RVA: 0x0001E8F6 File Offset: 0x0001CAF6
	// (set) Token: 0x060004DE RID: 1246 RVA: 0x0001E8FE File Offset: 0x0001CAFE
	public StorageProvider StorageProvider { get; private set; }

	// Token: 0x060004DF RID: 1247 RVA: 0x0001E907 File Offset: 0x0001CB07
	public float GetLastSaveTime()
	{
		return this.lastSaveTime;
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001E90F File Offset: 0x0001CB0F
	public void Awake()
	{
		this.SavedGame = new SavedGame(new ScenePrefabInstantiator(SRSingleton<GameContext>.Instance.LookupDirector), new SceneSavedGameInfoProvider());
		this.nextSaveTime = Time.time + 1440f;
		base.enabled = true;
		this.Initialize();
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x00003296 File Offset: 0x00001496
	public void Start()
	{
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001E94E File Offset: 0x0001CB4E
	private void Initialize()
	{
		this.StorageProvider = new FileStorageProvider();
		this.StorageProvider.Initialize();
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001E966 File Offset: 0x0001CB66
	public void OnSceneLoaded()
	{
		this.SetupDynamicObjects();
		if (base.gameObject == SRSingleton<GameContext>.Instance.gameObject)
		{
			this.LoadProfile();
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001E98B File Offset: 0x0001CB8B
	private void SetupDynamicObjects()
	{
		if (this.IsNewGame())
		{
			SRSingleton<DynamicObjectContainer>.Instance.RegisterDynamicObjectActors();
			return;
		}
		SRSingleton<DynamicObjectContainer>.Instance.DestroyDynamicObjectActors();
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0001E9AA File Offset: 0x0001CBAA
	public void Update()
	{
		if (!Levels.isSpecialNonAlloc() && Time.time >= this.nextSaveTime)
		{
			this.SaveAllNow();
		}
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0001E9C8 File Offset: 0x0001CBC8
	public bool SaveAllNow()
	{
		Log.Warning("Saving game and profile...", Array.Empty<object>());
		this.nextSaveTime = Time.time + 1440f;
		try
		{
			this.SaveGame();
			this.SaveProfile(false);
			this.StorageProvider.Flush();
		}
		catch (Exception e)
		{
			this.ErrorSaveFailure(e);
			return false;
		}
		this.lastSaveTime = Time.time;
		return true;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001EA3C File Offset: 0x0001CC3C
	public GameData.Summary LoadSummary(string saveName)
	{
		GameData.Summary result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			this.StorageProvider.GetGameData(saveName, memoryStream);
			if (memoryStream.Length == 0L)
			{
				Log.Warning("Datastream was empty when loading save.", new object[]
				{
					"saveName",
					saveName
				});
				result = new GameData.Summary(saveName);
			}
			else
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = this.SavedGame.LoadSummary(saveName, memoryStream);
			}
		}
		return result;
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001EAC0 File Offset: 0x0001CCC0
	private void ErrorSaveFailure(Exception e)
	{
		Log.Error("Error while saving.", new object[]
		{
			"Exception",
			e.Message,
			"Stack Trace",
			e.StackTrace
		});
		UnityEngine.Object.Instantiate<GameObject>(this.saveErrorPrefab).GetComponent<SaveErrorUI>().SetException(e, this.SavedGame.GetName());
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0001EB20 File Offset: 0x0001CD20
	public void OnApplicationQuit()
	{
		if (!Levels.isSpecial())
		{
			this.SaveGame();
			this.SaveProfile(false);
			this.StorageProvider.Flush();
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001EB42 File Offset: 0x0001CD42
	public void LoadNewGame(string displayName, Identifiable.Id gameIconId, PlayerState.GameMode gameMode, Action onError)
	{
		base.StartCoroutine(this.LoadNewGame_Coroutine(new AutoSaveDirector.LoadNewGameMetadata
		{
			displayName = displayName,
			gameMode = gameMode,
			gameIconId = gameIconId
		}, onError));
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001EB6D File Offset: 0x0001CD6D
	private IEnumerator LoadNewGame_Coroutine(AutoSaveDirector.LoadNewGameMetadata metadata, Action onError)
	{
		this.loadingGame = true;
		this.loadedGame = false;
		this.newGameMetadata = metadata;
		yield return this.OpenLoadingUI();
		yield return SRSingleton<GameContext>.Instance.DLCDirector.RefreshPackagesAsync();
		try
		{
			this.currentGameId = null;
			string gameSaveFileName = this.GetGameSaveFileName(metadata.displayName);
			string arg = "";
			int num = 1;
			while (this.StorageProvider.HasGameData(string.Format("{0}{1}", gameSaveFileName, arg)))
			{
				arg = string.Format("_{0}", num);
				num++;
			}
			this.SavedGame.CreateNew(string.Format("{0}{1}", gameSaveFileName, arg), metadata.displayName);
			SceneContext.onNextSceneAwake = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onNextSceneAwake, new SceneContext.SceneLoadDelegate(this.OnNextSceneAwake_NewGame));
			SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnNewGameLoaded));
			this.BeginSceneSwitch(onError);
			yield break;
		}
		catch (Exception ex)
		{
			Log.Error("Error while creating a new save file.", new object[]
			{
				"Exception",
				ex.Message,
				"Stack Trace",
				ex.StackTrace
			});
			LoadErrorUI.OpenLoadErrorUI(this.loadFileErrorPrefab, MessageUtil.Tcompose("e.pushfile_error", new string[]
			{
				this.SavedGame.GetName()
			}), true, "e.ok_button", delegate()
			{
				this.RevertToMainMenu(onError);
			});
			this.loadingUI.OnLoadingError();
			yield break;
		}
		yield break;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001EB8C File Offset: 0x0001CD8C
	private string GetGameSaveFileName(string displayName)
	{
		string text = Regex.Replace(displayName, "[^A-Za-z0-9]", "");
		text = text.Substring(0, Mathf.Min(25, text.Length));
		return string.Format("{0:yyyyMMddHHmmss}_{1}", DateTime.Now, text);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001EBD4 File Offset: 0x0001CDD4
	public Dictionary<string, List<GameData.Summary>> AvailableGamesByDisplayName()
	{
		return this.AvailableGames((GameData.Summary summary) => summary.displayName);
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001EBFB File Offset: 0x0001CDFB
	public Dictionary<string, List<GameData.Summary>> AvailableGamesByGameName()
	{
		return this.AvailableGames((GameData.Summary summary) => summary.name);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001EC24 File Offset: 0x0001CE24
	private Dictionary<string, List<GameData.Summary>> AvailableGames(Func<GameData.Summary, string> keyFunc)
	{
		List<string> availableGames = this.StorageProvider.GetAvailableGames();
		Dictionary<string, List<GameData.Summary>> dictionary = new Dictionary<string, List<GameData.Summary>>();
		foreach (string text in availableGames)
		{
			try
			{
				GameData.Summary summary = this.LoadSummary(text);
				string key = keyFunc(summary);
				List<GameData.Summary> list;
				if (!dictionary.TryGetValue(key, out list))
				{
					list = new List<GameData.Summary>();
					dictionary.Add(key, list);
				}
				list.Add(summary);
			}
			catch (Exception ex)
			{
				Log.Error("Failed to load summary for saved game.", new object[]
				{
					"name",
					text,
					"Exception",
					ex.ToString(),
					"Exception Stack Trace",
					ex.StackTrace
				});
			}
		}
		foreach (KeyValuePair<string, List<GameData.Summary>> keyValuePair in dictionary)
		{
			keyValuePair.Value.Sort(new Comparison<GameData.Summary>(this.CompareSummaryBySaveOrder));
		}
		return dictionary;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001ED5C File Offset: 0x0001CF5C
	private int CompareSummaryBySaveOrder(GameData.Summary s1, GameData.Summary s2)
	{
		int num = s2.saveNumber.CompareTo(s1.saveNumber);
		if (num == 0)
		{
			num = s2.saveTimestamp.CompareTo(s1.saveTimestamp);
		}
		return num;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001ED94 File Offset: 0x0001CF94
	public bool DisplayNameAvailable(string displayName)
	{
		foreach (string text in this.StorageProvider.GetAvailableGames())
		{
			Log.Debug(text, Array.Empty<object>());
			GameData.Summary summary = this.LoadSummary(text);
			if (string.Compare(displayName, summary.displayName, false) == 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001EE10 File Offset: 0x0001D010
	public bool GameExists(string gameName)
	{
		return this.StorageProvider.HasGameData(gameName);
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001EE20 File Offset: 0x0001D020
	public void DeleteGame(string gameName)
	{
		List<GameData.Summary> savesByGameName = this.GetSavesByGameName(gameName);
		for (int i = 0; i < savesByGameName.Count; i++)
		{
			string saveName = savesByGameName[i].saveName;
			if (this.StorageProvider.HasGameData(saveName))
			{
				this.DeleteSave(saveName);
			}
		}
		this.StorageProvider.Flush();
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001EE74 File Offset: 0x0001D074
	private List<GameData.Summary> GetSavesByGameName(string gameName)
	{
		List<GameData.Summary> result;
		if (!this.AvailableGamesByGameName().TryGetValue(gameName, out result))
		{
			result = new List<GameData.Summary>();
		}
		return result;
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001EE98 File Offset: 0x0001D098
	public void CleanupAutosaves(string gameName)
	{
		List<GameData.Summary> savesByGameName = this.GetSavesByGameName(gameName);
		List<string> list = new List<string>();
		for (int i = 5; i < savesByGameName.Count; i++)
		{
			string saveName = savesByGameName[i].saveName;
			if (this.StorageProvider.HasGameData(saveName))
			{
				Log.Warning("Cleaning up autosave file.", new object[]
				{
					"name",
					saveName
				});
				list.Add(saveName);
			}
		}
		this.StorageProvider.DeleteGamesData(list);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001EF0E File Offset: 0x0001D10E
	public void DeleteSave(string saveName)
	{
		this.StorageProvider.DeleteGameData(saveName);
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001EF1C File Offset: 0x0001D11C
	public bool IsNewGame()
	{
		return !this.loadedGame || Levels.isSpecial();
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001EF30 File Offset: 0x0001D130
	public bool HasContinue()
	{
		if (string.IsNullOrEmpty(this.ProfileManager.ContinueGameName))
		{
			return false;
		}
		GameData.Summary saveToContinue = this.GetSaveToContinue();
		return saveToContinue != null && this.StorageProvider.HasGameData(saveToContinue.saveName);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001EF70 File Offset: 0x0001D170
	public GameData.Summary GetSaveToContinue()
	{
		string continueGameName = this.ProfileManager.ContinueGameName;
		if (string.IsNullOrEmpty(continueGameName))
		{
			return null;
		}
		List<GameData.Summary> source;
		if (this.AvailableGamesByGameName().TryGetValue(continueGameName, out source))
		{
			return source.FirstOrDefault<GameData.Summary>();
		}
		return null;
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001EFAB File Offset: 0x0001D1AB
	public void SaveGameAndFlush()
	{
		this.SaveGame();
		this.StorageProvider.Flush();
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001EFC0 File Offset: 0x0001D1C0
	private void SaveGame()
	{
		if (this.loadingGame)
		{
			Log.Warning("Attempted to save game while loading, skipping.", Array.Empty<object>());
			return;
		}
		if (!string.IsNullOrEmpty(this.SavedGame.GetName()))
		{
			this.SavedGame.Pull(SRSingleton<SceneContext>.Instance.GameModel);
			string name = this.SavedGame.GetName();
			string displayName = this.SavedGame.GetDisplayName();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SavedGame.Save(memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				string nextFileName = this.GetNextFileName(name, 0, "{0}_{1}");
				if (this.currentGameId == null)
				{
					this.currentGameId = this.StorageProvider.GetGameId(nextFileName);
					Log.Warning("setting initial gameid", new object[]
					{
						"currentGameId",
						this.currentGameId,
						"nextFileName",
						nextFileName
					});
				}
				this.StorageProvider.StoreGameData(this.currentGameId, displayName, nextFileName, memoryStream);
			}
			this.ProfileManager.ContinueGameName = (this.SavedGame.GameState.summary.isGameOver ? string.Empty : name);
			this.CleanupAutosaves(name);
			return;
		}
		Log.Warning("Save game name was null or empty. Skipping save.", Array.Empty<object>());
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001F10C File Offset: 0x0001D30C
	private string GetNextFileName(string filename, int startingNumber, string format)
	{
		string text;
		do
		{
			text = string.Format(format, filename, startingNumber++);
		}
		while (this.StorageProvider.HasGameData(text));
		return text;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001F13C File Offset: 0x0001D33C
	private void LoadProfile()
	{
		Log.Debug("Storage provider initialized. Loading profile.", new object[]
		{
			(this.StorageProvider == null).ToString()
		});
		this.LoadFromStream(new Action<MemoryStream>(this.StorageProvider.GetProfileData), new Action<MemoryStream>(this.ProfileManager.LoadProfile), delegate
		{
			Log.Debug("No profile was found.", Array.Empty<object>());
			this.ProfileManager = new SavedProfile();
		});
		this.LoadSettings();
		this.ProfileManager.Push();
		if (AutoSaveDirector.firstLoad)
		{
			AutoSaveDirector.firstLoad = false;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs != null)
			{
				string[] array = commandLineArgs;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Contains("lowGraphics"))
					{
						Log.Debug("Forcing Lowest Quality Graphics", Array.Empty<object>());
						SRQualitySettings.ForceLowQuality();
						this.SaveProfile(false);
					}
				}
			}
			this.ProfileManager.Profile.RunUpgradeActions(this);
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001F218 File Offset: 0x0001D418
	private void LoadSettings()
	{
		if (this.StorageProvider.HasSettings())
		{
			this.LoadFromStream(new Action<MemoryStream>(this.StorageProvider.GetSettingsData), new Action<MemoryStream>(this.ProfileManager.LoadSettings), delegate
			{
				Log.Debug("No settings were found.", Array.Empty<object>());
			});
			return;
		}
		if (this.StorageProvider.HasProfile())
		{
			this.LoadFromStream(new Action<MemoryStream>(this.StorageProvider.GetProfileData), new Action<MemoryStream>(this.ProfileManager.LoadLegacySettings), delegate
			{
				Log.Debug("No profile data was found to load legacy settings.", Array.Empty<object>());
			});
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001F2D0 File Offset: 0x0001D4D0
	public bool SaveProfile()
	{
		return this.SaveProfile(true);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001F2DC File Offset: 0x0001D4DC
	private bool SaveProfile(bool forceFlush)
	{
		this.ProfileManager.Pull();
		if (this.StorageProvider.IsInitialized())
		{
			this.SaveStream(new Action<Stream>(this.ProfileManager.SaveProfile), new Action<MemoryStream>(this.StorageProvider.StoreProfileData));
			this.SaveStream(new Action<Stream>(this.ProfileManager.SaveSettings), new Action<MemoryStream>(this.StorageProvider.StoreSettingsData));
			if (forceFlush)
			{
				this.StorageProvider.Flush();
			}
			return true;
		}
		Log.Warning("Storage provider not initialized. Skipping profile and settings save.", Array.Empty<object>());
		return false;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001F374 File Offset: 0x0001D574
	private void LoadFromStream(Action<MemoryStream> openStream, Action<MemoryStream> load, Action onErr)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			openStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			if (memoryStream.Length > 0L)
			{
				load(memoryStream);
			}
			else
			{
				onErr();
			}
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001F3D0 File Offset: 0x0001D5D0
	private void SaveStream(Action<Stream> saveToStream, Action<MemoryStream> storeStream)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			saveToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			storeStream(memoryStream);
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001F418 File Offset: 0x0001D618
	public void ResetProfile()
	{
		Log.Info("Resetting profile.", Array.Empty<object>());
		SRQualitySettings.ResetProfile();
		SRSingleton<GameContext>.Instance.OptionsDirector.ResetProfile();
		SRSingleton<GameContext>.Instance.InputDirector.ResetProfile();
		SRSingleton<SceneContext>.Instance.AchievementsDirector.ResetProfile();
		SRSingleton<GameContext>.Instance.MessageDirector.SetCulture(SRSingleton<GameContext>.Instance.MessageDirector.defaultLang);
		this.ProfileManager.ContinueGameName = "";
		this.SaveProfile(false);
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001F49C File Offset: 0x0001D69C
	public bool IsLoadingGame()
	{
		return this.loadingGame;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001F4A4 File Offset: 0x0001D6A4
	public void BeginLoad(string gameName, string saveName, Action onError)
	{
		if (this.loadingGame)
		{
			return;
		}
		this.LoadSave(gameName, saveName, true, onError);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001F4BC File Offset: 0x0001D6BC
	private void OnNextSceneAwake_NewGame(SceneContext sceneContext)
	{
		sceneContext.GameModel.expectingPush = false;
		sceneContext.GameModeConfig.initGameMode = this.newGameMetadata.gameMode;
		sceneContext.GameModel.gameIconId = this.newGameMetadata.gameIconId;
		this.newGameMetadata = null;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001F508 File Offset: 0x0001D708
	private void OnNextSceneAwake_ExistingGame(SceneContext sceneContext)
	{
		sceneContext.GameModel.expectingPush = true;
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001F516 File Offset: 0x0001D716
	private void LoadSave(string gameName, string saveName, bool promptDLCPurgedException, Action onError)
	{
		base.StartCoroutine(this.LoadSave_Coroutine(gameName, saveName, promptDLCPurgedException, onError));
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001F52A File Offset: 0x0001D72A
	private IEnumerator LoadSave_Coroutine(string gameName, string saveName, bool promptDLCPurgedException, Action onError)
	{
		this.loadingGame = true;
		this.loadedGame = true;
		yield return this.OpenLoadingUI();
		yield return SRSingleton<GameContext>.Instance.DLCDirector.RefreshPackagesAsync();
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.StorageProvider.GetGameData(saveName, memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				this.SavedGame.Load(memoryStream);
				this.currentGameId = this.StorageProvider.GetGameId(saveName);
			}
			try
			{
				SRSingleton<GameContext>.Instance.DLCDirector.Purge(this.SavedGame.GameState);
			}
			catch (DLCPurgedException exception)
			{
				if (promptDLCPurgedException)
				{
					DLCPurgedExceptionUI.OnExceptionCaught(this.prefabDLCPurgedExceptionUI, exception, delegate
					{
						this.LoadSave(gameName, saveName, false, onError);
					}, delegate
					{
						this.RevertToMainMenu(onError);
					});
					this.loadingUI.OnLoadingError();
					yield break;
				}
			}
			SceneContext.onNextSceneAwake = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onNextSceneAwake, new SceneContext.SceneLoadDelegate(this.OnNextSceneAwake_ExistingGame));
			this.BeginSceneSwitch(onError);
			yield break;
		}
		catch (Exception ex)
		{
			Log.Error("Error while loading a save file.", new object[]
			{
				"save",
				saveName,
				"Exception",
				ex.Message,
				"Stack Trace",
				ex.StackTrace
			});
			LoadErrorUI.OpenLoadErrorUI(this.loadFileErrorPrefab, "e.file_load_failed", false, "e.yes_button", delegate()
			{
				this.LoadFallbackSave(gameName, saveName, true, onError);
			}, "e.no_button", delegate()
			{
				this.RevertToMainMenu(onError);
			});
			this.loadingUI.OnLoadingError();
			yield break;
		}
		yield break;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001F556 File Offset: 0x0001D756
	private void LoadFallbackSave(string gameName, string saveName, bool promptDLCPurgedException, Action onError)
	{
		base.StartCoroutine(this.LoadFallbackSave_Coroutine(gameName, saveName, promptDLCPurgedException, onError));
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001F56A File Offset: 0x0001D76A
	private IEnumerator LoadFallbackSave_Coroutine(string gameName, string saveName, bool promptDLCPurgedException, Action onError)
	{
		IEnumerable<GameData.Summary> summaries = this.GetSavesByGameName(gameName).SkipWhile((GameData.Summary s) => saveName.CompareTo(s.saveName) != 0).Skip(1);
		yield return this.OpenLoadingUI();
		int count = 0;
		Action <>9__2;
		Action <>9__3;
		foreach (GameData.Summary summary in summaries)
		{
			int num = count;
			count = num + 1;
			yield return new WaitForSeconds(0.1f);
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					this.StorageProvider.GetGameData(summary.saveName, memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					this.SavedGame.Load(memoryStream);
					this.currentGameId = this.StorageProvider.GetGameId(summary.saveName);
				}
				try
				{
					SRSingleton<GameContext>.Instance.DLCDirector.Purge(this.SavedGame.GameState);
				}
				catch (DLCPurgedException ex)
				{
					if (promptDLCPurgedException)
					{
						DLCPurgedExceptionUI prefab = this.prefabDLCPurgedExceptionUI;
						DLCPurgedException exception = ex;
						Action onContinue;
						if ((onContinue = <>9__2) == null)
						{
							onContinue = (<>9__2 = delegate()
							{
								this.LoadFallbackSave(gameName, saveName, false, onError);
							});
						}
						Action onCancel;
						if ((onCancel = <>9__3) == null)
						{
							onCancel = (<>9__3 = delegate()
							{
								this.RevertToMainMenu(onError);
							});
						}
						DLCPurgedExceptionUI.OnExceptionCaught(prefab, exception, onContinue, onCancel);
						this.loadingUI.OnLoadingError();
						yield break;
					}
				}
				SceneContext.onNextSceneAwake = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onNextSceneAwake, new SceneContext.SceneLoadDelegate(this.OnNextSceneAwake_ExistingGame));
				this.BeginSceneSwitch(onError);
				yield break;
			}
			catch (Exception ex2)
			{
				Log.Error("Failed to fallback to prior save.", new object[]
				{
					"save",
					summary.saveName,
					"Exception",
					ex2.Message,
					"Stack Trace",
					ex2.StackTrace
				});
			}
		}
		IEnumerator<GameData.Summary> enumerator = null;
		Log.Error(string.Format("Failed all fallback attempts. Attempted to load {0} files.", count), Array.Empty<object>());
		LoadErrorUI.OpenLoadErrorUI(this.loadFileErrorPrefab, "e.fallback_failed", true, "e.ok_button", delegate()
		{
			this.RevertToMainMenu(onError);
		});
		this.loadingUI.OnLoadingError();
		yield break;
		yield break;
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001F596 File Offset: 0x0001D796
	private void BeginSceneSwitch(Action onErr)
	{
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Combine(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnGameLoaded));
		SceneManager.LoadSceneAsync("worldGenerated", LoadSceneMode.Single);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001F5C4 File Offset: 0x0001D7C4
	private void OnNewGameLoaded(SceneContext sceneContext)
	{
		sceneContext.GameModel.OnNewGameLoaded();
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001F5D1 File Offset: 0x0001D7D1
	private void OnGameLoaded(SceneContext ctx)
	{
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnGameLoaded));
		base.StartCoroutine(this.OnGameLoadedCoroutine(ctx));
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001F601 File Offset: 0x0001D801
	private IEnumerator OnGameLoadedCoroutine(SceneContext ctx)
	{
		if (ctx.GameModel.expectingPush)
		{
			Exception ex = null;
			try
			{
				this.SavedGame.Push(ctx.GameModel);
			}
			catch (Exception ex2)
			{
				Log.Error("Error while populating scene from save game.", new object[]
				{
					"save",
					this.SavedGame.GetName(),
					"Exception",
					ex2.Message,
					"Stack Trace",
					ex2.StackTrace
				});
				ex = ex2;
				LoadErrorUI.OpenLoadErrorUI(this.loadFileErrorPrefab, MessageUtil.Tcompose("e.pushfile_error", new string[]
				{
					this.SavedGame.GetName()
				}), true, "e.ok_button", delegate()
				{
					this.RevertToMainMenu(delegate
					{
						Log.Debug("Falling back to main menu from worldGenerated.", Array.Empty<object>());
					});
				});
				this.loadingUI.OnLoadingError();
			}
			finally
			{
				ctx.GameModel.expectingPush = false;
			}
			if (ex != null)
			{
				yield break;
			}
		}
		ctx.TutorialDirector.SuppressTutorials();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this.CloseLoadingUI();
		if (this.IsNewGame() && ctx.GameModeConfig.GetModeSettings().newGamePrefab != null)
		{
			Destroyer.Monitor(UnityEngine.Object.Instantiate<GameObject>(ctx.GameModeConfig.GetModeSettings().newGamePrefab), delegate(Destroyer.Metadata metadata)
			{
				ctx.TutorialDirector.UnsuppressTutorials();
			});
		}
		else
		{
			ctx.TutorialDirector.UnsuppressTutorials();
		}
		ctx.NoteGameFullyLoaded();
		this.loadingGame = false;
		yield break;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001F618 File Offset: 0x0001D818
	private void RevertToMainMenu(Action onError)
	{
		SceneContext.onNextSceneAwake = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onNextSceneAwake, new SceneContext.SceneLoadDelegate(this.OnNextSceneAwake_NewGame));
		SceneContext.onNextSceneAwake = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onNextSceneAwake, new SceneContext.SceneLoadDelegate(this.OnNextSceneAwake_ExistingGame));
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnNewGameLoaded));
		SceneContext.onSceneLoaded = (SceneContext.SceneLoadDelegate)Delegate.Remove(SceneContext.onSceneLoaded, new SceneContext.SceneLoadDelegate(this.OnGameLoaded));
		this.loadingUI.isReturningToMenu = true;
		if (Levels.isMainMenu())
		{
			this.RevertToMainMenu_OnRevertComplete();
			onError();
			return;
		}
		SceneManager.sceneLoaded += this.RevertToMainMenu_OnSceneLoaded;
		SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001F6E2 File Offset: 0x0001D8E2
	private void RevertToMainMenu_OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.sceneLoaded -= this.RevertToMainMenu_OnSceneLoaded;
		this.RevertToMainMenu_OnRevertComplete();
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001F6FB File Offset: 0x0001D8FB
	private void RevertToMainMenu_OnRevertComplete()
	{
		this.loadingGame = false;
		this.CloseLoadingUI();
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001F70A File Offset: 0x0001D90A
	private IEnumerator OpenLoadingUI()
	{
		if (this.loadingUI == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.UITemplates.loadingUI);
			this.loadingUI = gameObject.GetRequiredComponent<LoadingUI>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		this.loadingUI.OnLoadingStart();
		yield return new WaitForEndOfFrame();
		yield break;
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001F719 File Offset: 0x0001D919
	private void CloseLoadingUI()
	{
		if (this.loadingUI != null)
		{
			Destroyer.Destroy(this.loadingUI.gameObject, "AutoSaveDirector.CloseLoadingUI");
			this.loadingUI = null;
		}
	}

	// Token: 0x04000508 RID: 1288
	public GameObject saveErrorPrefab;

	// Token: 0x04000509 RID: 1289
	public LoadErrorUI loadFileErrorPrefab;

	// Token: 0x0400050A RID: 1290
	public DLCPurgedExceptionUI prefabDLCPurgedExceptionUI;

	// Token: 0x0400050E RID: 1294
	private LoadingUI loadingUI;

	// Token: 0x0400050F RID: 1295
	private float nextSaveTime;

	// Token: 0x04000510 RID: 1296
	private bool loadedGame;

	// Token: 0x04000511 RID: 1297
	private bool loadingGame;

	// Token: 0x04000512 RID: 1298
	private AutoSaveDirector.LoadNewGameMetadata newGameMetadata;

	// Token: 0x04000513 RID: 1299
	private float lastSaveTime;

	// Token: 0x04000514 RID: 1300
	private const float SAVE_PERIOD = 1440f;

	// Token: 0x04000515 RID: 1301
	private const int MAX_AUTOSAVES = 5;

	// Token: 0x04000516 RID: 1302
	private string currentGameId;

	// Token: 0x04000517 RID: 1303
	private static bool firstLoad = true;

	// Token: 0x020000D8 RID: 216
	private class LoadNewGameMetadata
	{
		// Token: 0x04000518 RID: 1304
		public string displayName;

		// Token: 0x04000519 RID: 1305
		public PlayerState.GameMode gameMode;

		// Token: 0x0400051A RID: 1306
		public Identifiable.Id gameIconId;
	}
}
