using System;
using UnityEngine;

// Token: 0x02000114 RID: 276
public class DebugUI : SRBehaviour
{
	// Token: 0x040005C9 RID: 1481
	[Tooltip("Grid object to parent buttons to.")]
	public GameObject grid;

	// Token: 0x040005CA RID: 1482
	[Tooltip("Debug button prefab.")]
	public GameObject buttonPrefab;

	// Token: 0x040005CB RID: 1483
	[Tooltip("Tab button left/previous. (optional)")]
	public GameObject buttonTabLeft;

	// Token: 0x040005CC RID: 1484
	[Tooltip("Tab button right/next. (optional)")]
	public GameObject buttonTabRight;

	// Token: 0x040005CD RID: 1485
	[Tooltip("Number of buttons to display on each page.")]
	public int buttonsPerPage = 10;
}
