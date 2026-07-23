using System;
using UnityEngine;

// Token: 0x020007C8 RID: 1992
public class UnderwaterFog : MonoBehaviour
{
	// Token: 0x060029B5 RID: 10677 RVA: 0x0009CD03 File Offset: 0x0009AF03
	private void Start()
	{
		this.normalColor = RenderSettings.fogColor;
		this.normalDensity = RenderSettings.fogDensity;
		this.underwaterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x0009CD3C File Offset: 0x0009AF3C
	private void Update()
	{
		if (base.GetComponent<Collider>().bounds.Contains(Camera.main.transform.position))
		{
			this.SetUnderwater();
			return;
		}
		this.SetNormal();
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x0009CD7A File Offset: 0x0009AF7A
	private void SetNormal()
	{
		if (this.isUnderwater)
		{
			RenderSettings.fogColor = this.normalColor;
			RenderSettings.fogDensity = this.normalDensity;
			this.isUnderwater = false;
		}
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x0009CDA1 File Offset: 0x0009AFA1
	private void SetUnderwater()
	{
		if (!this.isUnderwater)
		{
			RenderSettings.fogColor = this.underwaterColor;
			RenderSettings.fogDensity = 0.05f;
			this.isUnderwater = true;
		}
	}

	// Token: 0x040028EF RID: 10479
	private Color normalColor;

	// Token: 0x040028F0 RID: 10480
	private float normalDensity;

	// Token: 0x040028F1 RID: 10481
	private Color underwaterColor;

	// Token: 0x040028F2 RID: 10482
	private bool isUnderwater;
}
