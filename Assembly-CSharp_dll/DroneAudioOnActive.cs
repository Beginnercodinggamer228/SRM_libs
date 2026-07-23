using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class DroneAudioOnActive : MonoBehaviour
{
	// Token: 0x0600069A RID: 1690 RVA: 0x0002327C File Offset: 0x0002147C
	public DroneAudioOnActive Init(SECTR_AudioCue cue)
	{
		this.instance.Stop(false);
		this.cue = cue;
		this.instance = SECTR_AudioSystem.Play(this.cue, base.transform.position, false);
		return this;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x000232AF File Offset: 0x000214AF
	public void OnEnable()
	{
		this.instance = SECTR_AudioSystem.Play(this.cue, base.transform.position, false);
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x000232CE File Offset: 0x000214CE
	public void OnDisable()
	{
		this.instance.Stop(false);
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x000232DC File Offset: 0x000214DC
	public void Update()
	{
		this.instance.Position = base.transform.position;
	}

	// Token: 0x0400062D RID: 1581
	[Tooltip("SFX cue to play while active.")]
	public SECTR_AudioCue cue;

	// Token: 0x0400062E RID: 1582
	private SECTR_AudioCueInstance instance;
}
