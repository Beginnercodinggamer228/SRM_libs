using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B6 RID: 182
[AddComponentMenu("SECTR/Stream/SECTR Region Loader")]
public class SECTR_RegionLoader : SECTR_Loader
{
	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600042A RID: 1066 RVA: 0x00019614 File Offset: 0x00017814
	public override bool Loaded
	{
		get
		{
			bool flag = true;
			int count = this.sectors.Count;
			int num = 0;
			while (num < count && flag)
			{
				SECTR_Sector sectr_Sector = this.sectors[num];
				if (sectr_Sector.Frozen)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component && !component.IsLoaded())
					{
						flag = false;
						break;
					}
				}
				num++;
			}
			return flag;
		}
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00019468 File Offset: 0x00017668
	private void Start()
	{
		base.LockSelf(true);
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00019674 File Offset: 0x00017874
	private void OnDisable()
	{
		int count = this.sectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sectr_Sector = this.sectors[i];
			if (sectr_Sector)
			{
				SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
				if (component)
				{
					component.RemoveReference();
				}
			}
		}
		this.sectors.Clear();
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000196D0 File Offset: 0x000178D0
	private void Update()
	{
		Vector3 position = base.transform.position;
		if (this.firstRegionCheck || (position - this.lastRegionCheckPos).sqrMagnitude >= 1f)
		{
			this.firstRegionCheck = false;
			this.lastRegionCheckPos = position;
			Bounds bounds = new Bounds(position, this.LoadSize);
			Bounds bounds2 = new Bounds(position, this.LoadSize * (1f + this.UnloadBuffer));
			SECTR_Sector.GetContaining(ref this.loadSectors, bounds, false);
			SECTR_Sector.GetContaining(ref this.unloadSectors, bounds2, false);
			int i = 0;
			int num = this.sectors.Count;
			while (i < num)
			{
				SECTR_Sector sectr_Sector = this.sectors[i];
				if (this.loadSectors.Contains(sectr_Sector))
				{
					this.loadSectors.Remove(sectr_Sector);
					i++;
				}
				else if (!this.unloadSectors.Contains(sectr_Sector))
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component)
					{
						component.RemoveReference();
					}
					this.sectors.RemoveAt(i);
					num--;
				}
				else
				{
					i++;
				}
			}
			num = this.loadSectors.Count;
			int value = this.LayersToLoad.value;
			if (num > 0)
			{
				for (i = 0; i < num; i++)
				{
					SECTR_Sector sectr_Sector2 = this.loadSectors[i];
					if (sectr_Sector2.Frozen && (value & 1 << sectr_Sector2.gameObject.layer) != 0)
					{
						SECTR_Chunk component2 = sectr_Sector2.GetComponent<SECTR_Chunk>();
						if (component2)
						{
							component2.AddReference();
						}
						this.sectors.Add(sectr_Sector2);
					}
				}
			}
			this.UpdateAwake();
		}
		if (this.locked && this.Loaded)
		{
			base.LockSelf(false);
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00019894 File Offset: 0x00017A94
	private void UpdateAwake()
	{
		Vector3 position = base.transform.position;
		Bounds bounds = new Bounds(position, this.WakeSize);
		Bounds bounds2 = new Bounds(position, this.WakeSize * (1f + this.UnloadBuffer));
		SECTR_Sector.GetContaining(ref this.loadSectors, bounds, false);
		SECTR_Sector.GetContaining(ref this.unloadSectors, bounds2, false);
		int i = 0;
		int num = this.wakeSectors.Count;
		while (i < num)
		{
			SECTR_Sector sectr_Sector = this.wakeSectors[i];
			if (this.loadSectors.Contains(sectr_Sector))
			{
				this.loadSectors.Remove(sectr_Sector);
				i++;
			}
			else if (!this.unloadSectors.Contains(sectr_Sector))
			{
				SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
				if (component)
				{
					component.RemoveWakeReference();
				}
				this.wakeSectors.RemoveAt(i);
				num--;
			}
			else
			{
				i++;
			}
		}
		num = this.loadSectors.Count;
		int value = this.LayersToLoad.value;
		if (num > 0)
		{
			for (i = 0; i < num; i++)
			{
				SECTR_Sector sectr_Sector2 = this.loadSectors[i];
				if (sectr_Sector2.Hibernate && (value & 1 << sectr_Sector2.gameObject.layer) != 0)
				{
					SECTR_Chunk component2 = sectr_Sector2.GetComponent<SECTR_Chunk>();
					if (component2)
					{
						component2.AddWakeReference();
					}
					this.wakeSectors.Add(sectr_Sector2);
				}
			}
		}
	}

	// Token: 0x04000416 RID: 1046
	private List<SECTR_Sector> sectors = new List<SECTR_Sector>(16);

	// Token: 0x04000417 RID: 1047
	private List<SECTR_Sector> wakeSectors = new List<SECTR_Sector>(16);

	// Token: 0x04000418 RID: 1048
	private List<SECTR_Sector> loadSectors = new List<SECTR_Sector>(16);

	// Token: 0x04000419 RID: 1049
	private List<SECTR_Sector> unloadSectors = new List<SECTR_Sector>(16);

	// Token: 0x0400041A RID: 1050
	private bool firstRegionCheck = true;

	// Token: 0x0400041B RID: 1051
	private Vector3 lastRegionCheckPos;

	// Token: 0x0400041C RID: 1052
	private const float REGION_UPDATE_DIST = 1f;

	// Token: 0x0400041D RID: 1053
	private const float REGION_UPDATE_DIST_SQR = 1f;

	// Token: 0x0400041E RID: 1054
	[SECTR_ToolTip("The dimensions of the volume in which stuff should be unhibernated/awake.")]
	public Vector3 WakeSize = new Vector3(20f, 10f, 20f);

	// Token: 0x0400041F RID: 1055
	[SECTR_ToolTip("The dimensions of the volume in which terrain chunks should be loaded.")]
	public Vector3 LoadSize = new Vector3(20f, 10f, 20f);

	// Token: 0x04000420 RID: 1056
	[SECTR_ToolTip("The distance from the load size that you need to move for a Sector to unload (as a percentage).", 0f, 1f)]
	public float UnloadBuffer = 0.1f;

	// Token: 0x04000421 RID: 1057
	[SECTR_ToolTip("If set, will only load Sectors in matching layers.")]
	public LayerMask LayersToLoad = -1;
}
