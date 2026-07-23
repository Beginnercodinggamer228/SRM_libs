using System;
using System.Collections;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public class AccessDoor : IdHandler, AccessDoorModel.Participant
{
	// Token: 0x060023E8 RID: 9192 RVA: 0x0008AC58 File Offset: 0x00088E58
	public virtual void Awake()
	{
		this.animOpenId = Animator.StringToHash("Open");
		this.animOpenImmediateId = Animator.StringToHash("OpenImmediate");
		Animator[] array = this.externalAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.AddComponent<AccessDoor.DoorAnimatorUpdater>().Init(this);
		}
		SRSingleton<SceneContext>.Instance.GameModel.RegisterDoor(base.id, base.gameObject);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x0008ACC8 File Offset: 0x00088EC8
	public virtual void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterDoor(base.id);
		}
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x0008ACEC File Offset: 0x00088EEC
	public void OnEnable()
	{
		if (this.model != null)
		{
			this.ForceUpdate(true);
		}
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x0008ACFD File Offset: 0x00088EFD
	public void InitModel(AccessDoorModel model)
	{
		model.progress = this.progress;
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x0008AD0B File Offset: 0x00088F0B
	public void SetModel(AccessDoorModel model)
	{
		this.model = model;
		this.MaybeRecountProgress();
		this.ForceUpdate(true);
	}

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x060023ED RID: 9197 RVA: 0x0008AD22 File Offset: 0x00088F22
	// (set) Token: 0x060023EE RID: 9198 RVA: 0x0008AD30 File Offset: 0x00088F30
	public AccessDoor.State CurrState
	{
		get
		{
			return this.model.state;
		}
		set
		{
			if (this.openCue != null && value == AccessDoor.State.OPEN && this.model.state != AccessDoor.State.OPEN)
			{
				base.StartCoroutine(this.DelayedPlayCue(this.openCueDelay));
			}
			this.model.state = value;
			this.MaybeRecountProgress();
			this.ForceUpdate(false);
		}
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x0008AD8A File Offset: 0x00088F8A
	public IEnumerator DelayedPlayCue(float delay)
	{
		yield return new WaitForSeconds(delay);
		SECTR_AudioSystem.Play(this.openCue, base.transform.position, false);
		yield break;
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x0008ADA0 File Offset: 0x00088FA0
	public virtual bool MaybeRecountProgress()
	{
		if (this.CurrState != AccessDoor.State.LOCKED)
		{
			ProgressDirector progressDirector = SRSingleton<SceneContext>.Instance.ProgressDirector;
			foreach (ProgressDirector.ProgressType progressType in this.progress)
			{
				int num = 0;
				foreach (AccessDoorModel accessDoorModel in SRSingleton<SceneContext>.Instance.GameModel.AllDoors().Values)
				{
					if (accessDoorModel.state != AccessDoor.State.LOCKED && Array.IndexOf<ProgressDirector.ProgressType>(accessDoorModel.progress, progressType) != -1)
					{
						num++;
					}
				}
				progressDirector.SetProgress(progressType, num);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x0008AE5C File Offset: 0x0008905C
	private void ForceUpdate(bool immediate)
	{
		this.updateRequest = true;
		this.updateRequestImmediate = (this.updateRequestImmediate || immediate);
		this.ForceUpdateBarrierController();
		Animator[] array = this.externalAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<AccessDoor.DoorAnimatorUpdater>().ForceUpdate();
		}
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x0008AEA6 File Offset: 0x000890A6
	public virtual void Update()
	{
		if (this.updateRequest)
		{
			this.ForceUpdateBarrierController();
			this.ForceUpdateAnimator();
			this.updateRequest = false;
			this.updateRequestImmediate = false;
		}
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x0008AECC File Offset: 0x000890CC
	private void ForceUpdateBarrierController()
	{
		BarrierController componentInChildren = base.GetComponentInChildren<BarrierController>();
		if (this.updateRequestImmediate && this.CurrState == AccessDoor.State.OPEN)
		{
			if (componentInChildren != null)
			{
				componentInChildren.SetIsOpen(true);
			}
			if (this.deactivateOnImmediateOpen != null)
			{
				GameObject[] array = this.deactivateOnImmediateOpen;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(false);
				}
			}
		}
		if (componentInChildren != null)
		{
			componentInChildren.SetIsOpen(this.CurrState == AccessDoor.State.OPEN);
		}
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x0008AF40 File Offset: 0x00089140
	private void ForceUpdateAnimator()
	{
		Animator componentInChildren = base.GetComponentInChildren<Animator>();
		if (this.updateRequestImmediate && this.CurrState == AccessDoor.State.OPEN && componentInChildren != null)
		{
			componentInChildren.SetTrigger(this.animOpenImmediateId);
		}
		if (componentInChildren != null)
		{
			componentInChildren.SetBool(this.animOpenId, this.CurrState == AccessDoor.State.OPEN);
		}
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x0008AF98 File Offset: 0x00089198
	protected override string IdPrefix()
	{
		return "door";
	}

	// Token: 0x040022FF RID: 8959
	public AccessDoor.DoorPurchaseItem doorPurchase;

	// Token: 0x04002300 RID: 8960
	public PediaDirector.Id lockedRegionId;

	// Token: 0x04002301 RID: 8961
	public AccessDoor[] linkedDoors;

	// Token: 0x04002302 RID: 8962
	[Tooltip("Progress to record when the door is unlocked.")]
	public ProgressDirector.ProgressType[] progress;

	// Token: 0x04002303 RID: 8963
	public SECTR_AudioCue openCue;

	// Token: 0x04002304 RID: 8964
	public float openCueDelay = 3f;

	// Token: 0x04002305 RID: 8965
	[Tooltip("Other elements to include in the open/close animation.")]
	public Animator[] externalAnimators;

	// Token: 0x04002306 RID: 8966
	public GameObject[] deactivateOnImmediateOpen;

	// Token: 0x04002307 RID: 8967
	private int animOpenId;

	// Token: 0x04002308 RID: 8968
	private int animOpenImmediateId;

	// Token: 0x04002309 RID: 8969
	private AccessDoorModel model;

	// Token: 0x0400230A RID: 8970
	private bool updateRequest = true;

	// Token: 0x0400230B RID: 8971
	private bool updateRequestImmediate;

	// Token: 0x020006BB RID: 1723
	public enum State
	{
		// Token: 0x0400230D RID: 8973
		LOCKED,
		// Token: 0x0400230E RID: 8974
		OPEN,
		// Token: 0x0400230F RID: 8975
		CLOSED
	}

	// Token: 0x020006BC RID: 1724
	[Serializable]
	public class DoorPurchaseItem
	{
		// Token: 0x04002310 RID: 8976
		public Sprite icon;

		// Token: 0x04002311 RID: 8977
		public Sprite img;

		// Token: 0x04002312 RID: 8978
		public int cost;
	}

	// Token: 0x020006BD RID: 1725
	private class DoorAnimatorUpdater : MonoBehaviour
	{
		// Token: 0x060023F8 RID: 9208 RVA: 0x0008AFB9 File Offset: 0x000891B9
		public void Init(AccessDoor door)
		{
			this.door = door;
			this.animOpenId = Animator.StringToHash("Open");
			this.ForceUpdate();
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x0008AFD8 File Offset: 0x000891D8
		public void OnEnable()
		{
			this.ForceUpdate();
		}

		// Token: 0x060023FA RID: 9210 RVA: 0x0008AFE0 File Offset: 0x000891E0
		public void Update()
		{
			if (this.updateRequest)
			{
				this.ForceUpdateBarrierController();
				this.ForceUpdateAnimator();
				this.updateRequest = false;
			}
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x0008AFFD File Offset: 0x000891FD
		public void ForceUpdate()
		{
			this.updateRequest = true;
			this.ForceUpdateBarrierController();
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x0008B00C File Offset: 0x0008920C
		private void ForceUpdateBarrierController()
		{
			if (this.door != null && this.door.model != null)
			{
				BarrierController componentInChildren = base.GetComponentInChildren<BarrierController>();
				if (componentInChildren != null)
				{
					componentInChildren.SetIsOpen(this.door.model.state == AccessDoor.State.OPEN);
				}
			}
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x0008B060 File Offset: 0x00089260
		private void ForceUpdateAnimator()
		{
			if (this.door != null && this.door.model != null)
			{
				Animator componentInChildren = base.GetComponentInChildren<Animator>();
				if (componentInChildren != null)
				{
					componentInChildren.SetBool(this.animOpenId, this.door.model.state == AccessDoor.State.OPEN);
				}
			}
		}

		// Token: 0x04002313 RID: 8979
		private AccessDoor door;

		// Token: 0x04002314 RID: 8980
		private int animOpenId;

		// Token: 0x04002315 RID: 8981
		private bool updateRequest = true;
	}
}
