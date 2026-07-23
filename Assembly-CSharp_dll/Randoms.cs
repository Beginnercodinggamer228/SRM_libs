using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000696 RID: 1686
public class Randoms
{
	// Token: 0x06002326 RID: 8998 RVA: 0x00088D0C File Offset: 0x00086F0C
	public Randoms()
	{
		this.rand = new Random();
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x00088D2A File Offset: 0x00086F2A
	public Randoms(int seed)
	{
		this.rand = new Random(seed);
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x00088D49 File Offset: 0x00086F49
	public int GetInt(int high)
	{
		return this.NextInt(high);
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x00088D52 File Offset: 0x00086F52
	public int GetInt()
	{
		return this.rand.Next();
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x00088D5F File Offset: 0x00086F5F
	public int GetInRange(int low, int high)
	{
		if (low == high)
		{
			return low;
		}
		return low + this.NextInt(high - low);
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x00088D72 File Offset: 0x00086F72
	public float GetFloat(float high)
	{
		return (float)(this.rand.NextDouble() * (double)high);
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x00088D83 File Offset: 0x00086F83
	public float GetInRange(float low, float high)
	{
		if (low == high)
		{
			return low;
		}
		return (float)((double)low + this.rand.NextDouble() * (double)(high - low));
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x00088D9F File Offset: 0x00086F9F
	public bool GetChance(int n)
	{
		return this.NextInt(n) == 0;
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x00088DAB File Offset: 0x00086FAB
	public bool GetProbability(float p)
	{
		return this.rand.NextDouble() < (double)p;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x00088DBC File Offset: 0x00086FBC
	public bool GetBoolean()
	{
		return this.NextInt(2) == 0;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x00088DC8 File Offset: 0x00086FC8
	public float GetNormal(float mean, float dev)
	{
		return (float)this.NextGaussian() * dev + mean;
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x00088DD5 File Offset: 0x00086FD5
	public T Pick<T>(T[] vals)
	{
		return vals[this.GetInt(vals.Length)];
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x00088DE8 File Offset: 0x00086FE8
	public T Pick<T>(IEnumerator<T> iterator, T ifEmpty)
	{
		if (!iterator.MoveNext())
		{
			return ifEmpty;
		}
		T result = iterator.Current;
		int num = 2;
		while (iterator.Current != null && iterator.MoveNext())
		{
			T t = iterator.Current;
			if (this.NextInt(num) == 0)
			{
				result = t;
			}
			num++;
		}
		return result;
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x00088E36 File Offset: 0x00087036
	public T Pick<T>(IEnumerable<T> enumerable, T ifEmpty)
	{
		return this.Pick<T>(enumerable.GetEnumerator(), ifEmpty);
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x00088E45 File Offset: 0x00087045
	public IEnumerable<T> Pick<T>(List<T> collection, int count)
	{
		List<int> options = Enumerable.Range(0, collection.Count<T>()).ToList<int>();
		int ii = 0;
		while (ii < count && options.Any<int>())
		{
			yield return collection[this.Pluck<int>(options, 0)];
			int num = ii + 1;
			ii = num;
		}
		yield break;
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x00088E63 File Offset: 0x00087063
	public IEnumerable<T> Pick<T>(List<T> collection, int count, Func<T, float> weightFunction)
	{
		List<int> options = Enumerable.Range(0, collection.Count<T>()).ToList<int>();
		Func<int, float> optionsWeightFunction = (int idx) => weightFunction(collection[idx]);
		int ii = 0;
		while (ii < count && options.Any<int>())
		{
			int randomIndex = this.Pick<int>(options, optionsWeightFunction, -1);
			if (randomIndex == -1)
			{
				break;
			}
			yield return collection[randomIndex];
			options.Remove(randomIndex);
			int num = ii + 1;
			ii = num;
		}
		yield break;
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x00088E88 File Offset: 0x00087088
	public IEnumerable<T> Pick<T>(List<T> collection, int min, int max)
	{
		return this.Pick<T>(collection, this.GetInRange(min, max));
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x00088E99 File Offset: 0x00087099
	public T Pick<T>(ICollection<T> iterable, T ifEmpty)
	{
		return this.PickPluck<T>(iterable, ifEmpty, false);
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x00088EA4 File Offset: 0x000870A4
	public T Pick<T>(IDictionary<T, float> weightMap, T ifEmpty)
	{
		T result = ifEmpty;
		float num = 0f;
		foreach (KeyValuePair<T, float> keyValuePair in weightMap)
		{
			float value = keyValuePair.Value;
			if ((double)value > 0.0)
			{
				num += value;
				if (num == value || this.GetFloat(num) < value)
				{
					result = keyValuePair.Key;
				}
			}
			else if ((double)value < 0.0)
			{
				throw new ArgumentException("weightMap", "Weight less than 0: " + keyValuePair);
			}
		}
		return result;
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x00088F50 File Offset: 0x00087150
	public T Pick<T>(ICollection<T> iterable, Func<T, float> weightFunction, T ifEmpty)
	{
		T result = ifEmpty;
		float num = 0f;
		foreach (T t in iterable)
		{
			float num2 = weightFunction(t);
			if ((double)num2 > 0.0)
			{
				num += num2;
				if (num == num2 || this.GetFloat(num) < num2)
				{
					result = t;
				}
			}
			else if ((double)num2 < 0.0)
			{
				throw new ArgumentException("weightMap", "Weight less than 0: " + t);
			}
		}
		return result;
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x00088FF4 File Offset: 0x000871F4
	public T Pluck<T>(ICollection<T> iterable, T ifEmpty)
	{
		return this.PickPluck<T>(iterable, ifEmpty, true);
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x00089000 File Offset: 0x00087200
	protected T PickPluck<T>(ICollection<T> coll, T ifEmpty, bool remove)
	{
		int count = coll.Count;
		if (count == 0)
		{
			return ifEmpty;
		}
		if (coll is IList<T>)
		{
			IList<T> list = (IList<T>)coll;
			int index = this.NextInt(count);
			T result = list[index];
			if (remove)
			{
				list.RemoveAt(index);
			}
			return result;
		}
		IEnumerator<T> enumerator = coll.GetEnumerator();
		enumerator.MoveNext();
		for (int i = this.NextInt(count); i > 0; i--)
		{
			enumerator.MoveNext();
		}
		T result2;
		try
		{
			result2 = enumerator.Current;
		}
		finally
		{
			if (remove)
			{
				coll.Remove(enumerator.Current);
			}
		}
		return result2;
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x0008909C File Offset: 0x0008729C
	protected static void Swap<T>(IList<T> list, int ii, int jj)
	{
		T value = list[jj];
		list[jj] = list[ii];
		list[ii] = value;
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000890C8 File Offset: 0x000872C8
	protected static void Swap<T>(T[] array, int ii, int jj)
	{
		T t = array[ii];
		array[ii] = array[jj];
		array[jj] = t;
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000890F4 File Offset: 0x000872F4
	private int NextInt(int n)
	{
		if (n <= 0)
		{
			throw new ArgumentOutOfRangeException("n", "must be positive");
		}
		int num;
		int num2;
		do
		{
			num = this.rand.Next();
			num2 = num % n;
		}
		while (num - num2 + (n - 1) < 0);
		return num2;
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x00089130 File Offset: 0x00087330
	private double NextGaussian()
	{
		object obj = this.gaussLock;
		double result;
		lock (obj)
		{
			if (this.haveNextNextGaussian)
			{
				this.haveNextNextGaussian = false;
				result = this.nextNextGaussian;
			}
			else
			{
				double num;
				double num2;
				double num3;
				do
				{
					num = 2.0 * this.rand.NextDouble() - 1.0;
					num2 = 2.0 * this.rand.NextDouble() - 1.0;
					num3 = num * num + num2 * num2;
				}
				while (num3 >= 1.0 || num3 == 0.0);
				double num4 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
				this.nextNextGaussian = num2 * num4;
				this.haveNextNextGaussian = true;
				result = num * num4;
			}
		}
		return result;
	}

	// Token: 0x04002293 RID: 8851
	public static Randoms SHARED = new Randoms();

	// Token: 0x04002294 RID: 8852
	private readonly Random rand;

	// Token: 0x04002295 RID: 8853
	private double nextNextGaussian;

	// Token: 0x04002296 RID: 8854
	private bool haveNextNextGaussian;

	// Token: 0x04002297 RID: 8855
	private object gaussLock = new object();
}
