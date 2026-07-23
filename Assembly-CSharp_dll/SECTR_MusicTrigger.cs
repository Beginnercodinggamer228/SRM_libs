using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Music Trigger")]
public class SECTR_MusicTrigger : MonoBehaviour
{
	// Token: 0x060002E8 RID: 744 RVA: 0x0001276D File Offset: 0x0001096D
	private void OnEnable()
	{
		if (this.activator)
		{
			this._Play();
		}
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00012782 File Offset: 0x00010982
	private void OnDisable()
	{
		if (this.StopOnExit)
		{
			this._Stop(false);
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00012793 File Offset: 0x00010993
	private void OnTriggerEnter(Collider other)
	{
		if (this.activator == null)
		{
			if (this.Cue != null)
			{
				this._Play();
			}
			else
			{
				this._Stop(false);
			}
			this.activator = other;
		}
	}

	// Token: 0x060002EB RID: 747 RVA: 0x000127C7 File Offset: 0x000109C7
	private void OnTriggerExit(Collider other)
	{
		if (this.StopOnExit && other == this.activator)
		{
			this._Stop(false);
			this.activator = null;
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x000127ED File Offset: 0x000109ED
	private void _Play()
	{
		if (this.Cue != null)
		{
			SECTR_AudioSystem.PlayMusic(this.Cue);
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00012808 File Offset: 0x00010A08
	private void _Stop(bool stopImmediately)
	{
		SECTR_AudioSystem.StopMusic(stopImmediately);
	}

	// Token: 0x0400030D RID: 781
	private Collider activator;

	// Token: 0x0400030E RID: 782
	[SECTR_ToolTip("The Cue to play as music. If null, this trigger will stop the current music.", null, false)]
	public SECTR_AudioCue Cue;

	// Token: 0x0400030F RID: 783
	[SECTR_ToolTip("Should music be forced to loop when playing.")]
	public bool Loop = true;

	// Token: 0x04000310 RID: 784
	[SECTR_ToolTip("Should the music stop when leaving the trigger.")]
	public bool StopOnExit;
}
