using System;
using UnityEngine;

// Token: 0x0200006D RID: 109
[RequireComponent(typeof(Light))]
public class CFX_LightIntensityFade : MonoBehaviour
{
	// Token: 0x060001E0 RID: 480 RVA: 0x0000E458 File Offset: 0x0000C658
	private void Start()
	{
		this.baseIntensity = base.GetComponent<Light>().intensity;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000E46B File Offset: 0x0000C66B
	private void OnEnable()
	{
		this.p_lifetime = 0f;
		this.p_delay = this.delay;
		if (this.delay > 0f)
		{
			base.GetComponent<Light>().enabled = false;
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000E4A0 File Offset: 0x0000C6A0
	private void Update()
	{
		if (this.p_delay > 0f)
		{
			this.p_delay -= Time.deltaTime;
			if (this.p_delay <= 0f)
			{
				base.GetComponent<Light>().enabled = true;
			}
			return;
		}
		if (this.p_lifetime / this.duration < 1f)
		{
			base.GetComponent<Light>().intensity = Mathf.Lerp(this.baseIntensity, this.finalIntensity, this.p_lifetime / this.duration);
			this.p_lifetime += Time.deltaTime;
			return;
		}
		if (this.autodestruct)
		{
			Destroyer.Destroy(base.gameObject, "CFX_LightIntensityFade.Update");
		}
	}

	// Token: 0x04000240 RID: 576
	public float duration = 1f;

	// Token: 0x04000241 RID: 577
	public float delay;

	// Token: 0x04000242 RID: 578
	public float finalIntensity;

	// Token: 0x04000243 RID: 579
	private float baseIntensity;

	// Token: 0x04000244 RID: 580
	public bool autodestruct;

	// Token: 0x04000245 RID: 581
	private float p_lifetime;

	// Token: 0x04000246 RID: 582
	private float p_delay;
}
