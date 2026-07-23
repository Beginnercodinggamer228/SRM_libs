using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020001EB RID: 491
public class GadgetChickenCloner : SRBehaviour
{
	// Token: 0x06000A4E RID: 2638 RVA: 0x0002C9AC File Offset: 0x0002ABAC
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.animator = base.GetComponentInParent<Animator>();
		this.animator.SetBool(GadgetChickenCloner.ANIMATION_ACTIVE, false);
		this.audioSource = base.GetComponent<SECTR_PointSource>();
		this.spinnerBlur.SetActive(false);
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0002CA00 File Offset: 0x0002AC00
	public void Update()
	{
		if (this.timeDirector.OnPassedTime(this.animatorDeactivateTime))
		{
			this.animator.SetBool(GadgetChickenCloner.ANIMATION_ACTIVE, false);
			this.audioSource.Cue = this.onAnimationEndSFX;
			this.audioSource.Play();
		}
		if (!Mathf.Approximately(this.spinnerRotationSpeed, 0f))
		{
			this.spinner.transform.Rotate(0f, 0f, Time.deltaTime * this.spinnerRotationSpeed);
			this.spinnerBlur.SetActive(this.spinnerRotationSpeed >= GadgetChickenCloner.MIN_BLUR_ROTATION_SPEED);
		}
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0002CAA0 File Offset: 0x0002ACA0
	public void OnTriggerEnter(Collider collider)
	{
		if (Identifiable.MEAT_CLASS.Contains(Identifiable.GetId(collider.gameObject)) && this.ignored.Add(collider.gameObject))
		{
			Destroyer.DestroyActor(collider.gameObject, "GadgetChickenCloner.OnTriggerEnter", false);
			Quaternion quaternion = Quaternion.LookRotation((Vector3.Angle(collider.gameObject.GetComponent<Rigidbody>().velocity, base.transform.forward) > 90f) ? Vector3.back : Vector3.forward);
			if (!this.animator.GetBool(GadgetChickenCloner.ANIMATION_ACTIVE))
			{
				this.audioSource.Cue = this.onAnimationStartSFX;
				this.audioSource.Play();
				this.audioSource.Cue = this.onAnimationRunSFX;
				this.audioSource.Play();
			}
			this.animator.SetBool(GadgetChickenCloner.ANIMATION_ACTIVE, true);
			this.animatorDeactivateTime = this.timeDirector.HoursFromNow(0.016666668f);
			if (Randoms.SHARED.GetProbability(0.5f))
			{
				SRBehaviour.SpawnAndPlayFX(this.onSuccessFX, base.gameObject, Vector3.zero, quaternion);
				RegionRegistry.RegionSetId setId = collider.gameObject.GetComponent<RegionMember>().setId;
				GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.GetId(collider.gameObject));
				GameObject[] array = new GameObject[2];
				List<Identifiable.Id> allFashions = collider.gameObject.GetRequiredComponent<AttachFashions>().GetAllFashions();
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = SRBehaviour.InstantiateActor(prefab, setId, base.transform.position, base.transform.rotation * quaternion, false);
					array[i] = gameObject;
					gameObject.GetRequiredComponent<Vacuumable>().Launch(Vacuumable.LaunchSource.CHICKEN_CLONER);
					gameObject.GetRequiredComponent<AttachFashions>().SetFashions(allFashions);
					gameObject.GetComponent<Rigidbody>().velocity = (Quaternion.Euler(0f, (float)((i == 0) ? 20 : -20), 0f) * gameObject.transform.forward).normalized * 10f;
					gameObject.transform.DOScale(gameObject.transform.localScale, 0.2f).From(0.2f, true).SetEase(Ease.Linear);
					this.ignored.Add(gameObject);
				}
				PhysicsUtil.IgnoreCollision(array[0], array[1], 0.2f);
				return;
			}
			SRBehaviour.SpawnAndPlayFX(this.onFailureFX, base.gameObject, Vector3.zero, quaternion);
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0002CD18 File Offset: 0x0002AF18
	public void OnTriggerExit(Collider collider)
	{
		this.ignored.RemoveWhere((GameObject go) => go == null || go == collider.gameObject);
	}

	// Token: 0x04000860 RID: 2144
	[Tooltip("GameObject to be rotated along the z-axis; animation property.")]
	public GameObject spinner;

	// Token: 0x04000861 RID: 2145
	[Tooltip("GameObject representing the blurred spinner; activated/deactivated based off current speed.")]
	public GameObject spinnerBlur;

	// Token: 0x04000862 RID: 2146
	[Tooltip("Rotation speed of the 'spinner' GameObject; animation property.")]
	public float spinnerRotationSpeed;

	// Token: 0x04000863 RID: 2147
	[Tooltip("FX played when a chicken enters the cloner, and is successfully cloned.")]
	public GameObject onSuccessFX;

	// Token: 0x04000864 RID: 2148
	[Tooltip("FX played when a chicken enters the cloner, and is not cloned.")]
	public GameObject onFailureFX;

	// Token: 0x04000865 RID: 2149
	[Tooltip("SFX played when then cloning animation begins. (non-looping)")]
	public SECTR_AudioCue onAnimationStartSFX;

	// Token: 0x04000866 RID: 2150
	[Tooltip("SFX played while the cloning animation runs. (looping)")]
	public SECTR_AudioCue onAnimationRunSFX;

	// Token: 0x04000867 RID: 2151
	[Tooltip("SFX played when then cloning animation ends. (non-looping)")]
	public SECTR_AudioCue onAnimationEndSFX;

	// Token: 0x04000868 RID: 2152
	private static readonly int ANIMATION_ACTIVE = Animator.StringToHash("ACTIVE");

	// Token: 0x04000869 RID: 2153
	private static readonly float MIN_BLUR_ROTATION_SPEED = 400f;

	// Token: 0x0400086A RID: 2154
	private TimeDirector timeDirector;

	// Token: 0x0400086B RID: 2155
	private SECTR_PointSource audioSource;

	// Token: 0x0400086C RID: 2156
	private Animator animator;

	// Token: 0x0400086D RID: 2157
	private HashSet<GameObject> ignored = new HashSet<GameObject>();

	// Token: 0x0400086E RID: 2158
	private double animatorDeactivateTime;
}
