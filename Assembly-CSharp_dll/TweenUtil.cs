using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class TweenUtil : MonoBehaviour
{
	// Token: 0x0600239E RID: 9118 RVA: 0x00089FFC File Offset: 0x000881FC
	public static Tweener ScaleIn(GameObject obj, float time, Ease easeType = Ease.OutQuad)
	{
		Vector3 fromValue = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 localScale = obj.transform.localScale;
		if (obj.GetComponent<ScaleYOnlyMarker>() != null)
		{
			fromValue = new Vector3(localScale.x, 0.001f, localScale.z);
		}
		else if (obj.GetComponent<ScaleZOnlyMarker>() != null)
		{
			fromValue = new Vector3(localScale.x, localScale.y, 0.001f);
		}
		return obj.transform.DOScale(obj.transform.localScale, time).From(fromValue, true).SetEase(easeType);
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x0008A0A0 File Offset: 0x000882A0
	public static Tweener ScaleOut(GameObject obj, float time, Ease easeType = Ease.InQuad)
	{
		Vector3 endValue = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 localScale = obj.transform.localScale;
		if (obj.GetComponent<ScaleYOnlyMarker>() != null)
		{
			endValue = new Vector3(localScale.x, 0.001f, localScale.z);
		}
		else if (obj.GetComponent<ScaleZOnlyMarker>() != null)
		{
			endValue = new Vector3(localScale.x, localScale.y, 0.001f);
		}
		return obj.transform.DOScale(endValue, time).SetEase(easeType);
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x0008A130 File Offset: 0x00088330
	public static Tweener ScaleTo(GameObject obj, Vector3 scaleTo, float time, Ease easeType = Ease.InOutQuad)
	{
		return obj.transform.DOScale(scaleTo, time).SetEase(easeType);
	}

	// Token: 0x040022CE RID: 8910
	private const float MIN_SCALE = 0.001f;
}
