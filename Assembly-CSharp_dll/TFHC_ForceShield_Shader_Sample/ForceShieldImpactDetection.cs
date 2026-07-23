using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x02000C2C RID: 3116
	public class ForceShieldImpactDetection : MonoBehaviour
	{
		// Token: 0x060057FF RID: 22527 RVA: 0x001097A3 File Offset: 0x001079A3
		private void Start()
		{
			this.mat = base.GetComponent<Renderer>().material;
		}

		// Token: 0x06005800 RID: 22528 RVA: 0x001097B8 File Offset: 0x001079B8
		private void Update()
		{
			if (this.hitTime > 0f)
			{
				this.hitTime -= Time.deltaTime * 1000f;
				if (this.hitTime < 0f)
				{
					this.hitTime = 0f;
				}
				this.mat.SetFloat("_HitTime", this.hitTime);
			}
		}

		// Token: 0x06005801 RID: 22529 RVA: 0x00109818 File Offset: 0x00107A18
		private void OnCollisionEnter(Collision collision)
		{
			foreach (ContactPoint contactPoint in collision.contacts)
			{
				this.mat.SetVector("_HitPosition", base.transform.InverseTransformPoint(contactPoint.point));
				this.hitTime = 500f;
				this.mat.SetFloat("_HitTime", this.hitTime);
			}
		}

		// Token: 0x0400437E RID: 17278
		private float hitTime;

		// Token: 0x0400437F RID: 17279
		private Material mat;
	}
}
