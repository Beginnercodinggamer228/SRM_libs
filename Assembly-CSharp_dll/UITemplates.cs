using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200066B RID: 1643
public class UITemplates : SRBehaviour
{
	// Token: 0x0600220A RID: 8714 RVA: 0x00083CA9 File Offset: 0x00081EA9
	public GameObject CreateCreditsPrefab(bool aboutCredits)
	{
		return UnityEngine.Object.Instantiate<GameObject>(aboutCredits ? this.aboutCreditsPrefab : this.storyCreditsPrefab);
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x00083CC4 File Offset: 0x00081EC4
	public void Awake()
	{
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.InitBundles));
		Dictionary<InputDeviceStyle, Dictionary<string, Sprite>> dictionary = new Dictionary<InputDeviceStyle, Dictionary<string, Sprite>>();
		dictionary.Add(InputDeviceStyle.XboxOne, this.xboxButtonIconList.ToDictionary((UITemplates.IconEntry e) => e.keyStr, (UITemplates.IconEntry e) => e.icon));
		dictionary.Add(InputDeviceStyle.PlayStation4, this.ps4ButtonIconList.ToDictionary((UITemplates.IconEntry e) => e.keyStr, (UITemplates.IconEntry e) => e.icon));
		dictionary.Add(InputDeviceStyle.Unknown, this.mouseIconList.ToDictionary((UITemplates.IconEntry e) => e.keyStr, (UITemplates.IconEntry e) => e.icon));
		this.deviceButtonIconDict = dictionary;
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x00083DE7 File Offset: 0x00081FE7
	public void InitBundles(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
		this.rangeBundle = msgDir.GetBundle("range");
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x00083E0C File Offset: 0x0008200C
	public GameObject CreatePurchaseButton(string itemName, Sprite costIcon, int cost, UnityAction onButton)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.purchaseButtonPrefab);
		Button component = gameObject.GetComponent<Button>();
		TMP_Text component2 = gameObject.transform.Find("ItemName").gameObject.GetComponent<TMP_Text>();
		Image component3 = gameObject.transform.Find("Bottom/CostIcon").gameObject.GetComponent<Image>();
		TMP_Text component4 = gameObject.transform.Find("Bottom/CostAmount").gameObject.GetComponent<TMP_Text>();
		component2.text = itemName;
		component3.sprite = costIcon;
		component4.text = cost.ToString();
		component.onClick.AddListener(onButton);
		return gameObject;
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x00083EA4 File Offset: 0x000820A4
	public GameObject CreateTeleportButton(string teleportName, bool enableTeleport, UnityAction onButton)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.teleportButtonPrefab);
		Button component = gameObject.GetComponent<Button>();
		component.interactable = enableTeleport;
		gameObject.transform.Find("TeleportName").gameObject.GetComponent<TMP_Text>().text = this.rangeBundle.Get("m.teleporter." + teleportName);
		component.onClick.AddListener(onButton);
		return gameObject;
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x00083F0C File Offset: 0x0008210C
	public GameObject CreatePurchaseUI(Sprite titleIcon, string titleKey, PurchaseUI.Purchasable[] purchasables, bool hideNubuckCost, PurchaseUI.OnClose onClose, bool unavailInMainList = false)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.purchaseUIPrefab);
		PurchaseUI component = gameObject.GetComponent<PurchaseUI>();
		component.Init(titleIcon, titleKey, onClose);
		foreach (PurchaseUI.Purchasable purchasable in purchasables)
		{
			component.AddButton(purchasable, unavailInMainList);
		}
		if (hideNubuckCost)
		{
			component.HideNubuckCost();
		}
		component.SelectFirst();
		return gameObject;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x00083F66 File Offset: 0x00082166
	public GameObject CreateRancherChoiceUI(List<string> rancherIds)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.rancherChoicePrefab);
		gameObject.GetComponent<RancherChoiceUI>().Init(rancherIds);
		return gameObject;
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x00083F7F File Offset: 0x0008217F
	public GameObject CreateErrorDialog(string errorMsg)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.errorDialogPrefab);
		gameObject.transform.Find("MainPanel/Message").GetComponent<TMP_Text>().text = this.uiBundle.Xlate(errorMsg);
		return gameObject;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x00083FB2 File Offset: 0x000821B2
	public GameObject CreateErrorDialogWithArgs(string errorMsg, params object[] args)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.errorDialogPrefab);
		gameObject.transform.Find("MainPanel/Message").GetComponent<TMP_Text>().text = this.uiBundle.Get(errorMsg, args);
		return gameObject;
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x00083FE6 File Offset: 0x000821E6
	public GameObject CreateConfirmDialog(string confirmMsg, ConfirmUI.OnConfirm onConfirm)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.confirmDialogPrefab);
		gameObject.transform.Find("MainPanel/Message").GetComponent<TMP_Text>().text = this.uiBundle.Xlate(confirmMsg);
		gameObject.GetComponent<ConfirmUI>().onConfirm = onConfirm;
		return gameObject;
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x00084028 File Offset: 0x00082228
	public Sprite GetButtonIcon(InputDeviceStyle inputDevice, string keyStr, out bool iconFound)
	{
		bool flag = InputDirector.UsingGamepad();
		InputDeviceStyle key = InputDeviceStyle.Unknown;
		if (flag)
		{
			key = ((inputDevice == InputDeviceStyle.PlayStation2 || inputDevice == InputDeviceStyle.PlayStation3 || inputDevice == InputDeviceStyle.PlayStation4) ? InputDeviceStyle.PlayStation4 : InputDeviceStyle.XboxOne);
		}
		iconFound = false;
		if (keyStr != null && this.deviceButtonIconDict.ContainsKey(key))
		{
			Dictionary<string, Sprite> dictionary = this.deviceButtonIconDict[key];
			if (dictionary.ContainsKey(keyStr))
			{
				iconFound = true;
				return dictionary[keyStr];
			}
		}
		return this.unknownButtonIcon;
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x0008408A File Offset: 0x0008228A
	internal Sprite GetSteamButtonIcon(int originId, out bool iconFound)
	{
		if (originId >= 0 && originId < this.steamButtonIcons.Length)
		{
			iconFound = true;
			return this.steamButtonIcons[originId];
		}
		iconFound = false;
		return this.unknownButtonIcon;
	}

	// Token: 0x040021D5 RID: 8661
	public GameObject purchaseButtonPrefab;

	// Token: 0x040021D6 RID: 8662
	public GameObject teleportButtonPrefab;

	// Token: 0x040021D7 RID: 8663
	public GameObject labelPrefab;

	// Token: 0x040021D8 RID: 8664
	public GameObject errorDialogPrefab;

	// Token: 0x040021D9 RID: 8665
	public GameObject confirmDialogPrefab;

	// Token: 0x040021DA RID: 8666
	public GameObject purchaseUIPrefab;

	// Token: 0x040021DB RID: 8667
	public GameObject decorizerUIPrefab;

	// Token: 0x040021DC RID: 8668
	public GameObject availUpgradePrefab;

	// Token: 0x040021DD RID: 8669
	public GameObject mailPrefab;

	// Token: 0x040021DE RID: 8670
	public GameObject storyCreditsPrefab;

	// Token: 0x040021DF RID: 8671
	public GameObject aboutCreditsPrefab;

	// Token: 0x040021E0 RID: 8672
	public GameObject rancherChatPrefab;

	// Token: 0x040021E1 RID: 8673
	public GameObject rancherChoicePrefab;

	// Token: 0x040021E2 RID: 8674
	public SECTR_AudioCue clickCue;

	// Token: 0x040021E3 RID: 8675
	public SECTR_AudioCue errorCue;

	// Token: 0x040021E4 RID: 8676
	public SECTR_AudioCue purchaseCue;

	// Token: 0x040021E5 RID: 8677
	public SECTR_AudioCue purchaseExpansionCue;

	// Token: 0x040021E6 RID: 8678
	public SECTR_AudioCue purchasePlotCue;

	// Token: 0x040021E7 RID: 8679
	public SECTR_AudioCue purchaseUpgradeCue;

	// Token: 0x040021E8 RID: 8680
	public SECTR_AudioCue purchasePersonalUpgradeCue;

	// Token: 0x040021E9 RID: 8681
	public SECTR_AudioCue purchaseBlueprintCue;

	// Token: 0x040021EA RID: 8682
	public SECTR_AudioCue fabricateGadgetCue;

	// Token: 0x040021EB RID: 8683
	public SECTR_AudioCue placeGadgetCue;

	// Token: 0x040021EC RID: 8684
	public SECTR_AudioCue removeGadgetCue;

	// Token: 0x040021ED RID: 8685
	public Sprite currencyIcon;

	// Token: 0x040021EE RID: 8686
	public MessageBundle uiBundle;

	// Token: 0x040021EF RID: 8687
	public MessageBundle rangeBundle;

	// Token: 0x040021F0 RID: 8688
	public GameObject loadingUI;

	// Token: 0x040021F1 RID: 8689
	public GameObject demoUI;

	// Token: 0x040021F2 RID: 8690
	[FormerlySerializedAs("buttonIconList")]
	public UITemplates.IconEntry[] xboxButtonIconList;

	// Token: 0x040021F3 RID: 8691
	public UITemplates.IconEntry[] ps4ButtonIconList;

	// Token: 0x040021F4 RID: 8692
	public Sprite[] steamButtonIcons;

	// Token: 0x040021F5 RID: 8693
	public UITemplates.IconEntry[] mouseIconList;

	// Token: 0x040021F6 RID: 8694
	public Sprite unknownButtonIcon;

	// Token: 0x040021F7 RID: 8695
	private Dictionary<InputDeviceStyle, Dictionary<string, Sprite>> deviceButtonIconDict;

	// Token: 0x0200066C RID: 1644
	[Serializable]
	public class IconEntry
	{
		// Token: 0x040021F8 RID: 8696
		public string keyStr;

		// Token: 0x040021F9 RID: 8697
		public Sprite icon;
	}
}
