using System;
using System.Collections.Generic;

namespace DLCPackage
{
	// Token: 0x02000A2E RID: 2606
	public class StateComparer : IEqualityComparer<State>
	{
		// Token: 0x06004605 RID: 17925 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(State a, State b)
		{
			return a == b;
		}

		// Token: 0x06004606 RID: 17926 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(State a)
		{
			return (int)a;
		}

		// Token: 0x040033E1 RID: 13281
		public static StateComparer Instance = new StateComparer();
	}
}
