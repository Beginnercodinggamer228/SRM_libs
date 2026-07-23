using System;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class FashionRemover : MonoBehaviour
{
	// Token: 0x06001441 RID: 5185 RVA: 0x0004E57C File Offset: 0x0004C77C
	public void OnCollisionEnter(Collision col)
	{
		if (!this.used)
		{
			AttachFashions component = col.gameObject.GetComponent<AttachFashions>();
			if (component != null)
			{
				component.DetachAll(this);
				base.GetComponent<DestroyOnTouching>().NoteDestroying();
				Destroyer.DestroyActor(base.gameObject, "FashionRemover.OnCollisionEnter", false);
				this.used = true;
			}
		}
	}

	// Token: 0x04001306 RID: 4870
	public GameObject removeFX;

	// Token: 0x04001307 RID: 4871
	private bool used;
}
