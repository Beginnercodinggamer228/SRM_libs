using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200084E RID: 2126
public sealed class vp_DecalManager
{
	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06002CC3 RID: 11459 RVA: 0x000A8E04 File Offset: 0x000A7004
	// (set) Token: 0x06002CC4 RID: 11460 RVA: 0x000A8E0B File Offset: 0x000A700B
	public static float MaxDecals
	{
		get
		{
			return vp_DecalManager.m_MaxDecals;
		}
		set
		{
			vp_DecalManager.m_MaxDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06002CC5 RID: 11461 RVA: 0x000A8E18 File Offset: 0x000A7018
	// (set) Token: 0x06002CC6 RID: 11462 RVA: 0x000A8E1F File Offset: 0x000A701F
	public static float FadedDecals
	{
		get
		{
			return vp_DecalManager.m_FadedDecals;
		}
		set
		{
			if (value > vp_DecalManager.m_MaxDecals)
			{
				Debug.LogError("FadedDecals can't be larger than MaxDecals");
				return;
			}
			vp_DecalManager.m_FadedDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000A8E40 File Offset: 0x000A7040
	static vp_DecalManager()
	{
		vp_DecalManager.Refresh();
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000053FC File Offset: 0x000035FC
	private vp_DecalManager()
	{
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x000A8E90 File Offset: 0x000A7090
	public static void Add(GameObject decal)
	{
		if (vp_DecalManager.m_Decals.Contains(decal))
		{
			vp_DecalManager.m_Decals.Remove(decal);
		}
		Color color = decal.GetComponent<Renderer>().material.color;
		color.a = 1f;
		decal.GetComponent<Renderer>().material.color = color;
		vp_DecalManager.m_Decals.Add(decal);
		vp_DecalManager.FadeAndRemove();
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x000A8EF4 File Offset: 0x000A70F4
	private static void FadeAndRemove()
	{
		if ((float)vp_DecalManager.m_Decals.Count > vp_DecalManager.m_NonFadedDecals)
		{
			int num = 0;
			while ((float)num < (float)vp_DecalManager.m_Decals.Count - vp_DecalManager.m_NonFadedDecals)
			{
				if (vp_DecalManager.m_Decals[num] != null)
				{
					Color color = vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color;
					color.a -= vp_DecalManager.m_FadeAmount;
					vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color = color;
				}
				num++;
			}
		}
		if (vp_DecalManager.m_Decals[0] != null)
		{
			if (vp_DecalManager.m_Decals[0].GetComponent<Renderer>().material.color.a <= 0f)
			{
				vp_Utility.Destroy(vp_DecalManager.m_Decals[0]);
				vp_DecalManager.m_Decals.Remove(vp_DecalManager.m_Decals[0]);
				return;
			}
		}
		else
		{
			vp_DecalManager.m_Decals.RemoveAt(0);
		}
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x000A8FF4 File Offset: 0x000A71F4
	private static void Refresh()
	{
		if (vp_DecalManager.m_MaxDecals < vp_DecalManager.m_FadedDecals)
		{
			vp_DecalManager.m_MaxDecals = vp_DecalManager.m_FadedDecals;
		}
		vp_DecalManager.m_FadeAmount = vp_DecalManager.m_MaxDecals / vp_DecalManager.m_FadedDecals / vp_DecalManager.m_MaxDecals;
		vp_DecalManager.m_NonFadedDecals = vp_DecalManager.m_MaxDecals - vp_DecalManager.m_FadedDecals;
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x000A9034 File Offset: 0x000A7234
	private static void DebugOutput()
	{
		int num = 0;
		int num2 = 0;
		using (List<GameObject>.Enumerator enumerator = vp_DecalManager.m_Decals.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetComponent<Renderer>().material.color.a == 1f)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"Decal count: ",
			vp_DecalManager.m_Decals.Count,
			", Full: ",
			num,
			", Faded: ",
			num2
		}));
	}

	// Token: 0x04002AB3 RID: 10931
	public static readonly vp_DecalManager instance = new vp_DecalManager();

	// Token: 0x04002AB4 RID: 10932
	private static List<GameObject> m_Decals = new List<GameObject>();

	// Token: 0x04002AB5 RID: 10933
	private static float m_MaxDecals = 100f;

	// Token: 0x04002AB6 RID: 10934
	private static float m_FadedDecals = 20f;

	// Token: 0x04002AB7 RID: 10935
	private static float m_NonFadedDecals = 0f;

	// Token: 0x04002AB8 RID: 10936
	private static float m_FadeAmount = 0f;
}
