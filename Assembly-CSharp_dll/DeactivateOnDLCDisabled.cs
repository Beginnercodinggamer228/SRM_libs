using System;
using DLCPackage;
using UnityEngine;

// Token: 0x020006DD RID: 1757
public class DeactivateOnDLCDisabled : MonoBehaviour
{
	// Token: 0x060024A8 RID: 9384 RVA: 0x0008D41B File Offset: 0x0008B61B
	public void Start()
	{
		this.director = SRSingleton<GameContext>.Instance.DLCDirector;
		this.director.onPackageInstalled += this.CheckDLCState;
		this.CheckDLCState(this.requiredDlc);
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x0008D450 File Offset: 0x0008B650
	public void OnDestroy()
	{
		if (this.director != null)
		{
			this.director.onPackageInstalled -= this.CheckDLCState;
			this.director = null;
		}
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x0008D478 File Offset: 0x0008B678
	private void CheckDLCState(Id package)
	{
		if (package == this.requiredDlc)
		{
			base.gameObject.SetActive(this.director.IsPackageInstalledAndEnabled(this.requiredDlc));
		}
	}

	// Token: 0x0400239C RID: 9116
	public Id requiredDlc;

	// Token: 0x0400239D RID: 9117
	private DLCDirector director;
}
