using System;
using System.Collections.Generic;

// Token: 0x020006A2 RID: 1698
public abstract class SRComparer<T> : Comparer<T>
{
	// Token: 0x0600237B RID: 9083 RVA: 0x00089B50 File Offset: 0x00087D50
	public override int Compare(T a, T b)
	{
		foreach (Func<T, T, int> func in this.comparisons)
		{
			int num = func(a, b);
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x00089BB0 File Offset: 0x00087DB0
	public SRComparer<T> OrderBy<K>(Func<T, K> keyFunction)
	{
		this.comparisons.Add((T a, T b) => Comparer<K>.Default.Compare(keyFunction(a), keyFunction(b)));
		return this;
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x00089BE4 File Offset: 0x00087DE4
	public SRComparer<T> OrderByDescending<K>(Func<T, K> keyFunction)
	{
		this.comparisons.Add((T a, T b) => Comparer<K>.Default.Compare(keyFunction(b), keyFunction(a)));
		return this;
	}

	// Token: 0x040022C0 RID: 8896
	private List<Func<T, T, int>> comparisons = new List<Func<T, T, int>>();
}
