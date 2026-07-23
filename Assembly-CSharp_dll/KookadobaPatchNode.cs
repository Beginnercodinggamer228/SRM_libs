using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class KookadobaPatchNode : SRBehaviour, KookadobaNodeModel.Participant
{
	// Token: 0x06002638 RID: 9784 RVA: 0x000925A0 File Offset: 0x000907A0
	public void Awake()
	{
		this.jointTransform = this.spawnJoint.transform;
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
		this.zoneDirector.Register(this);
		SRSingleton<SceneContext>.Instance.GameModel.RegisterKookadobaNode(this);
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000925DB File Offset: 0x000907DB
	public void Start()
	{
		if (this.spawnJoint.connectedBody == null)
		{
			this.HidePatch();
		}
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x0006868D File Offset: 0x0006688D
	public void InitModel(KookadobaNodeModel model)
	{
		model.pos = base.transform.position;
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x00003296 File Offset: 0x00001496
	public void SetModel(KookadobaNodeModel model)
	{
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x000925F8 File Offset: 0x000907F8
	public void Grow()
	{
		if (this.spawnJoint.connectedBody != null)
		{
			ResourceCycle component = this.spawnJoint.connectedBody.GetComponent<ResourceCycle>();
			if (component != null)
			{
				component.UpdateToNow();
			}
			if (this.spawnJoint.connectedBody != null)
			{
				return;
			}
		}
		this.Grow(SRBehaviour.InstantiateActor(this.kookadobaPrefab, this.zoneDirector.regionSetId, this.spawnJoint.transform.position, this.spawnJoint.transform.rotation, false));
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x0009268C File Offset: 0x0009088C
	public void Grow(GameObject kookadoba)
	{
		this.bed.SetActive(true);
		this.spawnJoint.gameObject.SetActive(true);
		if (this.spawnJoint.connectedBody != null)
		{
			Destroyer.DestroyActor(this.spawnJoint.connectedBody.gameObject, "KookadobaPatchNode.Grow", false);
		}
		kookadoba.GetComponent<ResourceCycle>().Attach(this.spawnJoint, null, new ResourceCycle.DetachmentEvent(this.Harvested));
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x00092704 File Offset: 0x00090904
	public void Harvested()
	{
		this.HidePatch();
		if (this.spawnJoint.connectedBody != null)
		{
			Destroyer.Destroy(this.spawnJoint.connectedBody.gameObject, "KookadobaPatchNode.Harvested");
			this.spawnJoint.connectedBody = null;
		}
		if (base.gameObject.activeInHierarchy)
		{
			SRBehaviour.SpawnAndPlayFX(this.pulledFx, this.jointTransform.position, this.jointTransform.rotation);
		}
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x0009277F File Offset: 0x0009097F
	private void HidePatch()
	{
		this.bed.SetActive(false);
		this.spawnJoint.gameObject.SetActive(false);
	}

	// Token: 0x0400258D RID: 9613
	public GameObject bed;

	// Token: 0x0400258E RID: 9614
	public FixedJoint spawnJoint;

	// Token: 0x0400258F RID: 9615
	public GameObject kookadobaPrefab;

	// Token: 0x04002590 RID: 9616
	public GameObject pulledFx;

	// Token: 0x04002591 RID: 9617
	public GameObject disappearFx;

	// Token: 0x04002592 RID: 9618
	private Transform jointTransform;

	// Token: 0x04002593 RID: 9619
	private ZoneDirector zoneDirector;
}
