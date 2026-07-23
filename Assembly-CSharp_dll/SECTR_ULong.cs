using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
[Serializable]
public class SECTR_ULong
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060003C6 RID: 966 RVA: 0x000178D0 File Offset: 0x00015AD0
	// (set) Token: 0x060003C7 RID: 967 RVA: 0x000178F1 File Offset: 0x00015AF1
	public ulong value
	{
		get
		{
			ulong num = (ulong)((long)this.first);
			return (ulong)((long)this.second << 32 | (long)num);
		}
		set
		{
			this.first = (int)(value & (ulong)-1);
			this.second = (int)(value >> 32);
		}
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00017909 File Offset: 0x00015B09
	public SECTR_ULong(ulong newValue)
	{
		this.value = newValue;
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00017918 File Offset: 0x00015B18
	public SECTR_ULong()
	{
		this.value = 0UL;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00017928 File Offset: 0x00015B28
	public override string ToString()
	{
		return string.Format("[ULong: value={0}, firstHalf={1}, secondHalf={2}]", this.value, this.first, this.second);
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00017955 File Offset: 0x00015B55
	public static bool operator >(SECTR_ULong a, ulong b)
	{
		return a.value > b;
	}

	// Token: 0x060003CC RID: 972 RVA: 0x00017960 File Offset: 0x00015B60
	public static bool operator >(ulong a, SECTR_ULong b)
	{
		return a > b.value;
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0001796B File Offset: 0x00015B6B
	public static bool operator <(SECTR_ULong a, ulong b)
	{
		return a.value < b;
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00017976 File Offset: 0x00015B76
	public static bool operator <(ulong a, SECTR_ULong b)
	{
		return a < b.value;
	}

	// Token: 0x040003CB RID: 971
	[SerializeField]
	private int first;

	// Token: 0x040003CC RID: 972
	[SerializeField]
	private int second;
}
