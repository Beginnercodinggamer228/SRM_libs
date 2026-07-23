using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000712 RID: 1810
public class GingerPatchNode : IdHandler
{
	// Token: 0x060025C7 RID: 9671 RVA: 0x00090DA2 File Offset: 0x0008EFA2
	public void Awake()
	{
		this.jointTransform = this.spawnJoint.transform;
		GingerPatchNode.allGingerPatches.Add(this);
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
		this.zoneDirector.Register(this);
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x00090DD8 File Offset: 0x0008EFD8
	public void OnDestroy()
	{
		GingerPatchNode.allGingerPatches.Remove(this);
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x00090DE6 File Offset: 0x0008EFE6
	public void Start()
	{
		if (this.spawnJoint.connectedBody == null)
		{
			this.HidePatch();
		}
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x00090E01 File Offset: 0x0008F001
	public void Grow()
	{
		this.Grow(SRBehaviour.InstantiateActor(this.gingerPrefab, this.zoneDirector.regionSetId, this.spawnJoint.transform.position, this.spawnJoint.transform.rotation, false));
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x00090E40 File Offset: 0x0008F040
	public void Grow(GameObject ginger)
	{
		this.bed.SetActive(true);
		this.spawnJoint.gameObject.SetActive(true);
		if (this.spawnJoint.connectedBody != null)
		{
			Destroyer.DestroyActor(this.spawnJoint.connectedBody.gameObject, "GingerPatchNode.Grow", false);
		}
		ginger.GetComponent<ResourceCycle>().Attach(this.spawnJoint, null, new ResourceCycle.DetachmentEvent(this.Harvested));
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x00090EB8 File Offset: 0x0008F0B8
	public void Harvested()
	{
		this.HidePatch();
		if (this.spawnJoint.connectedBody != null)
		{
			Destroyer.DestroyActor(this.spawnJoint.connectedBody.gameObject, "GingerPatchNode.Harvested", false);
			this.spawnJoint.connectedBody = null;
		}
		SRBehaviour.SpawnAndPlayFX(this.pulledFx, this.jointTransform.position, this.jointTransform.rotation);
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x00090F28 File Offset: 0x0008F128
	public void HidePatchAndReset()
	{
		this.HidePatch();
		if (this.spawnJoint.connectedBody != null)
		{
			Destroyer.DestroyActor(this.spawnJoint.connectedBody.gameObject, "GingerPatchNode.Reset", false);
			this.spawnJoint.connectedBody = null;
			SRBehaviour.SpawnAndPlayFX(this.disappearFx, this.jointTransform.position, this.jointTransform.rotation);
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x00090F97 File Offset: 0x0008F197
	private void HidePatch()
	{
		this.bed.SetActive(false);
		this.spawnJoint.gameObject.SetActive(false);
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x00090FB6 File Offset: 0x0008F1B6
	protected override string IdPrefix()
	{
		return "gingerPatch";
	}

	// Token: 0x04002535 RID: 9525
	public GameObject bed;

	// Token: 0x04002536 RID: 9526
	public FixedJoint spawnJoint;

	// Token: 0x04002537 RID: 9527
	public GameObject gingerPrefab;

	// Token: 0x04002538 RID: 9528
	public GameObject pulledFx;

	// Token: 0x04002539 RID: 9529
	public GameObject disappearFx;

	// Token: 0x0400253A RID: 9530
	public static List<GingerPatchNode> allGingerPatches = new List<GingerPatchNode>();

	// Token: 0x0400253B RID: 9531
	private Transform jointTransform;

	// Token: 0x0400253C RID: 9532
	private ZoneDirector zoneDirector;
}
