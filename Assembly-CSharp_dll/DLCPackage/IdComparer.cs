using System;
using System.Collections.Generic;

namespace DLCPackage
{
	// Token: 0x02000A2C RID: 2604
	public class IdComparer : IEqualityComparer<Id>
	{
		// Token: 0x06004601 RID: 17921 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(Id a, Id b)
		{
			return a == b;
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(Id a)
		{
			return (int)a;
		}

		// Token: 0x040033DB RID: 13275
		public static IdComparer Instance = new IdComparer();
	}
}
