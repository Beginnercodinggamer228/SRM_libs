using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class FashionPod : SRBehaviour
{
	// Token: 0x0600254C RID: 9548 RVA: 0x0008F3AF File Offset: 0x0008D5AF
	public void Awake()
	{
		this.fashionPrefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.fashionId);
		this.region = base.GetComponentInParent<Region>();
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x0008F3D8 File Offset: 0x0008D5D8
	public void Update()
	{
		if (this.fashionJoint != null && this.fashionJoint.connectedBody == null)
		{
			Destroyer.Destroy(this.fashionJoint, "FashionPod.Update");
			this.fashionJoint = null;
		}
		if (this.fashionJoint == null && !Physics.CheckSphere(this.fashionItemPos.position, 0.4f))
		{
			GameObject toAttach = SRBehaviour.InstantiateActor(this.fashionPrefab, this.region.setId, this.fashionItemPos.position, this.fashionItemPos.rotation, false);
			ConfigurableJoint configurableJoint = this.fashionItemPos.gameObject.AddComponent<ConfigurableJoint>();
			SafeJointReference.AttachSafely(toAttach, configurableJoint, true);
			configurableJoint.anchor = Vector3.zero;
			configurableJoint.autoConfigureConnectedAnchor = false;
			configurableJoint.connectedAnchor = Vector3.zero;
			SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
			softJointLimitSpring.damper = 0.2f;
			softJointLimitSpring.spring = 1000f;
			configurableJoint.xMotion = ConfigurableJointMotion.Limited;
			configurableJoint.yMotion = ConfigurableJointMotion.Limited;
			configurableJoint.zMotion = ConfigurableJointMotion.Limited;
			configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
			configurableJoint.linearLimitSpring = softJointLimitSpring;
			configurableJoint.angularXLimitSpring = softJointLimitSpring;
			configurableJoint.angularYZLimitSpring = softJointLimitSpring;
			configurableJoint.breakForce = 20f;
			this.fashionJoint = configurableJoint;
			this.fashionItemPos.transform.localRotation = Quaternion.Euler(Vector3.zero);
			if (this.spawnFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.spawnFX, this.fashionItemPos.position, this.fashionItemPos.rotation);
			}
		}
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x0008F566 File Offset: 0x0008D766
	public void FixedUpdate()
	{
		if (this.fashionJoint != null)
		{
			this.fashionItemPos.transform.Rotate(Vector3.up, 90f * Time.fixedDeltaTime);
		}
	}

	// Token: 0x04002432 RID: 9266
	public Transform fashionItemPos;

	// Token: 0x04002433 RID: 9267
	public Identifiable.Id fashionId;

	// Token: 0x04002434 RID: 9268
	public GameObject spawnFX;

	// Token: 0x04002435 RID: 9269
	private GameObject fashionPrefab;

	// Token: 0x04002436 RID: 9270
	private Joint fashionJoint;

	// Token: 0x04002437 RID: 9271
	private Region region;

	// Token: 0x04002438 RID: 9272
	private const float CLEAR_RAD = 0.4f;
}
