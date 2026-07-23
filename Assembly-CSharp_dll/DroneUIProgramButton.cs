using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CF RID: 463
public class DroneUIProgramButton : MonoBehaviour
{
	// Token: 0x060009D3 RID: 2515 RVA: 0x0002B7AF File Offset: 0x000299AF
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnMessageBundlesChanged));
		}
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0002B7DC File Offset: 0x000299DC
	public DroneUIProgramButton Init(DroneMetadata.Program.BaseComponent element, DroneUIProgramButton.Title title = null)
	{
		this.name.text = element.GetName();
		this.image.sprite = element.GetImage();
		this.titleMetadata = title;
		SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnMessageBundlesChanged));
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnMessageBundlesChanged));
		return this;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0002B84C File Offset: 0x00029A4C
	private void OnMessageBundlesChanged(MessageDirector messageDirector)
	{
		if (this.titleMetadata != null)
		{
			this.title.gameObject.SetActive(true);
			this.title.text = string.Format("{0}{1}", messageDirector.GetBundle("ui").Get(string.Format("l.drone.{0}", this.titleMetadata.type.ToString().ToLowerInvariant())), (this.titleMetadata.index != null) ? string.Format(" {0}", this.titleMetadata.index.Value) : string.Empty);
			return;
		}
		this.title.gameObject.SetActive(false);
	}

	// Token: 0x04000820 RID: 2080
	[Tooltip("Selection title.")]
	public TMP_Text title;

	// Token: 0x04000821 RID: 2081
	[Tooltip("Selection name.")]
	public new TMP_Text name;

	// Token: 0x04000822 RID: 2082
	[Tooltip("Selection button.")]
	public Button button;

	// Token: 0x04000823 RID: 2083
	[Tooltip("Selection image.")]
	public Image image;

	// Token: 0x04000824 RID: 2084
	private DroneUIProgramButton.Title titleMetadata;

	// Token: 0x020001D0 RID: 464
	public class Title
	{
		// Token: 0x04000825 RID: 2085
		public DroneUIProgramButton.Title.Type type;

		// Token: 0x04000826 RID: 2086
		public int? index;

		// Token: 0x020001D1 RID: 465
		public enum Type
		{
			// Token: 0x04000828 RID: 2088
			TARGET,
			// Token: 0x04000829 RID: 2089
			SOURCE,
			// Token: 0x0400082A RID: 2090
			DESTINATION
		}
	}
}
