using System;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class PlaySoundOnHit : MonoBehaviour, ControllerCollisionListener
{
	// Token: 0x06000D0B RID: 3339 RVA: 0x000358AC File Offset: 0x00033AAC
	public void OnCollisionEnter(Collision col)
	{
		if (col.impulse.sqrMagnitude >= this.minForce * this.minForce)
		{
			this.MaybePlaySound();
		}
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x000358DC File Offset: 0x00033ADC
	public void OnControllerCollision(GameObject gameObj)
	{
		if (this.includeControllerCollisions)
		{
			this.MaybePlaySound();
		}
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x000358EC File Offset: 0x00033AEC
	private void MaybePlaySound()
	{
		if (Time.time >= this.nextTime)
		{
			if (this.hitCue != null)
			{
				SECTR_AudioSystem.Play(this.hitCue, base.transform.position, false);
			}
			this.nextTime = Time.time + this.minTimeBetween;
		}
	}

	// Token: 0x04000C3B RID: 3131
	[Tooltip("The audio cue to play on hit")]
	public SECTR_AudioCue hitCue;

	// Token: 0x04000C3C RID: 3132
	[Tooltip("Minimum time between playing sound, in seconds.")]
	public float minTimeBetween = 1f;

	// Token: 0x04000C3D RID: 3133
	[Tooltip("Minimum force to trigger the sound.")]
	public float minForce;

	// Token: 0x04000C3E RID: 3134
	[Tooltip("Whether we should count controller collisions for whether we play the hit.")]
	public bool includeControllerCollisions;

	// Token: 0x04000C3F RID: 3135
	private float nextTime;
}
