using System;
using System.Linq;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class GlitchImpostoFlicker : SRBehaviour
{
	// Token: 0x06001A1A RID: 6682 RVA: 0x00065A66 File Offset: 0x00063C66
	public void Awake()
	{
		if (Application.isPlaying)
		{
			this.metadata = SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch;
			this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		}
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x00065A94 File Offset: 0x00063C94
	public void OnEnable()
	{
		if (Application.isPlaying && this.hasStarted)
		{
			this.ResetNextFlickerTime();
		}
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x00065AAB File Offset: 0x00063CAB
	public void Start()
	{
		if (Application.isPlaying)
		{
			this.hasStarted = true;
			this.OnEnable();
		}
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x00065AC1 File Offset: 0x00063CC1
	public void OnDisable()
	{
		if (Application.isPlaying)
		{
			this.timeDirector.RemovePassedTimeDelegate(new Action(this.OnTimeReached));
		}
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x00065AE1 File Offset: 0x00063CE1
	private void ResetNextFlickerTime()
	{
		this.timeDirector.AddPassedTimeDelegate(this.timeDirector.HoursFromNow(this.metadata.impostoFlickerCooldownTime.GetRandom() * 0.016666668f), new Action(this.OnTimeReached));
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x00065B1C File Offset: 0x00063D1C
	private void OnTimeReached()
	{
		if (this.metadata.impostoFlickerFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.metadata.impostoFlickerFX, base.gameObject);
		}
		if (this.metadata.impostoFlickerCue != null)
		{
			SECTR_AudioSystem.Play(this.metadata.impostoFlickerCue, base.transform.position, false);
		}
		Tween tween = this.tween;
		if (tween != null)
		{
			tween.Kill(true);
		}
		this.tween = base.transform.DOPath(this.FlickerPath, this.metadata.impostoFlickerSpeed.GetRandom(), PathType.Linear, PathMode.Full3D, 10, null).SetEase(Ease.Linear).SetSpeedBased(true);
		this.ResetNextFlickerTime();
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001A20 RID: 6688 RVA: 0x00065BDC File Offset: 0x00063DDC
	private Vector3[] FlickerPath
	{
		get
		{
			if (this.path_cache == null)
			{
				this.path_cache = (from ii in Enumerable.Range(0, this.metadata.impostoFlickerPoints)
				select Quaternion.Euler(0f, (float)Randoms.SHARED.GetInRange(0, 360), 0f) * Vector3.forward * this.metadata.impostoFlickerRadius.GetRandom() + base.gameObject.transform.position).Concat(base.gameObject.transform.position.ToEnumerable<Vector3>()).ToArray<Vector3>();
			}
			return this.path_cache;
		}
	}

	// Token: 0x040019A6 RID: 6566
	private TimeDirector timeDirector;

	// Token: 0x040019A7 RID: 6567
	private GlitchMetadata metadata;

	// Token: 0x040019A8 RID: 6568
	private bool hasStarted;

	// Token: 0x040019A9 RID: 6569
	private Tween tween;

	// Token: 0x040019AA RID: 6570
	private Vector3[] path_cache;
}
