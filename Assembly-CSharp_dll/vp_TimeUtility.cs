using System;
using UnityEngine;

// Token: 0x02000842 RID: 2114
public static class vp_TimeUtility
{
	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06002C52 RID: 11346 RVA: 0x000A7614 File Offset: 0x000A5814
	// (set) Token: 0x06002C53 RID: 11347 RVA: 0x000A761B File Offset: 0x000A581B
	public static float TimeScale
	{
		get
		{
			return Time.timeScale;
		}
		set
		{
			value = vp_TimeUtility.ClampTimeScale(value);
			Time.timeScale = value;
			Time.fixedDeltaTime = vp_TimeUtility.InitialFixedTimeStep * Time.timeScale;
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06002C54 RID: 11348 RVA: 0x000A763B File Offset: 0x000A583B
	public static float AdjustedTimeScale
	{
		get
		{
			return 1f / (Time.timeScale * (0.02f / Time.fixedDeltaTime));
		}
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x000A7654 File Offset: 0x000A5854
	public static void FadeTimeScale(float targetTimeScale, float fadeSpeed)
	{
		if (vp_TimeUtility.TimeScale == targetTimeScale)
		{
			return;
		}
		targetTimeScale = vp_TimeUtility.ClampTimeScale(targetTimeScale);
		vp_TimeUtility.TimeScale = Mathf.Lerp(vp_TimeUtility.TimeScale, targetTimeScale, Time.deltaTime * 60f * fadeSpeed);
		if (Mathf.Abs(vp_TimeUtility.TimeScale - targetTimeScale) < 0.01f)
		{
			vp_TimeUtility.TimeScale = targetTimeScale;
		}
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x000A76A8 File Offset: 0x000A58A8
	private static float ClampTimeScale(float t)
	{
		if (t < vp_TimeUtility.m_MinTimeScale || t > vp_TimeUtility.m_MaxTimeScale)
		{
			t = Mathf.Clamp(t, vp_TimeUtility.m_MinTimeScale, vp_TimeUtility.m_MaxTimeScale);
			Debug.LogWarning(string.Concat(new object[]
			{
				"Warning: (vp_TimeUtility) TimeScale was clamped to within the supported range (",
				vp_TimeUtility.m_MinTimeScale,
				" - ",
				vp_TimeUtility.m_MaxTimeScale,
				")."
			}));
		}
		return t;
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06002C57 RID: 11351 RVA: 0x000A771A File Offset: 0x000A591A
	// (set) Token: 0x06002C58 RID: 11352 RVA: 0x000A7724 File Offset: 0x000A5924
	public static bool Paused
	{
		get
		{
			return vp_TimeUtility.m_Paused;
		}
		set
		{
			if (value)
			{
				if (vp_TimeUtility.m_Paused)
				{
					return;
				}
				vp_TimeUtility.m_Paused = true;
				vp_TimeUtility.m_TimeScaleOnPause = Time.timeScale;
				Time.timeScale = 0f;
				return;
			}
			else
			{
				if (!vp_TimeUtility.m_Paused)
				{
					return;
				}
				vp_TimeUtility.m_Paused = false;
				Time.timeScale = vp_TimeUtility.m_TimeScaleOnPause;
				vp_TimeUtility.m_TimeScaleOnPause = 1f;
				return;
			}
		}
	}

	// Token: 0x04002A81 RID: 10881
	private static float m_MinTimeScale = 0.1f;

	// Token: 0x04002A82 RID: 10882
	private static float m_MaxTimeScale = 1f;

	// Token: 0x04002A83 RID: 10883
	private static bool m_Paused = false;

	// Token: 0x04002A84 RID: 10884
	private static float m_TimeScaleOnPause = 1f;

	// Token: 0x04002A85 RID: 10885
	public static float InitialFixedTimeStep = Time.fixedDeltaTime;
}
