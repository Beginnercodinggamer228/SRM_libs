using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005B4 RID: 1460
public class LoadingUI : MonoBehaviour
{
	// Token: 0x06001E40 RID: 7744 RVA: 0x00072C18 File Offset: 0x00070E18
	public void Awake()
	{
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		int num = -1;
		for (;;)
		{
			int num2 = num + 1;
			if (!bundle.Exists("m.loadingtip." + num2))
			{
				break;
			}
			num = num2;
		}
		if (num >= 0)
		{
			int @int = Randoms.SHARED.GetInt(num + 1);
			this.tipText.text = bundle.Get("m.loadingtip." + @int);
		}
		if (this.bouncyIcons != null && this.bouncyIcons.Length != 0)
		{
			this.bouncySlime.sprite = Randoms.SHARED.Pick<Sprite>(this.bouncyIcons);
		}
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x00072CBC File Offset: 0x00070EBC
	public void OnEnable()
	{
		this.toDisable = UnityEngine.Object.FindObjectsOfType<DisableDuringLoading>();
		DisableDuringLoading[] array = this.toDisable;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x00072CF8 File Offset: 0x00070EF8
	public void OnDisable()
	{
		if (this.isReturningToMenu)
		{
			foreach (DisableDuringLoading disableDuringLoading in this.toDisable)
			{
				if (disableDuringLoading != null && disableDuringLoading.gameObject != null)
				{
					disableDuringLoading.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x00072D49 File Offset: 0x00070F49
	public void OnLoadingError()
	{
		this.deactivateOnLoadError.ForEach(delegate(GameObject go)
		{
			go.SetActive(false);
		});
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x00072D75 File Offset: 0x00070F75
	public void OnLoadingStart()
	{
		this.deactivateOnLoadError.ForEach(delegate(GameObject go)
		{
			go.SetActive(true);
		});
	}

	// Token: 0x04001D58 RID: 7512
	public Image bouncySlime;

	// Token: 0x04001D59 RID: 7513
	public TMP_Text tipText;

	// Token: 0x04001D5A RID: 7514
	public Sprite[] bouncyIcons;

	// Token: 0x04001D5B RID: 7515
	public GameObject autoSavePanel;

	// Token: 0x04001D5C RID: 7516
	[Tooltip("List of GameObjects to deactivate during a loading error.")]
	public List<GameObject> deactivateOnLoadError;

	// Token: 0x04001D5D RID: 7517
	[NonSerialized]
	public bool isReturningToMenu;

	// Token: 0x04001D5E RID: 7518
	private DisableDuringLoading[] toDisable;
}
