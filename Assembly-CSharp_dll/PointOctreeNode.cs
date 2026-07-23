using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class PointOctreeNode<T> where T : class
{
	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06002300 RID: 8960 RVA: 0x00087D83 File Offset: 0x00085F83
	// (set) Token: 0x06002301 RID: 8961 RVA: 0x00087D8B File Offset: 0x00085F8B
	public Vector3 Center { get; private set; }

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06002302 RID: 8962 RVA: 0x00087D94 File Offset: 0x00085F94
	// (set) Token: 0x06002303 RID: 8963 RVA: 0x00087D9C File Offset: 0x00085F9C
	public float SideLength { get; private set; }

	// Token: 0x06002304 RID: 8964 RVA: 0x00087DA5 File Offset: 0x00085FA5
	public PointOctreeNode(float baseLengthVal, float minSizeVal, Vector3 centerVal)
	{
		this.SetValues(baseLengthVal, minSizeVal, centerVal);
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x00087DC1 File Offset: 0x00085FC1
	public bool Add(T obj, Vector3 objPos)
	{
		if (!PointOctreeNode<T>.Encapsulates(this.bounds, objPos))
		{
			return false;
		}
		this.SubAdd(obj, objPos);
		return true;
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x00087DDC File Offset: 0x00085FDC
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

	// Token: 0x06002307 RID: 8967 RVA: 0x00087E84 File Offset: 0x00086084
	public T[] GetNearby(Ray ray, float maxDistance)
	{
		this.bounds.Expand(new Vector3(maxDistance, maxDistance, maxDistance));
		bool flag = this.bounds.IntersectRay(ray);
		this.bounds.size = this.actualBoundsSize;
		if (!flag)
		{
			return new T[0];
		}
		List<T> list = new List<T>();
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (PointOctreeNode<T>.DistanceToRay(ray, this.objects[i].Pos) <= maxDistance)
			{
				list.Add(this.objects[i].Obj);
			}
		}
		if (this.children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				T[] nearby = this.children[j].GetNearby(ray, maxDistance);
				if (nearby != null)
				{
					list.AddRange(nearby);
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x00087F4B File Offset: 0x0008614B
	public void SetChildren(PointOctreeNode<T>[] childOctrees)
	{
		if (childOctrees.Length != 8)
		{
			Log.Error("Child octree array must be length 8. Was length: " + childOctrees.Length, Array.Empty<object>());
			return;
		}
		this.children = childOctrees;
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x00087F78 File Offset: 0x00086178
	public void DrawAllBounds(float depth = 0f)
	{
		float num = depth / 7f;
		Gizmos.color = new Color(num, 0f, 1f - num);
		Bounds bounds = new Bounds(this.Center, new Vector3(this.SideLength, this.SideLength, this.SideLength));
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

	// Token: 0x0600230A RID: 8970 RVA: 0x00088010 File Offset: 0x00086210
	public void DrawAllObjects()
	{
		float num = this.SideLength / 20f;
		Gizmos.color = new Color(0f, 1f - num, num, 0.25f);
		foreach (PointOctreeNode<T>.OctreeObject octreeObject in this.objects)
		{
			Gizmos.DrawIcon(octreeObject.Pos, "marker.tif", true);
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

	// Token: 0x0600230B RID: 8971 RVA: 0x000880C0 File Offset: 0x000862C0
	public PointOctreeNode<T> ShrinkIfPossible(float minLength)
	{
		if (this.SideLength < 2f * minLength)
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
			PointOctreeNode<T>.OctreeObject octreeObject = this.objects[i];
			int num2 = this.BestFitChild(octreeObject.Pos);
			if (i != 0 && num2 != num)
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
			this.SetValues(this.SideLength / 2f, this.minSize, this.childBounds[num].center);
			return this;
		}
		return this.children[num];
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000881C0 File Offset: 0x000863C0
	private void SetValues(float baseLengthVal, float minSizeVal, Vector3 centerVal)
	{
		this.SideLength = baseLengthVal;
		this.minSize = minSizeVal;
		this.Center = centerVal;
		this.actualBoundsSize = new Vector3(this.SideLength, this.SideLength, this.SideLength);
		this.bounds = new Bounds(this.Center, this.actualBoundsSize);
		float num = this.SideLength / 4f;
		float num2 = this.SideLength / 2f;
		Vector3 size = new Vector3(num2, num2, num2);
		this.childBounds = new Bounds[8];
		this.childBounds[0] = new Bounds(this.Center + new Vector3(-num, num, -num), size);
		this.childBounds[1] = new Bounds(this.Center + new Vector3(num, num, -num), size);
		this.childBounds[2] = new Bounds(this.Center + new Vector3(-num, num, num), size);
		this.childBounds[3] = new Bounds(this.Center + new Vector3(num, num, num), size);
		this.childBounds[4] = new Bounds(this.Center + new Vector3(-num, -num, -num), size);
		this.childBounds[5] = new Bounds(this.Center + new Vector3(num, -num, -num), size);
		this.childBounds[6] = new Bounds(this.Center + new Vector3(-num, -num, num), size);
		this.childBounds[7] = new Bounds(this.Center + new Vector3(num, -num, num), size);
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x0008837C File Offset: 0x0008657C
	private void SubAdd(T obj, Vector3 objPos)
	{
		if (this.objects.Count < 8 || this.SideLength / 2f < this.minSize)
		{
			PointOctreeNode<T>.OctreeObject item = new PointOctreeNode<T>.OctreeObject
			{
				Obj = obj,
				Pos = objPos
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
				PointOctreeNode<T>.OctreeObject octreeObject = this.objects[i];
				num = this.BestFitChild(octreeObject.Pos);
				this.children[num].SubAdd(octreeObject.Obj, octreeObject.Pos);
				this.objects.Remove(octreeObject);
			}
		}
		num = this.BestFitChild(objPos);
		this.children[num].SubAdd(obj, objPos);
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x0008845C File Offset: 0x0008665C
	private void Split()
	{
		float num = this.SideLength / 4f;
		float baseLengthVal = this.SideLength / 2f;
		this.children = new PointOctreeNode<T>[8];
		this.children[0] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, num, -num));
		this.children[1] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, num, -num));
		this.children[2] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, num, num));
		this.children[3] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, num, num));
		this.children[4] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, -num, -num));
		this.children[5] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, -num, -num));
		this.children[6] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, -num, num));
		this.children[7] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, -num, num));
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000885D4 File Offset: 0x000867D4
	private void Merge()
	{
		for (int i = 0; i < 8; i++)
		{
			PointOctreeNode<T> pointOctreeNode = this.children[i];
			for (int j = pointOctreeNode.objects.Count - 1; j >= 0; j--)
			{
				PointOctreeNode<T>.OctreeObject item = pointOctreeNode.objects[j];
				this.objects.Add(item);
			}
		}
		this.children = null;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x0008862E File Offset: 0x0008682E
	private static bool Encapsulates(Bounds outerBounds, Vector3 point)
	{
		return outerBounds.Contains(point);
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x00088638 File Offset: 0x00086838
	private int BestFitChild(Vector3 objPos)
	{
		return ((objPos.x <= this.Center.x) ? 0 : 1) + ((objPos.y >= this.Center.y) ? 0 : 4) + ((objPos.z <= this.Center.z) ? 0 : 2);
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x0008868C File Offset: 0x0008688C
	private bool ShouldMerge()
	{
		int num = this.objects.Count;
		if (this.children != null)
		{
			foreach (PointOctreeNode<T> pointOctreeNode in this.children)
			{
				if (pointOctreeNode.children != null)
				{
					return false;
				}
				num += pointOctreeNode.objects.Count;
			}
		}
		return num <= 8;
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000886E8 File Offset: 0x000868E8
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

	// Token: 0x06002314 RID: 8980 RVA: 0x0008872C File Offset: 0x0008692C
	public static float DistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	// Token: 0x04002284 RID: 8836
	private float minSize;

	// Token: 0x04002285 RID: 8837
	private Bounds bounds;

	// Token: 0x04002286 RID: 8838
	private readonly List<PointOctreeNode<T>.OctreeObject> objects = new List<PointOctreeNode<T>.OctreeObject>();

	// Token: 0x04002287 RID: 8839
	private PointOctreeNode<T>[] children;

	// Token: 0x04002288 RID: 8840
	private Bounds[] childBounds;

	// Token: 0x04002289 RID: 8841
	private const int NUM_OBJECTS_ALLOWED = 8;

	// Token: 0x0400228A RID: 8842
	private Vector3 actualBoundsSize;

	// Token: 0x02000692 RID: 1682
	private class OctreeObject
	{
		// Token: 0x0400228B RID: 8843
		public T Obj;

		// Token: 0x0400228C RID: 8844
		public Vector3 Pos;
	}
}
