using System;
using System.Collections;
using System.Linq;
using DLCPackage;

// Token: 0x02000677 RID: 1655
public class UWPDLCProvider : DLCProvider
{
	// Token: 0x06002245 RID: 8773 RVA: 0x00084A3D File Offset: 0x00082C3D
	public UWPDLCProvider() : base(Enumerable.Empty<Id>())
	{
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x00084A4A File Offset: 0x00082C4A
	public override IEnumerator Refresh()
	{
		yield return null;
		yield break;
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000350A2 File Offset: 0x000332A2
	public override void ShowInStore(Id id)
	{
		throw new NotImplementedException();
	}
}
