using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000548 RID: 1352
public class BlueprintPopupUI : PopupUI<GadgetDefinition>, PopupDirector.Popup
{
	// Token: 0x06001C34 RID: 7220 RVA: 0x0006B9FB File Offset: 0x00069BFB
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.popupDir.PopupActivated(this);
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x0006BA2C File Offset: 0x00069C2C
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		TMP_Text component = base.transform.Find("UIContainer/MainPanel/TitlePanel/Title").GetComponent<TMP_Text>();
		TMP_Text component2 = base.transform.Find("UIContainer/MainPanel/IntroPanel/Intro").GetComponent<TMP_Text>();
		Image component3 = base.transform.Find("UIContainer/MainPanel/EntryImage").GetComponent<Image>();
		MessageBundle bundle = msgDir.GetBundle("pedia");
		string str = Enum.GetName(typeof(Gadget.Id), this.idEntry.id).ToLowerInvariant();
		component.text = bundle.Get("m.gadget.name." + str);
		component2.text = bundle.Get("m.gadget.desc." + str);
		component3.sprite = this.idEntry.icon;
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x0006BAE9 File Offset: 0x00069CE9
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.popupDir.PopupDeactivated(this);
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x0006BAFD File Offset: 0x00069CFD
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "BlueprintPopupUI.Update");
		}
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x0006B028 File Offset: 0x00069228
	public Gadget.Id GetId()
	{
		return this.idEntry.id;
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x0006BB1C File Offset: 0x00069D1C
	public static GameObject CreateBlueprintPopup(GadgetDefinition gadgetDefinition)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.GadgetDirector.gadgetPopupPrefab);
		gameObject.GetComponent<BlueprintPopupUI>().Init(gadgetDefinition);
		return gameObject;
	}

	// Token: 0x04001B37 RID: 6967
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001B38 RID: 6968
	protected float timeOfDeath;

	// Token: 0x04001B39 RID: 6969
	protected PopupDirector popupDir;
}
