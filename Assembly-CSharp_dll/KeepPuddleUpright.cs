using System;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class KeepPuddleUpright : KeepUpright, RegistryFixedUpdateable, RegistryLateUpdateable
{
	// Token: 0x06001568 RID: 5480 RVA: 0x000533C8 File Offset: 0x000515C8
	public override void Start()
	{
		base.Start();
		this.plexer = base.GetComponent<SlimeSubbehaviourPlexer>();
		this.toggleOnGrounded = base.GetComponentsInChildren<EnableBasedOnGrounded>(true);
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += delegate(SlimeAppearance appearance)
		{
			this.toggleOnGrounded = base.GetComponentsInChildren<EnableBasedOnGrounded>(true);
		};
		this.slimeRoot = base.transform.Find("prefab_slimeBase/bone_root/bone_slime");
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x0005342D File Offset: 0x0005162D
	public void RegistryLateUpdate()
	{
		if (this.slimeRoot != null)
		{
			this.slimeRoot.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00053448 File Offset: 0x00051648
	public override void RegistryFixedUpdate()
	{
		if (this.plexer == null)
		{
			return;
		}
		bool flag = this.plexer.IsGrounded();
		if (flag)
		{
			RaycastHit raycastHit = this.plexer.GroundHit();
			if (raycastHit.rigidbody == null)
			{
				base.DoUpright(raycastHit.normal);
			}
			else
			{
				base.DoUpright(Vector3.up);
			}
		}
		else
		{
			base.DoUpright(Vector3.up);
		}
		foreach (EnableBasedOnGrounded enableBasedOnGrounded in this.toggleOnGrounded)
		{
			if (enableBasedOnGrounded != null)
			{
				enableBasedOnGrounded.gameObject.SetActive(enableBasedOnGrounded.enableOnGrounded ^ flag);
			}
		}
	}

	// Token: 0x0400145D RID: 5213
	private EnableBasedOnGrounded[] toggleOnGrounded;

	// Token: 0x0400145E RID: 5214
	private SlimeSubbehaviourPlexer plexer;

	// Token: 0x0400145F RID: 5215
	private Transform slimeRoot;

	// Token: 0x04001460 RID: 5216
	private SlimeAppearanceApplicator slimeAppearanceApplicator;
}
