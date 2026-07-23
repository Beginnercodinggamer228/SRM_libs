using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200059F RID: 1439
public class InstrumentPopupUI : PopupUI<EchoNoteGameMetadata>, PopupDirector.Popup
{
	// Token: 0x06001DE4 RID: 7652 RVA: 0x00071D3F File Offset: 0x0006FF3F
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.PopupDirector.PopupActivated(this);
		Destroyer.Destroy(base.gameObject, this.lifetime, "InstrumentPopupUI", false, false);
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x00071D69 File Offset: 0x0006FF69
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.PopupDirector.PopupDeactivated(this);
		}
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x00071D8E File Offset: 0x0006FF8E
	public override void Init(EchoNoteGameMetadata instrument)
	{
		base.Init(instrument);
		this.instrumentIcon.sprite = instrument.icon;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x00071DA8 File Offset: 0x0006FFA8
	public override void OnBundleAvailable(MessageDirector messageDirector)
	{
		MessageBundle bundle = messageDirector.GetBundle("actor");
		this.instrumentName.text = bundle.Get(this.idEntry.xlateKey);
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
	public bool ShouldClear()
	{
		return false;
	}

	// Token: 0x04001D09 RID: 7433
	[Tooltip("Lifetime of the popup (seconds).")]
	public float lifetime;

	// Token: 0x04001D0A RID: 7434
	[Tooltip("Text representing the instrument name.")]
	public TMP_Text instrumentName;

	// Token: 0x04001D0B RID: 7435
	[Tooltip("Image representing the instrument icon.")]
	public Image instrumentIcon;

	// Token: 0x020005A0 RID: 1440
	public class PopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x06001DEA RID: 7658 RVA: 0x00071DE5 File Offset: 0x0006FFE5
		public PopupCreator(EchoNoteGameMetadata instrument)
		{
			this.instrument = instrument;
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x00071DF4 File Offset: 0x0006FFF4
		public override void Create()
		{
			UnityEngine.Object.Instantiate<InstrumentPopupUI>(SRSingleton<SceneContext>.Instance.InstrumentDirector.popupUI).Init(this.instrument);
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x00071E18 File Offset: 0x00070018
		public override bool Equals(object other)
		{
			InstrumentPopupUI.PopupCreator popupCreator = other as InstrumentPopupUI.PopupCreator;
			return popupCreator != null && popupCreator.instrument == this.instrument;
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x00071E42 File Offset: 0x00070042
		public override int GetHashCode()
		{
			return this.instrument.GetHashCode();
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x04001D0C RID: 7436
		private readonly EchoNoteGameMetadata instrument;
	}
}
