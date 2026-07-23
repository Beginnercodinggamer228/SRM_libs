using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using Noise;
using UnityEngine;

// Token: 0x020006EF RID: 1775
public class EconomyDirector : SRBehaviour, WorldModel.Participant
{
	// Token: 0x06002509 RID: 9481 RVA: 0x0008E4E4 File Offset: 0x0008C6E4
	public void InitForLevel()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.nextUpdateTime = 0.0;
		if (this.saturationRecovery < 0f || this.saturationRecovery > 1f)
		{
			throw new ArgumentException("Saturation Recovery must be [0-1]");
		}
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x0008E548 File Offset: 0x0008C748
	public void InitModel(WorldModel model)
	{
		model.econSeed = new Randoms().GetFloat(1000000f);
		foreach (EconomyDirector.ValueMap valueMap in this.baseValueMap)
		{
			this.currValueMap[valueMap.accept.id] = new EconomyDirector.CurrValueEntry(valueMap.value, valueMap.value, valueMap.value, valueMap.fullSaturation);
			model.marketSaturation[valueMap.accept.id] = valueMap.fullSaturation * 0.5f;
		}
		this.ResetPrices(model, 0);
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x0008E5E0 File Offset: 0x0008C7E0
	public void SetModel(WorldModel model)
	{
		this.worldModel = model;
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x0008E5EC File Offset: 0x0008C7EC
	public void ResetPrices(WorldModel worldModel, int day)
	{
		foreach (KeyValuePair<Identifiable.Id, EconomyDirector.CurrValueEntry> keyValuePair in this.currValueMap)
		{
			if (this.nextUpdateTime > 0.0)
			{
				Dictionary<Identifiable.Id, float> marketSaturation = worldModel.marketSaturation;
				Identifiable.Id key = keyValuePair.Key;
				marketSaturation[key] *= 1f - this.saturationRecovery;
			}
			float targetValue = this.GetTargetValue(worldModel, keyValuePair.Key, keyValuePair.Value.baseValue, keyValuePair.Value.fullSaturation, (float)day);
			keyValuePair.Value.prevValue = keyValuePair.Value.currValue;
			keyValuePair.Value.currValue = targetValue;
		}
		if (this.didUpdateDelegate != null)
		{
			this.didUpdateDelegate();
		}
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x0008E6E0 File Offset: 0x0008C8E0
	public void Update()
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic && this.timeDir.HasReached(this.nextUpdateTime))
		{
			this.ResetPrices(this.worldModel, this.timeDir.CurrDay());
			this.nextUpdateTime = this.timeDir.GetNextHour(0f);
		}
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x0008E743 File Offset: 0x0008C943
	public bool IsMarketShutdown()
	{
		return SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic && this.timeDir.CurrHour() * 60f < this.dailyShutdownMins;
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x0008E778 File Offset: 0x0008C978
	public int? GetCurrValue(Identifiable.Id id)
	{
		if (this.currValueMap.ContainsKey(id))
		{
			return new int?(Mathf.RoundToInt(this.currValueMap[id].currValue));
		}
		return null;
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x0008E7B8 File Offset: 0x0008C9B8
	public int? GetChangeInValue(Identifiable.Id id)
	{
		if (this.currValueMap.ContainsKey(id))
		{
			return new int?(Mathf.RoundToInt(this.currValueMap[id].currValue) - Mathf.RoundToInt(this.currValueMap[id].prevValue));
		}
		return null;
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x0008E810 File Offset: 0x0008CA10
	public void RegisterSold(Identifiable.Id id, int count)
	{
		Dictionary<Identifiable.Id, float> marketSaturation = this.worldModel.marketSaturation;
		marketSaturation[id] += (float)count;
		if (this.onRegisterSold != null)
		{
			this.onRegisterSold(id);
		}
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x0008E850 File Offset: 0x0008CA50
	private float GetTargetValue(WorldModel worldModel, Identifiable.Id id, float baseValue, float fullSaturation, float day)
	{
		if (!SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic)
		{
			return baseValue * 1.5f;
		}
		float num = 1f + Mathf.Clamp01((fullSaturation - worldModel.marketSaturation[id]) / fullSaturation);
		float num2 = Noise.PerlinNoise((double)day, worldModel.econSeed, -10000f, 10f, 0.6f, 1f) + 0.7f;
		float num3 = Noise.PerlinNoise((double)day, worldModel.econSeed, (float)(id.GetHashCode() * 10000), 10f, 0.6f, 1f) + 0.7f;
		return baseValue * num * num2 * num3;
	}

	// Token: 0x040023EB RID: 9195
	public EconomyDirector.ValueMap[] baseValueMap;

	// Token: 0x040023EC RID: 9196
	public float saturationSensitivity = 0.05f;

	// Token: 0x040023ED RID: 9197
	public float saturationRecovery = 0.25f;

	// Token: 0x040023EE RID: 9198
	public float dailyShutdownMins = 5f;

	// Token: 0x040023EF RID: 9199
	private double nextUpdateTime;

	// Token: 0x040023F0 RID: 9200
	private TimeDirector timeDir;

	// Token: 0x040023F1 RID: 9201
	public EconomyDirector.DidUpdate didUpdateDelegate;

	// Token: 0x040023F2 RID: 9202
	public EconomyDirector.OnRegisterSold onRegisterSold;

	// Token: 0x040023F3 RID: 9203
	private WorldModel worldModel;

	// Token: 0x040023F4 RID: 9204
	private Dictionary<Identifiable.Id, EconomyDirector.CurrValueEntry> currValueMap = new Dictionary<Identifiable.Id, EconomyDirector.CurrValueEntry>(Identifiable.idComparer);

	// Token: 0x020006F0 RID: 1776
	[Serializable]
	public class ValueMap
	{
		// Token: 0x040023F5 RID: 9205
		public Identifiable accept;

		// Token: 0x040023F6 RID: 9206
		public float value;

		// Token: 0x040023F7 RID: 9207
		public float fullSaturation;
	}

	// Token: 0x020006F1 RID: 1777
	// (Invoke) Token: 0x06002516 RID: 9494
	public delegate void DidUpdate();

	// Token: 0x020006F2 RID: 1778
	// (Invoke) Token: 0x0600251A RID: 9498
	public delegate void OnRegisterSold(Identifiable.Id id);

	// Token: 0x020006F3 RID: 1779
	private class CurrValueEntry
	{
		// Token: 0x0600251D RID: 9501 RVA: 0x0008E93A File Offset: 0x0008CB3A
		public CurrValueEntry(float baseValue, float currValue, float prevValue, float fullSaturation)
		{
			this.baseValue = baseValue;
			this.currValue = currValue;
			this.prevValue = prevValue;
			this.fullSaturation = fullSaturation;
		}

		// Token: 0x040023F8 RID: 9208
		public readonly float baseValue;

		// Token: 0x040023F9 RID: 9209
		public float currValue;

		// Token: 0x040023FA RID: 9210
		public float prevValue;

		// Token: 0x040023FB RID: 9211
		public float fullSaturation;
	}
}
