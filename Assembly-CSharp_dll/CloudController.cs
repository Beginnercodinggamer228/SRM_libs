using System;
using UnityEngine;

// Token: 0x020006D9 RID: 1753
public class CloudController : MonoBehaviour, AmbianceDirector.DaynessListener
{
	// Token: 0x0600249A RID: 9370 RVA: 0x0008D1B8 File Offset: 0x0008B3B8
	public void Awake()
	{
		this.mat = base.GetComponent<Renderer>().material;
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x0008D1CB File Offset: 0x0008B3CB
	public void Start()
	{
		SRSingleton<SceneContext>.Instance.AmbianceDirector.RegisterDaynessListener(this);
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x0008D1DD File Offset: 0x0008B3DD
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.AmbianceDirector.UnregisterDaynessListener(this);
		}
		Destroyer.Destroy(this.mat, "CloudController.OnDestroy");
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x0008D20C File Offset: 0x0008B40C
	public void SetDayness(float dayness)
	{
		this.mat.SetFloat("_Dayness", dayness);
	}

	// Token: 0x0400238E RID: 9102
	private Material mat;
}
