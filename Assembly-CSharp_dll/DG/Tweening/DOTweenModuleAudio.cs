using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Audio;

namespace DG.Tweening
{
	// Token: 0x02000BDB RID: 3035
	public static class DOTweenModuleAudio
	{
		// Token: 0x060056D4 RID: 22228 RVA: 0x00106D54 File Offset: 0x00104F54
		public static TweenerCore<float, float, FloatOptions> DOFade(this AudioSource target, float endValue, float duration)
		{
			if (endValue < 0f)
			{
				endValue = 0f;
			}
			else if (endValue > 1f)
			{
				endValue = 1f;
			}
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => target.volume, delegate(float x)
			{
				target.volume = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x060056D5 RID: 22229 RVA: 0x00106DBC File Offset: 0x00104FBC
		public static TweenerCore<float, float, FloatOptions> DOPitch(this AudioSource target, float endValue, float duration)
		{
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => target.pitch, delegate(float x)
			{
				target.pitch = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x060056D6 RID: 22230 RVA: 0x00106E04 File Offset: 0x00105004
		public static TweenerCore<float, float, FloatOptions> DOSetFloat(this AudioMixer target, string floatName, float endValue, float duration)
		{
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(delegate()
			{
				float result;
				target.GetFloat(floatName, out result);
				return result;
			}, delegate(float x)
			{
				target.SetFloat(floatName, x);
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x060056D7 RID: 22231 RVA: 0x00106E51 File Offset: 0x00105051
		public static int DOComplete(this AudioMixer target, bool withCallbacks = false)
		{
			return DOTween.Complete(target, withCallbacks);
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x00106E5A File Offset: 0x0010505A
		public static int DOKill(this AudioMixer target, bool complete = false)
		{
			return DOTween.Kill(target, complete);
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x00106E63 File Offset: 0x00105063
		public static int DOFlip(this AudioMixer target)
		{
			return DOTween.Flip(target);
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x00106E6B File Offset: 0x0010506B
		public static int DOGoto(this AudioMixer target, float to, bool andPlay = false)
		{
			return DOTween.Goto(target, to, andPlay);
		}

		// Token: 0x060056DB RID: 22235 RVA: 0x00106E75 File Offset: 0x00105075
		public static int DOPause(this AudioMixer target)
		{
			return DOTween.Pause(target);
		}

		// Token: 0x060056DC RID: 22236 RVA: 0x00106E7D File Offset: 0x0010507D
		public static int DOPlay(this AudioMixer target)
		{
			return DOTween.Play(target);
		}

		// Token: 0x060056DD RID: 22237 RVA: 0x00106E85 File Offset: 0x00105085
		public static int DOPlayBackwards(this AudioMixer target)
		{
			return DOTween.PlayBackwards(target);
		}

		// Token: 0x060056DE RID: 22238 RVA: 0x00106E8D File Offset: 0x0010508D
		public static int DOPlayForward(this AudioMixer target)
		{
			return DOTween.PlayForward(target);
		}

		// Token: 0x060056DF RID: 22239 RVA: 0x00106E95 File Offset: 0x00105095
		public static int DORestart(this AudioMixer target)
		{
			return DOTween.Restart(target, true, -1f);
		}

		// Token: 0x060056E0 RID: 22240 RVA: 0x00106EA3 File Offset: 0x001050A3
		public static int DORewind(this AudioMixer target)
		{
			return DOTween.Rewind(target, true);
		}

		// Token: 0x060056E1 RID: 22241 RVA: 0x00106EAC File Offset: 0x001050AC
		public static int DOSmoothRewind(this AudioMixer target)
		{
			return DOTween.SmoothRewind(target);
		}

		// Token: 0x060056E2 RID: 22242 RVA: 0x00106EB4 File Offset: 0x001050B4
		public static int DOTogglePause(this AudioMixer target)
		{
			return DOTween.TogglePause(target);
		}
	}
}
