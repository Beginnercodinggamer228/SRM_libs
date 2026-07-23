using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E0 RID: 480
[CreateAssetMenu(fileName = "EchoNoteGordo", menuName = "Echo Notes/Create Cluster")]
public class EchoNoteClusterMetadata : ScriptableObject
{
	// Token: 0x0400084C RID: 2124
	[Tooltip("List of ordered clip indices in 'cue'; add multiple clips separated by commas. (eg. '1, 2, 3').")]
	public List<string> clips;

	// Token: 0x0400084D RID: 2125
	[Tooltip("Distance between each clip (generation only).")]
	[Range(0f, 20f)]
	public float distance = 2f;
}
