using System;

// Token: 0x0200010D RID: 269
public class DeactivateOnHeld : SRBehaviour
{
	// Token: 0x060005EF RID: 1519 RVA: 0x00021E40 File Offset: 0x00020040
	public void Start()
	{
		this.parent = base.GetComponentInParent<Vacuumable>();
		if (this.parent != null)
		{
			this.parent.onSetHeld += this.OnSetHeld;
			this.OnSetHeld(this.parent.isHeld());
		}
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00021E8F File Offset: 0x0002008F
	public void OnDestroy()
	{
		if (this.parent != null)
		{
			this.parent.onSetHeld -= this.OnSetHeld;
			this.parent = null;
		}
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00021EBD File Offset: 0x000200BD
	private void OnSetHeld(bool held)
	{
		base.gameObject.SetActive(!held);
	}

	// Token: 0x040005B8 RID: 1464
	private Vacuumable parent;
}
