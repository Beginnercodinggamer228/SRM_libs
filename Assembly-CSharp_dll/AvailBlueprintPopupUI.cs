using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053D RID: 1341
public class AvailBlueprintPopupUI : PopupUI<GadgetDefinition>, PopupDirector.Popup
{
	// Token: 0x06001BE0 RID: 7136 RVA: 0x0006AF05 File Offset: 0x00069105
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.popupDir.PopupActivated(this);
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x0006AF38 File Offset: 0x00069138
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

	// Token: 0x06001BE2 RID: 7138 RVA: 0x0006AFF5 File Offset: 0x000691F5
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.popupDir.PopupDeactivated(this);
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x0006B009 File Offset: 0x00069209
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "AvailBlueprintPopupUI.Update");
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x0006B028 File Offset: 0x00069228
	public Gadget.Id GetId()
	{
		return this.idEntry.id;
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x0006B035 File Offset: 0x00069235
	public static GameObject CreateAvailBlueprintPopup(GadgetDefinition gadgetDefintion)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.GadgetDirector.availBlueprintPopupPrefab);
		gameObject.GetComponent<AvailBlueprintPopupUI>().Init(gadgetDefintion);
		return gameObject;
	}

	// Token: 0x04001B12 RID: 6930
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001B13 RID: 6931
	protected float timeOfDeath;

	// Token: 0x04001B14 RID: 6932
	protected PopupDirector popupDir;
}
