using System;
using UnityEngine;

namespace UiParticles
{
	// Token: 0x02000BD9 RID: 3033
	internal static class SetPropertyUtility
	{
		// Token: 0x060056C4 RID: 22212 RVA: 0x00106780 File Offset: 0x00104980
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x001067CF File Offset: 0x001049CF
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x001067F0 File Offset: 0x001049F0
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
