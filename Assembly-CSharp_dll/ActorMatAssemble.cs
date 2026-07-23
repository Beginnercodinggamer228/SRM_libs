using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class ActorMatAssemble : MonoBehaviour
{
	// Token: 0x06000480 RID: 1152 RVA: 0x0001D168 File Offset: 0x0001B368
	public void Update()
	{
		if (!this.assembleComplete)
		{
			this.assembleValue += Time.deltaTime / this.assembleDuration * (float)(this.assembleDirection ? 1 : -1);
			this.assembleValue = Mathf.Clamp(this.assembleValue, 0f, 1f);
			this.assembleComplete = (this.assembleValue == (float)(this.assembleDirection ? 1 : 0));
			foreach (GameObject gameObject in this.objectsToAssemble)
			{
				gameObject.SetActive(this.assembleValue != 0f);
				gameObject.GetComponent<Renderer>().material.SetFloat("_Assemble", this.assembleValue);
			}
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001D24C File Offset: 0x0001B44C
	public bool Assemble(bool direction)
	{
		this.assembleDirection = direction;
		this.assembleComplete = (this.assembleValue == (float)(this.assembleDirection ? 1 : 0));
		return !this.assembleComplete;
	}

	// Token: 0x040004A4 RID: 1188
	public List<GameObject> objectsToAssemble;

	// Token: 0x040004A5 RID: 1189
	public float assembleDuration = 1.5f;

	// Token: 0x040004A6 RID: 1190
	private float assembleValue;

	// Token: 0x040004A7 RID: 1191
	private bool assembleDirection;

	// Token: 0x040004A8 RID: 1192
	private bool assembleComplete;
}
