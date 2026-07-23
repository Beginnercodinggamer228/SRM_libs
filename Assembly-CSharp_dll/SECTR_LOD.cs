using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BF RID: 191
[ExecuteInEditMode]
[RequireComponent(typeof(SECTR_Member))]
public class SECTR_LOD : MonoBehaviour
{
	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600045E RID: 1118 RVA: 0x0001C400 File Offset: 0x0001A600
	public static List<SECTR_LOD> All
	{
		get
		{
			return SECTR_LOD.allLODs;
		}
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001C408 File Offset: 0x0001A608
	public void SelectLOD(Camera renderCamera)
	{
		if (renderCamera)
		{
			if (!this.boundsUpdated)
			{
				this._CalculateBounds();
			}
			Vector3 b = base.transform.localToWorldMatrix.MultiplyPoint3x4(this.boundsOffset);
			float num = Vector3.Distance(renderCamera.transform.position, b);
			float num2 = this.boundsRadius / (Mathf.Tan(renderCamera.fieldOfView * 0.5f * 0.017453292f) * num) * 2f;
			int num3 = -1;
			int count = this.LODs.Count;
			for (int i = 0; i < count; i++)
			{
				float num4 = this.LODs[i].Threshold;
				if (i == this.activeLOD)
				{
					num4 -= 0.05f;
				}
				if (num2 >= num4)
				{
					num3 = i;
					break;
				}
			}
			if (num3 != this.activeLOD)
			{
				this._ActivateLOD(num3);
			}
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001C4E4 File Offset: 0x0001A6E4
	private void OnEnable()
	{
		SECTR_LOD.allLODs.Add(this);
		this.cachedMember = base.GetComponent<SECTR_Member>();
		SECTR_CullingCamera sectr_CullingCamera = (SECTR_CullingCamera.All.Count > 0) ? SECTR_CullingCamera.All[0] : null;
		if (sectr_CullingCamera)
		{
			this.SelectLOD(sectr_CullingCamera.GetComponent<Camera>());
			return;
		}
		this._ActivateLOD(0);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001C540 File Offset: 0x0001A740
	private void OnDisable()
	{
		SECTR_LOD.allLODs.Remove(this);
		this.cachedMember = null;
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0001C558 File Offset: 0x0001A758
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.localToWorldMatrix.MultiplyPoint(this.boundsOffset), this.boundsRadius);
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0001C5A0 File Offset: 0x0001A7A0
	private void _ActivateLOD(int lodIndex)
	{
		this.toHide.Clear();
		this.toShow.Clear();
		if (this.activeLOD >= 0 && this.activeLOD < this.LODs.Count)
		{
			SECTR_LOD.LODSet lodset = this.LODs[this.activeLOD];
			int count = lodset.LODEntries.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_LOD.LODEntry lodentry = lodset.LODEntries[i];
				if (lodentry.gameObject)
				{
					this.toHide.Add(lodentry.gameObject);
				}
			}
		}
		if (lodIndex >= 0 && lodIndex < this.LODs.Count)
		{
			SECTR_LOD.LODSet lodset2 = this.LODs[lodIndex];
			int count2 = lodset2.LODEntries.Count;
			for (int j = 0; j < count2; j++)
			{
				SECTR_LOD.LODEntry lodentry2 = lodset2.LODEntries[j];
				if (lodentry2.gameObject)
				{
					this.toHide.Remove(lodentry2.gameObject);
					this.toShow.Add(lodentry2);
				}
			}
		}
		int count3 = this.toHide.Count;
		for (int k = 0; k < count3; k++)
		{
			this.toHide[k].SetActive(false);
		}
		int count4 = this.toShow.Count;
		for (int l = 0; l < count4; l++)
		{
			SECTR_LOD.LODEntry lodentry3 = this.toShow[l];
			lodentry3.gameObject.SetActive(true);
			if (lodentry3.lightmapSource)
			{
				Renderer component = lodentry3.gameObject.GetComponent<Renderer>();
				if (component)
				{
					component.lightmapIndex = lodentry3.lightmapSource.lightmapIndex;
					component.lightmapScaleOffset = lodentry3.lightmapSource.lightmapScaleOffset;
				}
			}
		}
		this.cachedMember.ForceUpdate(true, false);
		this.activeLOD = lodIndex;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001C780 File Offset: 0x0001A980
	private void _CalculateBounds()
	{
		Bounds bounds = default(Bounds);
		int count = this.LODs.Count;
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			SECTR_LOD.LODSet lodset = this.LODs[i];
			int count2 = lodset.LODEntries.Count;
			for (int j = 0; j < count2; j++)
			{
				GameObject gameObject = lodset.LODEntries[j].gameObject;
				Renderer renderer = gameObject ? gameObject.GetComponent<Renderer>() : null;
				if (renderer && renderer.bounds.extents != Vector3.zero)
				{
					if (!flag)
					{
						bounds = renderer.bounds;
						flag = true;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
		}
		this.boundsOffset = base.transform.worldToLocalMatrix.MultiplyPoint(bounds.center);
		this.boundsRadius = bounds.extents.magnitude;
		this.boundsUpdated = true;
	}

	// Token: 0x04000469 RID: 1129
	[SerializeField]
	[HideInInspector]
	private Vector3 boundsOffset;

	// Token: 0x0400046A RID: 1130
	[SerializeField]
	[HideInInspector]
	private float boundsRadius;

	// Token: 0x0400046B RID: 1131
	[SerializeField]
	[HideInInspector]
	private bool boundsUpdated;

	// Token: 0x0400046C RID: 1132
	private int activeLOD = -1;

	// Token: 0x0400046D RID: 1133
	private SECTR_Member cachedMember;

	// Token: 0x0400046E RID: 1134
	private List<GameObject> toHide = new List<GameObject>(32);

	// Token: 0x0400046F RID: 1135
	private List<SECTR_LOD.LODEntry> toShow = new List<SECTR_LOD.LODEntry>(32);

	// Token: 0x04000470 RID: 1136
	private static List<SECTR_LOD> allLODs = new List<SECTR_LOD>(128);

	// Token: 0x04000471 RID: 1137
	public List<SECTR_LOD.LODSet> LODs = new List<SECTR_LOD.LODSet>();

	// Token: 0x020000C0 RID: 192
	[Serializable]
	public class LODEntry
	{
		// Token: 0x04000472 RID: 1138
		public GameObject gameObject;

		// Token: 0x04000473 RID: 1139
		public Renderer lightmapSource;
	}

	// Token: 0x020000C1 RID: 193
	[Serializable]
	public class LODSet
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x0001C8D1 File Offset: 0x0001AAD1
		public List<SECTR_LOD.LODEntry> LODEntries
		{
			get
			{
				return this.lodEntries;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x0001C8D9 File Offset: 0x0001AAD9
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x0001C8E1 File Offset: 0x0001AAE1
		public float Threshold
		{
			get
			{
				return this.threshold;
			}
			set
			{
				this.threshold = value;
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001C8EC File Offset: 0x0001AAEC
		public SECTR_LOD.LODEntry Add(GameObject gameObject, Renderer lightmapSource)
		{
			if (this.GetEntry(gameObject) == null)
			{
				SECTR_LOD.LODEntry lodentry = new SECTR_LOD.LODEntry();
				lodentry.gameObject = gameObject;
				lodentry.lightmapSource = lightmapSource;
				this.lodEntries.Add(lodentry);
				return lodentry;
			}
			return null;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001C928 File Offset: 0x0001AB28
		public void Remove(GameObject gameObject)
		{
			int i = 0;
			while (i < this.lodEntries.Count)
			{
				if (this.lodEntries[i].gameObject == gameObject)
				{
					this.lodEntries.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001C974 File Offset: 0x0001AB74
		public SECTR_LOD.LODEntry GetEntry(GameObject gameObject)
		{
			int count = this.lodEntries.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_LOD.LODEntry lodentry = this.lodEntries[i];
				if (lodentry.gameObject == gameObject)
				{
					return lodentry;
				}
			}
			return null;
		}

		// Token: 0x04000474 RID: 1140
		[SerializeField]
		private List<SECTR_LOD.LODEntry> lodEntries = new List<SECTR_LOD.LODEntry>(16);

		// Token: 0x04000475 RID: 1141
		[SerializeField]
		private float threshold;
	}
}
