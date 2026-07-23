using System;
using DLCPackage;

// Token: 0x020000FD RID: 253
public class DLCPurgedException : Exception
{
	// Token: 0x060005BE RID: 1470 RVA: 0x000219AB File Offset: 0x0001FBAB
	public DLCPurgedException(Id[] packages)
	{
		this.packages = packages;
	}

	// Token: 0x04000598 RID: 1432
	public readonly Id[] packages;
}
