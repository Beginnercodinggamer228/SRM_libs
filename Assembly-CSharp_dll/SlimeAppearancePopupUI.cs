using System;
using Assets.Script.Util.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200061E RID: 1566
public class SlimeAppearancePopupUI : PopupUI<SlimeAppearance>, PopupDirector.Popup
{
	// Token: 0x060020DA RID: 8410 RVA: 0x0007DA19 File Offset: 0x0007BC19
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.PopupDirector.PopupActivated(this);
		Destroyer.Destroy(base.gameObject, this.lifetime, "SlimeAppearancePopupUI", false, false);
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x0007DA43 File Offset: 0x0007BC43
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.PopupDirector.PopupDeactivated(this);
		}
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x0007DA68 File Offset: 0x0007BC68
	public override void Init(SlimeAppearance appearance)
	{
		base.Init(appearance);
		this.appearanceIcon.sprite = appearance.Icon;
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x0007DA84 File Offset: 0x0007BC84
	public override void OnBundleAvailable(MessageDirector messageDirector)
	{
		MessageBundle bundle = messageDirector.GetBundle("actor");
		this.appearanceName.text = bundle.Get(this.idEntry.NameXlateKey);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x0400202C RID: 8236
	[Tooltip("Lifetime of the popup (seconds).")]
	public float lifetime;

	// Token: 0x0400202D RID: 8237
	[Tooltip("Text representing the appearance name.")]
	public TMP_Text appearanceName;

	// Token: 0x0400202E RID: 8238
	[Tooltip("Image representing the appearance icon.")]
	public Image appearanceIcon;

	// Token: 0x0200061F RID: 1567
	public class PopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x060020E0 RID: 8416 RVA: 0x0007DAC1 File Offset: 0x0007BCC1
		public PopupCreator(SlimeAppearance appearance)
		{
			this.appearance = appearance;
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x0007DAD0 File Offset: 0x0007BCD0
		public override void Create()
		{
			UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.appearancePopupUI).GetRequiredComponent<SlimeAppearancePopupUI>().Init(this.appearance);
		}

		// Token: 0x060020E2 RID: 8418 RVA: 0x0007DAF6 File Offset: 0x0007BCF6
		public override bool Equals(object other)
		{
			return other is SlimeAppearancePopupUI.PopupCreator && ((SlimeAppearancePopupUI.PopupCreator)other).appearance == this.appearance;
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x0007DB18 File Offset: 0x0007BD18
		public override int GetHashCode()
		{
			return this.appearance.GetHashCode();
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x0400202F RID: 8239
		private readonly SlimeAppearance appearance;
	}
}
