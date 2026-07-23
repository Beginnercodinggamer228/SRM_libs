using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000795 RID: 1941
public class TeleportSource : SRBehaviour
{
	// Token: 0x06002876 RID: 10358 RVA: 0x00099706 File Offset: 0x00097906
	public virtual void Awake()
	{
		this.network = SRSingleton<SceneContext>.Instance.TeleportNetwork;
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x00099718 File Offset: 0x00097918
	public void OnDisable()
	{
		this.waitForTriggerExit = false;
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x00099724 File Offset: 0x00097924
	public void OnTriggerEnter(Collider collider)
	{
		if (this.network.IsLinkFullyActive(this) && PhysicsUtil.IsPlayerMainCollider(collider))
		{
			TeleportablePlayer component = collider.gameObject.GetComponent<TeleportablePlayer>();
			if (component != null)
			{
				this.network.TeleportToDestination(component, this, this.destinationSetName, new Func<List<TeleportDestination>, TeleportDestination>(this.PickDestination));
			}
		}
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x0009977D File Offset: 0x0009797D
	public void OnTriggerExit(Collider collider)
	{
		if (PhysicsUtil.IsPlayerMainCollider(collider))
		{
			this.waitForTriggerExit = false;
		}
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x0009978E File Offset: 0x0009798E
	public virtual void OnDepart()
	{
		if (this.departFX != null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.departFX, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000997C0 File Offset: 0x000979C0
	public void Update()
	{
		bool flag = this.activated;
		this.activated = this.network.IsLinkFullyActive(this);
		if (this.activated && !flag && this.setProgressTypesOnActivate != null)
		{
			foreach (ProgressDirector.ProgressType type in this.setProgressTypesOnActivate)
			{
				SRSingleton<SceneContext>.Instance.ProgressDirector.SetProgress(type, 1);
			}
		}
		if (this.activeFX != null)
		{
			this.activeFX.SetActive(this.activated);
		}
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x00099842 File Offset: 0x00097A42
	public void ExternalActivate()
	{
		this.waitForExternalActivation = false;
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x0009984C File Offset: 0x00097A4C
	public virtual bool IsLinkActive()
	{
		return !this.waitForTriggerExit && !this.waitForExternalActivation && (!this.activationBlocker || !this.activationBlocker.activeSelf) && (this.activationProgress == ProgressDirector.ProgressType.NONE || SRSingleton<SceneContext>.Instance.ProgressDirector.HasProgress(this.activationProgress)) && (!(this.blockingGenerator != null) || (this.blockingGenerator.GetState() != QuicksilverEnergyGenerator.State.ACTIVE && this.blockingGenerator.GetState() != QuicksilverEnergyGenerator.State.COUNTDOWN));
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000998D8 File Offset: 0x00097AD8
	protected virtual TeleportDestination PickDestination(List<TeleportDestination> destinations)
	{
		return Randoms.SHARED.Pick<TeleportDestination>(destinations, null);
	}

	// Token: 0x04002818 RID: 10264
	public GameObject activationBlocker;

	// Token: 0x04002819 RID: 10265
	[Tooltip("Required progress to activate the teleporter.")]
	public ProgressDirector.ProgressType activationProgress = ProgressDirector.ProgressType.NONE;

	// Token: 0x0400281A RID: 10266
	[Tooltip("Progresses that are set when the teleporter becomes active.")]
	public ProgressDirector.ProgressType[] setProgressTypesOnActivate;

	// Token: 0x0400281B RID: 10267
	[Tooltip("QuicksilverEnergyGenerator that must not be active to use the teleporter. (optional)")]
	public QuicksilverEnergyGenerator blockingGenerator;

	// Token: 0x0400281C RID: 10268
	public GameObject departFX;

	// Token: 0x0400281D RID: 10269
	public GameObject activeFX;

	// Token: 0x0400281E RID: 10270
	public string destinationSetName;

	// Token: 0x0400281F RID: 10271
	[HideInInspector]
	public bool waitForExternalActivation;

	// Token: 0x04002820 RID: 10272
	[HideInInspector]
	public bool waitForTriggerExit;

	// Token: 0x04002821 RID: 10273
	private TeleportNetwork network;

	// Token: 0x04002822 RID: 10274
	private bool activated;
}
