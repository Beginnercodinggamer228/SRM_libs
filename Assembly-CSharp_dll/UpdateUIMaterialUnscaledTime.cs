using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006B0 RID: 1712
public class UpdateUIMaterialUnscaledTime : MonoBehaviour
{
	// Token: 0x060023AA RID: 9130 RVA: 0x0008A31F File Offset: 0x0008851F
	public void Awake()
	{
		this.unscaledTimeVarId = Shader.PropertyToID("_UnscaledTime");
		this.graphics = base.GetComponents<Graphic>();
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x0008A340 File Offset: 0x00088540
	public void Update()
	{
		Graphic[] array = this.graphics;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].materialForRendering.SetFloat(this.unscaledTimeVarId, Time.unscaledTime);
		}
	}

	// Token: 0x040022D4 RID: 8916
	public Material[] mats;

	// Token: 0x040022D5 RID: 8917
	private int unscaledTimeVarId;

	// Token: 0x040022D6 RID: 8918
	private Graphic[] graphics;
}
