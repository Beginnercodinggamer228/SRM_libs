using System;
using System.Collections.Generic;
using DLCPackage;
using UnityEngine;

// Token: 0x02000106 RID: 262
[CreateAssetMenu(fileName = "DLC", menuName = "DLC/Package Metadata")]
public class DLCPackageMetadata : ScriptableObject
{
	// Token: 0x040005A7 RID: 1447
	public Id id;

	// Token: 0x040005A8 RID: 1448
	public Sprite icon;

	// Token: 0x040005A9 RID: 1449
	public List<DLCPackageMetadata.Content> contents;

	// Token: 0x02000107 RID: 263
	[Serializable]
	public class Content
	{
		// Token: 0x040005AA RID: 1450
		public string id;

		// Token: 0x040005AB RID: 1451
		public Sprite image;

		// Token: 0x040005AC RID: 1452
		public Sprite imageLarge;
	}
}
