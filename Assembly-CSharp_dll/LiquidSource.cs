using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public class LiquidSource : IdHandler<LiquidSourceModel>
{
	// Token: 0x0600264B RID: 9803 RVA: 0x0009281B File Offset: 0x00090A1B
	protected override string IdPrefix()
	{
		return "LiquidSource";
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x00092822 File Offset: 0x00090A22
	protected override GameModel.Unregistrant Register(GameModel game)
	{
		return game.LiquidSources.Register(this);
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x00092830 File Offset: 0x00090A30
	protected override void InitModel(LiquidSourceModel model)
	{
		model.pos = base.transform.position;
		model.isScaling = false;
		model.unitsFilled = 0f;
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x00092855 File Offset: 0x00090A55
	protected override void SetModel(LiquidSourceModel model)
	{
		this.model = model;
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x00092860 File Offset: 0x00090A60
	public void FixedUpdate()
	{
		if (!this.CountsAsUnderwater())
		{
			return;
		}
		List<Rigidbody> list = new List<Rigidbody>();
		foreach (Rigidbody rigidbody in this.floating.Keys)
		{
			if (this.ShouldRemoveBody(rigidbody))
			{
				list.Add(rigidbody);
				if (rigidbody != null)
				{
					this.UpdateFloatingReactors(rigidbody, false);
				}
			}
			else
			{
				this.Buoyancy(rigidbody);
			}
		}
		foreach (Rigidbody key in list)
		{
			this.floating.Remove(key);
		}
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x00092928 File Offset: 0x00090B28
	private bool ShouldRemoveBody(Rigidbody body)
	{
		if (body == null || !body.gameObject.activeInHierarchy)
		{
			return true;
		}
		foreach (Collider collider in body.GetComponents<Collider>())
		{
			if (!collider.isTrigger && collider.enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x0009297C File Offset: 0x00090B7C
	public void OnTriggerEnter(Collider collider)
	{
		Rigidbody floatingRigidBody = this.GetFloatingRigidBody(collider);
		if (floatingRigidBody != null && this.floating.Increment(floatingRigidBody) == 1)
		{
			this.UpdateFloatingReactors(floatingRigidBody, true);
		}
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000929B4 File Offset: 0x00090BB4
	public void OnTriggerExit(Collider collider)
	{
		Rigidbody floatingRigidBody = this.GetFloatingRigidBody(collider);
		if (floatingRigidBody != null && this.floating.Decrement(floatingRigidBody) == 0)
		{
			this.UpdateFloatingReactors(floatingRigidBody, false);
		}
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000929E8 File Offset: 0x00090BE8
	private Rigidbody GetFloatingRigidBody(Collider collider)
	{
		if (collider.isTrigger || !this.CountsAsUnderwater())
		{
			return null;
		}
		Identifiable componentInParent = collider.GetComponentInParent<Identifiable>();
		if (componentInParent == null || Identifiable.IsWater(componentInParent.id) || Identifiable.IsEcho(componentInParent.id) || Identifiable.IsEchoNote(componentInParent.id))
		{
			return null;
		}
		return collider.GetComponentInParent<Rigidbody>();
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x00092A46 File Offset: 0x00090C46
	public bool CountsAsUnderwater()
	{
		return this.waterTop != null;
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x00092A54 File Offset: 0x00090C54
	private void Buoyancy(Rigidbody body)
	{
		if (!this.CountsAsUnderwater())
		{
			return;
		}
		BuoyancyOffset component = body.GetComponent<BuoyancyOffset>();
		List<Vector3> list = new List<Vector3>();
		float num = 1f;
		if (component != null && component.centersOfBuoyancy.Count > 0)
		{
			foreach (Vector3 position in component.centersOfBuoyancy)
			{
				list.Add(body.transform.TransformPoint(position));
			}
			num = component.buoyancyFactor;
		}
		else
		{
			list.Add(body.transform.position);
		}
		foreach (Vector3 vector in list)
		{
			float num2 = this.waterTop.position.y - vector.y;
			if (num2 > 0f)
			{
				float num3 = num2 * this.floatForcePerDepth;
				float y = body.velocity.y;
				Vector3 force = -Physics.gravity * (num * (num3 - y * this.bounceDamp) / (float)list.Count);
				body.AddForceAtPosition(force, vector);
			}
		}
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x00092BA4 File Offset: 0x00090DA4
	public void OnDisable()
	{
		this.floating.Clear();
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x00092BB4 File Offset: 0x00090DB4
	private void UpdateFloatingReactors(Rigidbody body, bool isFloating)
	{
		foreach (FloatingReactor floatingReactor in body.GetComponentsInParent<FloatingReactor>())
		{
			if (floatingReactor != null)
			{
				floatingReactor.SetIsFloating(isFloating);
			}
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public virtual bool Available()
	{
		return true;
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void ConsumeLiquid()
	{
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public virtual bool ReplacesExistingLiquidAmmo()
	{
		return false;
	}

	// Token: 0x04002599 RID: 9625
	public Identifiable.Id liquidId;

	// Token: 0x0400259A RID: 9626
	[Tooltip("A position marking the top of the water at which objects should float.")]
	public Transform waterTop;

	// Token: 0x0400259B RID: 9627
	public float bounceDamp = 0.8f;

	// Token: 0x0400259C RID: 9628
	public float floatForcePerDepth = 10f;

	// Token: 0x0400259D RID: 9629
	private ReferenceCount<Rigidbody> floating = new ReferenceCount<Rigidbody>();

	// Token: 0x0400259E RID: 9630
	protected LiquidSourceModel model;
}
