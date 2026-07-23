using System;
using UnityEngine;

// Token: 0x020007E7 RID: 2023
public class SENBDLGlowingOrbitingCube : MonoBehaviour
{
	// Token: 0x06002A4F RID: 10831 RVA: 0x0009EAE2 File Offset: 0x0009CCE2
	private Vector3 Vec3(float x)
	{
		return new Vector3(x, x, x);
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x0009EAEC File Offset: 0x0009CCEC
	private void Start()
	{
		base.transform.localScale = this.Vec3(1.5f);
		this.pulseSpeed = UnityEngine.Random.Range(4f, 8f);
		this.phase = UnityEngine.Random.Range(0f, 6.2831855f);
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x0009EB3C File Offset: 0x0009CD3C
	private void Update()
	{
		Color color = SENBDLGlobal.mainCube.glowColor;
		color.r = 1f - color.r;
		color.g = 1f - color.g;
		color.b = 1f - color.b;
		color = Color.Lerp(color, Color.white, 0.1f);
		color *= Mathf.Pow(Mathf.Sin(Time.time * this.pulseSpeed + this.phase) * 0.49f + 0.51f, 2f);
		base.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
		base.GetComponent<Light>().color = color;
	}

	// Token: 0x04002963 RID: 10595
	private float pulseSpeed;

	// Token: 0x04002964 RID: 10596
	private float phase;
}
