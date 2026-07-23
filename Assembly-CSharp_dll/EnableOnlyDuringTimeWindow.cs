using System;
using UnityEngine;

// Token: 0x020006F5 RID: 1781
public class EnableOnlyDuringTimeWindow : MonoBehaviour
{
	// Token: 0x06002521 RID: 9505 RVA: 0x0008E9D1 File Offset: 0x0008CBD1
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x0008E9E4 File Offset: 0x0008CBE4
	public void Update()
	{
		float num = this.timeDir.CurrHour();
		bool active = (this.startHour <= num && num <= this.endHour) || (this.startHour > this.endHour && (num >= this.startHour || num <= this.endHour));
		GameObject[] array = this.toEnable;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(active);
		}
	}

	// Token: 0x040023FF RID: 9215
	public float startHour;

	// Token: 0x04002400 RID: 9216
	public float endHour;

	// Token: 0x04002401 RID: 9217
	public GameObject[] toEnable;

	// Token: 0x04002402 RID: 9218
	private TimeDirector timeDir;
}
