using System;

// Token: 0x020003C9 RID: 969
public class ExplodeIndicatorMarker : SRBehaviour
{
	// Token: 0x06001433 RID: 5171 RVA: 0x0004E1A4 File Offset: 0x0004C3A4
	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
		if (active)
		{
			SECTR_AudioSource component = base.GetComponent<SECTR_AudioSource>();
			if (component != null)
			{
				component.Play();
			}
		}
	}
}
