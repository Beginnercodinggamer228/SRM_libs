using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DLCPackage;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000F7 RID: 247
public class DLCManageUI : BaseUI
{
	// Token: 0x060005A5 RID: 1445 RVA: 0x000213BC File Offset: 0x0001F5BC
	public static bool IsEnabled()
	{
		DLCDirector director = SRSingleton<GameContext>.Instance.DLCDirector;
		return (Levels.isSpecial() || SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enableDLC) && director.GetSupportedPackages().Any((Id id) => director.HasReached(id, State.AVAILABLE));
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0002141C File Offset: 0x0001F61C
	public override void Awake()
	{
		base.Awake();
		this.includedInPackage = UnityEngine.Object.Instantiate<GameObject>(this.includedInPackagePrefab.gameObject).GetComponent<DLCManageUI_IncludedInPackage>();
		this.purchaseUI = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(this.icon, "t.dlc", new PurchaseUI.Purchasable[0], true, new PurchaseUI.OnClose(this.Close), false).GetComponent<PurchaseUI>();
		this.includedInPackage.transform.SetParent(this.purchaseUI.customizationPanel.transform, false);
		this.purchaseUI.transform.SetParent(base.transform, false);
		this.purchaseUI.Resize(450f, 600f);
		this.purchaseUI.ReselectOnReturnFromPedia();
		this.statusArea = this.purchaseUI.statusArea;
		base.StartCoroutine(this.Refresh_Coroutine());
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x000214FA File Offset: 0x0001F6FA
	public void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			base.StartCoroutine(this.Refresh_Coroutine());
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0002150C File Offset: 0x0001F70C
	private IEnumerator Refresh_Coroutine()
	{
		float minLoadingTime = Time.unscaledTime + 0.25f;
		this.purchaseUI.SetActivePanels(PurchaseUI.Panel.LOADING);
		this.purchaseUI.ClearButtons();
		DLCDirector director = SRSingleton<GameContext>.Instance.DLCDirector;
		yield return director.RegisterPackagesAsync();
		yield return new WaitUntil(() => Time.unscaledTime >= minLoadingTime);
		IEnumerable<DLCPackageMetadata> source = director.LoadPackageMetadatas();
		Func<DLCPackageMetadata, IEnumerable<PurchaseUI.Purchasable>> <>9__1;
		Func<DLCPackageMetadata, IEnumerable<PurchaseUI.Purchasable>> selector;
		if ((selector = <>9__1) == null)
		{
			selector = (<>9__1 = delegate(DLCPackageMetadata package)
			{
				UnityAction <>9__3;
				PurchaseUI.OnSelected <>9__4;
				Func<bool> <>9__5;
				Func<bool> <>9__6;
				return package.contents.Select(delegate(DLCPackageMetadata.Content item)
				{
					PurchaseUI.Purchasable purchasable2 = new PurchaseUI.Purchasable();
					purchasable2.nameKey = string.Format("m.dlc.{0}.contents.{1}", package.id.ToString().ToLowerInvariant(), item.id);
					purchasable2.descKey = string.Format("m.dlc.{0}.contents.{1}.desc", package.id.ToString().ToLowerInvariant(), item.id);
					purchasable2.icon = item.image;
					purchasable2.mainImg = item.imageLarge;
					UnityAction onPurchase;
					if ((onPurchase = <>9__3) == null)
					{
						onPurchase = (<>9__3 = delegate()
						{
							director.ShowPackageInStore(package.id);
						});
					}
					purchasable2.onPurchase = onPurchase;
					PurchaseUI.OnSelected onSelected;
					if ((onSelected = <>9__4) == null)
					{
						onSelected = (<>9__4 = delegate(PurchaseUI.Purchasable p)
						{
							this.OnPackageSelected(package);
						});
					}
					purchasable2.onSelected = onSelected;
					Func<bool> unlocked;
					if ((unlocked = <>9__5) == null)
					{
						unlocked = (<>9__5 = (() => director.HasReached(package.id, State.AVAILABLE)));
					}
					purchasable2.unlocked = unlocked;
					Func<bool> avail;
					if ((avail = <>9__6) == null)
					{
						avail = (<>9__6 = (() => !director.HasReached(package.id, State.INSTALLED)));
					}
					purchasable2.avail = avail;
					purchasable2.btnOverride = (director.HasReached(package.id, State.INSTALLED) ? "b.dlc.installed" : "b.dlc.view_in_store");
					return purchasable2;
				});
			});
		}
		foreach (PurchaseUI.Purchasable purchasable in source.SelectMany(selector).ToArray<PurchaseUI.Purchasable>())
		{
			this.purchaseUI.AddButton(purchasable, false);
		}
		this.purchaseUI.SetActivePanels(PurchaseUI.Panel.DEFAULT);
		this.purchaseUI.SelectFirst();
		yield break;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0002151B File Offset: 0x0001F71B
	public override void OnBundlesAvailable(MessageDirector msg)
	{
		base.OnBundlesAvailable(msg);
		if (this.purchaseUI != null)
		{
			this.purchaseUI.Rebuild(false);
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00021540 File Offset: 0x0001F740
	private void OnPackageSelected(DLCPackageMetadata package)
	{
		if (package.contents.Count > 1)
		{
			string text = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia").Get(string.Format("m.dlc.{0}", package.id.ToString().ToLowerInvariant()));
			this.includedInPackage.text.text = this.uiBundle.Get("m.dlc.included_in", new string[]
			{
				text
			});
			this.includedInPackage.icon.sprite = package.icon;
			this.includedInPackage.gameObject.SetActive(true);
			return;
		}
		this.includedInPackage.gameObject.SetActive(false);
	}

	// Token: 0x04000582 RID: 1410
	[Tooltip("Icon displayed at the top of the modal (see PurchaseUI).")]
	public Sprite icon;

	// Token: 0x04000583 RID: 1411
	[Tooltip("Prefab showing the 'included in...' text/icon.")]
	public DLCManageUI_IncludedInPackage includedInPackagePrefab;

	// Token: 0x04000584 RID: 1412
	private DLCManageUI_IncludedInPackage includedInPackage;

	// Token: 0x04000585 RID: 1413
	private const float MIN_LOADING_TIME = 0.25f;

	// Token: 0x04000586 RID: 1414
	private PurchaseUI purchaseUI;
}
