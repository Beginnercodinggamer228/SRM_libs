using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200022C RID: 556
public class MusicDirector : SRBehaviour
{
	// Token: 0x06000BF4 RID: 3060 RVA: 0x00032380 File Offset: 0x00030580
	private bool SetupTimeDirector()
	{
		if (!this.timeDirectorSet && SRSingleton<SceneContext>.Instance != null)
		{
			this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
			this.timeDirectorSet = true;
		}
		return this.timeDirectorSet;
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x000323B4 File Offset: 0x000305B4
	public void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x000323C7 File Offset: 0x000305C7
	public void OnDestroy()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x000323DC File Offset: 0x000305DC
	public void Update()
	{
		if (this.SetupTimeDirector() && this.timeDirector.IsFastForwarding())
		{
			return;
		}
		if (this.suppressors.Count > 0)
		{
			this.current.instance.Stop(false);
			return;
		}
		MusicDirector.Music music = this.queue.Peek();
		SECTR_AudioCue currentCue = music.GetCurrentCue();
		if (this.current.cue != currentCue)
		{
			MusicDirector.CurrentMetadata currentMetadata = new MusicDirector.CurrentMetadata
			{
				music = music,
				cue = currentCue,
				priority = music.Priority,
				startTime = Time.unscaledTime,
				offset = 0f
			};
			if (this.current.instance.Active)
			{
				if (currentMetadata.music.GetWaitForFadeOut(this.current.music))
				{
					currentMetadata.startTime += this.current.cue.FadeOutTime;
				}
				if (currentMetadata.music.GetStartMidway(this.current.music))
				{
					currentMetadata.offset += currentMetadata.cue.MinClipLength() * Randoms.SHARED.GetInRange(0.1f, 0.3f);
				}
			}
			this.current.instance.Stop(false);
			this.current = currentMetadata;
		}
		if (!this.current.instance.Active && Time.unscaledTime >= this.current.startTime)
		{
			this.current.cue.Loops = this.current.music.Loops;
			this.current.instance = SECTR_AudioSystem.Play(this.current.cue, Vector3.zero, false);
			this.current.instance.TimeSeconds = this.current.offset;
			if (Mathf.Approximately(this.current.offset, 0f))
			{
				this.current.instance.SkipFadeIn();
			}
			if (!this.current.music.Loops)
			{
				this.current.startTime = float.MaxValue;
			}
			this.current.offset = 0f;
		}
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00032601 File Offset: 0x00030801
	public void RegisterSuppressor(UnityEngine.Object obj)
	{
		this.suppressors.Add(obj);
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00032610 File Offset: 0x00030810
	public void DeregisterSuppressor(UnityEngine.Object obj)
	{
		this.suppressors.Remove(obj);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0003261F File Offset: 0x0003081F
	public void ForceStopCurrent()
	{
		this.current.instance.Stop(true);
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00032632 File Offset: 0x00030832
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.SetupTimeDirector();
		this.queue = new MusicDirector.PriorityQueue();
		this.Enqueue(this.GetSceneCue(scene), MusicDirector.Priority.DEFAULT);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00032654 File Offset: 0x00030854
	private SECTR_AudioCue GetSceneCue(Scene scene)
	{
		if (scene.name == "MainMenu")
		{
			return this.menuMusic;
		}
		return this.defaultMusic;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00032678 File Offset: 0x00030878
	public void OnRegionsChanged(RegionMember member)
	{
		MusicDirector.Music regionMusic = this.GetRegionMusic(member);
		if (regionMusic != null)
		{
			this.Enqueue(regionMusic, true);
		}
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00032698 File Offset: 0x00030898
	private MusicDirector.Music GetRegionMusic(RegionMember member)
	{
		for (int i = member.regions.Count - 1; i >= 0; i--)
		{
			RegionBackgroundMusic component = member.regions[i].gameObject.GetComponent<RegionBackgroundMusic>();
			if (component != null && component.backgroundMusic != null)
			{
				return component.backgroundMusic;
			}
		}
		HashSet<ZoneDirector.Zone> source = ZoneDirector.Zones(member);
		ZoneDirector.Zone zone = source.Any<ZoneDirector.Zone>() ? source.Max<ZoneDirector.Zone>() : ZoneDirector.Zone.NONE;
		if (this.ranchWistfulMusic.Enabled())
		{
			if (zone == ZoneDirector.Zone.RANCH)
			{
				return this.ranchWistfulMusic;
			}
			this.ranchWistfulMusic.Stop();
		}
		switch (zone)
		{
		case ZoneDirector.Zone.RANCH:
			return this.ranchMusic;
		case ZoneDirector.Zone.REEF:
			return this.reefMusic;
		case ZoneDirector.Zone.QUARRY:
			return this.quarryMusic;
		case ZoneDirector.Zone.MOSS:
			return this.mossMusic;
		case ZoneDirector.Zone.DESERT:
			return this.desertMusic;
		case ZoneDirector.Zone.SEA:
			return this.seaMusic;
		case ZoneDirector.Zone.RUINS:
			return this.ruinsMusic;
		case ZoneDirector.Zone.RUINS_TRANSITION:
			return this.ruinsTransMusic;
		case ZoneDirector.Zone.WILDS:
			return this.wildsMusic;
		case ZoneDirector.Zone.OGDEN_RANCH:
			return this.ogdenRanchMusic;
		case ZoneDirector.Zone.VALLEY:
			return this.valleyMusic;
		case ZoneDirector.Zone.MOCHI_RANCH:
			return this.mochiRanchMusic;
		case ZoneDirector.Zone.SLIMULATIONS:
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.musicSlimulation;
		case ZoneDirector.Zone.VIKTOR_LAB:
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.musicViktorLab;
		}
		return null;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x000327EA File Offset: 0x000309EA
	public void SetTarrMode(bool enabled)
	{
		this.Enqueue(this.tarrMusic, MusicDirector.Priority.TARR, enabled);
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x000327FB File Offset: 0x000309FB
	public void SetOasisMode(bool enabled)
	{
		this.Enqueue(this.oasisMusic, enabled);
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0003280A File Offset: 0x00030A0A
	public void SetEventGordoMode(bool enabled)
	{
		this.Enqueue(this.eventGordoMusic, MusicDirector.Priority.PARTY_GORDO, enabled);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0003281B File Offset: 0x00030A1B
	public void SetValleyRaceMode(bool enabled)
	{
		this.Enqueue(this.valleyRaceMusic, MusicDirector.Priority.VALLEY_RACE, enabled);
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x0003282C File Offset: 0x00030A2C
	public void SetCreditsMode(bool enabled)
	{
		this.Enqueue(this.creditsMusic, enabled);
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0003283B File Offset: 0x00030A3B
	public void EnableSlimeHoopMode(double endTime)
	{
		SRSingleton<SceneContext>.Instance.TimeDirector.AddPassedTimeDelegate(endTime, delegate
		{
			this.Dequeue(MusicDirector.Priority.SLIME_HOOP);
		});
		this.Enqueue(this.slimeHoopMusic, MusicDirector.Priority.SLIME_HOOP, true);
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00032868 File Offset: 0x00030A68
	public void SetHouseMode(bool enabled)
	{
		this.Enqueue(this.houseMusic, enabled);
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00032877 File Offset: 0x00030A77
	public void SetWistfulRanchMode()
	{
		this.ranchWistfulMusic.ResetEndTime();
		this.Enqueue(this.ranchWistfulMusic, true);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00032891 File Offset: 0x00030A91
	public void SetWigglyMode()
	{
		this.Enqueue(this.wigglyMusic, true);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x000328A0 File Offset: 0x00030AA0
	public void SetRushWarningMode(SECTR_AudioCue music)
	{
		if (music != null)
		{
			this.Enqueue(music, MusicDirector.Priority.RUSH_MODE_WARNING);
			return;
		}
		this.Dequeue(MusicDirector.Priority.RUSH_MODE_WARNING);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x000328BD File Offset: 0x00030ABD
	public void SetRushCreditsMode(bool enabled)
	{
		this.Enqueue(this.rushModeCreditsMusic, enabled);
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x000328CC File Offset: 0x00030ACC
	public void SetFirestormMode(FirestormActivator.Mode mode)
	{
		if (mode == FirestormActivator.Mode.ACTIVE)
		{
			this.Enqueue(this.firestormMusic, MusicDirector.Priority.FIRESTORM);
			return;
		}
		if (mode == FirestormActivator.Mode.PREPARING)
		{
			this.Enqueue(this.firestormPrepMusic, MusicDirector.Priority.FIRESTORM);
			return;
		}
		this.Dequeue(MusicDirector.Priority.FIRESTORM);
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x000328FC File Offset: 0x00030AFC
	private void Enqueue(SECTR_AudioCue cue, MusicDirector.Priority priority)
	{
		this.Enqueue(cue, priority, true);
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00032907 File Offset: 0x00030B07
	private void Enqueue(SECTR_AudioCue cue, MusicDirector.Priority priority, bool enabled)
	{
		this.Enqueue(new MusicDirector.Music.Basic
		{
			cue = cue,
			priority = priority
		}, enabled);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00032923 File Offset: 0x00030B23
	private void Enqueue(MusicDirector.Music music, bool enabled)
	{
		if (enabled)
		{
			this.queue.Enqueue(music);
			return;
		}
		this.queue.Dequeue(music.Priority);
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00032946 File Offset: 0x00030B46
	private void Dequeue(MusicDirector.Priority priority)
	{
		this.queue.Dequeue(priority);
	}

	// Token: 0x04000AD7 RID: 2775
	public SECTR_AudioCue defaultMusic;

	// Token: 0x04000AD8 RID: 2776
	public SECTR_AudioCue menuMusic;

	// Token: 0x04000AD9 RID: 2777
	public SECTR_AudioCue tarrMusic;

	// Token: 0x04000ADA RID: 2778
	public SECTR_AudioCue slimeHoopMusic;

	// Token: 0x04000ADB RID: 2779
	public SECTR_AudioCue firestormPrepMusic;

	// Token: 0x04000ADC RID: 2780
	public SECTR_AudioCue firestormMusic;

	// Token: 0x04000ADD RID: 2781
	public SECTR_AudioCue valleyRaceMusic;

	// Token: 0x04000ADE RID: 2782
	public SECTR_AudioCue rushModeWarning1Music;

	// Token: 0x04000ADF RID: 2783
	public SECTR_AudioCue rushModeWarning2Music;

	// Token: 0x04000AE0 RID: 2784
	public SECTR_AudioCue eventGordoMusic;

	// Token: 0x04000AE1 RID: 2785
	public MusicDirector.Music.Credits creditsMusic;

	// Token: 0x04000AE2 RID: 2786
	public MusicDirector.Music.Credits rushModeCreditsMusic;

	// Token: 0x04000AE3 RID: 2787
	public MusicDirector.Music.Zone.Default ranchMusic;

	// Token: 0x04000AE4 RID: 2788
	public MusicDirector.Music.Zone_Wistful ranchWistfulMusic;

	// Token: 0x04000AE5 RID: 2789
	public MusicDirector.Music.Zone_Wiggly wigglyMusic;

	// Token: 0x04000AE6 RID: 2790
	public MusicDirector.Music.Zone.Default reefMusic;

	// Token: 0x04000AE7 RID: 2791
	public MusicDirector.Music.Zone.Default quarryMusic;

	// Token: 0x04000AE8 RID: 2792
	public MusicDirector.Music.Zone.Default mossMusic;

	// Token: 0x04000AE9 RID: 2793
	public MusicDirector.Music.Zone.Default desertMusic;

	// Token: 0x04000AEA RID: 2794
	public MusicDirector.Music.Zone.Default seaMusic;

	// Token: 0x04000AEB RID: 2795
	public MusicDirector.Music.Zone.Default ruinsMusic;

	// Token: 0x04000AEC RID: 2796
	public MusicDirector.Music.Zone.Default ruinsTransMusic;

	// Token: 0x04000AED RID: 2797
	public MusicDirector.Music.Zone.Default wildsMusic;

	// Token: 0x04000AEE RID: 2798
	public MusicDirector.Music.Zone.Default ogdenRanchMusic;

	// Token: 0x04000AEF RID: 2799
	public MusicDirector.Music.Zone.Default mochiRanchMusic;

	// Token: 0x04000AF0 RID: 2800
	public MusicDirector.Music.Zone.Default valleyMusic;

	// Token: 0x04000AF1 RID: 2801
	public MusicDirector.Music.Zone.Special oasisMusic;

	// Token: 0x04000AF2 RID: 2802
	public MusicDirector.Music.House houseMusic;

	// Token: 0x04000AF3 RID: 2803
	private const MusicDirector.Priority PRIORITY_ZONE = MusicDirector.Priority.ZONE_DEFAULT;

	// Token: 0x04000AF4 RID: 2804
	private const MusicDirector.Priority PRIORITY_EVENT = MusicDirector.Priority.VALLEY_RACE;

	// Token: 0x04000AF5 RID: 2805
	private const MusicDirector.Priority PRIORITY_HIGH = MusicDirector.Priority.HOUSE;

	// Token: 0x04000AF6 RID: 2806
	private const int HOUR_MORNING = 6;

	// Token: 0x04000AF7 RID: 2807
	private const int HOUR_AFTERNOON = 12;

	// Token: 0x04000AF8 RID: 2808
	private const int HOUR_NIGHT = 18;

	// Token: 0x04000AF9 RID: 2809
	private MusicDirector.PriorityQueue queue = new MusicDirector.PriorityQueue();

	// Token: 0x04000AFA RID: 2810
	private MusicDirector.CurrentMetadata current = new MusicDirector.CurrentMetadata();

	// Token: 0x04000AFB RID: 2811
	private HashSet<UnityEngine.Object> suppressors = new HashSet<UnityEngine.Object>();

	// Token: 0x04000AFC RID: 2812
	private TimeDirector timeDirector;

	// Token: 0x04000AFD RID: 2813
	private bool timeDirectorSet;

	// Token: 0x0200022D RID: 557
	public abstract class Music
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000C11 RID: 3089
		public abstract MusicDirector.Priority Priority { get; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000C12 RID: 3090
		public abstract bool Loops { get; }

		// Token: 0x06000C13 RID: 3091
		public abstract SECTR_AudioCue GetCurrentCue();

		// Token: 0x06000C14 RID: 3092
		public abstract bool GetWaitForFadeOut(MusicDirector.Music previous);

		// Token: 0x06000C15 RID: 3093
		public abstract bool GetStartMidway(MusicDirector.Music previous);

		// Token: 0x0200022E RID: 558
		public class Basic : MusicDirector.Music
		{
			// Token: 0x17000165 RID: 357
			// (get) Token: 0x06000C17 RID: 3095 RVA: 0x00032987 File Offset: 0x00030B87
			public override MusicDirector.Priority Priority
			{
				get
				{
					return this.priority;
				}
			}

			// Token: 0x17000166 RID: 358
			// (get) Token: 0x06000C18 RID: 3096 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool Loops
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06000C19 RID: 3097 RVA: 0x0003298F File Offset: 0x00030B8F
			public override SECTR_AudioCue GetCurrentCue()
			{
				return this.cue;
			}

			// Token: 0x06000C1A RID: 3098 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C1B RID: 3099 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x04000AFE RID: 2814
			public SECTR_AudioCue cue;

			// Token: 0x04000AFF RID: 2815
			public MusicDirector.Priority priority;
		}

		// Token: 0x0200022F RID: 559
		public abstract class Zone : MusicDirector.Music
		{
			// Token: 0x17000167 RID: 359
			// (get) Token: 0x06000C1D RID: 3101 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool Loops
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06000C1E RID: 3102 RVA: 0x000329A0 File Offset: 0x00030BA0
			public override SECTR_AudioCue GetCurrentCue()
			{
				int num = Mathf.FloorToInt(SRSingleton<SceneContext>.Instance.TimeDirector.CurrHour());
				if (num >= 6 && num < 18)
				{
					return this.background;
				}
				return this.nightBackground;
			}

			// Token: 0x06000C1F RID: 3103 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return true;
			}

			// Token: 0x06000C20 RID: 3104 RVA: 0x000329D8 File Offset: 0x00030BD8
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return previous.Priority >= MusicDirector.Priority.VALLEY_RACE && previous.Priority < MusicDirector.Priority.HOUSE;
			}

			// Token: 0x04000B00 RID: 2816
			[Tooltip("Background music played during the day.")]
			public SECTR_AudioCue background;

			// Token: 0x04000B01 RID: 2817
			[Tooltip("Background music played during the night.")]
			public SECTR_AudioCue nightBackground;

			// Token: 0x02000230 RID: 560
			[Serializable]
			public class Default : MusicDirector.Music.Zone
			{
				// Token: 0x17000168 RID: 360
				// (get) Token: 0x06000C22 RID: 3106 RVA: 0x0002764E File Offset: 0x0002584E
				public override MusicDirector.Priority Priority
				{
					get
					{
						return MusicDirector.Priority.ZONE_DEFAULT;
					}
				}
			}

			// Token: 0x02000231 RID: 561
			[Serializable]
			public class Special : MusicDirector.Music.Zone
			{
				// Token: 0x17000169 RID: 361
				// (get) Token: 0x06000C24 RID: 3108 RVA: 0x000329FB File Offset: 0x00030BFB
				public override MusicDirector.Priority Priority
				{
					get
					{
						return MusicDirector.Priority.ZONE_SPECIAL;
					}
				}
			}
		}

		// Token: 0x02000232 RID: 562
		[Serializable]
		public class Zone_Wistful : MusicDirector.Music
		{
			// Token: 0x1700016A RID: 362
			// (get) Token: 0x06000C26 RID: 3110 RVA: 0x0002764E File Offset: 0x0002584E
			public override MusicDirector.Priority Priority
			{
				get
				{
					return MusicDirector.Priority.ZONE_DEFAULT;
				}
			}

			// Token: 0x1700016B RID: 363
			// (get) Token: 0x06000C27 RID: 3111 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool Loops
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06000C28 RID: 3112 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return true;
			}

			// Token: 0x06000C29 RID: 3113 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C2A RID: 3114 RVA: 0x00032A00 File Offset: 0x00030C00
			public override SECTR_AudioCue GetCurrentCue()
			{
				double? num = this.endTime;
				double num2 = SRSingleton<SceneContext>.Instance.TimeDirector.WorldTime();
				if (num.GetValueOrDefault() < num2 & num != null)
				{
					RegionMember requiredComponent = SRSingleton<SceneContext>.Instance.Player.GetRequiredComponent<RegionMember>();
					SRSingleton<GameContext>.Instance.MusicDirector.OnRegionsChanged(requiredComponent);
				}
				return this.cue;
			}

			// Token: 0x06000C2B RID: 3115 RVA: 0x00032A60 File Offset: 0x00030C60
			public void ResetEndTime()
			{
				TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
				this.endTime = new double?(timeDirector.GetNextDawnAfterNextDusk());
			}

			// Token: 0x06000C2C RID: 3116 RVA: 0x00032A8C File Offset: 0x00030C8C
			public bool Enabled()
			{
				TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
				double? num = this.endTime;
				double num2 = timeDirector.WorldTime();
				return num.GetValueOrDefault() >= num2 & num != null;
			}

			// Token: 0x06000C2D RID: 3117 RVA: 0x00032AC5 File Offset: 0x00030CC5
			public void Stop()
			{
				this.endTime = null;
			}

			// Token: 0x04000B02 RID: 2818
			[Tooltip("Background music to play during the wistful ranch.")]
			public SECTR_AudioCue cue;

			// Token: 0x04000B03 RID: 2819
			private double? endTime;
		}

		// Token: 0x02000233 RID: 563
		[Serializable]
		public class Zone_Wiggly : MusicDirector.Music
		{
			// Token: 0x1700016C RID: 364
			// (get) Token: 0x06000C2F RID: 3119 RVA: 0x0002764E File Offset: 0x0002584E
			public override MusicDirector.Priority Priority
			{
				get
				{
					return MusicDirector.Priority.ZONE_DEFAULT;
				}
			}

			// Token: 0x1700016D RID: 365
			// (get) Token: 0x06000C30 RID: 3120 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool Loops
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000C31 RID: 3121 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C32 RID: 3122 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C33 RID: 3123 RVA: 0x00032AD4 File Offset: 0x00030CD4
			public override SECTR_AudioCue GetCurrentCue()
			{
				MusicDirector musicDirector = SRSingleton<GameContext>.Instance.MusicDirector;
				if (musicDirector.current.cue == this.cue && !musicDirector.current.instance.Active)
				{
					RegionMember requiredComponent = SRSingleton<SceneContext>.Instance.Player.GetRequiredComponent<RegionMember>();
					musicDirector.OnRegionsChanged(requiredComponent);
				}
				return this.cue;
			}

			// Token: 0x04000B04 RID: 2820
			[Tooltip("Background wiggly music played after the chime changer button is pressed).")]
			public SECTR_AudioCue cue;
		}

		// Token: 0x02000234 RID: 564
		[Serializable]
		public class House : MusicDirector.Music
		{
			// Token: 0x1700016E RID: 366
			// (get) Token: 0x06000C35 RID: 3125 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool Loops
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06000C36 RID: 3126 RVA: 0x00032B34 File Offset: 0x00030D34
			public override SECTR_AudioCue GetCurrentCue()
			{
				int num = Mathf.FloorToInt(SRSingleton<SceneContext>.Instance.TimeDirector.CurrHour());
				if (num >= 6 && num < 12)
				{
					return this.morning;
				}
				if (num >= 12 && num < 18)
				{
					return this.afternoon;
				}
				return this.night;
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x06000C37 RID: 3127 RVA: 0x00032B7D File Offset: 0x00030D7D
			public override MusicDirector.Priority Priority
			{
				get
				{
					return MusicDirector.Priority.HOUSE;
				}
			}

			// Token: 0x06000C38 RID: 3128 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C39 RID: 3129 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x04000B05 RID: 2821
			[Tooltip("Background music played during the morning.")]
			public SECTR_AudioCue morning;

			// Token: 0x04000B06 RID: 2822
			[Tooltip("Background music played during the afternoon.")]
			public SECTR_AudioCue afternoon;

			// Token: 0x04000B07 RID: 2823
			[Tooltip("Background music played during the night.")]
			public SECTR_AudioCue night;
		}

		// Token: 0x02000235 RID: 565
		[Serializable]
		public class Credits : MusicDirector.Music
		{
			// Token: 0x17000170 RID: 368
			// (get) Token: 0x06000C3B RID: 3131 RVA: 0x00032B84 File Offset: 0x00030D84
			public override MusicDirector.Priority Priority
			{
				get
				{
					return MusicDirector.Priority.CREDITS;
				}
			}

			// Token: 0x17000171 RID: 369
			// (get) Token: 0x06000C3C RID: 3132 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool Loops
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000C3D RID: 3133 RVA: 0x00032B8B File Offset: 0x00030D8B
			public override SECTR_AudioCue GetCurrentCue()
			{
				return this.cue;
			}

			// Token: 0x06000C3E RID: 3134 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetWaitForFadeOut(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x06000C3F RID: 3135 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool GetStartMidway(MusicDirector.Music previous)
			{
				return false;
			}

			// Token: 0x04000B08 RID: 2824
			[Tooltip("Background music to play once during credits.")]
			public SECTR_AudioCue cue;
		}
	}

	// Token: 0x02000236 RID: 566
	public enum Priority
	{
		// Token: 0x04000B0A RID: 2826
		DEFAULT,
		// Token: 0x04000B0B RID: 2827
		ZONE_DEFAULT = 10,
		// Token: 0x04000B0C RID: 2828
		ZONE_SPECIAL,
		// Token: 0x04000B0D RID: 2829
		VALLEY_RACE = 100,
		// Token: 0x04000B0E RID: 2830
		TARR,
		// Token: 0x04000B0F RID: 2831
		RUSH_MODE_WARNING,
		// Token: 0x04000B10 RID: 2832
		FIRESTORM,
		// Token: 0x04000B11 RID: 2833
		SLIME_HOOP,
		// Token: 0x04000B12 RID: 2834
		PARTY_GORDO,
		// Token: 0x04000B13 RID: 2835
		HOUSE = 900,
		// Token: 0x04000B14 RID: 2836
		CREDITS = 1000
	}

	// Token: 0x02000237 RID: 567
	private class CurrentMetadata
	{
		// Token: 0x04000B15 RID: 2837
		public MusicDirector.Music music;

		// Token: 0x04000B16 RID: 2838
		public SECTR_AudioCue cue;

		// Token: 0x04000B17 RID: 2839
		public SECTR_AudioCueInstance instance;

		// Token: 0x04000B18 RID: 2840
		public MusicDirector.Priority priority;

		// Token: 0x04000B19 RID: 2841
		public float startTime;

		// Token: 0x04000B1A RID: 2842
		public float offset;
	}

	// Token: 0x02000238 RID: 568
	private class PriorityQueue
	{
		// Token: 0x06000C42 RID: 3138 RVA: 0x00032B94 File Offset: 0x00030D94
		public void Enqueue(MusicDirector.Music music)
		{
			for (int i = 0; i < this.queue.Count; i++)
			{
				if (this.queue[i].Priority == music.Priority)
				{
					this.queue[i] = music;
					return;
				}
				if (music.Priority > this.queue[i].Priority)
				{
					this.queue.Insert(i, music);
					return;
				}
			}
			this.queue.Add(music);
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x00032C14 File Offset: 0x00030E14
		public void Dequeue(MusicDirector.Priority priority)
		{
			for (int i = 0; i < this.queue.Count; i++)
			{
				if (this.queue[i].Priority == priority)
				{
					this.queue.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x00032C58 File Offset: 0x00030E58
		public MusicDirector.Music Peek()
		{
			if (this.queue.Count == 0)
			{
				throw new InvalidOperationException();
			}
			return this.queue[0];
		}

		// Token: 0x04000B1B RID: 2843
		private List<MusicDirector.Music> queue = new List<MusicDirector.Music>();
	}
}
