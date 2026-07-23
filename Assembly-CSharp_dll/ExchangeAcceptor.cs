using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class ExchangeAcceptor : SRBehaviour, VacShootAccelerator
{
	// Token: 0x0600106C RID: 4204 RVA: 0x0004195C File Offset: 0x0003FB5C
	public void Awake()
	{
		this.exchangeDir = SRSingleton<SceneContext>.Instance.ExchangeDirector;
		this.acceptAudio = base.GetComponent<SECTR_AudioSource>();
		this.awarders = base.transform.parent.GetComponentsInChildren<ExchangeDirector.Awarder>();
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00041990 File Offset: 0x0003FB90
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		Identifiable component = col.gameObject.GetComponent<Identifiable>();
		if (component != null && !this.acceptedThisFrame.Contains(col.gameObject) && this.TryAcceptAllOfferTypes(component.id))
		{
			if (this.storeFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.storeFX, col.transform.position, col.transform.rotation);
				this.acceptAudio.Play();
			}
			this.acceptedThisFrame.Add(col.gameObject);
			Destroyer.DestroyActor(col.gameObject, "ExchangeAcceptor.OnTriggerEnter", false);
			this.accelerationInput.OnTriggered();
		}
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00041A48 File Offset: 0x0003FC48
	private bool TryAcceptAllOfferTypes(Identifiable.Id id)
	{
		bool flag = false;
		foreach (ExchangeDirector.OfferType type in this.offerTypes)
		{
			flag |= this.exchangeDir.TryAccept(type, id, this.awarders);
		}
		return flag;
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x00041A87 File Offset: 0x0003FC87
	public void LateUpdate()
	{
		this.acceptedThisFrame.Clear();
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x00041A94 File Offset: 0x0003FC94
	public float GetVacShootSpeedFactor()
	{
		return this.accelerationInput.Factor;
	}

	// Token: 0x04000F2D RID: 3885
	public GameObject storeFX;

	// Token: 0x04000F2E RID: 3886
	public ExchangeDirector.OfferType[] offerTypes;

	// Token: 0x04000F2F RID: 3887
	private ExchangeDirector.Awarder[] awarders;

	// Token: 0x04000F30 RID: 3888
	private ExchangeDirector exchangeDir;

	// Token: 0x04000F31 RID: 3889
	private SECTR_AudioSource acceptAudio;

	// Token: 0x04000F32 RID: 3890
	private HashSet<GameObject> acceptedThisFrame = new HashSet<GameObject>();

	// Token: 0x04000F33 RID: 3891
	private VacAccelerationHelper accelerationInput = VacAccelerationHelper.CreateInput();
}
