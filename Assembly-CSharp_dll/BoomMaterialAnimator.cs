using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200039D RID: 925
public class BoomMaterialAnimator : SRBehaviour
{
	// Token: 0x0600134F RID: 4943 RVA: 0x0004B36C File Offset: 0x0004956C
	public void Awake()
	{
		List<Material> list = new List<Material>();
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			if (renderer.sharedMaterial.HasProperty("_CrackAmount"))
			{
				list.Add(renderer.material);
			}
		}
		this.boomMaterials = list.ToArray();
		this.boomSlime = base.GetComponent<BoomMaterialAnimator.BoomMaterialInformer>();
		this.Update();
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x0004B3D4 File Offset: 0x000495D4
	public void Update()
	{
		float readiness = this.boomSlime.GetReadiness();
		float recoveriness = this.boomSlime.GetRecoveriness();
		if (Mathf.Abs(recoveriness - this.lastRecoveriness) >= 0.05f || Mathf.Abs(readiness - this.lastReadiness) >= 0.05f)
		{
			float num = (recoveriness > 0.4f) ? 1f : (recoveriness * 2.5f);
			foreach (Material material in this.boomMaterials)
			{
				material.SetFloat("_CrackAmount", (1f - num) * Mathf.Lerp(0.1f, 1f, readiness));
				material.SetFloat("_Char", num);
			}
			this.lastRecoveriness = recoveriness;
			this.lastReadiness = readiness;
		}
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0004B494 File Offset: 0x00049694
	public void OnDestroy()
	{
		Material[] array = this.boomMaterials;
		for (int i = 0; i < array.Length; i++)
		{
			Destroyer.Destroy(array[i], "BoomMaterialAnimator.OnDestroy");
		}
	}

	// Token: 0x04001208 RID: 4616
	private Material[] boomMaterials;

	// Token: 0x04001209 RID: 4617
	private BoomMaterialAnimator.BoomMaterialInformer boomSlime;

	// Token: 0x0400120A RID: 4618
	private float lastReadiness = float.PositiveInfinity;

	// Token: 0x0400120B RID: 4619
	private float lastRecoveriness = float.PositiveInfinity;

	// Token: 0x0400120C RID: 4620
	private const string CRACK_AMOUNT_PROP = "_CrackAmount";

	// Token: 0x0400120D RID: 4621
	private const string CHAR_PROP = "_Char";

	// Token: 0x0200039E RID: 926
	public interface BoomMaterialInformer
	{
		// Token: 0x06001353 RID: 4947
		float GetReadiness();

		// Token: 0x06001354 RID: 4948
		float GetRecoveriness();
	}
}
