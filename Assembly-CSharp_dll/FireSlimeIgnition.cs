using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D7 RID: 983
public class FireSlimeIgnition : RegisteredActorBehaviour, Ignitable, LiquidConsumer, RegistryUpdateable, ControllerCollisionListener
{
	// Token: 0x06001467 RID: 5223 RVA: 0x0004F02D File Offset: 0x0004D22D
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0004F03F File Offset: 0x0004D23F
	public override void Start()
	{
		base.Start();
		this.ExtractFire();
		base.GetComponent<SlimeAppearanceApplicator>().OnAppearanceChanged += delegate(SlimeAppearance appearance)
		{
			this.ExtractFire();
		};
		this.Ignite(base.gameObject);
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0004F070 File Offset: 0x0004D270
	private void ExtractFire()
	{
		FireIndicatorMarker componentInChildren = base.GetComponentInChildren<FireIndicatorMarker>();
		if (componentInChildren != null)
		{
			this.fireFXObj = componentInChildren.gameObject;
			this.fireFXObj.SetActive(this.isIgnited);
		}
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0004F0AC File Offset: 0x0004D2AC
	public void OnCollisionEnter(Collision col)
	{
		if (this.isIgnited)
		{
			Ignitable component = col.gameObject.GetComponent<Ignitable>();
			if (component != null)
			{
				component.Ignite(base.gameObject);
			}
		}
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0004F0DC File Offset: 0x0004D2DC
	public void OnControllerCollision(GameObject gameObj)
	{
		if (this.isIgnited)
		{
			Ignitable component = gameObj.GetComponent<Ignitable>();
			if (component != null)
			{
				component.Ignite(base.gameObject);
			}
		}
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0004F108 File Offset: 0x0004D308
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.waterSources.Add(component);
			this.Extinguish();
		}
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0004F14C File Offset: 0x0004D34C
	public void OnTriggerExit(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.waterSources.Remove(component);
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0004F188 File Offset: 0x0004D388
	public void RegistryUpdate()
	{
		if (!this.isIgnited && this.timeDir.HasReached(this.reigniteAtTime))
		{
			this.Ignite(base.gameObject);
		}
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0004F1B4 File Offset: 0x0004D3B4
	public void Ignite(GameObject igniter)
	{
		this.waterSources.RemoveAll((LiquidSource w) => w == null || w.gameObject == null);
		if (this.waterSources.Count > 0)
		{
			return;
		}
		this.isIgnited = true;
		if (this.fireFXObj != null)
		{
			this.fireFXObj.SetActive(true);
		}
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0004F21C File Offset: 0x0004D41C
	public void Extinguish()
	{
		this.isIgnited = false;
		if (this.fireFXObj != null)
		{
			this.fireFXObj.SetActive(false);
		}
		this.reigniteAtTime = this.timeDir.HoursFromNow(0.5f);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x0004F255 File Offset: 0x0004D455
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.Extinguish();
		}
	}

	// Token: 0x04001335 RID: 4917
	private bool isIgnited;

	// Token: 0x04001336 RID: 4918
	private GameObject fireFXObj;

	// Token: 0x04001337 RID: 4919
	private double reigniteAtTime = double.PositiveInfinity;

	// Token: 0x04001338 RID: 4920
	private TimeDirector timeDir;

	// Token: 0x04001339 RID: 4921
	private List<LiquidSource> waterSources = new List<LiquidSource>();

	// Token: 0x0400133A RID: 4922
	private const float EXTINGUISH_HRS = 0.5f;
}
