using System;

namespace Microsoft.Xbox
{
	// Token: 0x02000BD7 RID: 3031
	internal class HR
	{
		// Token: 0x060056C0 RID: 22208 RVA: 0x00106771 File Offset: 0x00104971
		internal static bool SUCCEEDED(int hr)
		{
			return hr >= 0;
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x0010677A File Offset: 0x0010497A
		internal static bool FAILED(int hr)
		{
			return hr < 0;
		}
	}
}
