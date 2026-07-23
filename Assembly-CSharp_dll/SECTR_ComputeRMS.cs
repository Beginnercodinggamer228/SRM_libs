using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000086 RID: 134
[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
[AddComponentMenu("")]
public class SECTR_ComputeRMS : MonoBehaviour
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060002D3 RID: 723 RVA: 0x00012018 File Offset: 0x00010218
	public float Progress
	{
		get
		{
			if (this.hdrBakeList != null)
			{
				int count = this.hdrBakeList.Count;
				int count2 = this.activeBakeList.Count;
				float a = (float)(this.hdrBakeIndex - count2) / (float)count;
				float b = (float)this.hdrBakeIndex / (float)count;
				float num = 1f;
				for (int i = 0; i < count2; i++)
				{
					SECTR_ComputeRMS sectr_ComputeRMS = this.activeBakeList[i];
					if (sectr_ComputeRMS)
					{
						num = Mathf.Min(num, sectr_ComputeRMS.Progress);
					}
				}
				return Mathf.Lerp(a, b, num);
			}
			AudioSource component = base.GetComponent<AudioSource>();
			if (component)
			{
				return component.time / component.clip.length;
			}
			return 1f;
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00003296 File Offset: 0x00001496
	private void OnEnable()
	{
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00003296 File Offset: 0x00001496
	private void OnDisable()
	{
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x000120D4 File Offset: 0x000102D4
	private void Update()
	{
		bool flag;
		if (this.hdrBakeList != null)
		{
			int count = this.hdrBakeList.Count;
			flag = (count == 0);
			if (!flag)
			{
				if (this.activeBakeList.Count == 0)
				{
					if (this.hdrBakeIndex == count)
					{
						flag = true;
					}
					else
					{
						int num = Mathf.Min(this.hdrBakeIndex + 4, count);
						for (int i = this.hdrBakeIndex; i < num; i++)
						{
							SECTR_ComputeRMS.BakeInfo bakeInfo = this.hdrBakeList[i];
							SECTR_ComputeRMS sectr_ComputeRMS = new GameObject("Bake " + bakeInfo.cue.name + bakeInfo.clipData.Clip.name)
							{
								transform = 
								{
									parent = base.transform,
									localPosition = Vector3.zero
								},
								hideFlags = HideFlags.HideAndDontSave
							}.AddComponent<SECTR_ComputeRMS>();
							sectr_ComputeRMS._StartCompute(bakeInfo.cue, bakeInfo.clipData);
							this.activeBakeList.Add(sectr_ComputeRMS);
						}
						this.hdrBakeIndex = num;
					}
				}
				else
				{
					bool flag2 = true;
					int count2 = this.activeBakeList.Count;
					for (int j = 0; j < count2; j++)
					{
						if (this.activeBakeList[j] != null)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						this.activeBakeList.Clear();
					}
				}
			}
		}
		else
		{
			int count3 = this.samples.Count;
			flag = (this.clipData == null);
			if (!flag && count3 > 0)
			{
				int num2 = AudioSettings.outputSampleRate * this.numChannels;
				int num3 = (int)(this.clipData.Clip.length * (float)num2);
				int num4 = num2 / 10;
				AudioSource component = base.GetComponent<AudioSource>();
				if ((!component.isPlaying && count3 >= num3 - num4) || (component.isPlaying && count3 >= num3))
				{
					int num5 = Mathf.CeilToInt((float)count3 / (float)num2) + 1;
					float[] array = new float[num5];
					int num6 = 0;
					for (int k = 1; k < num5; k++)
					{
						float num7 = 0f;
						int num8 = 0;
						int num9 = 0;
						while (num9 < num2 && num6 < count3)
						{
							num8++;
							float num10 = this.samples[num6++];
							num7 += num10 * num10;
							num9++;
						}
						num7 = Mathf.Sqrt(num7 / (float)num8);
						if (Mathf.Abs(num7) < 0.001f && k == num5 - 1 && num5 > 2)
						{
							array[k] = array[k - 1];
						}
						else
						{
							array[k] = Mathf.Clamp(20f * Mathf.Log10(num7), -160f, 160f);
						}
					}
					if (this.cue.Loops)
					{
						array[0] = array[array.Length - 1];
					}
					else
					{
						array[0] = array[1];
					}
					for (int l = 0; l < num5; l++)
					{
						if (array[l] > -160f)
						{
							break;
						}
					}
					flag = true;
				}
				else if (!component.isPlaying)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			Destroyer.Destroy(base.gameObject, "SECTR_ComputeRMS.Update");
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000123F2 File Offset: 0x000105F2
	private void OnAudioFilterRead(float[] samples, int numChannels)
	{
		this.numChannels = numChannels;
		this.samples.AddRange(samples);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00012408 File Offset: 0x00010608
	public void _StartCompute(SECTR_AudioCue cue, SECTR_AudioCue.ClipData clipData)
	{
		this.cue = cue;
		this.clipData = clipData;
		AudioSource component = base.GetComponent<AudioSource>();
		component.clip = clipData.Clip;
		component.dopplerLevel = 0f;
		component.ignoreListenerPause = true;
		component.ignoreListenerVolume = true;
		component.bypassListenerEffects = true;
		component.bypassReverbZones = true;
		component.maxDistance = float.MaxValue;
		component.minDistance = float.MaxValue;
		component.rolloffMode = AudioRolloffMode.Linear;
		component.playOnAwake = false;
		component.loop = false;
		component.volume = 1f;
		this.samples.Clear();
		base.GetComponent<AudioSource>().Play();
	}

	// Token: 0x040002F6 RID: 758
	private List<SECTR_ComputeRMS.BakeInfo> hdrBakeList;

	// Token: 0x040002F7 RID: 759
	private List<SECTR_ComputeRMS> activeBakeList = new List<SECTR_ComputeRMS>();

	// Token: 0x040002F8 RID: 760
	private int hdrBakeIndex;

	// Token: 0x040002F9 RID: 761
	private SECTR_AudioCue cue;

	// Token: 0x040002FA RID: 762
	private SECTR_AudioCue.ClipData clipData;

	// Token: 0x040002FB RID: 763
	private List<float> samples = new List<float>();

	// Token: 0x040002FC RID: 764
	private int numChannels;

	// Token: 0x02000087 RID: 135
	private struct BakeInfo
	{
		// Token: 0x060002DA RID: 730 RVA: 0x000124C5 File Offset: 0x000106C5
		public BakeInfo(SECTR_AudioCue cue, SECTR_AudioCue.ClipData clipData)
		{
			this.cue = cue;
			this.clipData = clipData;
		}

		// Token: 0x040002FD RID: 765
		public SECTR_AudioCue cue;

		// Token: 0x040002FE RID: 766
		public SECTR_AudioCue.ClipData clipData;
	}
}
