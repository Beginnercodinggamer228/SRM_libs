using System;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class TarrBoundingSphere : RegisteredActorBehaviour, RegistryLateUpdateable
{
	// Token: 0x060018E3 RID: 6371 RVA: 0x00060FE2 File Offset: 0x0005F1E2
	public static void ResetTarrData()
	{
		TarrBoundingSphere.allSpheres = new BoundingSphere[100];
		TarrBoundingSphere.sphereCount = 0;
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x00060FF8 File Offset: 0x0005F1F8
	public void RegistryLateUpdate()
	{
		if (TarrBoundingSphere.sphereCount == TarrBoundingSphere.allSpheres.Length)
		{
			Array.Resize<BoundingSphere>(ref TarrBoundingSphere.allSpheres, TarrBoundingSphere.allSpheres.Length + 100);
		}
		TarrBoundingSphere.allSpheres[TarrBoundingSphere.sphereCount].position = base.transform.position;
		TarrBoundingSphere.allSpheres[TarrBoundingSphere.sphereCount].radius = 2f;
		TarrBoundingSphere.sphereCount++;
	}

	// Token: 0x0400189D RID: 6301
	public const int TARR_BOUNDING_SPHERE_START_COUNT = 100;

	// Token: 0x0400189E RID: 6302
	private const int ARRAY_RESIZE_STEP = 100;

	// Token: 0x0400189F RID: 6303
	public static BoundingSphere[] allSpheres = new BoundingSphere[100];

	// Token: 0x040018A0 RID: 6304
	public static int sphereCount = 0;

	// Token: 0x040018A1 RID: 6305
	public static int nearbyTarr = 0;
}
