using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A3 RID: 163
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Core/SECTR Sector")]
public class SECTR_Sector : SECTR_Member
{
	// Token: 0x06000394 RID: 916 RVA: 0x00016E8A File Offset: 0x0001508A
	private SECTR_Sector()
	{
		this.isSector = true;
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000395 RID: 917 RVA: 0x00016EB2 File Offset: 0x000150B2
	public new static List<SECTR_Sector> All
	{
		get
		{
			return SECTR_Sector.allSectors;
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00016EB9 File Offset: 0x000150B9
	public static SECTR_Sector.SectorSetId GetSectorSetId()
	{
		if (SECTR_Sector.currSectorsTree == SECTR_Sector.sectorsTrees[SECTR_Sector.SectorSetId.DESERT])
		{
			return SECTR_Sector.SectorSetId.DESERT;
		}
		return SECTR_Sector.SectorSetId.HOME;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00016ED0 File Offset: 0x000150D0
	public static void SetCurrSectorSet(SECTR_Sector.SectorSetId setId, bool forceActivation = false)
	{
		if (!forceActivation && SECTR_Sector.currSectorsTree == SECTR_Sector.sectorsTrees[setId])
		{
			return;
		}
		foreach (KeyValuePair<SECTR_Sector.SectorSetId, BoundsQuadtree<SECTR_Sector>> keyValuePair in SECTR_Sector.sectorsTrees)
		{
			bool flag = keyValuePair.Key == setId;
			List<SECTR_Sector> list = new List<SECTR_Sector>();
			list = keyValuePair.Value.GetAll(ref list);
			foreach (SECTR_Sector sectr_Sector in list)
			{
				sectr_Sector.GetComponent<SECTR_Chunk>().SetCanProxy(flag);
			}
			if (SECTR_Sector.managedWithSets.ContainsKey(keyValuePair.Key))
			{
				foreach (GameObject gameObject in SECTR_Sector.managedWithSets[keyValuePair.Key])
				{
					gameObject.SetActive(flag);
				}
			}
		}
		SECTR_Sector.currSectorsTree = SECTR_Sector.sectorsTrees[setId];
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00017008 File Offset: 0x00015208
	public static void SetCurrSectorSetForPos(Vector3 pos)
	{
		SECTR_Sector.SetCurrSectorSet(SECTR_Sector.GetSectorSetForPos(pos), false);
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00017016 File Offset: 0x00015216
	public static SECTR_Sector.SectorSetId GetSectorSetForPos(Vector3 pos)
	{
		if (pos.y > 900f)
		{
			return SECTR_Sector.SectorSetId.DESERT;
		}
		return SECTR_Sector.SectorSetId.HOME;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00017028 File Offset: 0x00015228
	public static bool IsCurrSectorSet(SECTR_Sector.SectorSetId setId)
	{
		return SECTR_Sector.currSectorsTree == null || SECTR_Sector.currSectorsTree == SECTR_Sector.sectorsTrees[setId];
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00017045 File Offset: 0x00015245
	public static void ManageWithSectorSet(GameObject obj, SECTR_Sector.SectorSetId setId)
	{
		if (!SECTR_Sector.managedWithSets.ContainsKey(setId))
		{
			SECTR_Sector.managedWithSets[setId] = new List<GameObject>();
		}
		SECTR_Sector.managedWithSets[setId].Add(obj);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00017075 File Offset: 0x00015275
	public static void ReleaseFromSectorSet(GameObject obj, SECTR_Sector.SectorSetId setId)
	{
		if (SECTR_Sector.managedWithSets.ContainsKey(setId))
		{
			SECTR_Sector.managedWithSets[setId].Remove(obj);
		}
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00017098 File Offset: 0x00015298
	public static void GetContaining(ref List<SECTR_Sector> sectors, Vector3 position)
	{
		sectors.Clear();
		int count = SECTR_Sector.allSectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sectr_Sector = SECTR_Sector.allSectors[i];
			if (sectr_Sector.TotalBounds.Contains(position))
			{
				sectors.Add(sectr_Sector);
			}
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000170E8 File Offset: 0x000152E8
	public static void GetContaining(ref List<SECTR_Sector> sectors, Bounds bounds, bool checkAllSectorSets = false)
	{
		sectors.Clear();
		if (checkAllSectorSets)
		{
			using (Dictionary<SECTR_Sector.SectorSetId, BoundsQuadtree<SECTR_Sector>>.ValueCollection.Enumerator enumerator = SECTR_Sector.sectorsTrees.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BoundsQuadtree<SECTR_Sector> boundsQuadtree = enumerator.Current;
					boundsQuadtree.GetColliding(bounds, ref sectors);
				}
				return;
			}
		}
		if (SECTR_Sector.currSectorsTree != null)
		{
			SECTR_Sector.currSectorsTree.GetColliding(bounds, ref sectors);
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x0600039F RID: 927 RVA: 0x00017160 File Offset: 0x00015360
	// (set) Token: 0x060003A0 RID: 928 RVA: 0x00017168 File Offset: 0x00015368
	public bool Visited
	{
		get
		{
			return this.visited;
		}
		set
		{
			this.visited = value;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060003A1 RID: 929 RVA: 0x00017171 File Offset: 0x00015371
	public List<SECTR_Portal> Portals
	{
		get
		{
			return this.portals;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060003A2 RID: 930 RVA: 0x00017179 File Offset: 0x00015379
	public List<SECTR_Member> Members
	{
		get
		{
			return this.members;
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00017184 File Offset: 0x00015384
	public void ConnectTerrainNeighbors()
	{
		Terrain componentInChildren = base.GetComponentInChildren<Terrain>();
		if (componentInChildren)
		{
			componentInChildren.SetNeighbors(this.LeftTerrain ? this.LeftTerrain.GetComponentInChildren<Terrain>() : null, this.TopTerrain ? this.TopTerrain.GetComponentInChildren<Terrain>() : null, this.RightTerrain ? this.RightTerrain.GetComponentInChildren<Terrain>() : null, this.BottomTerrain ? this.BottomTerrain.GetComponentInChildren<Terrain>() : null);
		}
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00017214 File Offset: 0x00015414
	public void DisonnectTerrainNeighbors()
	{
		Terrain componentInChildren = base.GetComponentInChildren<Terrain>();
		if (componentInChildren)
		{
			componentInChildren.SetNeighbors(null, null, null, null);
		}
		if (this.TopTerrain)
		{
			Terrain componentInChildren2 = this.TopTerrain.GetComponentInChildren<Terrain>();
			if (componentInChildren2)
			{
				componentInChildren2.SetNeighbors(this.TopTerrain.LeftTerrain ? this.TopTerrain.LeftTerrain.GetComponentInChildren<Terrain>() : null, this.TopTerrain.TopTerrain ? this.TopTerrain.TopTerrain.GetComponentInChildren<Terrain>() : null, this.TopTerrain.RightTerrain ? this.TopTerrain.RightTerrain.GetComponentInChildren<Terrain>() : null, null);
			}
		}
		if (this.BottomTerrain)
		{
			Terrain componentInChildren3 = this.BottomTerrain.GetComponentInChildren<Terrain>();
			if (componentInChildren3)
			{
				componentInChildren3.SetNeighbors(this.BottomTerrain.LeftTerrain ? this.BottomTerrain.LeftTerrain.GetComponentInChildren<Terrain>() : null, null, this.BottomTerrain.RightTerrain ? this.BottomTerrain.RightTerrain.GetComponentInChildren<Terrain>() : null, this.BottomTerrain.BottomTerrain ? this.BottomTerrain.BottomTerrain.GetComponentInChildren<Terrain>() : null);
			}
		}
		if (this.LeftTerrain)
		{
			Terrain componentInChildren4 = this.LeftTerrain.GetComponentInChildren<Terrain>();
			if (componentInChildren4)
			{
				componentInChildren4.SetNeighbors(this.LeftTerrain.LeftTerrain ? this.LeftTerrain.LeftTerrain.GetComponentInChildren<Terrain>() : null, this.LeftTerrain.TopTerrain ? this.LeftTerrain.TopTerrain.GetComponentInChildren<Terrain>() : null, null, this.LeftTerrain.BottomTerrain ? this.LeftTerrain.BottomTerrain.GetComponentInChildren<Terrain>() : null);
			}
		}
		if (this.RightTerrain)
		{
			Terrain componentInChildren5 = this.RightTerrain.GetComponentInChildren<Terrain>();
			if (componentInChildren5)
			{
				componentInChildren5.SetNeighbors(null, this.RightTerrain.TopTerrain ? this.RightTerrain.TopTerrain.GetComponentInChildren<Terrain>() : null, this.RightTerrain.RightTerrain ? this.RightTerrain.RightTerrain.GetComponentInChildren<Terrain>() : null, this.RightTerrain.BottomTerrain ? this.RightTerrain.BottomTerrain.GetComponentInChildren<Terrain>() : null);
			}
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000174A5 File Offset: 0x000156A5
	public void Register(SECTR_Portal portal)
	{
		if (!this.portals.Contains(portal))
		{
			this.portals.Add(portal);
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x000174C1 File Offset: 0x000156C1
	public void Deregister(SECTR_Portal portal)
	{
		this.portals.Remove(portal);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000174D0 File Offset: 0x000156D0
	public void Register(SECTR_Member member)
	{
		this.members.Add(member);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x000174DE File Offset: 0x000156DE
	public void Deregister(SECTR_Member member)
	{
		this.members.Remove(member);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000174ED File Offset: 0x000156ED
	public override void Awake()
	{
		base.Awake();
		this.cellDir = base.GetComponent<CellDirector>();
		if (Application.isPlaying && SECTR_Sector.currSectorsTree == null)
		{
			SECTR_Sector.SetCurrSectorSet(SECTR_Sector.SectorSetId.HOME, true);
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00017518 File Offset: 0x00015718
	protected override void OnEnable()
	{
		SECTR_Sector.allSectors.Add(this);
		SECTR_Sector.SectorSetId setId = this.GetSetId();
		SECTR_Sector.sectorsTrees[setId].Add(this, base.TotalBounds);
		if (Application.isPlaying)
		{
			base.GetComponent<SECTR_Chunk>().SetCanProxy(SECTR_Sector.currSectorsTree == SECTR_Sector.sectorsTrees[setId]);
		}
		if (this.TopTerrain || this.BottomTerrain || this.RightTerrain || this.LeftTerrain)
		{
			this.ConnectTerrainNeighbors();
		}
		base.OnEnable();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x000175B4 File Offset: 0x000157B4
	protected override void OnDisable()
	{
		List<SECTR_Member> list = new List<SECTR_Member>(this.members);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Member sectr_Member = list[i];
			if (sectr_Member)
			{
				sectr_Member.SectorDisabled(this);
			}
		}
		SECTR_Sector.allSectors.Remove(this);
		SECTR_Sector.sectorsTrees[this.GetSetId()].Remove(this);
		base.OnDisable();
	}

	// Token: 0x060003AC RID: 940 RVA: 0x00017620 File Offset: 0x00015820
	public SECTR_Sector.SectorSetId GetSetId()
	{
		ZoneDirector[] componentsInParent = base.GetComponentsInParent<ZoneDirector>(true);
		if (componentsInParent.Length == 0)
		{
			return SECTR_Sector.SectorSetId.HOME;
		}
		if (componentsInParent[0].zone != ZoneDirector.Zone.DESERT)
		{
			return SECTR_Sector.SectorSetId.HOME;
		}
		return SECTR_Sector.SectorSetId.DESERT;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0001764C File Offset: 0x0001584C
	public override void NonOffsetLateUpdate()
	{
		Bounds totalBounds = base.TotalBounds;
		base.NonOffsetLateUpdate();
		if (totalBounds != base.TotalBounds)
		{
			SECTR_Sector.SectorSetId setId = this.GetSetId();
			SECTR_Sector.sectorsTrees[setId].Remove(this);
			SECTR_Sector.sectorsTrees[setId].Add(this, base.TotalBounds);
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x000176A4 File Offset: 0x000158A4
	public override void OffsetLateUpdate()
	{
		Bounds totalBounds = base.TotalBounds;
		base.OffsetLateUpdate();
		if (totalBounds != base.TotalBounds)
		{
			SECTR_Sector.SectorSetId setId = this.GetSetId();
			SECTR_Sector.sectorsTrees[setId].Remove(this);
			SECTR_Sector.sectorsTrees[setId].Add(this, base.TotalBounds);
		}
	}

	// Token: 0x040003B0 RID: 944
	private List<SECTR_Portal> portals = new List<SECTR_Portal>(8);

	// Token: 0x040003B1 RID: 945
	private List<SECTR_Member> members = new List<SECTR_Member>(32);

	// Token: 0x040003B2 RID: 946
	private bool visited;

	// Token: 0x040003B3 RID: 947
	private static List<SECTR_Sector> allSectors = new List<SECTR_Sector>(128);

	// Token: 0x040003B4 RID: 948
	private static Dictionary<SECTR_Sector.SectorSetId, BoundsQuadtree<SECTR_Sector>> sectorsTrees = new Dictionary<SECTR_Sector.SectorSetId, BoundsQuadtree<SECTR_Sector>>(SECTR_Sector.SectorSetIdComparer.Instance)
	{
		{
			SECTR_Sector.SectorSetId.HOME,
			new BoundsQuadtree<SECTR_Sector>(1000f, Vector3.zero, 250f, 1.2f)
		},
		{
			SECTR_Sector.SectorSetId.DESERT,
			new BoundsQuadtree<SECTR_Sector>(1000f, Vector3.up * 1000f, 250f, 1.2f)
		}
	};

	// Token: 0x040003B5 RID: 949
	private static Dictionary<SECTR_Sector.SectorSetId, List<GameObject>> managedWithSets = new Dictionary<SECTR_Sector.SectorSetId, List<GameObject>>();

	// Token: 0x040003B6 RID: 950
	private static BoundsQuadtree<SECTR_Sector> currSectorsTree = null;

	// Token: 0x040003B7 RID: 951
	[SECTR_ToolTip("The terrain Sector attached on the top side of this Sector.")]
	public SECTR_Sector TopTerrain;

	// Token: 0x040003B8 RID: 952
	[SECTR_ToolTip("The terrain Sector attached on the bottom side of this Sector.")]
	public SECTR_Sector BottomTerrain;

	// Token: 0x040003B9 RID: 953
	[SECTR_ToolTip("The terrain Sector attached on the left side of this Sector.")]
	public SECTR_Sector LeftTerrain;

	// Token: 0x040003BA RID: 954
	[SECTR_ToolTip("The terrain Sector attached on the right side of this Sector.")]
	public SECTR_Sector RightTerrain;

	// Token: 0x040003BB RID: 955
	public CellDirector cellDir;

	// Token: 0x020000A4 RID: 164
	public enum SectorSetId
	{
		// Token: 0x040003BD RID: 957
		UNSET = -1,
		// Token: 0x040003BE RID: 958
		HOME,
		// Token: 0x040003BF RID: 959
		DESERT
	}

	// Token: 0x020000A5 RID: 165
	private class SectorSetIdComparer : IEqualityComparer<SECTR_Sector.SectorSetId>
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(SECTR_Sector.SectorSetId x, SECTR_Sector.SectorSetId y)
		{
			return x == y;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(SECTR_Sector.SectorSetId id)
		{
			return (int)id;
		}

		// Token: 0x040003C0 RID: 960
		public static readonly SECTR_Sector.SectorSetIdComparer Instance = new SECTR_Sector.SectorSetIdComparer();
	}
}
