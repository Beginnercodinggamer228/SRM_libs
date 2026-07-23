using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class ModelTransformUpdater : MonoBehaviour, PlayerModel.Participant
{
	// Token: 0x06000E76 RID: 3702 RVA: 0x0003A8A5 File Offset: 0x00038AA5
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
		this.playerEventHandler = base.GetComponent<vp_FPPlayerEventHandler>();
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0003A8C3 File Offset: 0x00038AC3
	public void InitModel(PlayerModel model)
	{
		model.position = base.transform.position;
		model.rotation = base.transform.rotation;
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0003A8E7 File Offset: 0x00038AE7
	public void SetModel(PlayerModel model)
	{
		this.model = model;
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0003A8F0 File Offset: 0x00038AF0
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
		this.playerEventHandler.Position.Set(pos);
		this.playerEventHandler.Rotation.Set(rot.eulerAngles);
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0003A929 File Offset: 0x00038B29
	public void LateUpdate()
	{
		this.model.position = base.transform.position;
		this.model.rotation = base.transform.rotation;
	}

	// Token: 0x04000D8E RID: 3470
	private PlayerModel model;

	// Token: 0x04000D8F RID: 3471
	private vp_FPPlayerEventHandler playerEventHandler;
}
