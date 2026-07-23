using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006DF RID: 1759
public class DeactivateOnNotGameMode : SRBehaviour, GameModel.Participant
{
	// Token: 0x060024B1 RID: 9393 RVA: 0x0008D49F File Offset: 0x0008B69F
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterGameModelParticipant(this);
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x0008D4FD File Offset: 0x0008B6FD
	public void OnEnable()
	{
		if (this.mode != null && !this.whiteList.Contains(this.mode.Value))
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GameModel model)
	{
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x0008D530 File Offset: 0x0008B730
	public void SetModel(GameModel model)
	{
		this.mode = new PlayerState.GameMode?(model.currGameMode);
		this.OnEnable();
	}

	// Token: 0x040023A0 RID: 9120
	[Tooltip("List of game modes that will activate the object.")]
	public List<PlayerState.GameMode> whiteList;

	// Token: 0x040023A1 RID: 9121
	private PlayerState.GameMode? mode;
}
