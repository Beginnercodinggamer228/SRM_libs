using System;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
	// Token: 0x02000BF5 RID: 3061
	public static class DOTweenModuleUI
	{
		// Token: 0x06005734 RID: 22324 RVA: 0x00107C94 File Offset: 0x00105E94
		public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup target, float endValue, float duration)
		{
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => target.alpha, delegate(float x)
			{
				target.alpha = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005735 RID: 22325 RVA: 0x00107CDC File Offset: 0x00105EDC
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this Graphic target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005736 RID: 22326 RVA: 0x00107D24 File Offset: 0x00105F24
		public static TweenerCore<Color, Color, ColorOptions> DOFade(this Graphic target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005737 RID: 22327 RVA: 0x00107D6C File Offset: 0x00105F6C
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this Image target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x00107DB4 File Offset: 0x00105FB4
		public static TweenerCore<Color, Color, ColorOptions> DOFade(this Image target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005739 RID: 22329 RVA: 0x00107DFC File Offset: 0x00105FFC
		public static TweenerCore<float, float, FloatOptions> DOFillAmount(this Image target, float endValue, float duration)
		{
			if (endValue > 1f)
			{
				endValue = 1f;
			}
			else if (endValue < 0f)
			{
				endValue = 0f;
			}
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => target.fillAmount, delegate(float x)
			{
				target.fillAmount = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600573A RID: 22330 RVA: 0x00107E64 File Offset: 0x00106064
		public static Sequence DOGradientColor(this Image target, Gradient gradient, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			GradientColorKey[] colorKeys = gradient.colorKeys;
			int num = colorKeys.Length;
			for (int i = 0; i < num; i++)
			{
				GradientColorKey gradientColorKey = colorKeys[i];
				if (i == 0 && gradientColorKey.time <= 0f)
				{
					target.color = gradientColorKey.color;
				}
				else
				{
					float duration2 = (i == num - 1) ? (duration - sequence.Duration(false)) : (duration * ((i == 0) ? gradientColorKey.time : (gradientColorKey.time - colorKeys[i - 1].time)));
					sequence.Append(target.DOColor(gradientColorKey.color, duration2).SetEase(Ease.Linear));
				}
			}
			return sequence;
		}

		// Token: 0x0600573B RID: 22331 RVA: 0x00107F14 File Offset: 0x00106114
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOFlexibleSize(this LayoutElement target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => new Vector2(target.flexibleWidth, target.flexibleHeight), delegate(Vector2 x)
			{
				target.flexibleWidth = x.x;
				target.flexibleHeight = x.y;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600573C RID: 22332 RVA: 0x00107F60 File Offset: 0x00106160
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOMinSize(this LayoutElement target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => new Vector2(target.minWidth, target.minHeight), delegate(Vector2 x)
			{
				target.minWidth = x.x;
				target.minHeight = x.y;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600573D RID: 22333 RVA: 0x00107FAC File Offset: 0x001061AC
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOPreferredSize(this LayoutElement target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => new Vector2(target.preferredWidth, target.preferredHeight), delegate(Vector2 x)
			{
				target.preferredWidth = x.x;
				target.preferredHeight = x.y;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600573E RID: 22334 RVA: 0x00107FF8 File Offset: 0x001061F8
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this Outline target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.effectColor, delegate(Color x)
			{
				target.effectColor = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600573F RID: 22335 RVA: 0x00108040 File Offset: 0x00106240
		public static TweenerCore<Color, Color, ColorOptions> DOFade(this Outline target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.effectColor, delegate(Color x)
			{
				target.effectColor = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005740 RID: 22336 RVA: 0x00108088 File Offset: 0x00106288
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOScale(this Outline target, Vector2 endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.effectDistance, delegate(Vector2 x)
			{
				target.effectDistance = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005741 RID: 22337 RVA: 0x001080D0 File Offset: 0x001062D0
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition, delegate(Vector2 x)
			{
				target.anchoredPosition = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005742 RID: 22338 RVA: 0x0010811C File Offset: 0x0010631C
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPosX(this RectTransform target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition, delegate(Vector2 x)
			{
				target.anchoredPosition = x;
			}, new Vector2(endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X, snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005743 RID: 22339 RVA: 0x00108174 File Offset: 0x00106374
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPosY(this RectTransform target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition, delegate(Vector2 x)
			{
				target.anchoredPosition = x;
			}, new Vector2(0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Y, snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005744 RID: 22340 RVA: 0x001081CC File Offset: 0x001063CC
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchorPos3D(this RectTransform target, Vector3 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition3D, delegate(Vector3 x)
			{
				target.anchoredPosition3D = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005745 RID: 22341 RVA: 0x00108218 File Offset: 0x00106418
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchorPos3DX(this RectTransform target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition3D, delegate(Vector3 x)
			{
				target.anchoredPosition3D = x;
			}, new Vector3(endValue, 0f, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X, snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005746 RID: 22342 RVA: 0x00108274 File Offset: 0x00106474
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchorPos3DY(this RectTransform target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition3D, delegate(Vector3 x)
			{
				target.anchoredPosition3D = x;
			}, new Vector3(0f, endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.Y, snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005747 RID: 22343 RVA: 0x001082D0 File Offset: 0x001064D0
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchorPos3DZ(this RectTransform target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.anchoredPosition3D, delegate(Vector3 x)
			{
				target.anchoredPosition3D = x;
			}, new Vector3(0f, 0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Z, snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x0010832C File Offset: 0x0010652C
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorMax(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.anchorMax, delegate(Vector2 x)
			{
				target.anchorMax = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005749 RID: 22345 RVA: 0x00108378 File Offset: 0x00106578
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorMin(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.anchorMin, delegate(Vector2 x)
			{
				target.anchorMin = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600574A RID: 22346 RVA: 0x001083C4 File Offset: 0x001065C4
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOPivot(this RectTransform target, Vector2 endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.pivot, delegate(Vector2 x)
			{
				target.pivot = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600574B RID: 22347 RVA: 0x0010840C File Offset: 0x0010660C
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOPivotX(this RectTransform target, float endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.pivot, delegate(Vector2 x)
			{
				target.pivot = x;
			}, new Vector2(endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X, false).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x00108464 File Offset: 0x00106664
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOPivotY(this RectTransform target, float endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.pivot, delegate(Vector2 x)
			{
				target.pivot = x;
			}, new Vector2(0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Y, false).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600574D RID: 22349 RVA: 0x001084BC File Offset: 0x001066BC
		public static TweenerCore<Vector2, Vector2, VectorOptions> DOSizeDelta(this RectTransform target, Vector2 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.sizeDelta, delegate(Vector2 x)
			{
				target.sizeDelta = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x0600574E RID: 22350 RVA: 0x00108508 File Offset: 0x00106708
		public static Tweener DOPunchAnchorPos(this RectTransform target, Vector2 punch, float duration, int vibrato = 10, float elasticity = 1f, bool snapping = false)
		{
			return DOTween.Punch(() => target.anchoredPosition, delegate(Vector3 x)
			{
				target.anchoredPosition = x;
			}, punch, duration, vibrato, elasticity).SetTarget(target).SetOptions(snapping);
		}

		// Token: 0x0600574F RID: 22351 RVA: 0x0010855C File Offset: 0x0010675C
		public static Tweener DOShakeAnchorPos(this RectTransform target, float duration, float strength = 100f, int vibrato = 10, float randomness = 90f, bool snapping = false, bool fadeOut = true)
		{
			return DOTween.Shake(() => target.anchoredPosition, delegate(Vector3 x)
			{
				target.anchoredPosition = x;
			}, duration, strength, vibrato, randomness, true, fadeOut).SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake).SetOptions(snapping);
		}

		// Token: 0x06005750 RID: 22352 RVA: 0x001085B4 File Offset: 0x001067B4
		public static Tweener DOShakeAnchorPos(this RectTransform target, float duration, Vector2 strength, int vibrato = 10, float randomness = 90f, bool snapping = false, bool fadeOut = true)
		{
			return DOTween.Shake(() => target.anchoredPosition, delegate(Vector3 x)
			{
				target.anchoredPosition = x;
			}, duration, strength, vibrato, randomness, fadeOut).SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake).SetOptions(snapping);
		}

		// Token: 0x06005751 RID: 22353 RVA: 0x00108610 File Offset: 0x00106810
		public static Sequence DOJumpAnchorPos(this RectTransform target, Vector2 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
		{
			if (numJumps < 1)
			{
				numJumps = 1;
			}
			float startPosY = 0f;
			float offsetY = -1f;
			bool offsetYSet = false;
			Sequence s = DOTween.Sequence();
			Tween t = DOTween.To(() => target.anchoredPosition, delegate(Vector2 x)
			{
				target.anchoredPosition = x;
			}, new Vector2(0f, jumpPower), duration / (float)(numJumps * 2)).SetOptions(AxisConstraint.Y, snapping).SetEase(Ease.OutQuad).SetRelative<Tweener>().SetLoops(numJumps * 2, LoopType.Yoyo).OnStart(delegate
			{
				startPosY = target.anchoredPosition.y;
			});
			s.Append(DOTween.To(() => target.anchoredPosition, delegate(Vector2 x)
			{
				target.anchoredPosition = x;
			}, new Vector2(endValue.x, 0f), duration).SetOptions(AxisConstraint.X, snapping).SetEase(Ease.Linear)).Join(t).SetTarget(target).SetEase(DOTween.defaultEaseType);
			s.OnUpdate(delegate
			{
				if (!offsetYSet)
				{
					offsetYSet = true;
					offsetY = (s.isRelative ? endValue.y : (endValue.y - startPosY));
				}
				Vector2 anchoredPosition = target.anchoredPosition;
				anchoredPosition.y += DOVirtual.EasedValue(0f, offsetY, s.ElapsedDirectionalPercentage(), Ease.OutQuad);
				target.anchoredPosition = anchoredPosition;
			});
			return s;
		}

		// Token: 0x06005752 RID: 22354 RVA: 0x00108744 File Offset: 0x00106944
		public static Tweener DONormalizedPos(this ScrollRect target, Vector2 endValue, float duration, bool snapping = false)
		{
			return DOTween.To(() => new Vector2(target.horizontalNormalizedPosition, target.verticalNormalizedPosition), delegate(Vector2 x)
			{
				target.horizontalNormalizedPosition = x.x;
				target.verticalNormalizedPosition = x.y;
			}, endValue, duration).SetOptions(snapping).SetTarget(target);
		}

		// Token: 0x06005753 RID: 22355 RVA: 0x00108790 File Offset: 0x00106990
		public static Tweener DOHorizontalNormalizedPos(this ScrollRect target, float endValue, float duration, bool snapping = false)
		{
			return DOTween.To(() => target.horizontalNormalizedPosition, delegate(float x)
			{
				target.horizontalNormalizedPosition = x;
			}, endValue, duration).SetOptions(snapping).SetTarget(target);
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x001087DC File Offset: 0x001069DC
		public static Tweener DOVerticalNormalizedPos(this ScrollRect target, float endValue, float duration, bool snapping = false)
		{
			return DOTween.To(() => target.verticalNormalizedPosition, delegate(float x)
			{
				target.verticalNormalizedPosition = x;
			}, endValue, duration).SetOptions(snapping).SetTarget(target);
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x00108828 File Offset: 0x00106A28
		public static TweenerCore<float, float, FloatOptions> DOValue(this Slider target, float endValue, float duration, bool snapping = false)
		{
			TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => target.value, delegate(float x)
			{
				target.value = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x00108874 File Offset: 0x00106A74
		public static TweenerCore<Color, Color, ColorOptions> DOColor(this Text target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x001088BC File Offset: 0x00106ABC
		public static TweenerCore<Color, Color, ColorOptions> DOFade(this Text target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.color, delegate(Color x)
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x00108904 File Offset: 0x00106B04
		public static TweenerCore<string, string, StringOptions> DOText(this Text target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
		{
			TweenerCore<string, string, StringOptions> tweenerCore = DOTween.To(() => target.text, delegate(string x)
			{
				target.text = x;
			}, endValue, duration);
			tweenerCore.SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(target);
			return tweenerCore;
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x00108954 File Offset: 0x00106B54
		public static Tweener DOBlendableColor(this Graphic target, Color endValue, float duration)
		{
			endValue -= target.color;
			Color to = new Color(0f, 0f, 0f, 0f);
			return DOTween.To(() => to, delegate(Color x)
			{
				Color b = x - to;
				to = x;
				target.color += b;
			}, endValue, duration).Blendable<Color, Color, ColorOptions>().SetTarget(target);
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x001089D0 File Offset: 0x00106BD0
		public static Tweener DOBlendableColor(this Image target, Color endValue, float duration)
		{
			endValue -= target.color;
			Color to = new Color(0f, 0f, 0f, 0f);
			return DOTween.To(() => to, delegate(Color x)
			{
				Color b = x - to;
				to = x;
				target.color += b;
			}, endValue, duration).Blendable<Color, Color, ColorOptions>().SetTarget(target);
		}

		// Token: 0x0600575B RID: 22363 RVA: 0x00108A4C File Offset: 0x00106C4C
		public static Tweener DOBlendableColor(this Text target, Color endValue, float duration)
		{
			endValue -= target.color;
			Color to = new Color(0f, 0f, 0f, 0f);
			return DOTween.To(() => to, delegate(Color x)
			{
				Color b = x - to;
				to = x;
				target.color += b;
			}, endValue, duration).Blendable<Color, Color, ColorOptions>().SetTarget(target);
		}

		// Token: 0x02000BF6 RID: 3062
		public static class Utils
		{
			// Token: 0x0600575C RID: 22364 RVA: 0x00108AC8 File Offset: 0x00106CC8
			public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
			{
				Vector2 b = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
				Vector2 vector = RectTransformUtility.WorldToScreenPoint(null, from.position);
				vector += b;
				Vector2 b2;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(to, vector, null, out b2);
				Vector2 b3 = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
				return to.anchoredPosition + b2 - b3;
			}
		}
	}
}
