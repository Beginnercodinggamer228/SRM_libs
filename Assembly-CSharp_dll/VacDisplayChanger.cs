using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DF RID: 735
public class VacDisplayChanger : MonoBehaviour
{
	// Token: 0x06000FB5 RID: 4021 RVA: 0x0003DFE0 File Offset: 0x0003C1E0
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		PlayerState playerState = this.playerState;
		playerState.onAmmoModeChanged = (PlayerState.OnAmmoModeChanged)Delegate.Combine(playerState.onAmmoModeChanged, new PlayerState.OnAmmoModeChanged(this.SetDisplayMode));
		this.SetDisplayMode(this.playerState.GetAmmoMode());
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0003E035 File Offset: 0x0003C235
	public void OnDestroy()
	{
		PlayerState playerState = this.playerState;
		playerState.onAmmoModeChanged = (PlayerState.OnAmmoModeChanged)Delegate.Remove(playerState.onAmmoModeChanged, new PlayerState.OnAmmoModeChanged(this.SetDisplayMode));
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0003E060 File Offset: 0x0003C260
	public void SetDisplayMode(PlayerState.AmmoMode mode)
	{
		bool flag = mode == PlayerState.AmmoMode.DEFAULT;
		bool flag2 = mode == PlayerState.AmmoMode.NIMBLE_VALLEY;
		foreach (GameObject gameObject in this.displayDefault)
		{
			gameObject.SetActive(flag);
		}
		foreach (GameObject gameObject2 in this.displayNimbleValley)
		{
			gameObject2.SetActive(flag2);
		}
		if (this.assembler.Assemble(flag2))
		{
			if (flag)
			{
				SECTR_AudioSystem.Play(this.onTransformToDefaultCue2D, Vector3.zero, false);
				return;
			}
			if (flag2)
			{
				SECTR_AudioSystem.Play(this.onTransformToNimbleValleyCue2D, Vector3.zero, false);
			}
		}
	}

	// Token: 0x04000E7B RID: 3707
	[Header("Default Game Mode")]
	[Tooltip("GameObject display on the default ammo mode.")]
	public List<GameObject> displayDefault;

	// Token: 0x04000E7C RID: 3708
	[Tooltip("SFX played when the display transforms to default ammo mode. (optional)")]
	public SECTR_AudioCue onTransformToDefaultCue2D;

	// Token: 0x04000E7D RID: 3709
	[Header("Nimble Valley Game Mode")]
	[Tooltip("GameObject display on the Nimble Valley ammo mode.")]
	public List<GameObject> displayNimbleValley;

	// Token: 0x04000E7E RID: 3710
	[Tooltip("Nimble Valley assembler.")]
	public ActorMatAssemble assembler;

	// Token: 0x04000E7F RID: 3711
	[Tooltip("SFX played when the display transforms to Nimble Valley ammo mode. (optional)")]
	public SECTR_AudioCue onTransformToNimbleValleyCue2D;

	// Token: 0x04000E80 RID: 3712
	private PlayerState playerState;
}
