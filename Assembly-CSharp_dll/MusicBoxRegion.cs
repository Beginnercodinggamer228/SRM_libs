using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000325 RID: 805
public class MusicBoxRegion : SRBehaviour
{
	// Token: 0x06001106 RID: 4358 RVA: 0x000440A0 File Offset: 0x000422A0
	public void OnDisable()
	{
		foreach (SlimeEmotions slimeEmotions in this.currentEmotions)
		{
			if (slimeEmotions != null)
			{
				slimeEmotions.RemoveMusicBox(this);
				this.newEmotions.Add(slimeEmotions);
			}
		}
		List<SlimeEmotions> list = this.currentEmotions;
		list.Clear();
		this.currentEmotions = this.newEmotions;
		this.newEmotions = list;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x00044128 File Offset: 0x00042328
	public void OnTriggerEnter(Collider collider)
	{
		SlimeEmotions component = collider.gameObject.GetComponent<SlimeEmotions>();
		if (component != null)
		{
			component.AddMusicBox(this);
			this.currentEmotions.Add(component);
		}
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x00044160 File Offset: 0x00042360
	public void OnTriggerExit(Collider collider)
	{
		SlimeEmotions component = collider.gameObject.GetComponent<SlimeEmotions>();
		if (component != null)
		{
			component.RemoveMusicBox(this);
			this.currentEmotions.Remove(component);
		}
	}

	// Token: 0x04000FF4 RID: 4084
	public const float EXTRA_CALMING_FACTOR = 1f;

	// Token: 0x04000FF5 RID: 4085
	private List<SlimeEmotions> currentEmotions = new List<SlimeEmotions>();

	// Token: 0x04000FF6 RID: 4086
	private List<SlimeEmotions> newEmotions = new List<SlimeEmotions>();
}
