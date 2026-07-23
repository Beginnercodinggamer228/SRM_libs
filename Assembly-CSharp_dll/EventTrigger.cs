using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class EventTrigger : MonoBehaviour
{
	// Token: 0x06000A24 RID: 2596 RVA: 0x0002C37F File Offset: 0x0002A57F
	public void PrintEvent(string LOG)
	{
		Debug.Log(LOG);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00003296 File Offset: 0x00001496
	public void PlayAudio(AudioClip AUD)
	{
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0002C388 File Offset: 0x0002A588
	public void SpawnObject(AnimationEvent animationEvent)
	{
		string stringParameter = animationEvent.stringParameter;
		GameObject original = (GameObject)animationEvent.objectReferenceParameter;
		Transform transform = base.transform.Find(stringParameter);
		if (transform)
		{
			UnityEngine.Object.Instantiate<GameObject>(original, transform.position, transform.rotation);
			return;
		}
		UnityEngine.Object.Instantiate<GameObject>(original, base.transform.position, base.transform.rotation);
	}
}
