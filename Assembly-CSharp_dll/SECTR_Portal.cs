using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009F RID: 159
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Core/SECTR Portal")]
public class SECTR_Portal : SECTR_Hull
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000371 RID: 881 RVA: 0x00016781 File Offset: 0x00014981
	public static List<SECTR_Portal> All
	{
		get
		{
			return SECTR_Portal.allPortals;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000373 RID: 883 RVA: 0x000167DC File Offset: 0x000149DC
	// (set) Token: 0x06000372 RID: 882 RVA: 0x00016788 File Offset: 0x00014988
	public SECTR_Sector FrontSector
	{
		get
		{
			if (!this.frontSector || !this.frontSector.enabled)
			{
				return null;
			}
			return this.frontSector;
		}
		set
		{
			if (this.frontSector != value)
			{
				if (this.frontSector)
				{
					this.frontSector.Deregister(this);
				}
				this.frontSector = value;
				if (this.frontSector)
				{
					this.frontSector.Register(this);
				}
			}
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000375 RID: 885 RVA: 0x00016854 File Offset: 0x00014A54
	// (set) Token: 0x06000374 RID: 884 RVA: 0x00016800 File Offset: 0x00014A00
	public SECTR_Sector BackSector
	{
		get
		{
			if (!this.backSector || !this.backSector.enabled)
			{
				return null;
			}
			return this.backSector;
		}
		set
		{
			if (this.backSector != value)
			{
				if (this.backSector)
				{
					this.backSector.Deregister(this);
				}
				this.backSector = value;
				if (this.backSector)
				{
					this.backSector.Register(this);
				}
			}
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000376 RID: 886 RVA: 0x00016878 File Offset: 0x00014A78
	// (set) Token: 0x06000377 RID: 887 RVA: 0x00016880 File Offset: 0x00014A80
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

	// Token: 0x06000378 RID: 888 RVA: 0x00016889 File Offset: 0x00014A89
	public IEnumerable<SECTR_Sector> GetSectors()
	{
		yield return this.FrontSector;
		yield return this.BackSector;
		yield break;
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00016899 File Offset: 0x00014A99
	public void SetFlag(SECTR_Portal.PortalFlags flag, bool on)
	{
		if (on)
		{
			this.Flags |= flag;
			return;
		}
		this.Flags &= ~flag;
	}

	// Token: 0x0600037A RID: 890 RVA: 0x000168BC File Offset: 0x00014ABC
	private void OnEnable()
	{
		SECTR_Portal.allPortals.Add(this);
		if (this.frontSector)
		{
			this.frontSector.Register(this);
		}
		if (this.backSector)
		{
			this.backSector.Register(this);
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x000168FB File Offset: 0x00014AFB
	private void OnDisable()
	{
		SECTR_Portal.allPortals.Remove(this);
		if (this.frontSector)
		{
			this.frontSector.Deregister(this);
		}
		if (this.backSector)
		{
			this.backSector.Deregister(this);
		}
	}

	// Token: 0x040003A2 RID: 930
	[SerializeField]
	[HideInInspector]
	private SECTR_Sector frontSector;

	// Token: 0x040003A3 RID: 931
	[SerializeField]
	[HideInInspector]
	private SECTR_Sector backSector;

	// Token: 0x040003A4 RID: 932
	private bool visited;

	// Token: 0x040003A5 RID: 933
	private static List<SECTR_Portal> allPortals = new List<SECTR_Portal>(128);

	// Token: 0x040003A6 RID: 934
	[SECTR_ToolTip("Flags for this Portal. Used in graph traversals and the like.", null, typeof(SECTR_Portal.PortalFlags))]
	public SECTR_Portal.PortalFlags Flags;

	// Token: 0x020000A0 RID: 160
	[Flags]
	public enum PortalFlags
	{
		// Token: 0x040003A8 RID: 936
		Closed = 1,
		// Token: 0x040003A9 RID: 937
		Locked = 2,
		// Token: 0x040003AA RID: 938
		PassThrough = 4
	}
}
