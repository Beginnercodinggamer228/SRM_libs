using System;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class Fashion : MonoBehaviour
{
	// Token: 0x0600143F RID: 5183 RVA: 0x0004E524 File Offset: 0x0004C724
	public void OnCollisionEnter(Collision col)
	{
		if (!this.used)
		{
			AttachFashions component = col.gameObject.GetComponent<AttachFashions>();
			if (component != null)
			{
				component.Attach(this, false);
				base.GetComponent<DestroyOnTouching>().NoteDestroying();
				Destroyer.DestroyActor(base.gameObject, "Fashion.OnCollisionEnter", false);
				this.used = true;
			}
		}
	}

	// Token: 0x040012FF RID: 4863
	public Fashion.Slot slot;

	// Token: 0x04001300 RID: 4864
	public GameObject attachPrefab;

	// Token: 0x04001301 RID: 4865
	public GameObject attachFX;

	// Token: 0x04001302 RID: 4866
	private bool used;

	// Token: 0x020003CE RID: 974
	public enum Slot
	{
		// Token: 0x04001304 RID: 4868
		TOP,
		// Token: 0x04001305 RID: 4869
		FRONT
	}
}
