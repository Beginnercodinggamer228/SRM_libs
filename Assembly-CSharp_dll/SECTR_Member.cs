using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000099 RID: 153
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Core/SECTR Member")]
public class SECTR_Member : MonoBehaviour
{
	// Token: 0x0600033A RID: 826 RVA: 0x00015144 File Offset: 0x00013344
	public void ForceUpdateBounds()
	{
		this.OffsetLateUpdate();
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x0600033B RID: 827 RVA: 0x0001514C File Offset: 0x0001334C
	public static List<SECTR_Member> All
	{
		get
		{
			return SECTR_Member.allMembers;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x0600033C RID: 828 RVA: 0x00015153 File Offset: 0x00013353
	public bool CullEachChild
	{
		get
		{
			return this.ChildCulling == SECTR_Member.ChildCullModes.Individual || (this.ChildCulling == SECTR_Member.ChildCullModes.Default && this.isSector);
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600033D RID: 829 RVA: 0x00015170 File Offset: 0x00013370
	public List<SECTR_Sector> Sectors
	{
		get
		{
			return this.sectors;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600033E RID: 830 RVA: 0x00015178 File Offset: 0x00013378
	public List<SECTR_Member.Child> Children
	{
		get
		{
			if (!this.childProxy)
			{
				return this.children;
			}
			return this.childProxy.children;
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x0600033F RID: 831 RVA: 0x00015199 File Offset: 0x00013399
	public List<SECTR_Member.Child> Renderers
	{
		get
		{
			if (!this.childProxy)
			{
				return this.renderers;
			}
			return this.childProxy.renderers;
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000340 RID: 832 RVA: 0x000151BA File Offset: 0x000133BA
	public bool ShadowCaster
	{
		get
		{
			if (!this.childProxy)
			{
				return this.shadowCaster;
			}
			return this.childProxy.shadowCaster;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000341 RID: 833 RVA: 0x000151DB File Offset: 0x000133DB
	public List<SECTR_Member.Child> ShadowCasters
	{
		get
		{
			if (!this.childProxy)
			{
				return this.shadowCasters;
			}
			return this.childProxy.shadowCasters;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000342 RID: 834 RVA: 0x000151FC File Offset: 0x000133FC
	public List<SECTR_Member.Child> Lights
	{
		get
		{
			if (!this.childProxy)
			{
				return this.lights;
			}
			return this.childProxy.lights;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000343 RID: 835 RVA: 0x0001521D File Offset: 0x0001341D
	public bool ShadowLight
	{
		get
		{
			if (!this.childProxy)
			{
				return this.shadowLight;
			}
			return this.childProxy.shadowLight;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000344 RID: 836 RVA: 0x0001523E File Offset: 0x0001343E
	public List<SECTR_Member.Child> ShadowLights
	{
		get
		{
			if (!this.childProxy)
			{
				return this.shadowLights;
			}
			return this.childProxy.shadowLights;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000345 RID: 837 RVA: 0x0001525F File Offset: 0x0001345F
	public List<SECTR_Member.Child> Terrains
	{
		get
		{
			if (!this.childProxy)
			{
				return this.terrains;
			}
			return this.childProxy.terrains;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000346 RID: 838 RVA: 0x00015280 File Offset: 0x00013480
	public Bounds TotalBounds
	{
		get
		{
			return this.totalBounds;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000347 RID: 839 RVA: 0x00015288 File Offset: 0x00013488
	public Bounds RenderBounds
	{
		get
		{
			if (!this.childProxy)
			{
				return this.renderBounds;
			}
			return this.childProxy.renderBounds;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000348 RID: 840 RVA: 0x000152A9 File Offset: 0x000134A9
	public bool HasRenderBounds
	{
		get
		{
			if (!this.childProxy)
			{
				return this.hasRenderBounds;
			}
			return this.childProxy.hasRenderBounds;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000349 RID: 841 RVA: 0x000152CA File Offset: 0x000134CA
	public Bounds LightBounds
	{
		get
		{
			if (!this.childProxy)
			{
				return this.lightBounds;
			}
			return this.childProxy.lightBounds;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600034A RID: 842 RVA: 0x000152EB File Offset: 0x000134EB
	public bool HasLightBounds
	{
		get
		{
			if (!this.childProxy)
			{
				return this.hasLightBounds;
			}
			return this.childProxy.hasLightBounds;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600034C RID: 844 RVA: 0x00015324 File Offset: 0x00013524
	// (set) Token: 0x0600034B RID: 843 RVA: 0x0001530C File Offset: 0x0001350C
	public bool Frozen
	{
		get
		{
			return this.frozen;
		}
		set
		{
			if (this.isSector)
			{
				this.frozen = value;
				this.isFrozen = value;
			}
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x0600034E RID: 846 RVA: 0x00015344 File Offset: 0x00013544
	// (set) Token: 0x0600034D RID: 845 RVA: 0x0001532C File Offset: 0x0001352C
	public bool Hibernate
	{
		get
		{
			return this.hibernate;
		}
		set
		{
			if (this.isSector)
			{
				this.hibernate = value;
				this.isHibernating = value;
			}
		}
	}

	// Token: 0x17000086 RID: 134
	// (set) Token: 0x0600034F RID: 847 RVA: 0x0001534C File Offset: 0x0001354C
	public SECTR_Member ChildProxy
	{
		set
		{
			this.childProxy = value;
		}
	}

	// Token: 0x17000087 RID: 135
	// (set) Token: 0x06000350 RID: 848 RVA: 0x00015355 File Offset: 0x00013555
	public bool NeverJoin
	{
		set
		{
			this.neverJoin = true;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000351 RID: 849 RVA: 0x0001535E File Offset: 0x0001355E
	public bool IsSector
	{
		get
		{
			return this.isSector;
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00015366 File Offset: 0x00013566
	public void ForceUpdate(bool updateChildren, bool checkAllSectorSets = false)
	{
		if (updateChildren)
		{
			this._UpdateChildren();
		}
		this.lastPosition = base.transform.position;
		if (!this.isSector && !this.neverJoin)
		{
			this._UpdateSectorMembership(checkAllSectorSets);
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0001539C File Offset: 0x0001359C
	public void SectorDisabled(SECTR_Sector sector)
	{
		if (sector)
		{
			this.sectors.Remove(sector);
			if (this.Changed != null)
			{
				this.leftSectors.Clear();
				this.leftSectors.Add(sector);
				this.Changed(this.leftSectors, null);
			}
		}
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000354 RID: 852 RVA: 0x000153F0 File Offset: 0x000135F0
	// (remove) Token: 0x06000355 RID: 853 RVA: 0x00015428 File Offset: 0x00013628
	public event SECTR_Member.MembershipChanged Changed;

	// Token: 0x06000356 RID: 854 RVA: 0x0001545D File Offset: 0x0001365D
	public virtual void Start()
	{
		this.started = true;
		this.ForceUpdate(true, true);
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00015470 File Offset: 0x00013670
	protected virtual void OnEnable()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null && base.GetComponent<SECTR_Hibernator>() == null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.RegisterMember(this);
		}
		SECTR_Member.allMembers.Add(this);
		if (this.bakedOnlyLights != null)
		{
			int count = this.bakedOnlyLights.Count;
			this.bakedOnlyTable = new Dictionary<Light, Light>(count);
			for (int i = 0; i < count; i++)
			{
				Light light = this.bakedOnlyLights[i];
				if (light)
				{
					this.bakedOnlyTable[light] = light;
				}
			}
		}
		if (this.started && Application.isPlaying)
		{
			this.ForceUpdate(true, true);
		}
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0001552C File Offset: 0x0001372C
	protected virtual void OnDisable()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null && base.GetComponent<SECTR_Hibernator>() == null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.DeregisterMember(this);
		}
		if (this.Changed != null && this.sectors.Count > 0)
		{
			this.Changed(this.sectors, null);
		}
		if (!this.isSector && !this.neverJoin)
		{
			int count = this.sectors.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = this.sectors[i];
				if (sectr_Sector)
				{
					sectr_Sector.Deregister(this);
				}
			}
			this.sectors.Clear();
		}
		this.bakedOnlyTable = null;
		SECTR_Member.allMembers.Remove(this);
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00015600 File Offset: 0x00013800
	private void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.SECTRDirector != null)
		{
			SRSingleton<SceneContext>.Instance.SECTRDirector.DeregisterMember(this);
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00015631 File Offset: 0x00013831
	public virtual void Awake()
	{
		this.memberTransform = base.transform;
		this.memberHibernator = base.GetComponent<SECTR_Hibernator>();
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600035B RID: 859 RVA: 0x0001564B File Offset: 0x0001384B
	public bool IsHibernating
	{
		get
		{
			return this.memberHibernator != null && this.memberHibernator.isHibernating;
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00015664 File Offset: 0x00013864
	public virtual void OffsetLateUpdate()
	{
		if (!this.isSector && !this.neverJoin && (this.lastMembershipPos == null || (base.transform.localPosition - this.lastMembershipPos.Value).sqrMagnitude > 1f))
		{
			if (this.lastMembershipPos != null)
			{
				Vector3 vector = base.transform.localPosition - this.lastMembershipPos.Value;
				if (vector.x != 0f || vector.y != 0f || vector.z != 0f)
				{
					this.totalBounds.center = this.totalBounds.center + vector;
					this.renderBounds.center = this.renderBounds.center + vector;
					this.lightBounds.center = this.lightBounds.center + vector;
				}
			}
			this._UpdateSectorMembership(false);
			this.lastMembershipPos = new Vector3?(base.transform.localPosition);
		}
		this.lastPosition = base.transform.localPosition;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00015788 File Offset: 0x00013988
	public virtual void NonOffsetLateUpdate()
	{
		this._UpdateChildren();
		Vector3 position = base.transform.position;
		if (!this.isSector && !this.neverJoin)
		{
			this._UpdateSectorMembership(false);
			this.lastMembershipPos = new Vector3?(position);
		}
		if (!this.isSector && !this.neverJoin && (this.lastMembershipPos == null || (position - this.lastMembershipPos.Value).sqrMagnitude > 1f))
		{
			this._UpdateSectorMembership(false);
			this.lastMembershipPos = new Vector3?(position);
		}
		this.lastPosition = position;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00015824 File Offset: 0x00013A24
	private void _UpdateChildren()
	{
		if (this.frozen || this.childProxy)
		{
			return;
		}
		bool flag = SECTR_Modules.VIS && this.DirShadowCaster && this.DirShadowCaster.type == LightType.Directional && this.DirShadowCaster.shadows > LightShadows.None;
		Vector3 shadowVec = flag ? (this.DirShadowCaster.transform.forward * this.DirShadowDistance) : Vector3.zero;
		int count = this.children.Count;
		this.hasLightBounds = false;
		this.hasRenderBounds = false;
		this.shadowCaster = false;
		this.shadowLight = false;
		this.renderers.Clear();
		this.lights.Clear();
		this.terrains.Clear();
		if (SECTR_Modules.VIS)
		{
			this.shadowCasters.Clear();
			this.shadowLights.Clear();
		}
		if ((this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Start || this.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Offset) && count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				SECTR_Member.Child child = this.children[i];
				child.Init(child.gameObject, child.renderer, child.light, child.terrain, child.member, flag, shadowVec);
				Renderer renderer = child.renderer;
				if (renderer != null && !(renderer is ParticleSystemRenderer))
				{
					if (!this.hasRenderBounds)
					{
						this.renderBounds = child.rendererBounds;
						this.hasRenderBounds = true;
					}
					else
					{
						this.renderBounds.Encapsulate(child.rendererBounds);
					}
					this.renderers.Add(child);
				}
				if (child.terrain)
				{
					if (!this.hasRenderBounds)
					{
						this.renderBounds = child.terrainBounds;
						this.hasRenderBounds = true;
					}
					else
					{
						this.renderBounds.Encapsulate(child.terrainBounds);
					}
					this.terrains.Add(child);
				}
				if (child.light)
				{
					if (SECTR_Modules.VIS && child.shadowLight)
					{
						this.shadowLights.Add(child);
						this.shadowLight = true;
					}
					if (!this.hasLightBounds)
					{
						this.lightBounds = child.lightBounds;
						this.hasLightBounds = true;
					}
					else
					{
						this.lightBounds.Encapsulate(child.lightBounds);
					}
					this.lights.Add(child);
				}
				if (SECTR_Modules.VIS && (child.terrainCastsShadows || child.rendererCastsShadows))
				{
					this.shadowCasters.Add(child);
					this.shadowCaster = true;
				}
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				this.childPool.Push(this.children[j]);
			}
			this.children.Clear();
			this._AddChildren(base.transform, flag, shadowVec);
		}
		this.lastPosition = base.transform.position;
		Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
		if (this.hasRenderBounds && (this.isSector || this.neverJoin))
		{
			this.totalBounds = this.renderBounds;
		}
		else if (this.hasRenderBounds && this.hasLightBounds)
		{
			this.totalBounds = this.renderBounds;
			this.totalBounds.Encapsulate(this.lightBounds);
		}
		else if (this.hasRenderBounds)
		{
			this.totalBounds = this.renderBounds;
			this.lightBounds = bounds;
		}
		else if (this.hasLightBounds)
		{
			this.totalBounds = this.lightBounds;
			this.renderBounds = bounds;
		}
		else
		{
			this.totalBounds = bounds;
			this.lightBounds = bounds;
			this.renderBounds = bounds;
		}
		this.totalBounds.Expand(this.ExtraBounds);
		if (this.isSector)
		{
			this.totalBounds.max = this.totalBounds.max + Vector3.up * 160f;
		}
		if (this.OverrideBounds)
		{
			this.totalBounds = this.BoundsOverride;
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00015C1C File Offset: 0x00013E1C
	private void _AddChildren(Transform childTransform, bool dirShadowCaster, Vector3 shadowVec)
	{
		if (childTransform.gameObject.activeSelf && (childTransform == base.transform || childTransform.GetComponent<SECTR_Member>() == null) && childTransform.GetComponent<IgnoreSectrBounds>() == null)
		{
			Light light = childTransform.GetComponent<Light>();
			Renderer component = childTransform.GetComponent<Renderer>();
			Terrain terrain = null;
			if (this.isSector)
			{
				terrain = childTransform.GetComponent<Terrain>();
			}
			if (this.bakedOnlyLights != null && light && light.bakingOutput.isBaked && LightmapSettings.lightmaps.Length != 0 && this.bakedOnlyTable != null && this.bakedOnlyTable.ContainsKey(light))
			{
				light = null;
			}
			if (component || light || terrain)
			{
				SECTR_Member.Child child;
				if (this.childPool.Count > 0)
				{
					child = this.childPool.Pop();
				}
				else
				{
					child = new SECTR_Member.Child();
				}
				child.Init(childTransform.gameObject, component, light, terrain, this, dirShadowCaster, shadowVec);
				if (child.renderer)
				{
					bool flag = true;
					if (component.GetType() == typeof(ParticleSystemRenderer))
					{
						flag = false;
					}
					if (flag)
					{
						if (!this.hasRenderBounds)
						{
							this.renderBounds = child.rendererBounds;
							this.hasRenderBounds = true;
						}
						else
						{
							this.renderBounds.Encapsulate(child.rendererBounds);
						}
					}
					this.renderers.Add(child);
				}
				if (child.light)
				{
					if (SECTR_Modules.VIS && child.shadowLight)
					{
						this.shadowLights.Add(child);
						this.shadowLight = true;
					}
					if (!this.hasLightBounds)
					{
						this.lightBounds = child.lightBounds;
						this.hasLightBounds = true;
					}
					else
					{
						this.lightBounds.Encapsulate(child.lightBounds);
					}
					this.lights.Add(child);
				}
				if (child.terrain)
				{
					if (!this.hasRenderBounds)
					{
						this.renderBounds = child.terrainBounds;
						this.hasRenderBounds = true;
					}
					else
					{
						this.renderBounds.Encapsulate(child.terrainBounds);
					}
					this.terrains.Add(child);
				}
				if (SECTR_Modules.VIS && (child.terrainCastsShadows || child.rendererCastsShadows))
				{
					this.shadowCasters.Add(child);
					this.shadowCaster = true;
				}
				this.children.Add(child);
			}
			int childCount = childTransform.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				this._AddChildren(childTransform.GetChild(i), dirShadowCaster, shadowVec);
			}
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00015EA4 File Offset: 0x000140A4
	private void _UpdateSectorMembership(bool checkAllSectorSets = false)
	{
		if (this.frozen || this.isSector || this.neverJoin)
		{
			return;
		}
		this.newSectors.Clear();
		this.leftSectors.Clear();
		if (this.PortalDetermined && this.sectors.Count > 0)
		{
			int count = this.sectors.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = this.sectors[i];
				SECTR_Portal sectr_Portal = this._CrossedPortal(sectr_Sector);
				if (sectr_Portal)
				{
					SECTR_Sector item = (sectr_Portal.FrontSector == sectr_Sector) ? sectr_Portal.BackSector : sectr_Portal.FrontSector;
					if (!this.newSectors.Contains(item))
					{
						this.newSectors.Add(item);
					}
					this.leftSectors.Add(sectr_Sector);
				}
			}
			count = this.newSectors.Count;
			for (int j = 0; j < count; j++)
			{
				SECTR_Sector sectr_Sector2 = this.newSectors[j];
				sectr_Sector2.Register(this);
				this.sectors.Add(sectr_Sector2);
			}
			count = this.leftSectors.Count;
			for (int k = 0; k < count; k++)
			{
				SECTR_Sector sectr_Sector3 = this.leftSectors[k];
				sectr_Sector3.Deregister(this);
				this.sectors.Remove(sectr_Sector3);
			}
		}
		else if (this.PortalDetermined && this.ForceStartSector && !this.usedStartSector)
		{
			this.ForceStartSector.Register(this);
			this.sectors.Add(this.ForceStartSector);
			this.newSectors.Add(this.ForceStartSector);
			this.usedStartSector = true;
		}
		else
		{
			SECTR_Sector.GetContaining(ref this.newSectors, this.TotalBounds, checkAllSectorSets);
			int l = 0;
			int num = this.sectors.Count;
			while (l < num)
			{
				SECTR_Sector sectr_Sector4 = this.sectors[l];
				if (this.newSectors.Contains(sectr_Sector4))
				{
					this.newSectors.Remove(sectr_Sector4);
					l++;
				}
				else
				{
					sectr_Sector4.Deregister(this);
					this.leftSectors.Add(sectr_Sector4);
					this.sectors.RemoveAt(l);
					num--;
				}
			}
			num = this.newSectors.Count;
			if (num > 0)
			{
				for (l = 0; l < num; l++)
				{
					SECTR_Sector sectr_Sector5 = this.newSectors[l];
					sectr_Sector5.Register(this);
					this.sectors.Add(sectr_Sector5);
				}
			}
		}
		if (this.Changed != null && (this.leftSectors.Count > 0 || this.newSectors.Count > 0))
		{
			this.Changed(this.leftSectors, this.newSectors);
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0001615C File Offset: 0x0001435C
	private SECTR_Portal _CrossedPortal(SECTR_Sector sector)
	{
		if (sector)
		{
			Vector3 lhs = base.transform.position - this.lastPosition;
			int count = sector.Portals.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Portal sectr_Portal = sector.Portals[i];
				if (sectr_Portal)
				{
					bool flag = sectr_Portal.FrontSector == sector;
					Plane plane = flag ? sectr_Portal.HullPlane : sectr_Portal.ReverseHullPlane;
					if ((flag ? sectr_Portal.BackSector : sectr_Portal.FrontSector) && Vector3.Dot(lhs, plane.normal) < 0f && plane.GetSide(base.transform.position) != plane.GetSide(this.lastPosition) && sectr_Portal.IsPointInHull(base.transform.position, lhs.magnitude))
					{
						return sectr_Portal;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x04000352 RID: 850
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> children = new List<SECTR_Member.Child>(16);

	// Token: 0x04000353 RID: 851
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> renderers = new List<SECTR_Member.Child>(16);

	// Token: 0x04000354 RID: 852
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> lights = new List<SECTR_Member.Child>(16);

	// Token: 0x04000355 RID: 853
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> terrains = new List<SECTR_Member.Child>(2);

	// Token: 0x04000356 RID: 854
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> shadowLights = SECTR_Modules.VIS ? new List<SECTR_Member.Child>(16) : null;

	// Token: 0x04000357 RID: 855
	[SerializeField]
	[HideInInspector]
	private List<SECTR_Member.Child> shadowCasters = SECTR_Modules.VIS ? new List<SECTR_Member.Child>(16) : null;

	// Token: 0x04000358 RID: 856
	[SerializeField]
	[HideInInspector]
	private Bounds totalBounds;

	// Token: 0x04000359 RID: 857
	[SerializeField]
	[HideInInspector]
	private Bounds renderBounds;

	// Token: 0x0400035A RID: 858
	[SerializeField]
	[HideInInspector]
	private Bounds lightBounds;

	// Token: 0x0400035B RID: 859
	[SerializeField]
	[HideInInspector]
	private bool hasRenderBounds;

	// Token: 0x0400035C RID: 860
	[SerializeField]
	[HideInInspector]
	private bool hasLightBounds;

	// Token: 0x0400035D RID: 861
	[SerializeField]
	[HideInInspector]
	private bool shadowCaster;

	// Token: 0x0400035E RID: 862
	[SerializeField]
	[HideInInspector]
	private bool shadowLight;

	// Token: 0x0400035F RID: 863
	[SerializeField]
	[HideInInspector]
	private bool frozen;

	// Token: 0x04000360 RID: 864
	[HideInInspector]
	public bool isFrozen;

	// Token: 0x04000361 RID: 865
	[SerializeField]
	[HideInInspector]
	private bool hibernate;

	// Token: 0x04000362 RID: 866
	[SerializeField]
	[HideInInspector]
	private bool neverJoin;

	// Token: 0x04000363 RID: 867
	[SerializeField]
	[HideInInspector]
	protected List<Light> bakedOnlyLights = SECTR_Modules.VIS ? new List<Light>(8) : null;

	// Token: 0x04000364 RID: 868
	[SerializeField]
	[HideInInspector]
	protected bool legacyBakeMode;

	// Token: 0x04000365 RID: 869
	protected bool isSector;

	// Token: 0x04000366 RID: 870
	private bool started;

	// Token: 0x04000367 RID: 871
	private bool usedStartSector;

	// Token: 0x04000368 RID: 872
	[NonSerialized]
	public List<SECTR_Sector> sectors = new List<SECTR_Sector>(4);

	// Token: 0x04000369 RID: 873
	private List<SECTR_Sector> newSectors = new List<SECTR_Sector>(4);

	// Token: 0x0400036A RID: 874
	private List<SECTR_Sector> leftSectors = new List<SECTR_Sector>(4);

	// Token: 0x0400036B RID: 875
	private Dictionary<Light, Light> bakedOnlyTable;

	// Token: 0x0400036C RID: 876
	private SECTR_Member childProxy;

	// Token: 0x0400036D RID: 877
	private Vector3 lastPosition = Vector3.zero;

	// Token: 0x0400036E RID: 878
	private Stack<SECTR_Member.Child> childPool = new Stack<SECTR_Member.Child>(32);

	// Token: 0x0400036F RID: 879
	private static LightmapSettings lightmapSettings;

	// Token: 0x04000370 RID: 880
	private static List<SECTR_Member> allMembers = new List<SECTR_Member>(256);

	// Token: 0x04000371 RID: 881
	private Vector3? lastMembershipPos;

	// Token: 0x04000372 RID: 882
	[SECTR_ToolTip("Set to true if Sector membership should only change when crossing a portal.")]
	public bool PortalDetermined;

	// Token: 0x04000373 RID: 883
	[SECTR_ToolTip("If set, forces the initial Sector to be the specified Sector.", "PortalDetermined")]
	public SECTR_Sector ForceStartSector;

	// Token: 0x04000374 RID: 884
	[SECTR_ToolTip("Determines how often the bounds are recomputed. More frequent updates requires more CPU.")]
	public SECTR_Member.BoundsUpdateModes BoundsUpdateMode = SECTR_Member.BoundsUpdateModes.Always;

	// Token: 0x04000375 RID: 885
	[SECTR_ToolTip("Adds a buffer on bounding box to compensate for minor imprecisions.")]
	public float ExtraBounds = 0.01f;

	// Token: 0x04000376 RID: 886
	[SECTR_ToolTip("Override computed bounds with the user specified bounds. Advanced users only.")]
	public bool OverrideBounds;

	// Token: 0x04000377 RID: 887
	[SECTR_ToolTip("User specified override bounds. Auto-populated with the current bounds when override is inactive.", "OverrideBounds")]
	public Bounds BoundsOverride;

	// Token: 0x04000378 RID: 888
	[SECTR_ToolTip("Optional shadow casting directional light to use in membership calculations. Bounds will be extruded away from light, if set.")]
	public Light DirShadowCaster;

	// Token: 0x04000379 RID: 889
	[SECTR_ToolTip("Distance by which to extend the bounds away from the shadow casting light.", "DirShadowCaster")]
	public float DirShadowDistance = 100f;

	// Token: 0x0400037A RID: 890
	[SECTR_ToolTip("Determines if this SectorCuller should cull individual children, or cull all children based on the aggregate bounds.")]
	public SECTR_Member.ChildCullModes ChildCulling;

	// Token: 0x0400037B RID: 891
	[HideInInspector]
	public bool isHibernating;

	// Token: 0x0400037D RID: 893
	private SECTR_Hibernator memberHibernator;

	// Token: 0x0400037E RID: 894
	[HideInInspector]
	[NonSerialized]
	public Transform memberTransform;

	// Token: 0x0200009A RID: 154
	[Serializable]
	public class Child
	{
		// Token: 0x06000364 RID: 868 RVA: 0x00016340 File Offset: 0x00014540
		public void Init(GameObject gameObject, Renderer renderer, Light light, Terrain terrain, SECTR_Member member, bool dirShadowCaster, Vector3 shadowVec)
		{
			if (gameObject == null)
			{
				return;
			}
			this.gameObject = gameObject;
			this.gameObjectHash = this.gameObject.GetInstanceID();
			this.member = member;
			this.renderer = ((renderer && renderer.enabled) ? renderer : null);
			this.light = ((light && light.enabled && (light.type == LightType.Point || light.type == LightType.Spot)) ? light : null);
			this.terrain = ((terrain && terrain.enabled) ? terrain : null);
			Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
			this.rendererBounds = ((this.renderer && gameObject.transform.lossyScale.sqrMagnitude > Mathf.Epsilon) ? this.renderer.bounds : bounds);
			this.lightBounds = ((this.light && gameObject.transform.lossyScale.sqrMagnitude > 0f) ? SECTR_Geometry.ComputeBounds(this.light) : bounds);
			this.terrainBounds = ((this.terrain && gameObject.transform.lossyScale.sqrMagnitude > 0f) ? SECTR_Geometry.ComputeBounds(this.terrain) : bounds);
			this.layer = gameObject.layer;
			if (!SECTR_Modules.VIS)
			{
				this.renderHash = 0;
				this.lightHash = 0;
				this.terrainHash = 0;
				this.shadowLight = false;
				this.rendererCastsShadows = false;
				this.terrainCastsShadows = false;
				this.shadowLightPosition = Vector3.zero;
				this.shadowLightRange = 0f;
				this.shadowLightType = LightType.Area;
				this.shadowCullingMask = 0;
				return;
			}
			this.renderHash = (this.renderer ? this.renderer.GetInstanceID() : 0);
			this.lightHash = (this.light ? this.light.GetInstanceID() : 0);
			this.terrainHash = (this.terrain ? this.terrain.GetInstanceID() : 0);
			bool flag = !member.legacyBakeMode || LightmapSettings.lightmapsMode == LightmapsMode.CombinedDirectional;
			this.shadowLight = (this.light && light.shadows != LightShadows.None && (!light.bakingOutput.isBaked || flag));
			this.rendererCastsShadows = (this.renderer != null && renderer.shadowCastingMode != ShadowCastingMode.Off && (renderer.lightmapIndex == -1 || flag));
			this.terrainCastsShadows = (this.terrain && terrain.shadowCastingMode != ShadowCastingMode.Off && (terrain.lightmapIndex == -1 || flag));
			if (dirShadowCaster)
			{
				if (this.rendererCastsShadows)
				{
					this.rendererBounds = SECTR_Geometry.ProjectBounds(this.rendererBounds, shadowVec);
				}
				if (this.terrainCastsShadows)
				{
					this.terrainBounds = SECTR_Geometry.ProjectBounds(this.terrainBounds, shadowVec);
				}
			}
			if (this.shadowLight)
			{
				this.shadowLightPosition = light.transform.position;
				this.shadowLightRange = light.range;
				this.shadowLightType = light.type;
				this.shadowCullingMask = light.cullingMask;
				return;
			}
			this.shadowLightPosition = Vector3.zero;
			this.shadowLightRange = 0f;
			this.shadowLightType = LightType.Area;
			this.shadowCullingMask = 0;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000166A2 File Offset: 0x000148A2
		public override bool Equals(object obj)
		{
			return obj is SECTR_Member.Child && this == (SECTR_Member.Child)obj;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x000166BA File Offset: 0x000148BA
		public override int GetHashCode()
		{
			return this.gameObjectHash;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000166C2 File Offset: 0x000148C2
		public static bool operator ==(SECTR_Member.Child x, SECTR_Member.Child y)
		{
			return x.gameObjectHash == y.gameObjectHash;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000166D2 File Offset: 0x000148D2
		public static bool operator !=(SECTR_Member.Child x, SECTR_Member.Child y)
		{
			return !(x == y);
		}

		// Token: 0x0400037F RID: 895
		public GameObject gameObject;

		// Token: 0x04000380 RID: 896
		public int gameObjectHash;

		// Token: 0x04000381 RID: 897
		public SECTR_Member member;

		// Token: 0x04000382 RID: 898
		public Renderer renderer;

		// Token: 0x04000383 RID: 899
		public int renderHash;

		// Token: 0x04000384 RID: 900
		public Light light;

		// Token: 0x04000385 RID: 901
		public int lightHash;

		// Token: 0x04000386 RID: 902
		public Terrain terrain;

		// Token: 0x04000387 RID: 903
		public int terrainHash;

		// Token: 0x04000388 RID: 904
		public Bounds rendererBounds;

		// Token: 0x04000389 RID: 905
		public Bounds lightBounds;

		// Token: 0x0400038A RID: 906
		public Bounds terrainBounds;

		// Token: 0x0400038B RID: 907
		public bool shadowLight;

		// Token: 0x0400038C RID: 908
		public bool rendererCastsShadows;

		// Token: 0x0400038D RID: 909
		public bool terrainCastsShadows;

		// Token: 0x0400038E RID: 910
		public LayerMask layer;

		// Token: 0x0400038F RID: 911
		public Vector3 shadowLightPosition;

		// Token: 0x04000390 RID: 912
		public float shadowLightRange;

		// Token: 0x04000391 RID: 913
		public LightType shadowLightType;

		// Token: 0x04000392 RID: 914
		public int shadowCullingMask;
	}

	// Token: 0x0200009B RID: 155
	public enum BoundsUpdateModes
	{
		// Token: 0x04000394 RID: 916
		Start,
		// Token: 0x04000395 RID: 917
		Movement,
		// Token: 0x04000396 RID: 918
		Always,
		// Token: 0x04000397 RID: 919
		Static,
		// Token: 0x04000398 RID: 920
		Offset
	}

	// Token: 0x0200009C RID: 156
	public enum ChildCullModes
	{
		// Token: 0x0400039A RID: 922
		Default,
		// Token: 0x0400039B RID: 923
		Group,
		// Token: 0x0400039C RID: 924
		Individual
	}

	// Token: 0x0200009D RID: 157
	// (Invoke) Token: 0x0600036B RID: 875
	public delegate void MembershipChanged(List<SECTR_Sector> left, List<SECTR_Sector> joined);
}
