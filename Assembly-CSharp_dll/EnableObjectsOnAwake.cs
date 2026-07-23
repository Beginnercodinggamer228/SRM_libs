using System;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class EnableObjectsOnAwake : MonoBehaviour
{
	// Token: 0x06000A22 RID: 2594 RVA: 0x0002C354 File Offset: 0x0002A554
	public void Awake()
	{
		GameObject[] array = this.toEnable;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
	}

	// Token: 0x04000854 RID: 2132
	public GameObject[] toEnable;
}
