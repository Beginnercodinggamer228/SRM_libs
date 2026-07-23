using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class DebugDirector : SRBehaviour
{
	// Token: 0x04000D1E RID: 3358
	[Tooltip("Debug UI prefab (default).")]
	public GameObject uiDefaultPrefab;

	// Token: 0x04000D1F RID: 3359
	[Tooltip("Debug UI prefab (gamepad).")]
	public GameObject uiGamepadPrefab;

	// Token: 0x04000D20 RID: 3360
	[Tooltip("Ginger tracker prefab.")]
	public GameObject gingerTrackerPrefab;

	// Token: 0x04000D21 RID: 3361
	[Tooltip("Slime Appearance Director")]
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x04000D22 RID: 3362
	[Tooltip("List of explicit prefabs to be spawnable.")]
	public List<GameObject> spawnablePrefabs;

	// Token: 0x04000D23 RID: 3363
	[Tooltip("List of ui prefabs to be instantiated.")]
	public List<GameObject> uiPrefabs;

	// Token: 0x04000D24 RID: 3364
	[Tooltip("List of Viktor imposto prefabs. (generated)")]
	public List<GameObject> impostos;

	// Token: 0x04000D25 RID: 3365
	[Tooltip("List of EchoNoteGameMetadata assets.")]
	public List<EchoNoteGameMetadata> echoNoteGameMetadatas;
}
