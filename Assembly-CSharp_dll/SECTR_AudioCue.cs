using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class SECTR_AudioCue : ScriptableObject
{
	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000210 RID: 528 RVA: 0x0000ECF4 File Offset: 0x0000CEF4
	// (set) Token: 0x0600020F RID: 527 RVA: 0x0000ECD4 File Offset: 0x0000CED4
	public SECTR_AudioCue Template
	{
		get
		{
			return this.template;
		}
		set
		{
			if (this.template != value && value != this)
			{
				this.template = value;
			}
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000212 RID: 530 RVA: 0x0000ED13 File Offset: 0x0000CF13
	// (set) Token: 0x06000211 RID: 529 RVA: 0x0000ECFC File Offset: 0x0000CEFC
	public SECTR_AudioBus Bus
	{
		get
		{
			return this.bus;
		}
		set
		{
			if (this.bus != value)
			{
				this.bus = value;
			}
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000213 RID: 531 RVA: 0x0000ED1B File Offset: 0x0000CF1B
	public SECTR_AudioCue SourceCue
	{
		get
		{
			if (!(this.template != null))
			{
				return this;
			}
			return this.template;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000214 RID: 532 RVA: 0x0000ED33 File Offset: 0x0000CF33
	public bool Is3D
	{
		get
		{
			return this.Spatialization > SECTR_AudioCue.Spatializations.Simple2D;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000215 RID: 533 RVA: 0x0000ED3E File Offset: 0x0000CF3E
	public bool IsLocal
	{
		get
		{
			return this.Spatialization == SECTR_AudioCue.Spatializations.Simple2D || this.Spatialization == SECTR_AudioCue.Spatializations.Infinite3D;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000216 RID: 534 RVA: 0x0000ED53 File Offset: 0x0000CF53
	public int ClipIndex
	{
		get
		{
			return this.clipPlaybackIndex;
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000ED5C File Offset: 0x0000CF5C
	public SECTR_AudioCue.ClipData GetNextClip()
	{
		if (UnityEngine.Random.Range(0f, 1f) > this.SourceCue.ChanceToPlay)
		{
			return null;
		}
		int count = this.AudioClips.Count;
		if (count == 1)
		{
			return this.AudioClips[0];
		}
		if (count > 0)
		{
			switch (this.PlaybackMode)
			{
			case SECTR_AudioCue.PlaybackModes.Random:
				return this.AudioClips[UnityEngine.Random.Range(0, count)];
			case SECTR_AudioCue.PlaybackModes.Shuffle:
				this.clipPlaybackIndex++;
				if (this.clipPlaybackIndex >= count)
				{
					this.clipPlaybackIndex = 0;
					this.needsShuffling = true;
				}
				if (this.needsShuffling)
				{
					this._ShuffleClips();
					this.needsShuffling = false;
				}
				return this.AudioClips[this.clipPlaybackIndex];
			case SECTR_AudioCue.PlaybackModes.Loop:
			{
				int num = this.clipPlaybackIndex + 1;
				this.clipPlaybackIndex = num;
				this.clipPlaybackIndex = num % count;
				return this.AudioClips[this.clipPlaybackIndex];
			}
			case SECTR_AudioCue.PlaybackModes.PingPong:
				if (this.pingPongIncrement)
				{
					this.clipPlaybackIndex++;
					this.pingPongIncrement = (this.clipPlaybackIndex < this.AudioClips.Count - 1);
				}
				else
				{
					this.clipPlaybackIndex--;
					this.pingPongIncrement = (this.clipPlaybackIndex <= 0);
				}
				return this.AudioClips[this.clipPlaybackIndex];
			}
		}
		return null;
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000EEC0 File Offset: 0x0000D0C0
	public float MinClipLength()
	{
		float num = float.MaxValue;
		bool flag = false;
		int count = this.AudioClips.Count;
		for (int i = 0; i < count; i++)
		{
			AudioClip clip = this.AudioClips[i].Clip;
			if (clip)
			{
				num = Mathf.Min(num, clip.length);
				flag = true;
			}
		}
		if (!flag)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000EF24 File Offset: 0x0000D124
	public float MaxClipLength()
	{
		float num = 0f;
		int count = this.AudioClips.Count;
		for (int i = 0; i < count; i++)
		{
			AudioClip clip = this.AudioClips[i].Clip;
			if (clip)
			{
				num = Mathf.Max(num, clip.length);
			}
		}
		return num;
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000EF77 File Offset: 0x0000D177
	public void ResetClipIndex()
	{
		this.needsShuffling = true;
		this.pingPongIncrement = true;
		this.clipPlaybackIndex = -1;
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000EF8E File Offset: 0x0000D18E
	private void OnEnable()
	{
		this.ResetClipIndex();
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00003296 File Offset: 0x00001496
	private void OnDisable()
	{
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000EF98 File Offset: 0x0000D198
	private void _ShuffleClips()
	{
		System.Random random = new System.Random();
		int i = this.AudioClips.Count;
		while (i >= 1)
		{
			i--;
			int index = random.Next(i + 1);
			SECTR_AudioCue.ClipData value = this.AudioClips[index];
			this.AudioClips[index] = this.AudioClips[i];
			this.AudioClips[i] = value;
		}
	}

	// Token: 0x04000260 RID: 608
	[SerializeField]
	[HideInInspector]
	private SECTR_AudioCue template;

	// Token: 0x04000261 RID: 609
	[SerializeField]
	[HideInInspector]
	private SECTR_AudioBus bus;

	// Token: 0x04000262 RID: 610
	private int clipPlaybackIndex = -1;

	// Token: 0x04000263 RID: 611
	private bool needsShuffling = true;

	// Token: 0x04000264 RID: 612
	private bool pingPongIncrement = true;

	// Token: 0x04000265 RID: 613
	[SECTR_ToolTip("List of Audio Clips for this Cue to choose from.")]
	public List<SECTR_AudioCue.ClipData> AudioClips = new List<SECTR_AudioCue.ClipData>();

	// Token: 0x04000266 RID: 614
	[SECTR_ToolTip("The rules for selecting which audio clip to play next")]
	public SECTR_AudioCue.PlaybackModes PlaybackMode;

	// Token: 0x04000267 RID: 615
	[SECTR_ToolTip("Determines if the sound should be mixed in HDR or LDR.")]
	public bool HDR;

	// Token: 0x04000268 RID: 616
	[SECTR_ToolTip("The loudness, in dB(SPL), of this HDR Cue.")]
	public Vector2 Loudness = new Vector2(50f, 50f);

	// Token: 0x04000269 RID: 617
	[SECTR_ToolTip("The volume of this Cue.")]
	public Vector2 Volume = Vector2.one;

	// Token: 0x0400026A RID: 618
	[SECTR_ToolTip("The pitch adjustment of this Cue.")]
	public Vector2 Pitch = Vector2.one;

	// Token: 0x0400026B RID: 619
	[SECTR_ToolTip("Set to true to auto-loop this Cue.")]
	public bool Loops;

	// Token: 0x0400026C RID: 620
	[SECTR_ToolTip("Cue priority, lower is more important.", 0f, 255f)]
	public int Priority = 128;

	// Token: 0x0400026D RID: 621
	[SECTR_ToolTip("Chance cue will play at all.", 0f, 1f)]
	public float ChanceToPlay = 1f;

	// Token: 0x0400026E RID: 622
	[SECTR_ToolTip("Prevent this Cue from recieving Audio Effects.")]
	public bool BypassEffects;

	// Token: 0x0400026F RID: 623
	[SECTR_ToolTip("Maximum number of instances of this Cue that can be played at once.", 1f, -1f)]
	public int MaxInstances = 10;

	// Token: 0x04000270 RID: 624
	[SECTR_ToolTip("Number of seconds over which to fade in the Cue when played.", 0f, -1f)]
	public float FadeInTime;

	// Token: 0x04000271 RID: 625
	[SECTR_ToolTip("Number of seconds over which to fade out the Cue when stopped.", 0f, -1f)]
	public float FadeOutTime;

	// Token: 0x04000272 RID: 626
	[SECTR_ToolTip("Sets rules for how to spatialize this sound.")]
	public SECTR_AudioCue.Spatializations Spatialization = SECTR_AudioCue.Spatializations.Local3D;

	// Token: 0x04000273 RID: 627
	[SECTR_ToolTip("Expands or narrows the range of speakers out of which this Cue plays.", 0f, 360f)]
	public float Spread;

	// Token: 0x04000274 RID: 628
	[SECTR_ToolTip("Moves the sound around the speaker field.", -1f, 1f)]
	public float Pan2D;

	// Token: 0x04000275 RID: 629
	[SECTR_ToolTip("Attenuation style of this clip.")]
	public SECTR_AudioCue.FalloffTypes Falloff;

	// Token: 0x04000276 RID: 630
	[SECTR_ToolTip("The range at which the sound is no longer audible.", 0f, -1f)]
	public float MaxDistance = 100f;

	// Token: 0x04000277 RID: 631
	[SECTR_ToolTip("The range within which the sound will be at peak volume/loudness.", 0f, -1f)]
	public float MinDistance = 10f;

	// Token: 0x04000278 RID: 632
	[SECTR_ToolTip("Scales the amount of doppler effect applied to this Cue.", 0f, 1f)]
	public float DopplerLevel;

	// Token: 0x04000279 RID: 633
	[SECTR_ToolTip("Prevents too many instances of a cue playing near one another.", 0f, -1f)]
	public int ProximityLimit;

	// Token: 0x0400027A RID: 634
	[SECTR_ToolTip("The size of the proximity limit check.", "ProximityLimit", 0f, -1f)]
	public float ProximityRange = 10f;

	// Token: 0x0400027B RID: 635
	[SECTR_ToolTip("Allows you to scale down the amount of occlusion applied to this Cue (when occluded).", 0f, 1f)]
	public float OcclusionScale = 1f;

	// Token: 0x0400027C RID: 636
	[SECTR_ToolTip("The chance that this cue will actually make a sound when played.", 0f, 1f)]
	public float PlayProbability = 1f;

	// Token: 0x0400027D RID: 637
	[SECTR_ToolTip("Random delay before start of playback.")]
	public Vector2 Delay = Vector2.zero;

	// Token: 0x02000074 RID: 116
	public enum PlaybackModes
	{
		// Token: 0x0400027F RID: 639
		Random,
		// Token: 0x04000280 RID: 640
		Shuffle,
		// Token: 0x04000281 RID: 641
		Loop,
		// Token: 0x04000282 RID: 642
		PingPong
	}

	// Token: 0x02000075 RID: 117
	public enum FalloffTypes
	{
		// Token: 0x04000284 RID: 644
		Linear,
		// Token: 0x04000285 RID: 645
		Logrithmic
	}

	// Token: 0x02000076 RID: 118
	public enum Spatializations
	{
		// Token: 0x04000287 RID: 647
		Simple2D,
		// Token: 0x04000288 RID: 648
		Infinite3D,
		// Token: 0x04000289 RID: 649
		Local3D,
		// Token: 0x0400028A RID: 650
		Occludable3D
	}

	// Token: 0x02000077 RID: 119
	[Serializable]
	public class ClipData
	{
		// Token: 0x0600021F RID: 543 RVA: 0x0000F0C5 File Offset: 0x0000D2C5
		public ClipData(AudioClip clip)
		{
			this.clip = clip;
			this.playedInShuffle = false;
			this.volume = 1f;
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000F0F1 File Offset: 0x0000D2F1
		public AudioClip Clip
		{
			get
			{
				return this.clip;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000F0F9 File Offset: 0x0000D2F9
		// (set) Token: 0x06000222 RID: 546 RVA: 0x0000F101 File Offset: 0x0000D301
		public float Volume
		{
			get
			{
				return this.volume;
			}
			set
			{
				this.volume = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000F10A File Offset: 0x0000D30A
		// (set) Token: 0x06000224 RID: 548 RVA: 0x0000F112 File Offset: 0x0000D312
		public bool PlayedInShuffle
		{
			get
			{
				return this.playedInShuffle;
			}
			set
			{
				this.playedInShuffle = value;
			}
		}

		// Token: 0x0400028B RID: 651
		[SerializeField]
		private AudioClip clip;

		// Token: 0x0400028C RID: 652
		[SerializeField]
		private bool playedInShuffle;

		// Token: 0x0400028D RID: 653
		[SerializeField]
		private float volume = 1f;

		// Token: 0x0400028E RID: 654
		[SerializeField]
		private SECTR_ULong bakeTimestamp;

		// Token: 0x0400028F RID: 655
		public AnimationCurve HDRCurve;
	}
}
