using System;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class MultiSegmentHook : SRBehaviour, Attachment
{
	// Token: 0x060015B9 RID: 5561 RVA: 0x00054744 File Offset: 0x00052944
	public void Init(GameObject source, GameObject target, Vector3 attachPoint, bool causeFear, float intermediateHeight)
	{
		this.bend.transform.position = source.transform.position + Vector3.up * intermediateHeight;
		this.bendBody = this.bend.GetComponent<Rigidbody>();
		this.segment1.Init(source, this.bend, Vector3.zero, causeFear, 0f);
		this.segment2.Init(this.bend, target, attachPoint, causeFear, 0f);
		this.segment1.SetPauseRetract(true);
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x000547D2 File Offset: 0x000529D2
	public void OnDestroy()
	{
		Destroyer.Destroy(this.bend, "MultiSegmentHook.OnDestroy");
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000547E4 File Offset: 0x000529E4
	public void Update()
	{
		if (this.bendBody != null && this.bendBody.isKinematic && (this.segment2 == null || this.segment2.parentJoint == null || this.segment2.parentJoint.maxDistance <= 0f))
		{
			this.SegmentOneRetract();
		}
		if (this.segment1 == null || this.segment1.parentJoint == null || this.segment1.parentJoint.maxDistance <= 0f)
		{
			Destroyer.Destroy(base.gameObject, "MultiSegmentHook.Update");
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00054892 File Offset: 0x00052A92
	public void SegmentOneRetract()
	{
		this.bendBody.isKinematic = false;
		this.segment1.SetPauseRetract(false);
	}

	// Token: 0x040014B6 RID: 5302
	public TentacleHook segment1;

	// Token: 0x040014B7 RID: 5303
	public TentacleHook segment2;

	// Token: 0x040014B8 RID: 5304
	public GameObject bend;

	// Token: 0x040014B9 RID: 5305
	private Rigidbody bendBody;
}
