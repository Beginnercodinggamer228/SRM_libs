using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class SphereOverlapTrigger : MonoBehaviour
{
	// Token: 0x0600238B RID: 9099 RVA: 0x00089E20 File Offset: 0x00088020
	public void OnTriggerEnter(Collider col)
	{
		this.colliders.Add(col);
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x00089E2E File Offset: 0x0008802E
	public void FixedUpdate()
	{
		this.hasDoneOneFixedUpdate = true;
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x00089E38 File Offset: 0x00088038
	public void LateUpdate()
	{
		if (!this.hasDoneOneFixedUpdate)
		{
			return;
		}
		try
		{
			if (this.onSphereOverlap != null)
			{
				this.onSphereOverlap(from c in this.colliders
				where c != null
				select c);
			}
		}
		finally
		{
			this.onSphereOverlap = null;
			Destroyer.Destroy(base.gameObject, "SphereOverlapTrigger.LateUpdate");
		}
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x00089EB8 File Offset: 0x000880B8
	public static GameObject CreateGameObject(Vector3 center, float radius, SphereOverlapTrigger.OnSphereOverlap onOverlap, int layer = 0)
	{
		GameObject gameObject = new GameObject("SphereOverlapTrigger");
		gameObject.transform.position = center;
		gameObject.layer = layer;
		SphereOverlapTrigger sphereOverlapTrigger = gameObject.AddComponent<SphereOverlapTrigger>();
		sphereOverlapTrigger.onSphereOverlap = (SphereOverlapTrigger.OnSphereOverlap)Delegate.Combine(sphereOverlapTrigger.onSphereOverlap, onOverlap);
		SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.radius = radius;
		sphereCollider.isTrigger = true;
		gameObject.AddComponent<Rigidbody>().isKinematic = true;
		return gameObject;
	}

	// Token: 0x040022C7 RID: 8903
	public SphereOverlapTrigger.OnSphereOverlap onSphereOverlap;

	// Token: 0x040022C8 RID: 8904
	private List<Collider> colliders = new List<Collider>();

	// Token: 0x040022C9 RID: 8905
	private bool hasDoneOneFixedUpdate;

	// Token: 0x020006A7 RID: 1703
	// (Invoke) Token: 0x06002391 RID: 9105
	public delegate void OnSphereOverlap(IEnumerable<Collider> colliders);
}
