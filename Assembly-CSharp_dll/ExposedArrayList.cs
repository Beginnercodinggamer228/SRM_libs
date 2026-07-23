using System;

// Token: 0x0200067E RID: 1662
public class ExposedArrayList<T>
{
	// Token: 0x0600225E RID: 8798 RVA: 0x00084DAF File Offset: 0x00082FAF
	public ExposedArrayList() : this(1000)
	{
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x00084DBC File Offset: 0x00082FBC
	public ExposedArrayList(int startSize)
	{
		this.Data = new T[startSize];
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x00084DD0 File Offset: 0x00082FD0
	public int GetCount()
	{
		return this._count;
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x00084DD8 File Offset: 0x00082FD8
	public void Add(T item)
	{
		if (this._count >= this.Data.Length)
		{
			Array.Resize<T>(ref this.Data, this.Data.Length + 100);
		}
		this.Data[this._count] = item;
		this._count++;
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x00084E2C File Offset: 0x0008302C
	public void Remove(T item)
	{
		int num = this.IndexOf(item);
		if (num > -1 && num <= this._count)
		{
			this.Data[num] = this.Data[this._count - 1];
			this.Data[this._count - 1] = default(T);
			this._count--;
		}
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x00084E98 File Offset: 0x00083098
	public void Clear()
	{
		for (int i = 0; i < this._count; i++)
		{
			this.Data[i] = default(T);
		}
		this._count = 0;
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x00084ED4 File Offset: 0x000830D4
	public int IndexOf(T item)
	{
		for (int i = 0; i < this._count; i++)
		{
			if (item.Equals(this.Data[i]))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x04002234 RID: 8756
	private const int ARRAY_START_SIZE = 1000;

	// Token: 0x04002235 RID: 8757
	private const int ARRAY_GROWTH_SIZE = 100;

	// Token: 0x04002236 RID: 8758
	public T[] Data;

	// Token: 0x04002237 RID: 8759
	private int _count;
}
