using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006DE RID: 1758
public class DeactivateOnGameMode : SRBehaviour, GameModel.Participant
{
	// Token: 0x060024AC RID: 9388 RVA: 0x0008D49F File Offset: 0x0008B69F
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGameModelParticipant(this);
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x0008D4B1 File Offset: 0x0008B6B1
	public void OnEnable()
	{
		if (this.mode != null && this.blackList.Contains(this.mode.Value))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GameModel model)
	{
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x0008D4E4 File Offset: 0x0008B6E4
	public void SetModel(GameModel model)
	{
		this.mode = new PlayerState.GameMode?(model.currGameMode);
		this.OnEnable();
	}

	// Token: 0x0400239E RID: 9118
	[Tooltip("List of game modes that will deactivate the object.")]
	public List<PlayerState.GameMode> blackList;

	// Token: 0x0400239F RID: 9119
	private PlayerState.GameMode? mode;
}
