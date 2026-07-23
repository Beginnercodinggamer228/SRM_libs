using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000479 RID: 1145
public class SlimeEmotions : RegisteredActorBehaviour, RegistryUpdateable, ActorModel.Participant
{
	// Token: 0x060017B5 RID: 6069 RVA: 0x0005C100 File Offset: 0x0005A300
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.member = base.GetComponent<RegionMember>();
		this.lastUpdateTime = this.timeDir.HoursFromNowOrStart(0f);
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.ModDirector.RegisterModsListener(new ModDirector.ModsListener(this.OnModsChanged));
		}
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x0005C167 File Offset: 0x0005A367
	public override void Start()
	{
		base.Start();
		if (!Identifiable.IsTarr(Identifiable.GetId(base.gameObject)) && this.member.IsInRegion(RegionRegistry.RegionSetId.SLIMULATIONS))
		{
			this.SetEmotionEnabled(SlimeEmotions.Emotion.HUNGER, false);
		}
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x0005C197 File Offset: 0x0005A397
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.musicBoxes.Clear();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.ModDirector.UnregisterModsListener(new ModDirector.ModsListener(this.OnModsChanged));
		}
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x0005C1D2 File Offset: 0x0005A3D2
	public void InitModel(ActorModel model)
	{
		((SlimeModel)model).MaybeSetInitEmotions(this.initAgitation, this.initFear, this.initHunger);
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x0005C1F1 File Offset: 0x0005A3F1
	public void SetModel(ActorModel model)
	{
		this.model = (SlimeModel)model;
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x0005C1FF File Offset: 0x0005A3FF
	private void OnModsChanged()
	{
		this.modHungerFactor = SRSingleton<SceneContext>.Instance.ModDirector.SlimeHungerFactor();
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x0005C216 File Offset: 0x0005A416
	public void RegistryUpdate()
	{
		this.UpdateToTime(this.timeDir.WorldTime());
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x0005C22C File Offset: 0x0005A42C
	public void UpdateToTime(double worldTime)
	{
		float num = (float)((worldTime - this.lastUpdateTime) * 0.00027777778450399637);
		if (num <= 0f)
		{
			return;
		}
		foreach (SlimeEmotions.EmotionState emotionState in this.model.allEmotions)
		{
			if (emotionState.enabled)
			{
				float num2 = emotionState.recoveryPerGameHour + this.GetBaseRecoveryAdjustment(emotionState.emotion);
				float recoveryFactor = this.GetRecoveryFactor(emotionState.emotion);
				float num3;
				if (num2 < 0f)
				{
					num3 = num2 / recoveryFactor;
				}
				else
				{
					num3 = num2 * recoveryFactor;
				}
				if (emotionState.currVal > emotionState.defVal)
				{
					emotionState.currVal = Mathf.Max(emotionState.defVal, emotionState.currVal - num3 * num);
				}
				else if (emotionState.currVal < emotionState.defVal)
				{
					emotionState.currVal = Mathf.Min(emotionState.defVal, emotionState.currVal + num3 * num);
				}
				else if (emotionState.defVal == 0f && num3 < 0f)
				{
					emotionState.currVal = Mathf.Max(emotionState.defVal, emotionState.currVal - num3 * num);
				}
				else if (emotionState.defVal == 1f && num3 < 0f)
				{
					emotionState.currVal = Mathf.Min(emotionState.defVal, emotionState.currVal + num3 * num);
				}
				emotionState.currVal = Mathf.Clamp01(emotionState.currVal);
			}
		}
		this.lastUpdateTime = worldTime;
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x0005C39C File Offset: 0x0005A59C
	private float GetBaseRecoveryAdjustment(SlimeEmotions.Emotion emotion)
	{
		if (emotion == SlimeEmotions.Emotion.AGITATION)
		{
			float num = 0f;
			if (this.GetCurr(SlimeEmotions.Emotion.HUNGER) >= 0.99f || this.GetCurr(SlimeEmotions.Emotion.FEAR) >= 0.99f)
			{
				num -= 0.416667f;
			}
			if (this.pollenSourceCount > 0)
			{
				num -= this.POLLEN_SOURCE_AGITATION_PER_HOUR;
			}
			return num;
		}
		return 0f;
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x0005C3F0 File Offset: 0x0005A5F0
	private float GetRecoveryFactor(SlimeEmotions.Emotion emotion)
	{
		if (emotion == SlimeEmotions.Emotion.HUNGER)
		{
			return this.modHungerFactor;
		}
		if (emotion == SlimeEmotions.Emotion.AGITATION)
		{
			float num = 1f;
			if (this.musicBoxes.Count > 0)
			{
				num += 1f;
			}
			if (this.nearbyFavoriteToyCount > 0)
			{
				num += this.FAVORITE_TOY_FACTOR;
			}
			else if (this.nearbyToyCount > 0)
			{
				num += this.NON_FAVORITE_TOY_FACTOR;
			}
			return num;
		}
		return 1f;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x0005C454 File Offset: 0x0005A654
	private SlimeEmotions.EmotionState GetEmotion(SlimeEmotions.Emotion emotion)
	{
		switch (emotion)
		{
		case SlimeEmotions.Emotion.HUNGER:
			return this.model.emotionHunger;
		case SlimeEmotions.Emotion.AGITATION:
			return this.model.emotionAgitation;
		case SlimeEmotions.Emotion.FEAR:
			return this.model.emotionFear;
		}
		return null;
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0005C494 File Offset: 0x0005A694
	public bool Adjust(SlimeEmotions.Emotion emotion, float adjust)
	{
		SlimeEmotions.EmotionState emotion2 = this.GetEmotion(emotion);
		if (emotion2 != null && emotion2.enabled)
		{
			emotion2.currVal = Mathf.Clamp(emotion2.currVal + adjust, 0f, 1f);
			return true;
		}
		return false;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x0005C4D4 File Offset: 0x0005A6D4
	public void SetAll(SlimeEmotions other)
	{
		foreach (object obj in Enum.GetValues(typeof(SlimeEmotions.Emotion)))
		{
			SlimeEmotions.Emotion emotion = (SlimeEmotions.Emotion)obj;
			SlimeEmotions.EmotionState emotion2 = this.GetEmotion(emotion);
			if (emotion2 != null && emotion2.enabled)
			{
				emotion2.currVal = other.GetEmotion(emotion).currVal;
			}
		}
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x0005C554 File Offset: 0x0005A754
	public void SetAll(Dictionary<SlimeEmotions.Emotion, float> other)
	{
		foreach (KeyValuePair<SlimeEmotions.Emotion, float> keyValuePair in other)
		{
			SlimeEmotions.EmotionState emotion = this.GetEmotion(keyValuePair.Key);
			if (emotion != null && emotion.enabled)
			{
				emotion.currVal = keyValuePair.Value;
			}
		}
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x0005C5C4 File Offset: 0x0005A7C4
	public IEnumerable<SlimeEmotions.EmotionState> GetAll()
	{
		return this.model.allEmotions;
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x0005C5D1 File Offset: 0x0005A7D1
	public float GetCurr(SlimeEmotions.Emotion emotion)
	{
		if (emotion != SlimeEmotions.Emotion.NONE)
		{
			return this.GetEmotion(emotion).currVal;
		}
		return 1f;
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x0005C5EC File Offset: 0x0005A7EC
	public float GetMax()
	{
		float num = 0f;
		foreach (SlimeEmotions.EmotionState emotionState in this.model.allEmotions)
		{
			num = Mathf.Max(num, emotionState.currVal);
		}
		return num;
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x0005C62B File Offset: 0x0005A82B
	public void AddMusicBox(MusicBoxRegion box)
	{
		this.musicBoxes.Add(box);
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x0005C639 File Offset: 0x0005A839
	public void RemoveMusicBox(MusicBoxRegion box)
	{
		this.musicBoxes.Remove(box);
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x0005C648 File Offset: 0x0005A848
	public void AddNearbyToy(bool isFavorite)
	{
		if (isFavorite)
		{
			this.nearbyFavoriteToyCount++;
			return;
		}
		this.nearbyToyCount++;
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x0005C66A File Offset: 0x0005A86A
	public void RemoveNearbyToy(bool isFavorite)
	{
		if (isFavorite)
		{
			this.nearbyFavoriteToyCount = Math.Max(0, this.nearbyFavoriteToyCount - 1);
			return;
		}
		this.nearbyToyCount = Math.Max(0, this.nearbyToyCount - 1);
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x0005C698 File Offset: 0x0005A898
	public void AddPollenSource()
	{
		this.pollenSourceCount++;
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x0005C6A8 File Offset: 0x0005A8A8
	public void RemovePollenSource()
	{
		this.pollenSourceCount = Math.Max(0, this.pollenSourceCount - 1);
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x0005C6C0 File Offset: 0x0005A8C0
	public void SetEmotionEnabled(SlimeEmotions.Emotion emotion, bool enabled)
	{
		SlimeEmotions.EmotionState emotion2 = this.GetEmotion(emotion);
		if (emotion2 != null)
		{
			emotion2.SetEnabled(enabled);
		}
	}

	// Token: 0x040016DA RID: 5850
	public const float HUNGRY_CUTOFF = 0.666f;

	// Token: 0x040016DB RID: 5851
	public const float STARVING_CUTOFF = 0.99f;

	// Token: 0x040016DC RID: 5852
	public const float ANGRY_CUTOFF = 0.9f;

	// Token: 0x040016DD RID: 5853
	public const float TERRIFIED_CUTOFF = 0.99f;

	// Token: 0x040016DE RID: 5854
	public SlimeEmotions.EmotionState initHunger = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.HUNGER, 0.5f, 1f, 1f, 0.5f);

	// Token: 0x040016DF RID: 5855
	public SlimeEmotions.EmotionState initAgitation = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.AGITATION, 0f, 0f, 1f, 0.333f);

	// Token: 0x040016E0 RID: 5856
	public SlimeEmotions.EmotionState initFear = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.FEAR, 0f, 0f, 1f, 5f);

	// Token: 0x040016E1 RID: 5857
	private double lastUpdateTime;

	// Token: 0x040016E2 RID: 5858
	private TimeDirector timeDir;

	// Token: 0x040016E3 RID: 5859
	private RegionMember member;

	// Token: 0x040016E4 RID: 5860
	private List<MusicBoxRegion> musicBoxes = new List<MusicBoxRegion>();

	// Token: 0x040016E5 RID: 5861
	private float modHungerFactor = 1f;

	// Token: 0x040016E6 RID: 5862
	private const float STARVING_AGITATION_PER_HOUR = 0.416667f;

	// Token: 0x040016E7 RID: 5863
	private float FAVORITE_TOY_FACTOR = 0.5f;

	// Token: 0x040016E8 RID: 5864
	private float NON_FAVORITE_TOY_FACTOR = 0.25f;

	// Token: 0x040016E9 RID: 5865
	private float POLLEN_SOURCE_AGITATION_PER_HOUR = 0.416667f;

	// Token: 0x040016EA RID: 5866
	private int nearbyFavoriteToyCount;

	// Token: 0x040016EB RID: 5867
	private int nearbyToyCount;

	// Token: 0x040016EC RID: 5868
	private int pollenSourceCount;

	// Token: 0x040016ED RID: 5869
	private SlimeModel model;

	// Token: 0x0200047A RID: 1146
	public enum Emotion
	{
		// Token: 0x040016EF RID: 5871
		HUNGER,
		// Token: 0x040016F0 RID: 5872
		AGITATION,
		// Token: 0x040016F1 RID: 5873
		FEAR,
		// Token: 0x040016F2 RID: 5874
		NONE
	}

	// Token: 0x0200047B RID: 1147
	[Serializable]
	public class EmotionState : ISerializable
	{
		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060017CE RID: 6094 RVA: 0x0005C78A File Offset: 0x0005A98A
		// (set) Token: 0x060017CF RID: 6095 RVA: 0x0005C792 File Offset: 0x0005A992
		public bool enabled { get; private set; }

		// Token: 0x060017D0 RID: 6096 RVA: 0x0005C79B File Offset: 0x0005A99B
		public EmotionState(SlimeEmotions.Emotion emotion, float currVal, float defVal, float sensitivity, float recoveryPerGameHour)
		{
			this.emotion = emotion;
			this.currVal = currVal;
			this.defVal = defVal;
			this.sensitivity = sensitivity;
			this.recoveryPerGameHour = recoveryPerGameHour;
			this.enabled = true;
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x0005C7CF File Offset: 0x0005A9CF
		public void SetEnabled(bool enabled)
		{
			if (this.enabled != enabled)
			{
				this.enabled = enabled;
				this.currVal = (this.enabled ? this.defVal : 0f);
			}
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x0005C7FC File Offset: 0x0005A9FC
		protected EmotionState(SerializationInfo info, StreamingContext context)
		{
			this.emotion = (SlimeEmotions.Emotion)info.GetInt32("emotion");
			this.currVal = info.GetSingle("currVal");
			this.defVal = info.GetSingle("defVal");
			this.sensitivity = info.GetSingle("sensitivity");
			this.recoveryPerGameHour = info.GetSingle("recoveryPerGameHour");
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x0005C864 File Offset: 0x0005AA64
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("currVal", this.currVal);
			info.AddValue("defVal", this.defVal);
			info.AddValue("sensitivity", this.sensitivity);
			info.AddValue("recoveryPerGameHour", this.recoveryPerGameHour);
			info.AddValue("emotion", (int)this.emotion);
		}

		// Token: 0x040016F3 RID: 5875
		public SlimeEmotions.Emotion emotion;

		// Token: 0x040016F4 RID: 5876
		public float currVal;

		// Token: 0x040016F5 RID: 5877
		public float defVal;

		// Token: 0x040016F6 RID: 5878
		public float sensitivity;

		// Token: 0x040016F7 RID: 5879
		public float recoveryPerGameHour;
	}
}
