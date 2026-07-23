using System;
using UnityEngine;

// Token: 0x020007E5 RID: 2021
public class SENBDLCameraAnimation : MonoBehaviour
{
	// Token: 0x06002A4C RID: 10828 RVA: 0x0009E9B0 File Offset: 0x0009CBB0
	private void Start()
	{
		this.randomRotation = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		this.randomRotation = Vector3.Normalize(this.randomRotation);
		this.randomModRotation = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		this.randomModRotation = Vector3.Normalize(this.randomModRotation);
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x0009EA50 File Offset: 0x0009CC50
	private void Update()
	{
		float d = 15f + Mathf.Pow(Mathf.Cos(Time.time * 3.1415927f / 15f) * 0.5f + 0.5f, 3f) * 35f;
		Vector3 position = Quaternion.Euler(this.randomRotation * Time.time * 25f) * (Vector3.up * d);
		base.transform.position = position;
		base.transform.LookAt(Vector3.zero);
	}

	// Token: 0x0400295E RID: 10590
	private Vector3 randomRotation;

	// Token: 0x0400295F RID: 10591
	private Vector3 randomModRotation;
}
