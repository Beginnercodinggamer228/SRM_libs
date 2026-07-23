using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053E RID: 1342
public class AvailUpgradePopupUI : PopupUI<PlayerState.Upgrade>, PopupDirector.Popup
{
	// Token: 0x06001BE8 RID: 7144 RVA: 0x0006B06A File Offset: 0x0006926A
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.popupDir.PopupActivated(this);
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x0006B09C File Offset: 0x0006929C
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		TMP_Text component = base.transform.Find("UIContainer/MainPanel/TitlePanel/Title").GetComponent<TMP_Text>();
		TMP_Text component2 = base.transform.Find("UIContainer/MainPanel/IntroPanel/Intro").GetComponent<TMP_Text>();
		Image component3 = base.transform.Find("UIContainer/MainPanel/EntryImage").GetComponent<Image>();
		MessageBundle bundle = msgDir.GetBundle("pedia");
		string str = Enum.GetName(typeof(PlayerState.Upgrade), this.idEntry).ToLowerInvariant();
		component.text = bundle.Get("m.upgrade.name.personal." + str);
		component2.text = bundle.Get("m.upgrade.desc.personal." + str);
		component3.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetUpgradeDefinition(this.idEntry).Icon;
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x0006B163 File Offset: 0x00069363
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.popupDir.PopupDeactivated(this);
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x0006B177 File Offset: 0x00069377
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "AvailUpgradePopupUI.Update");
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x0006B196 File Offset: 0x00069396
	public PlayerState.Upgrade GetId()
	{
		return this.idEntry;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x0006B19E File Offset: 0x0006939E
	public static GameObject CreateAvailUpgradePopup(PlayerState.Upgrade upgrade)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.UITemplates.availUpgradePrefab);
		gameObject.GetComponent<AvailUpgradePopupUI>().Init(upgrade);
		return gameObject;
	}

	// Token: 0x04001B15 RID: 6933
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001B16 RID: 6934
	protected float timeOfDeath;

	// Token: 0x04001B17 RID: 6935
	protected PopupDirector popupDir;
}
