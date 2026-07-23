using System;
using System.Collections.Generic;

// Token: 0x020000A2 RID: 162
public class SECTR_PriorityQueue<T> where T : IComparable<T>
{
	// Token: 0x06000386 RID: 902 RVA: 0x00016A33 File Offset: 0x00014C33
	public SECTR_PriorityQueue()
	{
		this.data = new List<T>(64);
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00016A48 File Offset: 0x00014C48
	public SECTR_PriorityQueue(int capacity)
	{
		this.data = new List<T>(capacity);
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000388 RID: 904 RVA: 0x00016A5C File Offset: 0x00014C5C
	// (set) Token: 0x06000389 RID: 905 RVA: 0x00003296 File Offset: 0x00001496
	public int Count
	{
		get
		{
			return this.data.Count;
		}
		set
		{
		}
	}

	// Token: 0x17000091 RID: 145
	public T this[int index]
	{
		get
		{
			if (index >= this.data.Count)
			{
				return default(T);
			}
			return this.data[index];
		}
		set
		{
			if (index < this.data.Count)
			{
				this.data[index] = value;
				this._Update(index);
			}
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00016AC4 File Offset: 0x00014CC4
	public void Enqueue(T item)
	{
		this.data.Add(item);
		int num;
		for (int i = this.data.Count - 1; i > 0; i = num)
		{
			num = (i - 1) / 2;
			T t = this.data[i];
			if (t.CompareTo(this.data[num]) >= 0)
			{
				break;
			}
			this._SwapElements(i, num);
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00016B2C File Offset: 0x00014D2C
	public T Dequeue()
	{
		int num = this.data.Count - 1;
		T result = this.data[0];
		this.data[0] = this.data[num];
		this.data.RemoveAt(num);
		num--;
		int num2 = 0;
		for (;;)
		{
			int num3 = num2 * 2 + 1;
			if (num3 > num)
			{
				break;
			}
			int num4 = num3 + 1;
			T t;
			if (num4 <= num)
			{
				t = this.data[num4];
				if (t.CompareTo(this.data[num3]) < 0)
				{
					num3 = num4;
				}
			}
			t = this.data[num2];
			if (t.CompareTo(this.data[num3]) <= 0)
			{
				break;
			}
			this._SwapElements(num2, num3);
			num2 = num3;
		}
		return result;
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00016BF8 File Offset: 0x00014DF8
	public T Peek()
	{
		if (this.data.Count <= 0)
		{
			return default(T);
		}
		return this.data[0];
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00016C2C File Offset: 0x00014E2C
	public override string ToString()
	{
		string text = "";
		for (int i = 0; i < this.data.Count; i++)
		{
			string str = text;
			T t = this.data[i];
			text = str + t.ToString() + " ";
		}
		return text + "count = " + this.data.Count;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00016C98 File Offset: 0x00014E98
	public bool IsConsistent()
	{
		if (this.data.Count > 0)
		{
			int num = this.data.Count - 1;
			for (int i = 0; i < this.data.Count; i++)
			{
				int num2 = 2 * i + 1;
				int num3 = 2 * i + 2;
				if (num2 <= num)
				{
					T t = this.data[i];
					if (t.CompareTo(this.data[num2]) > 0)
					{
						return false;
					}
				}
				if (num3 <= num)
				{
					T t = this.data[i];
					if (t.CompareTo(this.data[num3]) > 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00016D45 File Offset: 0x00014F45
	public void Clear()
	{
		this.data.Clear();
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00016D54 File Offset: 0x00014F54
	private void _SwapElements(int i, int j)
	{
		T value = this.data[i];
		this.data[i] = this.data[j];
		this.data[j] = value;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00016D94 File Offset: 0x00014F94
	private void _Update(int i)
	{
		int j;
		int num;
		for (j = i; j > 0; j = num)
		{
			num = (j - 1) / 2;
			T t = this.data[j];
			if (t.CompareTo(this.data[num]) >= 0)
			{
				break;
			}
			this._SwapElements(j, num);
		}
		if (j >= i)
		{
			for (;;)
			{
				int j2 = j;
				int num2 = 2 * j + 1;
				int num3 = 2 * j + 2;
				T t;
				if (this.data.Count > num2)
				{
					t = this.data[j];
					if (t.CompareTo(this.data[num2]) > 0)
					{
						this._SwapElements(num2, j2);
						j = num2;
						continue;
					}
				}
				if (this.data.Count <= num3)
				{
					break;
				}
				t = this.data[j];
				if (t.CompareTo(this.data[num3]) <= 0)
				{
					break;
				}
				this._SwapElements(num3, j2);
				j = num3;
			}
		}
	}

	// Token: 0x040003AF RID: 943
	private List<T> data;
}
