using System;
using System.Linq;
using DLCPackage;
using TMPro;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class DLCPurgedExceptionUI : BaseUI
{
	// Token: 0x060005BF RID: 1471 RVA: 0x000219BA File Offset: 0x0001FBBA
	public static DLCPurgedExceptionUI OnExceptionCaught(DLCPurgedExceptionUI prefab, DLCPurgedException exception, Action onContinue, Action onCancel)
	{
		DLCPurgedExceptionUI dlcpurgedExceptionUI = UnityEngine.Object.Instantiate<DLCPurgedExceptionUI>(prefab);
		dlcpurgedExceptionUI.exception = exception;
		dlcpurgedExceptionUI.onContinue = onContinue;
		dlcpurgedExceptionUI.onCancel = onCancel;
		dlcpurgedExceptionUI.RebuildUI();
		return dlcpurgedExceptionUI;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x000219DD File Offset: 0x0001FBDD
	public override void Update()
	{
		if (this.Closeable() && SRInput.PauseActions.cancel.WasPressed)
		{
			if (this.onCancel != null)
			{
				this.onCancel();
			}
			this.Close();
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00021A11 File Offset: 0x0001FC11
	public override void OnBundlesAvailable(MessageDirector messageDirector)
	{
		base.OnBundlesAvailable(messageDirector);
		this.RebuildUI();
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00021A20 File Offset: 0x0001FC20
	private void RebuildUI()
	{
		MessageDirector messageDirector = SRSingleton<GameContext>.Instance.MessageDirector;
		MessageBundle bundle = messageDirector.GetBundle("ui");
		MessageBundle pedia = messageDirector.GetBundle("pedia");
		this.message.SetText((this.exception != null) ? bundle.Get("e.file_load_failed.dlc_purged", new string[]
		{
			string.Join("\n", (from p in this.exception.packages
			select pedia.Get(string.Format("m.dlc.{0}", p.ToString().ToLowerInvariant()))).ToArray<string>())
		}) : string.Empty);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00021AB4 File Offset: 0x0001FCB4
	public void OnContinue()
	{
		if (this.onContinue != null)
		{
			this.onContinue();
		}
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00021AC9 File Offset: 0x0001FCC9
	public void OnShowPackageInStore()
	{
		if (this.exception != null && this.onCancel != null)
		{
			SRSingleton<GameContext>.Instance.DLCDirector.ShowPackageInStore(this.exception.packages.First<Id>());
			this.onCancel();
		}
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00021B05 File Offset: 0x0001FD05
	public void OnCancel()
	{
		if (this.onCancel != null)
		{
			this.onCancel();
		}
	}

	// Token: 0x04000599 RID: 1433
	[Tooltip("Text showing the error message.")]
	public TMP_Text message;

	// Token: 0x0400059A RID: 1434
	private DLCPurgedException exception;

	// Token: 0x0400059B RID: 1435
	private Action onContinue;

	// Token: 0x0400059C RID: 1436
	private Action onCancel;
}
