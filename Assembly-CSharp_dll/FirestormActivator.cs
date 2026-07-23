using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000704 RID: 1796
public class FirestormActivator : MonoBehaviour, WorldModel.Participant
{
	// Token: 0x0600257D RID: 9597 RVA: 0x0008FF1C File Offset: 0x0008E11C
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.ambianceDir = SRSingleton<SceneContext>.Instance.AmbianceDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x00041FFB File Offset: 0x000401FB
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x0008FF6C File Offset: 0x0008E16C
	public void Start()
	{
		this.member = base.GetComponent<RegionMember>();
		this.overlay = this.member.GetComponentInChildren<Overlay>();
		if (double.IsPositiveInfinity(this.worldModel.nextFirestormTime))
		{
			if (this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_DESERT))
			{
				this.worldModel.nextFirestormTime = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(8f, 15f) * 0.5f);
				return;
			}
			ProgressDirector progressDirector = this.progressDir;
			progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Combine(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.CheckProgress));
		}
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x0009000E File Offset: 0x0008E20E
	public void OnDestroy()
	{
		ProgressDirector progressDirector = this.progressDir;
		progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Remove(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.CheckProgress));
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x00090037 File Offset: 0x0008E237
	public void InitModel(WorldModel model)
	{
		model.nextFirestormTime = double.PositiveInfinity;
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x00090048 File Offset: 0x0008E248
	public void SetModel(WorldModel model)
	{
		this.worldModel = model;
		this.worldModel.nextFirecolumnTime = ((this.worldModel.currFirestormMode == FirestormActivator.Mode.PREPARING) ? this.timeDir.HoursFromNow(0.5f) : this.timeDir.WorldTime());
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x00090088 File Offset: 0x0008E288
	private void CheckProgress()
	{
		if (this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_DESERT) && double.IsPositiveInfinity(this.worldModel.nextFirestormTime))
		{
			this.worldModel.nextFirestormTime = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(3f, 5f));
		}
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000900DF File Offset: 0x0008E2DF
	public void Update()
	{
		this.MaybeShutdownFirestorm();
		this.MaybeStartFirestorm();
		this.MaybeTriggerNearbyColumns();
		this.MaybeUpdatePlayerState();
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000900F9 File Offset: 0x0008E2F9
	private void MaybeShutdownFirestorm()
	{
		if (this.timeDir.HasReached(this.worldModel.endFirestormTime))
		{
			this.worldModel.currFirestormMode = FirestormActivator.Mode.IDLE;
			this.worldModel.nextFirecolumnTime = double.PositiveInfinity;
		}
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x00090134 File Offset: 0x0008E334
	private void MaybeStartFirestorm()
	{
		if (this.timeDir.HasReached(this.worldModel.nextFirestormTime))
		{
			this.worldModel.nextFirecolumnTime = this.timeDir.HoursFromNow(0.5f);
			this.worldModel.currFirestormMode = FirestormActivator.Mode.PREPARING;
			this.worldModel.endFirestormTime = this.timeDir.HoursFromNow(Randoms.SHARED.GetInRange(1.5f, 2f));
			this.worldModel.endFirecolumnsTime = this.worldModel.endFirestormTime - 1800.0;
			this.worldModel.nextFirestormTime = this.timeDir.HoursFromNow(Randoms.SHARED.GetInRange(8f, 15f));
		}
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000901F8 File Offset: 0x0008E3F8
	private void MaybeTriggerNearbyColumns()
	{
		if (this.timeDir.HasReached(this.worldModel.nextFirecolumnTime) && !this.timeDir.HasReached(this.worldModel.endFirecolumnsTime))
		{
			this.worldModel.currFirestormMode = FirestormActivator.Mode.ACTIVE;
			List<FireColumn> list = new List<FireColumn>();
			foreach (Region region in this.member.regions)
			{
				FirestormController component = region.GetComponent<FirestormController>();
				if (component != null)
				{
					component.AddColumnsToList(list);
				}
			}
			if (list.Count > 0)
			{
				Dictionary<FireColumn, float> dictionary = new Dictionary<FireColumn, float>();
				foreach (FireColumn fireColumn in list)
				{
					if (!fireColumn.IsInOasis() && !fireColumn.IsFireActive())
					{
						dictionary[fireColumn] = 1f / Mathf.Max(0.1f, (fireColumn.transform.position - base.transform.position).sqrMagnitude);
					}
				}
				FireColumn fireColumn2 = Randoms.SHARED.Pick<FireColumn>(dictionary, null);
				if (fireColumn2 != null)
				{
					fireColumn2.ActivateFire();
				}
			}
			this.worldModel.nextFirecolumnTime = this.timeDir.HoursFromNow(Randoms.SHARED.GetInRange(this.minColumnDelayHrs, this.maxColumnDelayHrs));
		}
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x0009038C File Offset: 0x0008E58C
	public void RequestPlayerStateUpdate()
	{
		this.nextPlayerCheck = 0.0;
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000903A0 File Offset: 0x0008E5A0
	private void MaybeUpdatePlayerState()
	{
		if (this.timeDir.HasReached(this.nextPlayerCheck))
		{
			bool flag = false;
			using (List<Region>.Enumerator enumerator = this.member.regions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetComponent<FirestormController>() != null)
					{
						flag = true;
						break;
					}
				}
			}
			FirestormActivator.Mode mode = flag ? this.worldModel.currFirestormMode : FirestormActivator.Mode.IDLE;
			if (mode != this.worldModel.currFirestormAmbianceMode)
			{
				this.overlay.SetEnableFirestorm(mode > FirestormActivator.Mode.IDLE);
				this.ambianceDir.SetFirestormActive(mode > FirestormActivator.Mode.IDLE);
				this.musicDir.SetFirestormMode(mode);
				if (mode == FirestormActivator.Mode.ACTIVE)
				{
					this.activeFirestormWind = SECTR_AudioSystem.Play(this.firestormWind, base.gameObject.transform, Vector3.zero, true, null, false);
				}
				else if (mode == FirestormActivator.Mode.IDLE)
				{
					this.activeFirestormWind.Stop(false);
				}
				this.worldModel.currFirestormAmbianceMode = mode;
			}
			this.nextPlayerCheck = this.timeDir.HoursFromNow(0.083333336f);
		}
	}

	// Token: 0x04002470 RID: 9328
	public SECTR_AudioCue firestormWind;

	// Token: 0x04002471 RID: 9329
	public float minColumnDelayHrs = 0.05f;

	// Token: 0x04002472 RID: 9330
	public float maxColumnDelayHrs = 0.1f;

	// Token: 0x04002473 RID: 9331
	private SECTR_AudioCueInstance activeFirestormWind;

	// Token: 0x04002474 RID: 9332
	private TimeDirector timeDir;

	// Token: 0x04002475 RID: 9333
	private MusicDirector musicDir;

	// Token: 0x04002476 RID: 9334
	private AmbianceDirector ambianceDir;

	// Token: 0x04002477 RID: 9335
	private ProgressDirector progressDir;

	// Token: 0x04002478 RID: 9336
	private RegionMember member;

	// Token: 0x04002479 RID: 9337
	private Overlay overlay;

	// Token: 0x0400247A RID: 9338
	private double nextPlayerCheck;

	// Token: 0x0400247B RID: 9339
	private WorldModel worldModel;

	// Token: 0x0400247C RID: 9340
	private const float MIN_STORM_PERIOD = 8f;

	// Token: 0x0400247D RID: 9341
	private const float MAX_STORM_PERIOD = 15f;

	// Token: 0x0400247E RID: 9342
	private const float MIN_INIT_STORM_PERIOD = 3f;

	// Token: 0x0400247F RID: 9343
	private const float MAX_INIT_STORM_PERIOD = 5f;

	// Token: 0x04002480 RID: 9344
	private const float MIN_STORM_LENGTH = 1.5f;

	// Token: 0x04002481 RID: 9345
	private const float MAX_STORM_LENGTH = 2f;

	// Token: 0x04002482 RID: 9346
	private const float ENTER_DESERT_STORM_DELAY_HRS = 24f;

	// Token: 0x04002483 RID: 9347
	public const float END_STORM_RAMPDOWN = 0.5f;

	// Token: 0x04002484 RID: 9348
	public const float PREPARE_DELAY = 0.5f;

	// Token: 0x04002485 RID: 9349
	private const float PLAYER_CHECK_PERIOD = 0.083333336f;

	// Token: 0x02000705 RID: 1797
	public enum Mode
	{
		// Token: 0x04002487 RID: 9351
		IDLE,
		// Token: 0x04002488 RID: 9352
		PREPARING,
		// Token: 0x04002489 RID: 9353
		ACTIVE
	}
}
