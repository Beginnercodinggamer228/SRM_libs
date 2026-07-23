using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class GameModeConfig : MonoBehaviour, GameModel.Participant
{
	// Token: 0x06000AAD RID: 2733 RVA: 0x0002D968 File Offset: 0x0002BB68
	public void Awake()
	{
		this.modeSettings[PlayerState.GameMode.CLASSIC] = this.classicSettings;
		this.modeSettings[PlayerState.GameMode.CASUAL] = this.casualSettings;
		this.modeSettings[PlayerState.GameMode.TIME_LIMIT] = this.timeLimitSettings;
		this.modeSettings[PlayerState.GameMode.TIME_LIMIT_V2] = this.timeLimitV2Settings;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGameModelParticipant(this);
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00003296 File Offset: 0x00001496
	public void InitForLevel()
	{
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0002D9CD File Offset: 0x0002BBCD
	public void InitModel(GameModel model)
	{
		model.currGameMode = this.initGameMode;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0002D9DB File Offset: 0x0002BBDB
	public void SetModel(GameModel model)
	{
		this.gameModel = model;
		this.gameModel.ResetPlayerForGameMode(this.GetModeSettings());
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0002D9F5 File Offset: 0x0002BBF5
	public GameModeSettings GetModeSettings()
	{
		return this.modeSettings[this.gameModel.currGameMode];
	}

	// Token: 0x040008AB RID: 2219
	public GameModeSettings classicSettings;

	// Token: 0x040008AC RID: 2220
	public GameModeSettings casualSettings;

	// Token: 0x040008AD RID: 2221
	public GameModeSettings timeLimitSettings;

	// Token: 0x040008AE RID: 2222
	public GameModeSettings timeLimitV2Settings;

	// Token: 0x040008AF RID: 2223
	public PlayerState.GameMode initGameMode;

	// Token: 0x040008B0 RID: 2224
	private Dictionary<PlayerState.GameMode, GameModeSettings> modeSettings = new Dictionary<PlayerState.GameMode, GameModeSettings>();

	// Token: 0x040008B1 RID: 2225
	private GameModel gameModel;
}
