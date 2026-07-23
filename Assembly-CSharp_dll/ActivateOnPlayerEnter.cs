using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class ActivateOnPlayerEnter : MonoBehaviour
{
	// Token: 0x06002405 RID: 9221 RVA: 0x0008B145 File Offset: 0x00089345
	public void Awake()
	{
		this.toActivate.SetActive(false);
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x0008B154 File Offset: 0x00089354
	public void OnTriggerEnter(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.toActivate.SetActive(true);
		}
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x0008B188 File Offset: 0x00089388
	public void OnTriggerExit(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.toActivate.SetActive(false);
		}
	}

	// Token: 0x0400231A RID: 8986
	public GameObject toActivate;
}
