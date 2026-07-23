using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public static class TestUtil
{
	// Token: 0x06001B78 RID: 7032 RVA: 0x00003296 File Offset: 0x00001496
	public static void AreApproximatelyEqual(Vector3 vA, Vector3 vB, float tolerance, string message)
	{
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x00069378 File Offset: 0x00067578
	public static void AssertAreEqual<K, V>(Dictionary<K, V> expected, Dictionary<K, V> actual, Action<V, V> valueAssertion, string dictName)
	{
		foreach (KeyValuePair<K, V> keyValuePair in expected)
		{
			valueAssertion(keyValuePair.Value, actual[keyValuePair.Key]);
		}
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000693DC File Offset: 0x000675DC
	public static void AssertVersionedAreEqual<K, V1, V2>(Dictionary<K, V1> expected, Dictionary<K, V2> actual, Action<V1, V2> valueAssertion, string dictName)
	{
		foreach (KeyValuePair<K, V1> keyValuePair in expected)
		{
			valueAssertion(keyValuePair.Value, actual[keyValuePair.Key]);
		}
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x00069440 File Offset: 0x00067640
	public static void AssertAreEqual<T>(List<T> expected, List<T> actual, string field = "")
	{
		TestUtil.AssertAreEqual<T, T>(expected, actual, delegate(T a, T b, string m)
		{
		}, field);
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x0006946C File Offset: 0x0006766C
	public static void AssertAreEqual<T1, T2>(List<T1> expected, List<T2> actual, Action<T1, T2, string> valueAssertion, string field = "")
	{
		if (expected == null && actual == null)
		{
			return;
		}
		for (int i = 0; i < expected.Count; i++)
		{
			valueAssertion(expected[i], actual[i], string.Format("{0}[{1}]", field, i));
		}
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x000694B6 File Offset: 0x000676B6
	public static bool AssertNullness(object expected, object actual)
	{
		if (expected == null)
		{
			bool flag = actual == null;
		}
		return expected != null && actual != null;
	}

	// Token: 0x02000526 RID: 1318
	public class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		// Token: 0x06001B7E RID: 7038 RVA: 0x000694CC File Offset: 0x000676CC
		public SequenceComparer(IEqualityComparer<T> elemComparer = null)
		{
			this.elemComparer = elemComparer;
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x000694DB File Offset: 0x000676DB
		public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			return x.SequenceEqual(y, this.elemComparer);
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x000350A2 File Offset: 0x000332A2
		public int GetHashCode(IEnumerable<T> obj)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04001AB7 RID: 6839
		private IEqualityComparer<T> elemComparer;
	}

	// Token: 0x02000527 RID: 1319
	public class ListComparer<T> : IEqualityComparer<List<T>>
	{
		// Token: 0x06001B81 RID: 7041 RVA: 0x000694EA File Offset: 0x000676EA
		public ListComparer(IEqualityComparer<T> elemComparer = null)
		{
			this.elemComparer = elemComparer;
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x000694F9 File Offset: 0x000676F9
		public bool Equals(List<T> x, List<T> y)
		{
			return x.SequenceEqual(y, this.elemComparer);
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x000350A2 File Offset: 0x000332A2
		public int GetHashCode(List<T> obj)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04001AB8 RID: 6840
		private IEqualityComparer<T> elemComparer;
	}

	// Token: 0x02000528 RID: 1320
	public class ArrayComparer<T> : IEqualityComparer<T[]>
	{
		// Token: 0x06001B84 RID: 7044 RVA: 0x00069508 File Offset: 0x00067708
		public ArrayComparer(IEqualityComparer<T> elemComparer = null)
		{
			this.elemComparer = elemComparer;
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x00069517 File Offset: 0x00067717
		public bool Equals(T[] x, T[] y)
		{
			return x.SequenceEqual(y, this.elemComparer);
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x000350A2 File Offset: 0x000332A2
		public int GetHashCode(T[] obj)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04001AB9 RID: 6841
		private IEqualityComparer<T> elemComparer;
	}

	// Token: 0x02000529 RID: 1321
	public class DictComparer<K, V> : IEqualityComparer<IEnumerable<KeyValuePair<K, V>>>
	{
		// Token: 0x06001B87 RID: 7047 RVA: 0x00069526 File Offset: 0x00067726
		public DictComparer() : this(null, null)
		{
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x00069530 File Offset: 0x00067730
		public DictComparer(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valComparer)
		{
			this.keyValComparer = new TestUtil.KeyValueComparer<K, V>(keyComparer, valComparer);
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00069548 File Offset: 0x00067748
		public bool Equals(IEnumerable<KeyValuePair<K, V>> x, IEnumerable<KeyValuePair<K, V>> y)
		{
			if (x.Count<KeyValuePair<K, V>>() != y.Count<KeyValuePair<K, V>>())
			{
				return false;
			}
			List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>(y);
			foreach (KeyValuePair<K, V> x2 in x)
			{
				bool flag = false;
				foreach (KeyValuePair<K, V> keyValuePair in list)
				{
					if (this.keyValComparer.Equals(x2, keyValuePair))
					{
						flag = true;
						list.Remove(keyValuePair);
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x000350A2 File Offset: 0x000332A2
		public int GetHashCode(IEnumerable<KeyValuePair<K, V>> obj)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04001ABA RID: 6842
		private TestUtil.KeyValueComparer<K, V> keyValComparer;
	}

	// Token: 0x0200052A RID: 1322
	public class KeyValueComparer<K, V> : IEqualityComparer<KeyValuePair<K, V>>
	{
		// Token: 0x06001B8B RID: 7051 RVA: 0x00069604 File Offset: 0x00067804
		public KeyValueComparer(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valComparer)
		{
			this.keyComparer = keyComparer;
			this.valComparer = valComparer;
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x0006961C File Offset: 0x0006781C
		public bool Equals(KeyValuePair<K, V> x, KeyValuePair<K, V> y)
		{
			bool flag;
			if (this.keyComparer != null)
			{
				flag = this.keyComparer.Equals(x.Key, y.Key);
			}
			else
			{
				K key = x.Key;
				flag = key.Equals(y.Key);
			}
			if (!flag)
			{
				return false;
			}
			if (this.valComparer != null)
			{
				return this.valComparer.Equals(x.Value, y.Value);
			}
			V value = x.Value;
			return value.Equals(y.Value);
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x000696B4 File Offset: 0x000678B4
		public int GetHashCode(KeyValuePair<K, V> obj)
		{
			int hashCode;
			if (this.keyComparer != null)
			{
				hashCode = this.keyComparer.GetHashCode(obj.Key);
			}
			else
			{
				K key = obj.Key;
				hashCode = key.GetHashCode();
			}
			int hashCode2;
			if (this.valComparer != null)
			{
				hashCode2 = this.valComparer.GetHashCode(obj.Value);
			}
			else
			{
				V value = obj.Value;
				hashCode2 = value.GetHashCode();
			}
			return hashCode ^ hashCode2;
		}

		// Token: 0x04001ABB RID: 6843
		private IEqualityComparer<K> keyComparer;

		// Token: 0x04001ABC RID: 6844
		private IEqualityComparer<V> valComparer;
	}

	// Token: 0x0200052B RID: 1323
	public class Vector3Comparer : IEqualityComparer<Vector3>
	{
		// Token: 0x06001B8E RID: 7054 RVA: 0x00069724 File Offset: 0x00067924
		public Vector3Comparer(float tolerance = 0.001f, bool wraparound360 = false)
		{
			this.tolerance = tolerance;
			this.wraparound360 = wraparound360;
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x0006973C File Offset: 0x0006793C
		public bool Equals(Vector3 v1, Vector3 v2)
		{
			float num = Math.Abs(v1.x - v2.x);
			float num2 = Math.Abs(v1.y - v2.y);
			float num3 = Math.Abs(v1.z - v2.z);
			if (this.wraparound360)
			{
				while (num >= 360f - this.tolerance)
				{
					num -= 360f;
				}
				while (num2 >= 360f - this.tolerance)
				{
					num2 -= 360f;
				}
				while (num3 >= 360f - this.tolerance)
				{
					num3 -= 360f;
				}
			}
			return Math.Abs(num) <= this.tolerance && Math.Abs(num2) <= this.tolerance && Math.Abs(num3) <= this.tolerance;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00069804 File Offset: 0x00067A04
		public int GetHashCode(Vector3 obj)
		{
			return Math.Round((double)obj.x).GetHashCode() ^ Math.Round((double)obj.y).GetHashCode() ^ Math.Round((double)obj.z).GetHashCode();
		}

		// Token: 0x04001ABD RID: 6845
		private float tolerance;

		// Token: 0x04001ABE RID: 6846
		private bool wraparound360;
	}
}
