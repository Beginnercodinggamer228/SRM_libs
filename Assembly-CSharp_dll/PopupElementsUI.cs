using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005EC RID: 1516
public class PopupElementsUI : SRSingleton<PopupElementsUI>
{
	// Token: 0x06001FD4 RID: 8148 RVA: 0x00079539 File Offset: 0x00077739
	public void CreateCoinsPopup(int amount, PlayerState.CoinsType coinsType)
	{
		if (amount != 0 && coinsType != PlayerState.CoinsType.NONE)
		{
			this.queuedCoins.Enqueue(new PopupElementsUI.CoinsEntry(amount, coinsType));
		}
	}

	// Token: 0x06001FD5 RID: 8149 RVA: 0x00079554 File Offset: 0x00077754
	public void Update()
	{
		if (this.blockers.Count == 0 && this.queuedCoins.Count > 0 && Time.unscaledTime >= this.nextCoinAt)
		{
			CoinsPopupUI component = UnityEngine.Object.Instantiate<GameObject>(this.coinsPopup, this.container).GetComponent<CoinsPopupUI>();
			PopupElementsUI.CoinsEntry coinsEntry = this.queuedCoins.Dequeue();
			component.Init(coinsEntry.amount, this.GetSprite(coinsEntry.coinsType), this.GetColorOverride(coinsEntry.coinsType), this.GetSFXOverride(coinsEntry.coinsType));
			this.nextCoinAt = Time.unscaledTime + 0.1f;
		}
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x000795EB File Offset: 0x000777EB
	public void RegisterBlocker(GameObject blocker)
	{
		this.blockers.Add(blocker);
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x000795FA File Offset: 0x000777FA
	public void DeregisterBlocker(GameObject blocker)
	{
		this.blockers.Remove(blocker);
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x00079609 File Offset: 0x00077809
	private Sprite GetSprite(PlayerState.CoinsType coinsType)
	{
		if (coinsType == PlayerState.CoinsType.MOCHI)
		{
			return this.mochiIcon;
		}
		if (coinsType != PlayerState.CoinsType.DRONE)
		{
			return null;
		}
		return this.droneMetadata.coinsIcon;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x0007962C File Offset: 0x0007782C
	private Color? GetColorOverride(PlayerState.CoinsType coinsType)
	{
		if (coinsType == PlayerState.CoinsType.MOCHI)
		{
			return new Color?(this.mochiColor);
		}
		if (coinsType != PlayerState.CoinsType.DRONE)
		{
			return null;
		}
		return new Color?(this.droneMetadata.coinsColor);
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x00079669 File Offset: 0x00077869
	private SECTR_AudioCue GetSFXOverride(PlayerState.CoinsType coinsType)
	{
		if (coinsType == PlayerState.CoinsType.MOCHI)
		{
			return this.mochiCue;
		}
		if (coinsType != PlayerState.CoinsType.DRONE)
		{
			return null;
		}
		return this.droneMetadata.coinsCue;
	}

	// Token: 0x04001EFD RID: 7933
	public RectTransform container;

	// Token: 0x04001EFE RID: 7934
	public GameObject coinsPopup;

	// Token: 0x04001EFF RID: 7935
	public Sprite mochiIcon;

	// Token: 0x04001F00 RID: 7936
	public Color mochiColor;

	// Token: 0x04001F01 RID: 7937
	public SECTR_AudioCue mochiCue;

	// Token: 0x04001F02 RID: 7938
	public DroneMetadata droneMetadata;

	// Token: 0x04001F03 RID: 7939
	private HashSet<GameObject> blockers = new HashSet<GameObject>();

	// Token: 0x04001F04 RID: 7940
	private Queue<PopupElementsUI.CoinsEntry> queuedCoins = new Queue<PopupElementsUI.CoinsEntry>();

	// Token: 0x04001F05 RID: 7941
	private float nextCoinAt;

	// Token: 0x04001F06 RID: 7942
	private const float MIN_TIME_BETWEEN_COINS = 0.1f;

	// Token: 0x020005ED RID: 1517
	private class CoinsEntry
	{
		// Token: 0x06001FDC RID: 8156 RVA: 0x000796A7 File Offset: 0x000778A7
		public CoinsEntry(int amount, PlayerState.CoinsType coinsType)
		{
			this.amount = amount;
			this.coinsType = coinsType;
		}

		// Token: 0x04001F07 RID: 7943
		public int amount;

		// Token: 0x04001F08 RID: 7944
		public PlayerState.CoinsType coinsType;
	}
}
