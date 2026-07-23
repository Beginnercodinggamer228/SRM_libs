using System;
using UnityEngine;

// Token: 0x0200008C RID: 140
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Point Source")]
public class SECTR_PointSource : SECTR_AudioSource
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060002EF RID: 751 RVA: 0x0001281F File Offset: 0x00010A1F
	public override bool IsPlaying
	{
		get
		{
			return this.instance;
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0001282C File Offset: 0x00010A2C
	public override void Play()
	{
		if (this.IsPlaying && this.instance.Loops)
		{
			this.instance.Stop(false);
		}
		if (this.Cue != null)
		{
			if (this.Cue.Spatialization == SECTR_AudioCue.Spatializations.Infinite3D)
			{
				this.instance = SECTR_AudioSystem.Play(this.Cue, SECTR_AudioSystem.Listener, UnityEngine.Random.onUnitSphere, this.Loop, null, false);
			}
			else
			{
				this.instance = SECTR_AudioSystem.Play(this.Cue, base.transform, Vector3.zero, this.Loop, null, false);
			}
			if (this.instance)
			{
				this.instance.Volume = this.volume;
				this.instance.Pitch = this.pitch;
			}
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00012901 File Offset: 0x00010B01
	public override void Stop(bool stopImmediately)
	{
		this.instance.Stop(stopImmediately);
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0001290F File Offset: 0x00010B0F
	protected override void OnVolumePitchChanged()
	{
		if (this.instance)
		{
			this.instance.Volume = this.volume;
			this.instance.Pitch = this.pitch;
		}
	}

	// Token: 0x04000311 RID: 785
	protected SECTR_AudioCueInstance instance;
}
