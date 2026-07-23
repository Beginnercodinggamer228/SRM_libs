using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200086F RID: 2159
public class vp_Switch : vp_Interactable
{
	// Token: 0x06002DAC RID: 11692 RVA: 0x000AEF24 File Offset: 0x000AD124
	protected override void Start()
	{
		base.Start();
		if (this.AudioSource == null)
		{
			this.AudioSource = ((base.GetComponent<AudioSource>() == null) ? base.gameObject.AddComponent<AudioSource>() : base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000AEF61 File Offset: 0x000AD161
	public override bool TryInteract(vp_PlayerEventHandler player)
	{
		if (this.Target == null)
		{
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		this.PlaySound();
		this.Target.SendMessage(this.TargetMessage, SendMessageOptions.DontRequireReceiver);
		return true;
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000AEFA4 File Offset: 0x000AD1A4
	public virtual void PlaySound()
	{
		if (this.AudioSource == null)
		{
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

	// Token: 0x06002DAF RID: 11695 RVA: 0x000AF028 File Offset: 0x000AD228
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

	// Token: 0x04002BDA RID: 11226
	public GameObject Target;

	// Token: 0x04002BDB RID: 11227
	public string TargetMessage = "";

	// Token: 0x04002BDC RID: 11228
	public AudioSource AudioSource;

	// Token: 0x04002BDD RID: 11229
	public Vector2 SwitchPitchRange = new Vector2(1f, 1.5f);

	// Token: 0x04002BDE RID: 11230
	public List<AudioClip> SwitchSounds = new List<AudioClip>();
}
