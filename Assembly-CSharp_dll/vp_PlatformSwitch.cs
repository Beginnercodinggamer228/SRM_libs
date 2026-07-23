using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200086E RID: 2158
public class vp_PlatformSwitch : vp_Interactable
{
	// Token: 0x06002DA7 RID: 11687 RVA: 0x000AECA1 File Offset: 0x000ACEA1
	protected override void Start()
	{
		base.Start();
		if (this.AudioSource == null)
		{
			this.AudioSource = ((base.GetComponent<AudioSource>() == null) ? base.gameObject.AddComponent<AudioSource>() : base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000AECE0 File Offset: 0x000ACEE0
	public override bool TryInteract(vp_PlayerEventHandler player)
	{
		if (this.Platform == null)
		{
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		if (Time.time < this.m_Timeout)
		{
			return false;
		}
		this.PlaySound();
		if (vp_Gameplay.isMaster)
		{
			this.Platform.SendMessage("GoTo", (this.Platform.TargetedWaypoint == 0) ? 1 : 0, SendMessageOptions.DontRequireReceiver);
		}
		else if (this.InteractType == vp_Interactable.vp_InteractType.Normal)
		{
			base.SendMessage("ClientTryInteract");
		}
		this.m_Timeout = Time.time + this.SwitchTimeout;
		this.m_IsSwitched = !this.m_IsSwitched;
		return true;
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000AED8C File Offset: 0x000ACF8C
	public virtual void PlaySound()
	{
		if (this.AudioSource == null)
		{
			Debug.LogWarning("Audio Source is not set");
			return;
		}
		if (this.SwitchSounds.Count == 0)
		{
			return;
		}
		AudioClip audioClip = this.SwitchSounds[UnityEngine.Random.Range(0, this.SwitchSounds.Count)];
		if (audioClip == null)
		{
			return;
		}
		this.AudioSource.pitch = UnityEngine.Random.Range(this.SwitchPitchRange.x, this.SwitchPitchRange.y);
		this.AudioSource.PlayOneShot(audioClip);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000AEE1C File Offset: 0x000AD01C
	protected override void OnTriggerEnter(Collider col)
	{
		if (this.InteractType != vp_Interactable.vp_InteractType.Trigger)
		{
			return;
		}
		foreach (string b in this.RecipientTags)
		{
			if (col.gameObject.tag == b)
			{
				goto IL_4F;
			}
		}
		return;
		IL_4F:
		this.m_Player = col.transform.root.GetComponent<vp_PlayerEventHandler>();
		if (this.m_Player == null)
		{
			return;
		}
		if (this.m_Player.Interactable.Get() != null && this.m_Player.Interactable.Get().GetComponent<Collider>() == col)
		{
			return;
		}
		this.TryInteract(this.m_Player);
	}

	// Token: 0x04002BD3 RID: 11219
	public float SwitchTimeout;

	// Token: 0x04002BD4 RID: 11220
	public vp_MovingPlatform Platform;

	// Token: 0x04002BD5 RID: 11221
	public AudioSource AudioSource;

	// Token: 0x04002BD6 RID: 11222
	public Vector2 SwitchPitchRange = new Vector2(1f, 1.5f);

	// Token: 0x04002BD7 RID: 11223
	public List<AudioClip> SwitchSounds = new List<AudioClip>();

	// Token: 0x04002BD8 RID: 11224
	protected bool m_IsSwitched;

	// Token: 0x04002BD9 RID: 11225
	protected float m_Timeout;
}
