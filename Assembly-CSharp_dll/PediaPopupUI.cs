using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005DB RID: 1499
public class PediaPopupUI : PopupUI<PediaDirector.IdEntry>, PopupDirector.Popup
{
	// Token: 0x06001F78 RID: 8056 RVA: 0x000778F8 File Offset: 0x00075AF8
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.popupDir.PopupActivated(this);
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x00077928 File Offset: 0x00075B28
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		TMP_Text component = base.transform.Find("UIContainer/MainPanel/TitlePanel/Title").GetComponent<TMP_Text>();
		TMP_Text component2 = base.transform.Find("UIContainer/MainPanel/IntroPanel/Intro").GetComponent<TMP_Text>();
		Image component3 = base.transform.Find("UIContainer/MainPanel/EntryImage").GetComponent<Image>();
		MessageBundle bundle = msgDir.GetBundle("pedia");
		string str = Enum.GetName(typeof(PediaDirector.Id), this.idEntry.id).ToLowerInvariant();
		component.text = bundle.Get("t." + str);
		component2.text = bundle.Get("m.intro." + str);
		component3.sprite = this.idEntry.icon;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000779E5 File Offset: 0x00075BE5
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.popupDir.PopupDeactivated(this);
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000779F9 File Offset: 0x00075BF9
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "PediaPopupUI.Update");
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x00077A18 File Offset: 0x00075C18
	public void OpenPediaEntry()
	{
		SRSingleton<SceneContext>.Instance.PediaDirector.ShowPedia(this.idEntry.id);
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x00077A35 File Offset: 0x00075C35
	public PediaDirector.Id GetId()
	{
		return this.idEntry.id;
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x00077A42 File Offset: 0x00075C42
	public static GameObject CreatePediaPopup(PediaDirector.IdEntry pediaIdEntry)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.PediaDirector.pediaPopupPrefab);
		gameObject.GetComponent<PediaPopupUI>().Init(pediaIdEntry);
		return gameObject;
	}

	// Token: 0x04001EA0 RID: 7840
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001EA1 RID: 7841
	protected float timeOfDeath;

	// Token: 0x04001EA2 RID: 7842
	protected PopupDirector popupDir;
}
