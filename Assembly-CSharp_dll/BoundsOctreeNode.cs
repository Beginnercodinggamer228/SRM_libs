using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class BoundsOctreeNode<T>
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060022B8 RID: 8888 RVA: 0x0008601F File Offset: 0x0008421F
	// (set) Token: 0x060022B9 RID: 8889 RVA: 0x00086027 File Offset: 0x00084227
	public Vector3 Center { get; private set; }

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060022BA RID: 8890 RVA: 0x00086030 File Offset: 0x00084230
	// (set) Token: 0x060022BB RID: 8891 RVA: 0x00086038 File Offset: 0x00084238
	public float BaseLength { get; private set; }

	// Token: 0x060022BC RID: 8892 RVA: 0x00086041 File Offset: 0x00084241
	public BoundsOctreeNode(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		this.SetValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x0008605F File Offset: 0x0008425F
	public bool Add(T obj, Bounds objBounds)
	{
		if (!BoundsOctreeNode<T>.Encapsulates(this.bounds, objBounds))
		{
			return false;
		}
		this.SubAdd(obj, objBounds);
		return true;
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x0008607C File Offset: 0x0008427C
	public bool Remove(T obj)
	{
		bool flag = false;
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i].Obj.Equals(obj))
			{
				flag = this.objects.Remove(this.objects[i]);
				break;
			}
		}
		if (!flag && this.children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				flag = this.children[j].Remove(obj);
				if (flag)
				{
					break;
				}
			}
		}
		if (flag && this.children != null && this.ShouldMerge())
		{
			this.Merge();
		}
		return flag;
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x00086124 File Offset: 0x00084324
	public bool IsColliding(Bounds checkBounds)
	{
		if (!this.bounds.Intersects(checkBounds))
		{
			return false;
		}
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i].Bounds.Intersects(checkBounds))
			{
				return true;
			}
		}
		if (this.children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				if (this.children[j].IsColliding(checkBounds))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x0008619C File Offset: 0x0008439C
	public T[] GetColliding(Bounds checkBounds)
	{
		List<T> list = new List<T>();
		if (!this.bounds.Intersects(checkBounds))
		{
			return list.ToArray();
		}
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i].Bounds.Intersects(checkBounds))
			{
				list.Add(this.objects[i].Obj);
			}
		}
		if (this.children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				T[] colliding = this.children[j].GetColliding(checkBounds);
				if (colliding != null)
				{
					list.AddRange(colliding);
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x0008623D File Offset: 0x0008443D
	public void SetChildren(BoundsOctreeNode<T>[] childOctrees)
	{
		if (childOctrees.Length != 8)
		{
			Log.Error("Child octree array must be length 8. Was length: " + childOctrees.Length, Array.Empty<object>());
			return;
		}
		this.children = childOctrees;
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x0008626C File Offset: 0x0008446C
	public void DrawAllBounds(float depth = 0f)
	{
		float num = depth / 7f;
		Gizmos.color = new Color(num, 0f, 1f - num);
		Bounds bounds = new Bounds(this.Center, new Vector3(this.adjLength, this.adjLength, this.adjLength));
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		if (this.children != null)
		{
			depth += 1f;
			for (int i = 0; i < 8; i++)
			{
				this.children[i].DrawAllBounds(depth);
			}
		}
		Gizmos.color = Color.white;
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x00086304 File Offset: 0x00084504
	public void DrawAllObjects()
	{
		float num = this.BaseLength / 20f;
		Gizmos.color = new Color(0f, 1f - num, num, 0.25f);
		foreach (BoundsOctreeNode<T>.OctreeObject octreeObject in this.objects)
		{
			Gizmos.DrawCube(octreeObject.Bounds.center, octreeObject.Bounds.size);
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(octreeObject.Bounds.center, this.bounds.center);
		}
		if (this.children != null)
		{
			for (int i = 0; i < 8; i++)
			{
				this.children[i].DrawAllObjects();
			}
		}
		Gizmos.color = Color.white;
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000863E4 File Offset: 0x000845E4
	public BoundsOctreeNode<T> ShrinkIfPossible(float minLength)
	{
		if (this.BaseLength < 2f * minLength)
		{
			return this;
		}
		if (this.objects.Count == 0 && this.children.Length == 0)
		{
			return this;
		}
		int num = -1;
		for (int i = 0; i < this.objects.Count; i++)
		{
			BoundsOctreeNode<T>.OctreeObject octreeObject = this.objects[i];
			int num2 = this.BestFitChild(octreeObject.Bounds);
			if (i != 0 && num2 != num)
			{
				return this;
			}
			if (!BoundsOctreeNode<T>.Encapsulates(this.childBounds[num2], octreeObject.Bounds))
			{
				return this;
			}
			if (num < 0)
			{
				num = num2;
			}
		}
		if (this.children != null)
		{
			bool flag = false;
			for (int j = 0; j < this.children.Length; j++)
			{
				if (this.children[j].HasAnyObjects())
				{
					if (flag)
					{
						return this;
					}
					if (num >= 0 && num != j)
					{
						return this;
					}
					flag = true;
					num = j;
				}
			}
		}
		if (this.children == null)
		{
			this.SetValues(this.BaseLength / 2f, this.minSize, this.looseness, this.childBounds[num].center);
			return this;
		}
		return this.children[num];
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x00086504 File Offset: 0x00084704
	private void SetValues(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		this.BaseLength = baseLengthVal;
		this.minSize = minSizeVal;
		this.looseness = loosenessVal;
		this.Center = centerVal;
		this.adjLength = this.looseness * baseLengthVal;
		Vector3 size = new Vector3(this.adjLength, this.adjLength, this.adjLength);
		this.bounds = new Bounds(this.Center, size);
		float num = this.BaseLength / 4f;
		float num2 = this.BaseLength / 2f * this.looseness;
		Vector3 size2 = new Vector3(num2, num2, num2);
		this.childBounds = new Bounds[8];
		this.childBounds[0] = new Bounds(this.Center + new Vector3(-num, num, -num), size2);
		this.childBounds[1] = new Bounds(this.Center + new Vector3(num, num, -num), size2);
		this.childBounds[2] = new Bounds(this.Center + new Vector3(-num, num, num), size2);
		this.childBounds[3] = new Bounds(this.Center + new Vector3(num, num, num), size2);
		this.childBounds[4] = new Bounds(this.Center + new Vector3(-num, -num, -num), size2);
		this.childBounds[5] = new Bounds(this.Center + new Vector3(num, -num, -num), size2);
		this.childBounds[6] = new Bounds(this.Center + new Vector3(-num, -num, num), size2);
		this.childBounds[7] = new Bounds(this.Center + new Vector3(num, -num, num), size2);
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000866D4 File Offset: 0x000848D4
	private void SubAdd(T obj, Bounds objBounds)
	{
		if (this.objects.Count < 8 || this.BaseLength / 2f < this.minSize)
		{
			BoundsOctreeNode<T>.OctreeObject item = new BoundsOctreeNode<T>.OctreeObject
			{
				Obj = obj,
				Bounds = objBounds
			};
			this.objects.Add(item);
			return;
		}
		int num;
		if (this.children == null)
		{
			this.Split();
			if (this.children == null)
			{
				Debug.Log("Child creation failed for an unknown reason. Early exit.");
				return;
			}
			for (int i = this.objects.Count - 1; i >= 0; i--)
			{
				BoundsOctreeNode<T>.OctreeObject octreeObject = this.objects[i];
				num = this.BestFitChild(octreeObject.Bounds);
				if (BoundsOctreeNode<T>.Encapsulates(this.children[num].bounds, octreeObject.Bounds))
				{
					this.children[num].SubAdd(octreeObject.Obj, octreeObject.Bounds);
					this.objects.Remove(octreeObject);
				}
			}
		}
		num = this.BestFitChild(objBounds);
		if (BoundsOctreeNode<T>.Encapsulates(this.children[num].bounds, objBounds))
		{
			this.children[num].SubAdd(obj, objBounds);
			return;
		}
		BoundsOctreeNode<T>.OctreeObject item2 = new BoundsOctreeNode<T>.OctreeObject
		{
			Obj = obj,
			Bounds = objBounds
		};
		this.objects.Add(item2);
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x0008680C File Offset: 0x00084A0C
	private void Split()
	{
		float num = this.BaseLength / 4f;
		float baseLengthVal = this.BaseLength / 2f;
		this.children = new BoundsOctreeNode<T>[8];
		this.children[0] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, num, -num));
		this.children[1] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, num, -num));
		this.children[2] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, num, num));
		this.children[3] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, num, num));
		this.children[4] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, -num, -num));
		this.children[5] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, -num, -num));
		this.children[6] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, -num, num));
		this.children[7] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, -num, num));
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000869B4 File Offset: 0x00084BB4
	private void Merge()
	{
		for (int i = 0; i < 8; i++)
		{
			BoundsOctreeNode<T> boundsOctreeNode = this.children[i];
			for (int j = boundsOctreeNode.objects.Count - 1; j >= 0; j--)
			{
				BoundsOctreeNode<T>.OctreeObject item = boundsOctreeNode.objects[j];
				this.objects.Add(item);
			}
		}
		this.children = null;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x00014137 File Offset: 0x00012337
	private static bool Encapsulates(Bounds outerBounds, Bounds innerBounds)
	{
		return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x00086A10 File Offset: 0x00084C10
	private int BestFitChild(Bounds objBounds)
	{
		return ((objBounds.center.x <= this.Center.x) ? 0 : 1) + ((objBounds.center.y >= this.Center.y) ? 0 : 4) + ((objBounds.center.z <= this.Center.z) ? 0 : 2);
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x00086A78 File Offset: 0x00084C78
	private bool ShouldMerge()
	{
		int num = this.objects.Count;
		if (this.children != null)
		{
			foreach (BoundsOctreeNode<T> boundsOctreeNode in this.children)
			{
				if (boundsOctreeNode.children != null)
				{
					return false;
				}
				num += boundsOctreeNode.objects.Count;
			}
		}
		return num <= 8;
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x00086AD4 File Offset: 0x00084CD4
	private bool HasAnyObjects()
	{
		if (this.objects.Count > 0)
		{
			return true;
		}
		if (this.children != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.children[i].HasAnyObjects())
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04002262 RID: 8802
	private float looseness;

	// Token: 0x04002263 RID: 8803
	private float minSize;

	// Token: 0x04002264 RID: 8804
	private float adjLength;

	// Token: 0x04002265 RID: 8805
	private Bounds bounds;

	// Token: 0x04002266 RID: 8806
	private readonly List<BoundsOctreeNode<T>.OctreeObject> objects = new List<BoundsOctreeNode<T>.OctreeObject>();

	// Token: 0x04002267 RID: 8807
	private BoundsOctreeNode<T>[] children;

	// Token: 0x04002268 RID: 8808
	private Bounds[] childBounds;

	// Token: 0x04002269 RID: 8809
	private const int numObjectsAllowed = 8;

	// Token: 0x0200068C RID: 1676
	private class OctreeObject
	{
		// Token: 0x0400226A RID: 8810
		public T Obj;

		// Token: 0x0400226B RID: 8811
		public Bounds Bounds;
	}
}
