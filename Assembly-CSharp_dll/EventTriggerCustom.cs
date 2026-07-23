using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class EventTriggerCustom : MonoBehaviour
{
	// Token: 0x06000A28 RID: 2600 RVA: 0x0002C3F0 File Offset: 0x0002A5F0
	public void SpawnObjectCustom(AnimationEvent animationEvent)
	{
		string stringParameter = animationEvent.stringParameter;
		Transform transform = base.transform.Find(stringParameter);
		if (transform)
		{
			this.Spawn(this.objectToSpawn, transform);
			this.PlayAudio(this.audioToPlay, transform);
			return;
		}
		this.Spawn(this.objectToSpawn, base.transform);
		this.PlayAudio(this.audioToPlay, base.transform);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0002C458 File Offset: 0x0002A658
	private void Spawn(GameObject obj, Transform targetTransform)
	{
		UnityEngine.Object.Instantiate<GameObject>(obj, targetTransform.position, targetTransform.rotation);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0002C46D File Offset: 0x0002A66D
	private void PlayAudio(SECTR_AudioCue cue, Transform location)
	{
		if (cue != null)
		{
			if (this.cueInstance.Active)
			{
				this.cueInstance.Stop(true);
			}
			this.cueInstance = SECTR_AudioSystem.Play(cue, location.position, false);
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0002C4A4 File Offset: 0x0002A6A4
	public void OnDestroy()
	{
		if (this.cueInstance.Active)
		{
			this.cueInstance.Stop(true);
		}
	}

	// Token: 0x04000855 RID: 2133
	public GameObject objectToSpawn;

	// Token: 0x04000856 RID: 2134
	public SECTR_AudioCue audioToPlay;

	// Token: 0x04000857 RID: 2135
	private SECTR_AudioCueInstance cueInstance;
}
