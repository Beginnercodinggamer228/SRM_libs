using System;
using System.Collections;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001ED RID: 493
public class GadgetDashPad : ControllerCollisionListenerBehaviour
{
	// Token: 0x06000A56 RID: 2646 RVA: 0x0002CD96 File Offset: 0x0002AF96
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.player = SRSingleton<SceneContext>.Instance.GameModel.GetPlayerModel();
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0002CDC0 File Offset: 0x0002AFC0
	protected override void OnControllerCollisionEntered()
	{
		base.OnControllerCollisionEntered();
		this.player.runEnergyDepletionTime = double.MaxValue;
		GadgetDashPad.FXHelper.OnRunEnergyDepletionTimeChanged(this.hudFX);
		if (this.onActivationFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.onActivationFX, base.gameObject, Vector3.zero, Quaternion.identity);
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0002CE1C File Offset: 0x0002B01C
	protected override void OnControllerCollisionExited()
	{
		base.OnControllerCollisionExited();
		this.activationTime = Time.time + 0.75f;
		this.player.runEnergyDepletionTime = this.timeDirector.HoursFromNow(this.activationDuration * 0.016666668f);
		GadgetDashPad.FXHelper.OnRunEnergyDepletionTimeChanged(this.hudFX);
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0002CE6D File Offset: 0x0002B06D
	protected override bool Predicate(GameObject collision)
	{
		return collision == SRSingleton<SceneContext>.Instance.Player && Time.time >= this.activationTime;
	}

	// Token: 0x04000870 RID: 2160
	[Tooltip("FX played when the dash pad is activated.")]
	public GameObject onActivationFX;

	// Token: 0x04000871 RID: 2161
	[Tooltip("Time before energy begins being depleted again. (in-game minutes)")]
	public float activationDuration;

	// Token: 0x04000872 RID: 2162
	[Tooltip("2D HUD overlay properties.")]
	public GadgetDashPad.HudFX hudFX;

	// Token: 0x04000873 RID: 2163
	private TimeDirector timeDirector;

	// Token: 0x04000874 RID: 2164
	private PlayerModel player;

	// Token: 0x04000875 RID: 2165
	private const float COOLDOWN_DURATION = 0.75f;

	// Token: 0x04000876 RID: 2166
	private float activationTime;

	// Token: 0x020001EE RID: 494
	[Serializable]
	public class HudFX
	{
		// Token: 0x04000877 RID: 2167
		[Tooltip("SFX played during the activation of the HUD overlay. (2D, looping")]
		public SECTR_AudioCue onActiveSFX;

		// Token: 0x04000878 RID: 2168
		[Tooltip("SFX played at the deactivation of the HUD overlay. (2D, non-looping")]
		public SECTR_AudioCue onDeactivatedSFX;
	}

	// Token: 0x020001EF RID: 495
	private class FXHelper : SRSingleton<GadgetDashPad.FXHelper>
	{
		// Token: 0x06000A5C RID: 2652 RVA: 0x0002CE9B File Offset: 0x0002B09B
		public static void OnRunEnergyDepletionTimeChanged(GadgetDashPad.HudFX args)
		{
			SRSingleton<SceneContext>.Instance.StartCoroutine(GadgetDashPad.FXHelper.OnRunEnergyDepletionTimeChanged_Coroutine(args));
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0002CEAE File Offset: 0x0002B0AE
		private static IEnumerator OnRunEnergyDepletionTimeChanged_Coroutine(GadgetDashPad.HudFX args)
		{
			TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
			double time = SRSingleton<SceneContext>.Instance.GameModel.GetPlayerModel().runEnergyDepletionTime;
			if (SRSingleton<GadgetDashPad.FXHelper>.Instance == null)
			{
				GameObject gameObject = new GameObject("GadgetDashPad.FXHelper");
				gameObject.transform.SetParent(SRSingleton<DynamicObjectContainer>.Instance.transform);
				gameObject.AddComponent<GadgetDashPad.FXHelper>();
				SRSingleton<GadgetDashPad.FXHelper>.Instance.onDeactivatedSFX = args.onDeactivatedSFX;
				SRSingleton<GadgetDashPad.FXHelper>.Instance.onActiveSFXInstance = SECTR_AudioSystem.Play(args.onActiveSFX, Vector3.zero, true);
			}
			DestroyAfterTime destroyAfterTime = SRSingleton<GadgetDashPad.FXHelper>.Instance.gameObject.GetComponent<DestroyAfterTime>() ?? SRSingleton<GadgetDashPad.FXHelper>.Instance.gameObject.AddComponent<DestroyAfterTime>();
			destroyAfterTime.SetDeathTime(time);
			destroyAfterTime.destroyAsActor = false;
			Tween tween = SRSingleton<GadgetDashPad.FXHelper>.Instance.tween;
			if (tween != null)
			{
				tween.Kill(false);
			}
			yield return new WaitForEndOfFrame();
			float interval = (float)((time - timeDirector.WorldTime()) * 0.01666666753590107) - 1.25f;
			SRSingleton<GadgetDashPad.FXHelper>.Instance.tween = DOTween.Sequence().Append(DOTween.To(() => SRSingleton<GadgetDashPad.FXHelper>.Instance.alpha, new DOSetter<float>(SRSingleton<GadgetDashPad.FXHelper>.Instance.SetFXAlpha), 1f, 0.25f)).AppendInterval(interval).Append(DOTween.To(() => SRSingleton<GadgetDashPad.FXHelper>.Instance.alpha, new DOSetter<float>(SRSingleton<GadgetDashPad.FXHelper>.Instance.SetFXAlpha), 0f, 1f).OnStart(delegate
			{
				SRSingleton<GadgetDashPad.FXHelper>.Instance.OnFadeOutStart();
			}));
			yield break;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0002CEC0 File Offset: 0x0002B0C0
		public override void Awake()
		{
			base.Awake();
			this.overlay = SRSingleton<Overlay>.Instance.Play(SRSingleton<Overlay>.Instance.dashPadFX);
			this.meter = SRSingleton<HudUI>.Instance.energyMeter.Play(SRSingleton<HudUI>.Instance.energyMeter.dashPadFX);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0002CF14 File Offset: 0x0002B114
		public override void OnDestroy()
		{
			base.OnDestroy();
			Destroyer.Destroy(this.overlay, "GadgetDashPad.FXHelper.OnDestroy");
			Destroyer.Destroy(this.meter, "GadgetDashPad.FXHelper.OnDestroy");
			if (this.tween != null)
			{
				this.tween.Kill(false);
			}
			this.onActiveSFXInstance.Stop(true);
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0002CF68 File Offset: 0x0002B168
		private void SetFXAlpha(float alpha)
		{
			this.alpha = alpha;
			Renderer requiredComponent = this.overlay.GetRequiredComponent<Renderer>();
			requiredComponent.material.color = GadgetDashPad.FXHelper.GetColor(requiredComponent.material.color, this.alpha);
			Image requiredComponent2 = this.meter.GetRequiredComponent<Image>();
			requiredComponent2.color = GadgetDashPad.FXHelper.GetColor(requiredComponent2.color, this.alpha);
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x0002CFCA File Offset: 0x0002B1CA
		private void OnFadeOutStart()
		{
			SECTR_AudioSystem.Play(this.onDeactivatedSFX, Vector3.zero, false);
			this.onActiveSFXInstance.Stop(false);
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0002CFEA File Offset: 0x0002B1EA
		private static Color GetColor(Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		// Token: 0x04000879 RID: 2169
		private const float FX_FADE_TIME_IN = 0.25f;

		// Token: 0x0400087A RID: 2170
		private const float FX_FADE_TIME_OUT = 1f;

		// Token: 0x0400087B RID: 2171
		private GameObject overlay;

		// Token: 0x0400087C RID: 2172
		private GameObject meter;

		// Token: 0x0400087D RID: 2173
		private float alpha;

		// Token: 0x0400087E RID: 2174
		private SECTR_AudioCueInstance onActiveSFXInstance;

		// Token: 0x0400087F RID: 2175
		private SECTR_AudioCue onDeactivatedSFX;

		// Token: 0x04000880 RID: 2176
		private Tween tween;
	}
}
