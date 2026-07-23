using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("SECTR/Stream/SECTR Hibernator")]
public class SECTR_Hibernator : MonoBehaviour
{
	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060003FA RID: 1018 RVA: 0x00018522 File Offset: 0x00016722
	public bool isHibernating
	{
		get
		{
			return this.hibernating;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060003FB RID: 1019 RVA: 0x0001852C File Offset: 0x0001672C
	// (remove) Token: 0x060003FC RID: 1020 RVA: 0x00018564 File Offset: 0x00016764
	public event SECTR_Hibernator.HibernateCallback Awoke;

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060003FD RID: 1021 RVA: 0x0001859C File Offset: 0x0001679C
	// (remove) Token: 0x060003FE RID: 1022 RVA: 0x000185D4 File Offset: 0x000167D4
	public event SECTR_Hibernator.HibernateCallback Hibernated;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060003FF RID: 1023 RVA: 0x0001860C File Offset: 0x0001680C
	// (remove) Token: 0x06000400 RID: 1024 RVA: 0x00018644 File Offset: 0x00016844
	public event SECTR_Hibernator.HibernateCallback HibernateUpdate;

	// Token: 0x06000401 RID: 1025 RVA: 0x0001867C File Offset: 0x0001687C
	private void OnEnable()
	{
		this.cachedMember = base.GetComponent<SECTR_Member>();
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.RegisterHibernator(this);
			this.RegisterMember();
			this.OnUpdate();
		}
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000186D0 File Offset: 0x000168D0
	private void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.DeregisterHibernator(this);
			this.DeregisterMember();
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000186D0 File Offset: 0x000168D0
	private void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.DeregisterHibernator(this);
			this.DeregisterMember();
		}
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00018708 File Offset: 0x00016908
	private void RegisterMember()
	{
		SECTR_Member component = base.GetComponent<SECTR_Member>();
		if (component != null && SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.RegisterMember(component);
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00018754 File Offset: 0x00016954
	private void DeregisterMember()
	{
		SECTR_Member component = base.GetComponent<SECTR_Member>();
		if (component != null && SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.DeregisterMember(component);
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x000187A0 File Offset: 0x000169A0
	public void OnUpdate()
	{
		if (this.setId == SECTR_Sector.SectorSetId.UNSET)
		{
			this.setId = SECTR_Sector.GetSectorSetForPos(base.transform.position);
		}
		bool flag = SECTR_Sector.IsCurrSectorSet(this.setId);
		bool flag2 = !flag;
		int count = this.cachedMember.sectors.Count;
		bool flag3 = count > 0 || !flag;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sectr_Sector = this.cachedMember.sectors[i];
			if (sectr_Sector.isFrozen)
			{
				flag2 = true;
			}
			if (!sectr_Sector.isHibernating)
			{
				flag3 = false;
			}
		}
		if ((flag2 || flag3) && !this.hibernating)
		{
			this._Hibernate();
		}
		else if (!flag2 && !flag3 && this.hibernating)
		{
			this._WakeUp();
		}
		if (this.hibernating && this.HibernateUpdate != null)
		{
			this.HibernateUpdate();
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00018872 File Offset: 0x00016A72
	private void _WakeUp()
	{
		if (this.hibernating)
		{
			this.hibernating = false;
			this.RegisterMember();
			this._UpdateComponents();
			if (this.Awoke != null)
			{
				this.Awoke();
			}
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x000188A2 File Offset: 0x00016AA2
	private void _Hibernate()
	{
		if (!this.hibernating)
		{
			this.hibernating = true;
			this.DeregisterMember();
			this._UpdateComponents();
			if (this.Hibernated != null)
			{
				this.Hibernated();
			}
		}
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x000188D4 File Offset: 0x00016AD4
	private void _UpdateComponents()
	{
		if (this.HibernateBehaviors)
		{
			Behaviour[] array = this.HibernateChildren ? base.GetComponentsInChildren<Behaviour>() : base.GetComponents<Behaviour>();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				Behaviour behaviour = array[i];
				if (behaviour.GetType() != typeof(SECTR_Hibernator) && behaviour.GetType() != typeof(SECTR_Member))
				{
					behaviour.enabled = !this.hibernating;
				}
			}
		}
		if (this.HibernateRigidBodies)
		{
			Rigidbody[] array2 = this.HibernateChildren ? base.GetComponentsInChildren<Rigidbody>() : base.GetComponents<Rigidbody>();
			int num2 = array2.Length;
			for (int j = 0; j < num2; j++)
			{
				Rigidbody rigidbody = array2[j];
				if (this.hibernating)
				{
					this.vel = rigidbody.velocity;
					this.angularVel = rigidbody.angularVelocity;
					this.kinematic = rigidbody.isKinematic;
					rigidbody.Sleep();
					rigidbody.isKinematic = true;
				}
				else if (rigidbody.IsSleeping())
				{
					rigidbody.isKinematic = this.kinematic;
					rigidbody.WakeUp();
					rigidbody.velocity = this.vel;
					rigidbody.angularVelocity = this.angularVel;
				}
			}
		}
		if (this.HibernateColliders)
		{
			Collider[] array3 = this.HibernateChildren ? base.GetComponentsInChildren<Collider>() : base.GetComponents<Collider>();
			int num3 = array3.Length;
			for (int k = 0; k < num3; k++)
			{
				array3[k].enabled = !this.hibernating;
			}
		}
		if (this.HibernateRenderers)
		{
			Renderer[] array4 = this.HibernateChildren ? base.GetComponentsInChildren<Renderer>() : base.GetComponents<Renderer>();
			int num4 = array4.Length;
			for (int l = 0; l < num4; l++)
			{
				array4[l].enabled = !this.hibernating;
			}
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00018AA3 File Offset: 0x00016CA3
	internal void RecheckHibernation()
	{
		this.OnUpdate();
	}

	// Token: 0x040003EA RID: 1002
	private bool hibernating;

	// Token: 0x040003EB RID: 1003
	private SECTR_Member cachedMember;

	// Token: 0x040003EC RID: 1004
	[SECTR_ToolTip("Hibernate components on children as well as ones on this game object.")]
	public bool HibernateChildren = true;

	// Token: 0x040003ED RID: 1005
	[SECTR_ToolTip("Disable Behavior components during hibernation.")]
	public bool HibernateBehaviors = true;

	// Token: 0x040003EE RID: 1006
	[SECTR_ToolTip("Disable Collder components during hibernation.")]
	public bool HibernateColliders = true;

	// Token: 0x040003EF RID: 1007
	[SECTR_ToolTip("Disable RigidBody components during hibernation.")]
	public bool HibernateRigidBodies = true;

	// Token: 0x040003F0 RID: 1008
	[SECTR_ToolTip("Hide Render components during hibernation.")]
	public bool HibernateRenderers = true;

	// Token: 0x040003F4 RID: 1012
	private Vector3 vel;

	// Token: 0x040003F5 RID: 1013
	private Vector3 angularVel;

	// Token: 0x040003F6 RID: 1014
	private bool kinematic;

	// Token: 0x040003F7 RID: 1015
	private SECTR_Sector.SectorSetId setId = SECTR_Sector.SectorSetId.UNSET;

	// Token: 0x020000AE RID: 174
	// (Invoke) Token: 0x0600040D RID: 1037
	public delegate void HibernateCallback();
}
