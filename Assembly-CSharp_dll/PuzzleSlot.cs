using System;
using System.Collections;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200075D RID: 1885
public class PuzzleSlot : IdHandler, PuzzleSlotModel.Participant
{
	// Token: 0x06002756 RID: 10070 RVA: 0x00095704 File Offset: 0x00093904
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterSlot(base.id, base.gameObject);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x00095721 File Offset: 0x00093921
	public void RegisterLock(PuzzleSlotLockable puzzleLockable)
	{
		this.puzLockable = puzzleLockable;
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x0009572A File Offset: 0x0009392A
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterSlot(base.id);
		}
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x0009574E File Offset: 0x0009394E
	public void InitModel(PuzzleSlotModel model)
	{
		if (this.fillOnAwake)
		{
			model.filled = true;
		}
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x0009575F File Offset: 0x0009395F
	public void SetModel(PuzzleSlotModel model)
	{
		this.model = model;
		this.OnFilledChanged();
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x0009576E File Offset: 0x0009396E
	public void OnFilledChangedFromModel()
	{
		this.OnFilledChanged();
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x00095776 File Offset: 0x00093976
	private void OnFilledChanged()
	{
		if (this.model.filled)
		{
			this.ActivateOnFill();
		}
		if (this.puzLockable != null)
		{
			this.puzLockable.NotifySlotChanged(true);
		}
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000957A8 File Offset: 0x000939A8
	public void OnTriggerEnter(Collider collider)
	{
		if (!collider.isTrigger)
		{
			GameObject gameObject = collider.gameObject;
			Identifiable component = gameObject.GetComponent<Identifiable>();
			if (component != null && component.id == this.catchId && !this.model.filled)
			{
				this.model.filled = true;
				SRBehaviour.SpawnAndPlayFX(this.changeFX, gameObject.transform.position, gameObject.transform.rotation);
				this.ActivateOnFill();
				DestroyOnTouching component2 = gameObject.GetComponent<DestroyOnTouching>();
				if (component2 != null)
				{
					component2.NoteDestroying();
				}
				Destroyer.DestroyActor(gameObject, "PuzzleSlot.OnTriggerEnter", false);
				if (this.puzLockable != null)
				{
					this.puzLockable.NotifySlotChanged(false);
					SECTR_AudioCue cueForLastSlot = this.puzLockable.GetCueForLastSlot();
					SECTR_AudioSystem.Play(this.localFillCue, base.transform.position, false);
					base.StartCoroutine(this.DelayedPlayLockCue(cueForLastSlot));
				}
			}
		}
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x0009589D File Offset: 0x00093A9D
	public IEnumerator DelayedPlayLockCue(SECTR_AudioCue cue)
	{
		yield return new WaitForSeconds(0.5f);
		this.puzLockable.PlayCue(cue);
		yield break;
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000958B3 File Offset: 0x00093AB3
	public bool IsLocked()
	{
		return this.model == null || !this.model.filled;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000958CD File Offset: 0x00093ACD
	protected override string IdPrefix()
	{
		return "puz";
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000958D4 File Offset: 0x00093AD4
	private void ActivateOnFill()
	{
		if (this.activateOnFill != null)
		{
			GameObject[] array = this.activateOnFill;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}
	}

	// Token: 0x0400271C RID: 10012
	public Identifiable.Id catchId;

	// Token: 0x0400271D RID: 10013
	public GameObject changeFX;

	// Token: 0x0400271E RID: 10014
	public GameObject[] activateOnFill;

	// Token: 0x0400271F RID: 10015
	public bool fillOnAwake;

	// Token: 0x04002720 RID: 10016
	public SECTR_AudioCue localFillCue;

	// Token: 0x04002721 RID: 10017
	private PuzzleSlotLockable puzLockable;

	// Token: 0x04002722 RID: 10018
	private PuzzleSlotModel model;

	// Token: 0x04002723 RID: 10019
	private const float LOCK_CUE_DELAY = 0.5f;
}
