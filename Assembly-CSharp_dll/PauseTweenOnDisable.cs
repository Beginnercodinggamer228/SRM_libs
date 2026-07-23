using System;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class PauseTweenOnDisable : MonoBehaviour
{
	// Token: 0x06000CC5 RID: 3269 RVA: 0x00034954 File Offset: 0x00032B54
	public void OnEnable()
	{
		if (this.tween != null && this.tween.IsActive() && !this.tween.IsComplete())
		{
			this.tween.Play<Tween>();
		}
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x00034984 File Offset: 0x00032B84
	public void OnDisable()
	{
		if (this.tween != null && this.tween.IsActive() && !this.tween.IsComplete())
		{
			this.tween.Pause<Tween>();
		}
	}

	// Token: 0x04000B94 RID: 2964
	public Tween tween;
}
