using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
public class MarketUI : MonoBehaviour
{
	// Token: 0x06001EC3 RID: 7875 RVA: 0x00074B5C File Offset: 0x00072D5C
	public void Awake()
	{
		this.amountMap = new Dictionary<MarketUI.PlortEntry, GameObject>();
		this.econDir = SRSingleton<SceneContext>.Instance.EconomyDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x00074B9C File Offset: 0x00072D9C
	public void Start()
	{
		int i = 0;
		int num = 0;
		PlayerState.GameMode currGameMode = SRSingleton<SceneContext>.Instance.GameModel.currGameMode;
		foreach (MarketUI.PlortEntry plortEntry in this.plorts)
		{
			if (plortEntry.id != Identifiable.Id.SABER_PLORT || currGameMode != PlayerState.GameMode.TIME_LIMIT_V2)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.priceEntryPrefab);
				gameObject.GetComponent<PriceEntry>().itemIcon.sprite = this.lookupDir.GetIcon(plortEntry.id);
				this.amountMap[plortEntry] = gameObject;
				gameObject.transform.SetParent(this.pricesPanels[i].panel.transform, false);
				num++;
				if (num >= this.pricesPanels[i].entryCount)
				{
					i++;
					num = 0;
				}
			}
		}
		while (i < this.pricesPanels.Length)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.priceEntryEmptyPrefab).transform.SetParent(this.pricesPanels[i].panel.transform, false);
			num++;
			if (num >= this.pricesPanels[i].entryCount)
			{
				i++;
				num = 0;
			}
		}
		this.EconUpdate();
		EconomyDirector economyDirector = this.econDir;
		economyDirector.didUpdateDelegate = (EconomyDirector.DidUpdate)Delegate.Combine(economyDirector.didUpdateDelegate, new EconomyDirector.DidUpdate(this.EconUpdate));
		EconomyDirector economyDirector2 = this.econDir;
		economyDirector2.onRegisterSold = (EconomyDirector.OnRegisterSold)Delegate.Combine(economyDirector2.onRegisterSold, new EconomyDirector.OnRegisterSold(this.PlortCountUpdate));
		ProgressDirector progressDirector = this.progressDir;
		progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Combine(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.EconUpdate));
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x00074D38 File Offset: 0x00072F38
	public void OnDestroy()
	{
		EconomyDirector economyDirector = this.econDir;
		economyDirector.didUpdateDelegate = (EconomyDirector.DidUpdate)Delegate.Remove(economyDirector.didUpdateDelegate, new EconomyDirector.DidUpdate(this.EconUpdate));
		EconomyDirector economyDirector2 = this.econDir;
		economyDirector2.onRegisterSold = (EconomyDirector.OnRegisterSold)Delegate.Remove(economyDirector2.onRegisterSold, new EconomyDirector.OnRegisterSold(this.PlortCountUpdate));
		ProgressDirector progressDirector = this.progressDir;
		progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Remove(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.EconUpdate));
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x00074DBC File Offset: 0x00072FBC
	private void EconUpdate()
	{
		foreach (KeyValuePair<MarketUI.PlortEntry, GameObject> keyValuePair in this.amountMap)
		{
			PriceEntry component = keyValuePair.Value.GetComponent<PriceEntry>();
			component.amountText.text = this.econDir.GetCurrValue(keyValuePair.Key.id).Value.ToString();
			this.PlortCountUpdate(keyValuePair.Key, component);
		}
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x00074E58 File Offset: 0x00073058
	public void Update()
	{
		bool flag = this.econDir.IsMarketShutdown();
		this.pricesPanelGroup.SetActive(!flag);
		this.shutdownPanel.SetActive(flag);
		GameObject[] array = this.toShutdown;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x00074EB0 File Offset: 0x000730B0
	private void PlortCountUpdate(Identifiable.Id id)
	{
		foreach (KeyValuePair<MarketUI.PlortEntry, GameObject> keyValuePair in this.amountMap)
		{
			if (keyValuePair.Key.id == id)
			{
				this.PlortCountUpdate(keyValuePair.Key, keyValuePair.Value.GetComponent<PriceEntry>());
				break;
			}
		}
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x00074F28 File Offset: 0x00073128
	private void PlortCountUpdate(MarketUI.PlortEntry plort, PriceEntry price)
	{
		int num = 0;
		SRSingleton<SceneContext>.Instance.AchievementsDirector.GetGameIdDictStat(AchievementsDirector.GameIdDictStat.PLORT_TYPES_SOLD).TryGetValue(plort.id, out num);
		price.bonusFill.minValue = 0f;
		price.bonusFill.currValue = (float)num;
		price.bonusFill.maxValue = 25f;
		price.bonusFill.enabled = (SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2);
		int change = this.econDir.GetChangeInValue(plort.id) ?? 0;
		price.changeIcon.sprite = this.GetChangeIcon(plort.id, change, num);
		price.changeIcon.enabled = (price.changeIcon.sprite != null);
		price.changeAmountText.text = this.GetChangeText(plort.id, change);
		float a = this.IsPlortUnlocked(plort, num) ? 1f : 0.5f;
		price.amountText.color = this.AdjustAlpha(price.amountText.color, a);
		price.changeAmountText.color = this.AdjustAlpha(price.changeAmountText.color, a);
		price.changeIcon.color = this.AdjustAlpha(price.changeIcon.color, a);
		price.coinIcon.color = this.AdjustAlpha(price.coinIcon.color, a);
		price.itemIcon.color = this.AdjustAlpha(price.itemIcon.color, a);
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000750BA File Offset: 0x000732BA
	private Color AdjustAlpha(Color c, float a)
	{
		c.a = a;
		return c;
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000750C8 File Offset: 0x000732C8
	private Sprite GetChangeIcon(Identifiable.Id id, int change, int collected)
	{
		if (SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2)
		{
			if (!GameModeSettings.PlortBonusReached(collected))
			{
				return null;
			}
			return this.bonusCompleteImg;
		}
		else
		{
			if (!SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic)
			{
				return null;
			}
			if (change == 0)
			{
				return this.unchImg;
			}
			if (change >= 0)
			{
				return this.upImg;
			}
			return this.downImg;
		}
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x0007512C File Offset: 0x0007332C
	private string GetChangeText(Identifiable.Id id, int change)
	{
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().plortMarketDynamic)
		{
			return Math.Abs(change).ToString();
		}
		return string.Empty;
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x00075164 File Offset: 0x00073364
	private bool IsPlortUnlocked(MarketUI.PlortEntry plort, int collected)
	{
		if (collected > 0)
		{
			return true;
		}
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser)
		{
			return true;
		}
		if (plort.toUnlock.Length == 0)
		{
			return true;
		}
		foreach (ProgressDirector.ProgressType type in plort.toUnlock)
		{
			if (this.progressDir.HasProgress(type))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001DDA RID: 7642
	public MarketUI.PlortEntry[] plorts;

	// Token: 0x04001DDB RID: 7643
	public GameObject pricesPanelGroup;

	// Token: 0x04001DDC RID: 7644
	public MarketUI.PricesPanelEntry[] pricesPanels;

	// Token: 0x04001DDD RID: 7645
	public GameObject priceEntryPrefab;

	// Token: 0x04001DDE RID: 7646
	public GameObject priceEntryEmptyPrefab;

	// Token: 0x04001DDF RID: 7647
	public GameObject shutdownPanel;

	// Token: 0x04001DE0 RID: 7648
	public GameObject[] toShutdown;

	// Token: 0x04001DE1 RID: 7649
	public Sprite upImg;

	// Token: 0x04001DE2 RID: 7650
	public Sprite downImg;

	// Token: 0x04001DE3 RID: 7651
	public Sprite unchImg;

	// Token: 0x04001DE4 RID: 7652
	public Sprite bonusCompleteImg;

	// Token: 0x04001DE5 RID: 7653
	private EconomyDirector econDir;

	// Token: 0x04001DE6 RID: 7654
	private ProgressDirector progressDir;

	// Token: 0x04001DE7 RID: 7655
	private LookupDirector lookupDir;

	// Token: 0x04001DE8 RID: 7656
	private Dictionary<MarketUI.PlortEntry, GameObject> amountMap;

	// Token: 0x020005C7 RID: 1479
	[Serializable]
	public class PlortEntry
	{
		// Token: 0x04001DE9 RID: 7657
		public Identifiable.Id id;

		// Token: 0x04001DEA RID: 7658
		public ProgressDirector.ProgressType[] toUnlock;
	}

	// Token: 0x020005C8 RID: 1480
	[Serializable]
	public class PricesPanelEntry
	{
		// Token: 0x04001DEB RID: 7659
		public GameObject panel;

		// Token: 0x04001DEC RID: 7660
		public int entryCount;
	}
}
