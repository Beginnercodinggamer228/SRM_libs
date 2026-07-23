using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B8 RID: 952
public class DestroyAndShockOnTouching : SRBehaviour
{
	// Token: 0x060013DA RID: 5082 RVA: 0x0004CF8F File Offset: 0x0004B18F
	public void Awake()
	{
		this.id = base.GetComponent<Identifiable>().id;
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0004CFA2 File Offset: 0x0004B1A2
	public void NoteDestroying()
	{
		this.destroying = true;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0004CFAC File Offset: 0x0004B1AC
	private void DestroyAndShock()
	{
		if (this.destroying)
		{
			return;
		}
		this.destroying = true;
		if (this.destroyFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
		}
		if (this.shockRadius > 0f)
		{
			SphereOverlapTrigger.CreateGameObject(base.transform.position, this.shockRadius, delegate(IEnumerable<Collider> colliders)
			{
				HashSet<ReactToShock> hashSet = new HashSet<ReactToShock>();
				foreach (Collider collider in colliders)
				{
					foreach (ReactToShock item in collider.gameObject.GetComponentsInParent<ReactToShock>())
					{
						hashSet.Add(item);
					}
				}
				foreach (ReactToShock reactToShock in hashSet)
				{
					reactToShock.DoShock(this.id);
				}
			}, 15);
		}
		Destroyer.DestroyActor(base.gameObject, "DestroyAndShockOnTouching.DestroyAndShock", false);
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0004D03C File Offset: 0x0004B23C
	public void OnCollisionEnter(Collision col)
	{
		base.StartCoroutine(this.DestroyAndShockAtEndOfFrame());
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0004D04B File Offset: 0x0004B24B
	private IEnumerator DestroyAndShockAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.DestroyAndShock();
		yield break;
	}

	// Token: 0x0400129B RID: 4763
	[Tooltip("When we poof, how large an area is shocked.")]
	public float shockRadius;

	// Token: 0x0400129C RID: 4764
	[Tooltip("The effect to play when we poof.")]
	public GameObject destroyFX;

	// Token: 0x0400129D RID: 4765
	private bool destroying;

	// Token: 0x0400129E RID: 4766
	private Identifiable.Id id;
}
