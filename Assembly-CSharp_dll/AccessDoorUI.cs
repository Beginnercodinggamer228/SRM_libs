using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000536 RID: 1334
public class AccessDoorUI : BaseUI
{
	// Token: 0x06001BBA RID: 7098 RVA: 0x00069F7D File Offset: 0x0006817D
	public override void Awake()
	{
		base.Awake();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00069F98 File Offset: 0x00068198
	public void SetAccessDoor(AccessDoor door)
	{
		this.door = door;
		if (door.CurrState == AccessDoor.State.LOCKED)
		{
			GameObject gameObject = this.CreatePurchaseUI();
			gameObject.transform.SetParent(base.transform, false);
			this.statusArea = gameObject.GetComponent<PurchaseUI>().statusArea;
			return;
		}
		this.Close();
		door.CurrState = AccessDoor.State.OPEN;
		SRSingleton<SceneContext>.Instance.PediaDirector.ShowPedia(door.lockedRegionId);
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x0006A004 File Offset: 0x00068204
	protected GameObject CreatePurchaseUI()
	{
		PurchaseUI.Purchasable[] array = new PurchaseUI.Purchasable[1];
		array[0] = new PurchaseUI.Purchasable("t." + this.door.lockedRegionId.ToString().ToLowerInvariant(), this.door.doorPurchase.icon, this.door.doorPurchase.img, "m.intro." + this.door.lockedRegionId.ToString().ToLowerInvariant(), this.door.doorPurchase.cost, new PediaDirector.Id?(this.door.lockedRegionId), new UnityAction(this.UnlockDoor), () => true, () => true, null, null, null, null, false);
		PurchaseUI.Purchasable[] array2 = array;
		GameObject gameObject;
		(gameObject = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.titleIcon, MessageUtil.Qualify("ui", "t.access_door"), array2, false, new PurchaseUI.OnClose(this.Close), false)).GetComponent<PurchaseUI>().Select(array2[0]);
		GameObject result;
		(result = gameObject).GetComponent<PurchaseUI>().HideSelectionPanel();
		return result;
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x0006A148 File Offset: 0x00068348
	public void UnlockDoor()
	{
		if (this.playerState.GetCurrency() >= this.door.doorPurchase.cost)
		{
			this.playerState.SpendCurrency(this.door.doorPurchase.cost, false);
			this.door.CurrState = AccessDoor.State.OPEN;
			if (this.door.linkedDoors != null)
			{
				foreach (AccessDoor accessDoor in this.door.linkedDoors)
				{
					if (accessDoor.CurrState == AccessDoor.State.LOCKED)
					{
						accessDoor.CurrState = AccessDoor.State.CLOSED;
					}
				}
			}
			base.Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseExpansionCue);
			this.Close();
			SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow();
			return;
		}
		base.PlayErrorCue();
		base.Error("e.insuf_coins", false);
	}

	// Token: 0x04001AD8 RID: 6872
	public Sprite titleIcon;

	// Token: 0x04001AD9 RID: 6873
	private PlayerState playerState;

	// Token: 0x04001ADA RID: 6874
	private AccessDoor door;
}
