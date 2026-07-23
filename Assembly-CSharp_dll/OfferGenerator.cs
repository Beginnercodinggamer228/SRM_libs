using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class OfferGenerator
{
	// Token: 0x0600110C RID: 4364 RVA: 0x000441C8 File Offset: 0x000423C8
	public OfferGenerator(string rancherId, int numBlurbs, Identifiable.Id[] requests, Identifiable.Id[] rewards, Identifiable.Id[] rareRewards)
	{
		this.rancherId = rancherId;
		this.numBlurbs = numBlurbs;
		this.requests = new OfferGenerator.ItemGenerator(requests);
		this.rewards = new OfferGenerator.ItemGenerator(rewards);
		this.rareRewards = new OfferGenerator.ItemGenerator(rareRewards);
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0004424C File Offset: 0x0004244C
	public ExchangeDirector.Offer Generate(ExchangeDirector exchangeDir, List<Identifiable.Id> whitelist, double expireTime, double earlyExchangeTime, int retries, bool isFirstOffer, bool isGoldPlortOffer)
	{
		for (int i = 0; i < retries; i++)
		{
			ExchangeDirector.Offer offer = this.GenerateOneOffer(exchangeDir, whitelist, expireTime, earlyExchangeTime, isFirstOffer, isGoldPlortOffer);
			if (offer != null)
			{
				return offer;
			}
		}
		return null;
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0004427C File Offset: 0x0004247C
	public List<ExchangeDirector.RequestedItemEntry> GenerateRequestList(ExchangeDirector exchangeDir, List<Identifiable.Id> whitelist)
	{
		return this.GenerateRequestList(exchangeDir, whitelist, Randoms.SHARED.GetInRange(50, 100), new List<Identifiable.Id>());
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x00044299 File Offset: 0x00042499
	public int GetRandomBlurb()
	{
		return Randoms.SHARED.GetInRange(1, this.numBlurbs + 1);
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x000442B0 File Offset: 0x000424B0
	private List<ExchangeDirector.RequestedItemEntry> GenerateRequestList(ExchangeDirector exchangeDir, List<Identifiable.Id> whitelist, int requestValue, List<Identifiable.Id> used)
	{
		List<ExchangeDirector.RequestedItemEntry> list = new List<ExchangeDirector.RequestedItemEntry>();
		int inRange = Randoms.SHARED.GetInRange(2, 4);
		float[] array = new float[inRange];
		float num = 0f;
		for (int i = 0; i < inRange; i++)
		{
			array[i] = Randoms.SHARED.GetInRange(0.5f, 1.5f);
			num += array[i];
		}
		for (int j = 0; j < inRange; j++)
		{
			int value = Mathf.RoundToInt((float)requestValue * array[j] / num);
			ExchangeDirector.RequestedItemEntry requestedItemEntry = this.requests.Generate(exchangeDir, used, whitelist, value, true) as ExchangeDirector.RequestedItemEntry;
			if (requestedItemEntry == null)
			{
				return null;
			}
			list.Add(requestedItemEntry);
			used.Add(requestedItemEntry.id);
		}
		return list;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00044364 File Offset: 0x00042564
	private ExchangeDirector.Offer GenerateOneOffer(ExchangeDirector exchangeDir, List<Identifiable.Id> whitelist, double expireTime, double earlyExchangeTime, bool isFirstOffer, bool isGoldPlortOffer)
	{
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		bool flag = !isFirstOffer && Randoms.SHARED.GetProbability(0.125f);
		bool flag2 = flag && this.rareRewards.ContainsAny(whitelist) && Randoms.SHARED.GetProbability(0.5f);
		int inRange = Randoms.SHARED.GetInRange(50, 100);
		int num = Mathf.RoundToInt((float)inRange * (flag ? 2.5f : 1.75f));
		List<ExchangeDirector.RequestedItemEntry> list2 = this.GenerateRequestList(exchangeDir, whitelist, inRange, list);
		if (list2 == null)
		{
			return null;
		}
		List<ExchangeDirector.ItemEntry> list3 = new List<ExchangeDirector.ItemEntry>();
		if (isGoldPlortOffer)
		{
			list3.Add(new ExchangeDirector.ItemEntry(Identifiable.Id.GOLD_PLORT, 3));
			list.Add(Identifiable.Id.GOLD_PLORT);
		}
		else
		{
			int num2 = flag2 ? 1 : Randoms.SHARED.GetInRange(2, 4);
			float[] array = new float[num2];
			float num3 = 0f;
			for (int i = 0; i < num2; i++)
			{
				array[i] = Randoms.SHARED.GetInRange(0.5f, 1.5f);
				num3 += array[i];
			}
			for (int j = 0; j < num2; j++)
			{
				int value = flag2 ? 100 : Mathf.RoundToInt((float)num * array[j] / num3);
				ExchangeDirector.ItemEntry itemEntry = (flag2 ? this.rareRewards : this.rewards).Generate(exchangeDir, list, whitelist, value, false);
				if (itemEntry == null)
				{
					return null;
				}
				list3.Add(itemEntry);
				list.Add(itemEntry.id);
			}
			ExchangeDirector.ItemEntry item = new ExchangeDirector.ItemEntry(flag ? ExchangeDirector.NonIdentReward.NEWBUCKS_HUGE : Randoms.SHARED.Pick<ExchangeDirector.NonIdentReward>(this.NORM_CASH_REWARDS, ExchangeDirector.NonIdentReward.NEWBUCKS_SMALL));
			list3.Add(item);
		}
		int num4 = isFirstOffer ? 1 : this.GetRandomBlurb();
		return new ExchangeDirector.Offer((flag || isGoldPlortOffer) ? ("m.bonusoffer." + this.rancherId) : string.Concat(new object[]
		{
			"m.offer_",
			num4,
			".",
			this.rancherId
		}), this.rancherId, expireTime, earlyExchangeTime, list2, list3);
	}

	// Token: 0x04000FF8 RID: 4088
	public string rancherId;

	// Token: 0x04000FF9 RID: 4089
	private int numBlurbs;

	// Token: 0x04000FFA RID: 4090
	private OfferGenerator.ItemGenerator requests;

	// Token: 0x04000FFB RID: 4091
	private OfferGenerator.ItemGenerator rewards;

	// Token: 0x04000FFC RID: 4092
	private OfferGenerator.ItemGenerator rareRewards;

	// Token: 0x04000FFD RID: 4093
	private const int MIN_OFFER_VAL = 50;

	// Token: 0x04000FFE RID: 4094
	private const int MAX_OFFER_VAL = 100;

	// Token: 0x04000FFF RID: 4095
	private const float BONUS_PROB = 0.125f;

	// Token: 0x04001000 RID: 4096
	private const float RARE_PROB = 0.5f;

	// Token: 0x04001001 RID: 4097
	private const float REWARD_VAL_MULT = 1.75f;

	// Token: 0x04001002 RID: 4098
	private const float REWARD_BONUS_MULT = 2.5f;

	// Token: 0x04001003 RID: 4099
	private const ExchangeDirector.NonIdentReward BONUS_CASH_REWARD = ExchangeDirector.NonIdentReward.NEWBUCKS_HUGE;

	// Token: 0x04001004 RID: 4100
	private readonly Dictionary<ExchangeDirector.NonIdentReward, float> NORM_CASH_REWARDS = new Dictionary<ExchangeDirector.NonIdentReward, float>
	{
		{
			ExchangeDirector.NonIdentReward.NEWBUCKS_SMALL,
			3f
		},
		{
			ExchangeDirector.NonIdentReward.NEWBUCKS_MEDIUM,
			2f
		},
		{
			ExchangeDirector.NonIdentReward.NEWBUCKS_LARGE,
			1f
		}
	};

	// Token: 0x02000328 RID: 808
	private class ItemGenerator
	{
		// Token: 0x06001112 RID: 4370 RVA: 0x00044569 File Offset: 0x00042769
		public ItemGenerator(ICollection<Identifiable.Id> ids)
		{
			this.ids = ids;
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00044578 File Offset: 0x00042778
		public ExchangeDirector.ItemEntry Generate(ExchangeDirector exchangeDir, List<Identifiable.Id> disallowed, List<Identifiable.Id> whitelist, int value, bool isRequest)
		{
			List<Identifiable.Id> list = new List<Identifiable.Id>(this.ids);
			list.RemoveAll((Identifiable.Id id) => disallowed.Contains(id) || !whitelist.Contains(id));
			if (list.Count <= 0)
			{
				return null;
			}
			Identifiable.Id id2 = Randoms.SHARED.Pick<Identifiable.Id>(list, Identifiable.Id.NONE);
			int countForValue = exchangeDir.GetCountForValue(id2, value);
			if (countForValue == 0)
			{
				return null;
			}
			if (isRequest)
			{
				return new ExchangeDirector.RequestedItemEntry(id2, countForValue, 0);
			}
			return new ExchangeDirector.ItemEntry(id2, countForValue);
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x000445F4 File Offset: 0x000427F4
		public bool ContainsAny(List<Identifiable.Id> whitelist)
		{
			foreach (Identifiable.Id item in whitelist)
			{
				if (this.ids.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001005 RID: 4101
		public ICollection<Identifiable.Id> ids;
	}
}
