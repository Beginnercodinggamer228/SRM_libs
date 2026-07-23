using System;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x02000396 RID: 918
public class BiteEventAggregator : MonoBehaviour
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06001330 RID: 4912 RVA: 0x0004B0CC File Offset: 0x000492CC
	// (remove) Token: 0x06001331 RID: 4913 RVA: 0x0004B104 File Offset: 0x00049304
	public event BiteEventAggregator.EnableBiteEvent OnEnableBite;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06001332 RID: 4914 RVA: 0x0004B13C File Offset: 0x0004933C
	// (remove) Token: 0x06001333 RID: 4915 RVA: 0x0004B174 File Offset: 0x00049374
	public event BiteEventAggregator.DisableBiteEvent OnDisableBite;

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06001334 RID: 4916 RVA: 0x0004B1AC File Offset: 0x000493AC
	// (remove) Token: 0x06001335 RID: 4917 RVA: 0x0004B1E4 File Offset: 0x000493E4
	public event BiteEventAggregator.SpawnBubblesEvent OnSpawnBubbles;

	// Token: 0x06001336 RID: 4918 RVA: 0x0004B219 File Offset: 0x00049419
	public void Awake()
	{
		this.bodyAnim = base.gameObject.GetRequiredComponentInChildren(false);
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x0004B22D File Offset: 0x0004942D
	public void EnableBiteModel()
	{
		if (this.OnEnableBite != null)
		{
			this.OnEnableBite();
		}
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x0004B242 File Offset: 0x00049442
	public void DisableBiteModel()
	{
		if (this.OnDisableBite != null)
		{
			this.OnDisableBite();
		}
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x0004B257 File Offset: 0x00049457
	public void SpawnBubbles()
	{
		if (this.OnSpawnBubbles != null)
		{
			this.OnSpawnBubbles();
		}
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x0004B26C File Offset: 0x0004946C
	public bool IsBiteAnimationStateActive()
	{
		return this.bodyAnim.GetCurrentAnimatorStateInfo(0).IsName("Bite") || this.bodyAnim.GetCurrentAnimatorStateInfo(0).IsName("BiteQuick");
	}

	// Token: 0x04001203 RID: 4611
	private Animator bodyAnim;

	// Token: 0x02000397 RID: 919
	// (Invoke) Token: 0x0600133D RID: 4925
	public delegate void EnableBiteEvent();

	// Token: 0x02000398 RID: 920
	// (Invoke) Token: 0x06001341 RID: 4929
	public delegate void DisableBiteEvent();

	// Token: 0x02000399 RID: 921
	// (Invoke) Token: 0x06001345 RID: 4933
	public delegate void SpawnBubblesEvent();
}
