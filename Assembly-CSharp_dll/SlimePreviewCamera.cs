using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class SlimePreviewCamera : MonoBehaviour
{
	// Token: 0x060019C3 RID: 6595 RVA: 0x00064A0C File Offset: 0x00062C0C
	private void Update()
	{
		if (this.target != null && this.zoomControlsEnabled)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			this.cam.transform.RotateAround(this.target.position, Vector3.up, -1f * axis * this.angleSpeed * Time.deltaTime);
			Vector3 b = axis2 * (this.target.position - this.cam.transform.position).normalized * this.moveSpeed * Time.deltaTime;
			float num = Vector3.Distance(this.target.position, this.cam.transform.position + b);
			if (num >= this.minDistance && num <= this.maxDistance)
			{
				this.cam.transform.position += b;
			}
			this.cam.transform.LookAt(this.target.position + this.lookOffset);
		}
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x00064B3C File Offset: 0x00062D3C
	public void ResetCamToTarget(Transform target)
	{
		this.target = target;
		this.cam.transform.position = target.position + this.cameraOffset;
		this.cam.transform.LookAt(target);
	}

	// Token: 0x04001963 RID: 6499
	public Camera cam;

	// Token: 0x04001964 RID: 6500
	public Transform target;

	// Token: 0x04001965 RID: 6501
	public Vector3 lookOffset;

	// Token: 0x04001966 RID: 6502
	public float angleSpeed = 120f;

	// Token: 0x04001967 RID: 6503
	public float moveSpeed = 20f;

	// Token: 0x04001968 RID: 6504
	public float minDistance = 1.25f;

	// Token: 0x04001969 RID: 6505
	public float maxDistance = 10f;

	// Token: 0x0400196A RID: 6506
	public Vector3 cameraOffset;

	// Token: 0x0400196B RID: 6507
	public bool zoomControlsEnabled = true;
}
