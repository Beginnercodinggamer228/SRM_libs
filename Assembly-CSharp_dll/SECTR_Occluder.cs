using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C2 RID: 194
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("SECTR/Vis/SECTR Occluder")]
public class SECTR_Occluder : SECTR_Hull
{
	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x0600046F RID: 1135 RVA: 0x0001C9CC File Offset: 0x0001ABCC
	public static List<SECTR_Occluder> All
	{
		get
		{
			return SECTR_Occluder.allOccluders;
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001C9D4 File Offset: 0x0001ABD4
	public static List<SECTR_Occluder> GetOccludersInSector(SECTR_Sector sector)
	{
		List<SECTR_Occluder> result = null;
		SECTR_Occluder.occluderTable.TryGetValue(sector, out result);
		return result;
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000471 RID: 1137 RVA: 0x0001C9F2 File Offset: 0x0001ABF2
	public SECTR_Member Member
	{
		get
		{
			return this.cachedMember;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000472 RID: 1138 RVA: 0x0001C9FA File Offset: 0x0001ABFA
	public Vector3 MeshNormal
	{
		get
		{
			base.ComputeVerts();
			return this.meshNormal;
		}
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0001CA08 File Offset: 0x0001AC08
	public Matrix4x4 GetCullingMatrix(Vector3 cameraPos)
	{
		if (this.AutoOrient == SECTR_Occluder.OrientationAxis.None)
		{
			return base.transform.localToWorldMatrix;
		}
		base.ComputeVerts();
		Vector3 position = base.transform.position;
		Vector3 toDirection = cameraPos - position;
		switch (this.AutoOrient)
		{
		case SECTR_Occluder.OrientationAxis.XZ:
			toDirection.y = 0f;
			break;
		case SECTR_Occluder.OrientationAxis.XY:
			toDirection.z = 0f;
			break;
		case SECTR_Occluder.OrientationAxis.YZ:
			toDirection.x = 0f;
			break;
		}
		return Matrix4x4.TRS(position, Quaternion.FromToRotation(this.meshNormal, toDirection), base.transform.lossyScale);
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0001CAA5 File Offset: 0x0001ACA5
	private void OnEnable()
	{
		this.cachedMember = base.GetComponent<SECTR_Member>();
		this.cachedMember.Changed += this._MembershipChanged;
		SECTR_Occluder.allOccluders.Add(this);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001CAD5 File Offset: 0x0001ACD5
	private void OnDisable()
	{
		SECTR_Occluder.allOccluders.Remove(this);
		this.cachedMember.Changed -= this._MembershipChanged;
		this.cachedMember = null;
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001CB04 File Offset: 0x0001AD04
	private void _MembershipChanged(List<SECTR_Sector> left, List<SECTR_Sector> joined)
	{
		if (joined != null)
		{
			int count = joined.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = joined[i];
				if (sectr_Sector)
				{
					List<SECTR_Occluder> list;
					if (!SECTR_Occluder.occluderTable.TryGetValue(sectr_Sector, out list))
					{
						list = new List<SECTR_Occluder>(4);
						SECTR_Occluder.occluderTable[sectr_Sector] = list;
					}
					list.Add(this);
					this.currentSectors.Add(sectr_Sector);
				}
			}
		}
		if (left != null)
		{
			int count2 = left.Count;
			for (int j = 0; j < count2; j++)
			{
				SECTR_Sector sectr_Sector2 = left[j];
				if (sectr_Sector2 && this.currentSectors.Contains(sectr_Sector2))
				{
					List<SECTR_Occluder> list2;
					if (SECTR_Occluder.occluderTable.TryGetValue(sectr_Sector2, out list2))
					{
						list2.Remove(this);
					}
					this.currentSectors.Remove(sectr_Sector2);
				}
			}
		}
	}

	// Token: 0x04000476 RID: 1142
	private SECTR_Member cachedMember;

	// Token: 0x04000477 RID: 1143
	private List<SECTR_Sector> currentSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000478 RID: 1144
	private static List<SECTR_Occluder> allOccluders = new List<SECTR_Occluder>(32);

	// Token: 0x04000479 RID: 1145
	private static Dictionary<SECTR_Sector, List<SECTR_Occluder>> occluderTable = new Dictionary<SECTR_Sector, List<SECTR_Occluder>>(32);

	// Token: 0x0400047A RID: 1146
	[SECTR_ToolTip("The axes that should orient towards the camera during culling (if any).")]
	public SECTR_Occluder.OrientationAxis AutoOrient;

	// Token: 0x020000C3 RID: 195
	public enum OrientationAxis
	{
		// Token: 0x0400047C RID: 1148
		None,
		// Token: 0x0400047D RID: 1149
		XYZ,
		// Token: 0x0400047E RID: 1150
		XZ,
		// Token: 0x0400047F RID: 1151
		XY,
		// Token: 0x04000480 RID: 1152
		YZ
	}
}
