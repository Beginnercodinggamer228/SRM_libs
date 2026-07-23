using System;
using UnityEngine;

// Token: 0x020006C3 RID: 1731
public class AreaMarker : SRBehaviour
{
	// Token: 0x06002419 RID: 9241 RVA: 0x0008B880 File Offset: 0x00089A80
	public static GameObject CreateMarkerObject(Transform parent, Vector3 pos, Color color, float radius)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = pos;
		AreaMarker areaMarker = gameObject.AddComponent<AreaMarker>();
		areaMarker.color = color;
		areaMarker.radius = radius;
		return gameObject;
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x0008B8B2 File Offset: 0x00089AB2
	public static void Link(GameObject gameObj1, GameObject gameObj2, Color color)
	{
		AreaMarker.LinkMarker linkMarker = gameObj1.AddComponent<AreaMarker.LinkMarker>();
		linkMarker.color = color;
		linkMarker.from = gameObj1.transform.position;
		linkMarker.to = gameObj2.transform.position;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x0008B8E2 File Offset: 0x00089AE2
	public static void Link(Vector3 pos1, Vector3 pos2, Color color)
	{
		AreaMarker.LinkMarker linkMarker = new GameObject().AddComponent<AreaMarker.LinkMarker>();
		linkMarker.color = color;
		linkMarker.from = pos1;
		linkMarker.to = pos2;
	}

	// Token: 0x04002334 RID: 9012
	public Color color;

	// Token: 0x04002335 RID: 9013
	public float radius;

	// Token: 0x020006C4 RID: 1732
	private class LinkMarker : SRBehaviour
	{
		// Token: 0x04002336 RID: 9014
		public Color color;

		// Token: 0x04002337 RID: 9015
		public Vector3 from;

		// Token: 0x04002338 RID: 9016
		public Vector3 to;
	}
}
