using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class CFX_Demo_RandomDirectionTranslate : MonoBehaviour
{
	// Token: 0x060001D1 RID: 465 RVA: 0x0000E198 File Offset: 0x0000C398
	private void Start()
	{
		this.dir = new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)).normalized;
		this.dir.Scale(this.axis);
		this.dir += this.baseDir;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000E210 File Offset: 0x0000C410
	private void Update()
	{
		base.transform.Translate(this.dir * this.speed * Time.deltaTime);
		if (this.gravity)
		{
			base.transform.Translate(Physics.gravity * Time.deltaTime);
		}
	}

	// Token: 0x0400022E RID: 558
	public float speed = 30f;

	// Token: 0x0400022F RID: 559
	public Vector3 baseDir = Vector3.zero;

	// Token: 0x04000230 RID: 560
	public Vector3 axis = Vector3.forward;

	// Token: 0x04000231 RID: 561
	public bool gravity;

	// Token: 0x04000232 RID: 562
	private Vector3 dir;
}
