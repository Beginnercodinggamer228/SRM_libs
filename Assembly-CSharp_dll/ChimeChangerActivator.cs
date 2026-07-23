using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class ChimeChangerActivator : SRBehaviour, TechActivator
{
	// Token: 0x0600102E RID: 4142 RVA: 0x00040F44 File Offset: 0x0003F144
	public void Awake()
	{
		this.buttonAnimator = base.GetComponentInParent<Animator>();
		this.buttonAnimation = Animator.StringToHash("ButtonPressed");
		this.clipRegex = new Regex("([0-9]+)");
		this.clipIndexes = (from jingle in this.chimeSongList
		select (from clip in jingle.clips
		select this.ParseClips(clip).ToList<int>()).ToList<List<int>>()).ToList<List<List<int>>>();
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public void Activate()
	{
		if (this.nextActivationTime < Time.time)
		{
			this.nextActivationTime = Time.time + 0.4f;
			this.buttonAnimator.SetTrigger(this.buttonAnimation);
			if (this.currentCoroutine != null)
			{
				base.StopCoroutine(this.currentCoroutine);
			}
			SRSingleton<GameContext>.Instance.MusicDirector.SetWigglyMode();
			SRSingleton<SceneContext>.Instance.InstrumentDirector.SelectNextInstrument();
			AnalyticsUtil.CustomEvent("ChimeChangerActivated", new Dictionary<string, object>
			{
				{
					"NewInstrument",
					SRSingleton<SceneContext>.Instance.InstrumentDirector.currentInstrument.xlateKey
				}
			}, true);
			int num = Randoms.SHARED.GetInt(this.chimeSongList.Count);
			if (num == this.lastClip)
			{
				num = (num + 1) % this.chimeSongList.Count;
			}
			this.currentCoroutine = base.StartCoroutine(this.PlayClips(num));
			this.lastClip = num;
		}
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00041089 File Offset: 0x0003F289
	private IEnumerator PlayClips(int clipIndex)
	{
		foreach (List<int> list in this.clipIndexes[clipIndex])
		{
			foreach (int num in list)
			{
				SECTR_AudioSystem.Play(SRSingleton<SceneContext>.Instance.InstrumentDirector.currentInstrument.cue, null, base.transform.position, false, new int?(num - 1), false);
			}
			yield return new WaitForSeconds(this.chimeSongList[clipIndex].distance / 10f);
		}
		List<List<int>>.Enumerator enumerator = default(List<List<int>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x0004109F File Offset: 0x0003F29F
	private IEnumerable<int> ParseClips(string input)
	{
		foreach (object obj in this.clipRegex.Matches(input))
		{
			Match match = (Match)obj;
			yield return int.Parse(match.Value);
		}
		IEnumerator enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x04000F04 RID: 3844
	public ChimeSongList chimeSongList;

	// Token: 0x04000F05 RID: 3845
	private List<List<List<int>>> clipIndexes;

	// Token: 0x04000F06 RID: 3846
	private Animator buttonAnimator;

	// Token: 0x04000F07 RID: 3847
	private int buttonAnimation;

	// Token: 0x04000F08 RID: 3848
	private const float TIME_BETWEEN_ACTIVATIONS = 0.4f;

	// Token: 0x04000F09 RID: 3849
	private float nextActivationTime;

	// Token: 0x04000F0A RID: 3850
	private Coroutine currentCoroutine;

	// Token: 0x04000F0B RID: 3851
	private int lastClip = -1;

	// Token: 0x04000F0C RID: 3852
	private Regex clipRegex;
}
