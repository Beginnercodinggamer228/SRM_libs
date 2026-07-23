using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000124 RID: 292
[RequireComponent(typeof(AttachFashions))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(DroneNoClip))]
[RequireComponent(typeof(DroneSubbehaviourPlexer))]
[RequireComponent(typeof(KeepUpright))]
[RequireComponent(typeof(RegionMember))]
[RequireComponent(typeof(Rigidbody))]
public class Drone : SRBehaviour, GadgetModel.Participant
{
	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600063B RID: 1595 RVA: 0x00022740 File Offset: 0x00020940
	// (set) Token: 0x0600063C RID: 1596 RVA: 0x00022748 File Offset: 0x00020948
	public AttachFashions fashion { get; private set; }

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x0600063D RID: 1597 RVA: 0x00022751 File Offset: 0x00020951
	// (set) Token: 0x0600063E RID: 1598 RVA: 0x00022759 File Offset: 0x00020959
	public DroneAmmo ammo { get; private set; }

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x0600063F RID: 1599 RVA: 0x00022762 File Offset: 0x00020962
	// (set) Token: 0x06000640 RID: 1600 RVA: 0x0002276A File Offset: 0x0002096A
	public DroneAnimator animator { get; private set; }

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000641 RID: 1601 RVA: 0x00022773 File Offset: 0x00020973
	// (set) Token: 0x06000642 RID: 1602 RVA: 0x0002277B File Offset: 0x0002097B
	public DroneAudioOnActive onActiveCue { get; private set; }

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000643 RID: 1603 RVA: 0x00022784 File Offset: 0x00020984
	// (set) Token: 0x06000644 RID: 1604 RVA: 0x0002278C File Offset: 0x0002098C
	public DroneGadget gadget { get; private set; }

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000645 RID: 1605 RVA: 0x00022795 File Offset: 0x00020995
	public DroneMetadata metadata
	{
		get
		{
			return this.gadget.metadata;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000646 RID: 1606 RVA: 0x000227A2 File Offset: 0x000209A2
	// (set) Token: 0x06000647 RID: 1607 RVA: 0x000227AA File Offset: 0x000209AA
	public DroneMovement movement { get; private set; }

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000648 RID: 1608 RVA: 0x000227B3 File Offset: 0x000209B3
	// (set) Token: 0x06000649 RID: 1609 RVA: 0x000227BB File Offset: 0x000209BB
	public DroneNetwork network { get; private set; }

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x0600064A RID: 1610 RVA: 0x000227C4 File Offset: 0x000209C4
	// (set) Token: 0x0600064B RID: 1611 RVA: 0x000227CC File Offset: 0x000209CC
	public DroneNoClip noClip { get; private set; }

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x0600064C RID: 1612 RVA: 0x000227D5 File Offset: 0x000209D5
	public DroneStation station
	{
		get
		{
			return this.gadget.station;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x0600064D RID: 1613 RVA: 0x000227E2 File Offset: 0x000209E2
	// (set) Token: 0x0600064E RID: 1614 RVA: 0x000227EA File Offset: 0x000209EA
	public DroneSubbehaviourPlexer plexer { get; private set; }

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600064F RID: 1615 RVA: 0x000227F3 File Offset: 0x000209F3
	// (set) Token: 0x06000650 RID: 1616 RVA: 0x000227FB File Offset: 0x000209FB
	public KeepUpright upright { get; private set; }

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000651 RID: 1617 RVA: 0x00022804 File Offset: 0x00020A04
	public Region region
	{
		get
		{
			return this.gadget.region;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000652 RID: 1618 RVA: 0x00022811 File Offset: 0x00020A11
	// (set) Token: 0x06000653 RID: 1619 RVA: 0x00022819 File Offset: 0x00020A19
	public RegionMember regionMember { get; private set; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x00022822 File Offset: 0x00020A22
	// (set) Token: 0x06000655 RID: 1621 RVA: 0x0002282A File Offset: 0x00020A2A
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x00022833 File Offset: 0x00020A33
	// (set) Token: 0x06000657 RID: 1623 RVA: 0x0002283B File Offset: 0x00020A3B
	public TrailRenderer trail { get; private set; }

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000658 RID: 1624 RVA: 0x00022844 File Offset: 0x00020A44
	// (remove) Token: 0x06000659 RID: 1625 RVA: 0x0002287C File Offset: 0x00020A7C
	public event Drone.OnDestroyed onDestroyed;

	// Token: 0x0600065A RID: 1626 RVA: 0x000228B4 File Offset: 0x00020AB4
	public void Awake()
	{
		this.gadget = base.GetComponentInParent<DroneGadget>();
		this.animator = base.GetComponentInChildren<DroneAnimator>();
		this.trail = base.GetComponentInChildren<TrailRenderer>();
		this.plexer = base.GetComponent<DroneSubbehaviourPlexer>();
		this.movement = base.GetComponent<DroneMovement>();
		this.fashion = base.GetComponent<AttachFashions>();
		this.upright = base.GetComponent<KeepUpright>();
		this.noClip = base.GetComponent<DroneNoClip>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.onActiveCue = this.SFX(this.metadata.onActiveCue);
		this.ammo = new DroneAmmo();
		this.regionMember.regionsChanged += this.OnRegionsChanged;
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00022974 File Offset: 0x00020B74
	public void OnDestroy()
	{
		this.regionMember.regionsChanged -= this.OnRegionsChanged;
		if (this.network != null)
		{
			this.network.Deregister(this);
			this.network = null;
		}
		if (this.onDestroyed != null)
		{
			this.onDestroyed();
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000229D0 File Offset: 0x00020BD0
	public void InitModel(GadgetModel model)
	{
		DroneModel droneModel = (DroneModel)model;
		this.ammo.InitModel(droneModel.ammo);
		droneModel.position = base.transform.position;
		droneModel.rotation = base.transform.rotation;
		droneModel.currRegionSetId = this.region.setId;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00022A28 File Offset: 0x00020C28
	public void SetModel(GadgetModel model)
	{
		this.droneModel = (DroneModel)model;
		this.ammo.SetModel(((DroneModel)model).ammo);
		base.transform.position = this.droneModel.position;
		base.transform.rotation = this.droneModel.rotation;
		this.network = base.GetComponentInParent<DroneNetwork>();
		this.network.Register(this);
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00022A9B File Offset: 0x00020C9B
	public void LateUpdate()
	{
		this.droneModel.position = base.transform.position;
		this.droneModel.rotation = base.transform.rotation;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00022ACC File Offset: 0x00020CCC
	public void OnCollisionEnter(Collision collision)
	{
		Identifiable.Id id = Identifiable.GetId(collision.gameObject);
		if (id != Identifiable.Id.NONE && id != Identifiable.Id.PLAYER)
		{
			SECTR_AudioSystem.Play(this.metadata.onBoppedCue, base.transform.position, false);
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00022B0A File Offset: 0x00020D0A
	public DroneUI InstantiateDroneUI()
	{
		return UnityEngine.Object.Instantiate<GameObject>(this.metadata.droneUI.gameObject).GetComponent<DroneUI>().Init(this.gadget);
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00022B31 File Offset: 0x00020D31
	public DroneAudioOnActive SFX(SECTR_AudioCue cue)
	{
		return base.gameObject.AddComponent<DroneAudioOnActive>().Init(cue);
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00022B44 File Offset: 0x00020D44
	public DroneProgram.Orientation GetRestingOrientation()
	{
		return new DroneProgram.Orientation(this.station.guideRest.position - this.guideRest.localPosition, this.station.guideRest.rotation);
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00022B7C File Offset: 0x00020D7C
	public void TeleportToStation(bool includeFX)
	{
		if (includeFX && this.metadata.onTeleportFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.metadata.onTeleportFX, base.transform.position, base.transform.rotation);
		}
		this.rigidbody.velocity = Vector3.zero;
		this.rigidbody.angularVelocity = Vector3.zero;
		DroneProgram.Orientation restingOrientation = this.GetRestingOrientation();
		base.transform.position = restingOrientation.pos;
		base.transform.rotation = restingOrientation.rot;
		this.trail.Clear();
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00022C1A File Offset: 0x00020E1A
	public void ForceResting(bool includeFX)
	{
		this.ammo.Clear();
		this.TeleportToStation(includeFX);
		this.plexer.ForceResting();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00022C3C File Offset: 0x00020E3C
	public void OnGadgetDestroyed()
	{
		if (this.network != null)
		{
			this.network.Deregister(this);
			this.network = null;
		}
		if (this.ammo.Any())
		{
			base.transform.SetParent(SRSingleton<DynamicObjectContainer>.Instance.transform, true);
			this.plexer.ForceDumpAmmo(true);
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00022C9A File Offset: 0x00020E9A
	private void OnRegionsChanged(List<Region> exited, List<Region> joined)
	{
		if (exited != null && exited.Contains(this.region))
		{
			this.ForceResting(true);
			this.regionMember.UpdateRegionMembership(true);
		}
	}

	// Token: 0x040005F6 RID: 1526
	[Tooltip("Transform guide: resting position.")]
	public Transform guideRest;

	// Token: 0x040005F7 RID: 1527
	[Tooltip("Transform guide: ammo dump spawn position.")]
	public Transform guideDumpSpawn;

	// Token: 0x04000606 RID: 1542
	private DroneModel droneModel;

	// Token: 0x02000125 RID: 293
	// (Invoke) Token: 0x06000669 RID: 1641
	public delegate void OnDestroyed();
}
