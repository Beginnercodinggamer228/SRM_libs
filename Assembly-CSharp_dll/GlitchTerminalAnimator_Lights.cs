using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public class GlitchTerminalAnimator_Lights : SRBehaviour
{
	// Token: 0x06001AF7 RID: 6903 RVA: 0x00067FBC File Offset: 0x000661BC
	public void Awake()
	{
		this.animator = base.GetRequiredComponentInParent<GlitchTerminalAnimator>(false);
		this.renderer = base.GetRequiredComponent<Renderer>();
		this.renderer.sharedMaterial.SetFloat(this.PROPERTY_COLOR, 0f);
		this.renderer.sharedMaterial.SetFloat(this.PROPERTY_MULTIPLIER, 0f);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x00068018 File Offset: 0x00066218
	public void Update()
	{
		GlitchTerminalAnimator_Lights.State currentState = this.GetCurrentState();
		if (this.state != currentState)
		{
			this.OnStateChanged(this.state, currentState);
			this.state = currentState;
		}
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x0006804C File Offset: 0x0006624C
	private GlitchTerminalAnimator_Lights.State GetCurrentState()
	{
		if (!this.animator.animator.GetBool("state_sleeping"))
		{
			switch (this.animator.activator.GetLinkState())
			{
			case GlitchTerminalActivator.LinkState.INACTIVE_PROGRESS:
				return GlitchTerminalAnimator_Lights.State.DISABLED;
			case GlitchTerminalActivator.LinkState.INACTIVE_AMMO:
				return GlitchTerminalAnimator_Lights.State.ENABLED_RED;
			case GlitchTerminalActivator.LinkState.PRE_ACTIVE:
			case GlitchTerminalActivator.LinkState.ACTIVE:
				return GlitchTerminalAnimator_Lights.State.ENABLED_GREEN;
			}
		}
		return GlitchTerminalAnimator_Lights.State.DISABLED;
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x000680A0 File Offset: 0x000662A0
	private void OnStateChanged(GlitchTerminalAnimator_Lights.State previous, GlitchTerminalAnimator_Lights.State current)
	{
		if (previous == GlitchTerminalAnimator_Lights.State.DISABLED)
		{
			this.renderer.sharedMaterial.SetFloat(this.PROPERTY_COLOR, GlitchTerminalAnimator_Lights.GetStateColor(current));
			Tweener tweener = this.multiplierTween;
			if (tweener != null)
			{
				tweener.Kill(false);
			}
			this.multiplierTween = DOTween.To(() => this.renderer.sharedMaterial.GetFloat(this.PROPERTY_MULTIPLIER), new DOSetter<float>(this.OnUpdate_PropertyMultiplier), 1.25f, 0.8f).SetSpeedBased<TweenerCore<float, float, FloatOptions>>().SetEase(Ease.Linear);
			return;
		}
		if (current == GlitchTerminalAnimator_Lights.State.DISABLED)
		{
			Tweener tweener2 = this.multiplierTween;
			if (tweener2 != null)
			{
				tweener2.Kill(false);
			}
			this.multiplierTween = DOTween.To(() => this.renderer.sharedMaterial.GetFloat(this.PROPERTY_MULTIPLIER), new DOSetter<float>(this.OnUpdate_PropertyMultiplier), 0f, 0.8f).SetSpeedBased<TweenerCore<float, float, FloatOptions>>().SetEase(Ease.Linear);
			return;
		}
		Tweener tweener3 = this.colorTween;
		if (tweener3 != null)
		{
			tweener3.Kill(false);
		}
		this.colorTween = DOTween.To(() => this.renderer.sharedMaterial.GetFloat(this.PROPERTY_COLOR), new DOSetter<float>(this.OnUpdate_PropertyColor), GlitchTerminalAnimator_Lights.GetStateColor(current), 0.8f).SetSpeedBased<TweenerCore<float, float, FloatOptions>>().SetEase(Ease.Linear);
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x000681B0 File Offset: 0x000663B0
	private void OnUpdate_PropertyMultiplier(float value)
	{
		this.renderer.sharedMaterial.SetFloat(this.PROPERTY_MULTIPLIER, value);
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x000681C9 File Offset: 0x000663C9
	private void OnUpdate_PropertyColor(float value)
	{
		this.renderer.sharedMaterial.SetFloat(this.PROPERTY_COLOR, value);
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x000681E2 File Offset: 0x000663E2
	private static float GetStateColor(GlitchTerminalAnimator_Lights.State state)
	{
		if (state == GlitchTerminalAnimator_Lights.State.ENABLED_RED)
		{
			return 0f;
		}
		if (state == GlitchTerminalAnimator_Lights.State.ENABLED_GREEN)
		{
			return 0.5f;
		}
		throw new ArgumentException();
	}

	// Token: 0x04001A67 RID: 6759
	private GlitchTerminalAnimator animator;

	// Token: 0x04001A68 RID: 6760
	private const float TRANSITION_SPEED = 0.8f;

	// Token: 0x04001A69 RID: 6761
	private readonly int PROPERTY_COLOR = Shader.PropertyToID("_SpiralColor");

	// Token: 0x04001A6A RID: 6762
	private const float PROPERTY_COLOR_RED = 0f;

	// Token: 0x04001A6B RID: 6763
	private const float PROPERTY_COLOR_GREEN = 0.5f;

	// Token: 0x04001A6C RID: 6764
	private readonly int PROPERTY_MULTIPLIER = Shader.PropertyToID("_GlowMultiplier");

	// Token: 0x04001A6D RID: 6765
	private const float PROPERTY_MULTIPLIER_ON = 1.25f;

	// Token: 0x04001A6E RID: 6766
	private const float PROPERTY_MULTIPLIER_OFF = 0f;

	// Token: 0x04001A6F RID: 6767
	private GlitchTerminalAnimator_Lights.State state;

	// Token: 0x04001A70 RID: 6768
	private Renderer renderer;

	// Token: 0x04001A71 RID: 6769
	private Tweener multiplierTween;

	// Token: 0x04001A72 RID: 6770
	private Tweener colorTween;

	// Token: 0x02000509 RID: 1289
	private enum State
	{
		// Token: 0x04001A74 RID: 6772
		DISABLED,
		// Token: 0x04001A75 RID: 6773
		ENABLED_RED,
		// Token: 0x04001A76 RID: 6774
		ENABLED_GREEN
	}
}
