using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000796 RID: 1942
public class TeleportablePlayer : MonoBehaviour, PlayerModel.Participant
{
	// Token: 0x06002880 RID: 10368 RVA: 0x000998F5 File Offset: 0x00097AF5
	public void Awake()
	{
		this.firestormActivator = base.GetComponent<FirestormActivator>();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x00099913 File Offset: 0x00097B13
	public void SetModel(PlayerModel model)
	{
		this.model = model;
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x0009991C File Offset: 0x00097B1C
	public void TeleportTo(Vector3 position, RegionRegistry.RegionSetId regionSetId, Vector3? rotation = null, bool overlayEnabled = true, bool audioEnabled = true)
	{
		this.weaponVacuum.DropAllVacced();
		this.playerEventHandler.Position.Set(position);
		if (rotation != null)
		{
			this.playerEventHandler.Rotation.Set(rotation.Value);
			vp_FPController component = base.GetComponent<vp_FPController>();
			component.m_Velocity = Vector3.zero;
			component.Stop();
		}
		if (overlayEnabled)
		{
			SRSingleton<Overlay>.Instance.PlayTeleport();
		}
		if (audioEnabled)
		{
			SECTR_AudioSource component2 = base.GetComponent<SECTR_AudioSource>();
			if (component2 != null)
			{
				component2.Cue = this.teleportCue;
				component2.Play();
			}
		}
		this.model.SetCurrRegionSet(regionSetId);
		this.firestormActivator.RequestPlayerStateUpdate();
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x04002823 RID: 10275
	public WeaponVacuum weaponVacuum;

	// Token: 0x04002824 RID: 10276
	public vp_FPPlayerEventHandler playerEventHandler;

	// Token: 0x04002825 RID: 10277
	public SECTR_AudioCue teleportCue;

	// Token: 0x04002826 RID: 10278
	private FirestormActivator firestormActivator;

	// Token: 0x04002827 RID: 10279
	private PlayerModel model;
}
