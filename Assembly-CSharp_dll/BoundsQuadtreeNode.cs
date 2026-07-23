using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class BoundsQuadtreeNode<T>
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060022DC RID: 8924 RVA: 0x00086E63 File Offset: 0x00085063
	// (set) Token: 0x060022DD RID: 8925 RVA: 0x00086E6B File Offset: 0x0008506B
	public Vector3 Center { get; private set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060022DE RID: 8926 RVA: 0x00086E74 File Offset: 0x00085074
	// (set) Token: 0x060022DF RID: 8927 RVA: 0x00086E7C File Offset: 0x0008507C
	public float BaseLength { get; private set; }

	// Token: 0x060022E0 RID: 8928 RVA: 0x00086E85 File Offset: 0x00085085
	public BoundsQuadtreeNode(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		this.SetValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x00086EA3 File Offset: 0x000850A3
	public bool Add(T obj, Bounds objBounds)
	{
		if (!BoundsQuadtreeNode<T>.Encapsulates(this.bounds, objBounds))
		{
			return false;
		}
		this.SubAdd(obj, objBounds);
		return true;
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x00086EC0 File Offset: 0x000850C0
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
			for (int j = 0; j < this.children.Length; j++)
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

	// Token: 0x060022E3 RID: 8931 RVA: 0x00086F70 File Offset: 0x00085170
	public bool IsColliding(Bounds checkBounds)
	{
		if (this.objects.Count == 0 && (this.children == null || this.children.Length == 0))
		{
			return false;
		}
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
			for (int j = 0; j < this.children.Length; j++)
			{
				if (this.children[j].IsColliding(checkBounds))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x0008700C File Offset: 0x0008520C
	public bool IntersectsBounds(Bounds checkBounds)
	{
		return this.bounds.Intersects(checkBounds);
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x0008701A File Offset: 0x0008521A
	public bool ContainsPoint(Vector3 checkPoint)
	{
		return this.bounds.Contains(checkPoint);
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x00087028 File Offset: 0x00085228
	public void GetColliding(Bounds checkBounds, ref List<T> result)
	{
		if (this.objects.Count == 0 && (this.children == null || this.children.Length == 0))
		{
			return;
		}
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i].Bounds.Intersects(checkBounds))
			{
				result.Add(this.objects[i].Obj);
			}
		}
		if (this.children != null)
		{
			bool flag = checkBounds.min.y <= this.children[0].bounds.max.y;
			bool flag2 = checkBounds.max.y >= this.children[2].bounds.min.y;
			bool flag3 = checkBounds.min.x <= this.children[0].bounds.max.x;
			bool flag4 = checkBounds.max.x >= this.children[1].bounds.min.x;
			if (flag3)
			{
				if (flag)
				{
					this.children[0].GetColliding(checkBounds, ref result);
				}
				if (flag2)
				{
					this.children[2].GetColliding(checkBounds, ref result);
				}
			}
			if (flag4)
			{
				if (flag)
				{
					this.children[1].GetColliding(checkBounds, ref result);
				}
				if (flag2)
				{
					this.children[3].GetColliding(checkBounds, ref result);
				}
			}
		}
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x00087198 File Offset: 0x00085398
	public void GetColliding(Vector3 checkPoint, ref List<T> result)
	{
		if (this.objects.Count == 0 && (this.children == null || this.children.Length == 0))
		{
			return;
		}
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i].Bounds.Contains(checkPoint))
			{
				result.Add(this.objects[i].Obj);
			}
		}
		if (this.children != null)
		{
			bool flag = checkPoint.y <= this.children[0].bounds.max.y;
			bool flag2 = checkPoint.y >= this.children[2].bounds.min.y;
			bool flag3 = checkPoint.x <= this.children[0].bounds.max.x;
			bool flag4 = checkPoint.x >= this.children[1].bounds.min.x;
			if (flag3)
			{
				if (flag)
				{
					this.children[0].GetColliding(checkPoint, ref result);
				}
				if (flag2)
				{
					this.children[2].GetColliding(checkPoint, ref result);
				}
			}
			if (flag4)
			{
				if (flag)
				{
					this.children[1].GetColliding(checkPoint, ref result);
				}
				if (flag2)
				{
					this.children[3].GetColliding(checkPoint, ref result);
				}
			}
		}
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000872EE File Offset: 0x000854EE
	public void SetChildren(BoundsQuadtreeNode<T>[] childQuadtrees)
	{
		if (childQuadtrees.Length != 4)
		{
			Log.Error("Child quadtree array must be length 4. Was length: " + childQuadtrees.Length, Array.Empty<object>());
			return;
		}
		this.children = childQuadtrees;
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x0008731C File Offset: 0x0008551C
	public void DrawAllBounds(float depth = 0f)
	{
		float num = depth / 7f;
		Gizmos.color = new Color(num, 0f, 1f - num);
		Bounds bounds = new Bounds(this.Center, new Vector3(this.adjLength, 1000f, this.adjLength));
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		if (this.children != null)
		{
			depth += 1f;
			for (int i = 0; i < this.children.Length; i++)
			{
				this.children[i].DrawAllBounds(depth);
			}
		}
		Gizmos.color = Color.white;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000873BC File Offset: 0x000855BC
	public void DrawAllObjects()
	{
		float num = this.BaseLength / 20f;
		Gizmos.color = new Color(0f, 1f - num, num, 0.25f);
		foreach (BoundsQuadtreeNode<T>.QuadtreeObject quadtreeObject in this.objects)
		{
			Gizmos.DrawCube(quadtreeObject.Bounds.center, quadtreeObject.Bounds.size);
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(quadtreeObject.Bounds.center, this.bounds.center);
		}
		if (this.children != null)
		{
			for (int i = 0; i < this.children.Length; i++)
			{
				this.children[i].DrawAllObjects();
			}
		}
		Gizmos.color = Color.white;
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000874A4 File Offset: 0x000856A4
	public BoundsQuadtreeNode<T> ShrinkIfPossible(float minLength)
	{
		if (this.BaseLength < 2f * minLength)
		{
			return this;
		}
		if (this.objects.Count == 0 && (this.children == null || this.children.Length == 0))
		{
			return this;
		}
		int num = -1;
		for (int i = 0; i < this.objects.Count; i++)
		{
			BoundsQuadtreeNode<T>.QuadtreeObject quadtreeObject = this.objects[i];
			int num2 = this.BestFitChild(quadtreeObject.Bounds);
			if (i != 0 && num2 != num)
			{
				return this;
			}
			if (!BoundsQuadtreeNode<T>.Encapsulates(this.childBounds[num2], quadtreeObject.Bounds))
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

	// Token: 0x060022EC RID: 8940 RVA: 0x000875CC File Offset: 0x000857CC
	private void SetValues(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		this.BaseLength = baseLengthVal;
		this.minSize = minSizeVal;
		this.looseness = loosenessVal;
		this.Center = centerVal;
		this.adjLength = this.looseness * baseLengthVal;
		Vector3 size = new Vector3(this.adjLength, float.PositiveInfinity, this.adjLength);
		this.bounds = new Bounds(this.Center, size);
		float num = this.BaseLength / 4f;
		float num2 = this.BaseLength / 2f * this.looseness;
		Vector3 size2 = new Vector3(num2, float.PositiveInfinity, num2);
		this.childBounds = new Bounds[4];
		this.childBounds[0] = new Bounds(this.Center + new Vector3(-num, 0f, -num), size2);
		this.childBounds[1] = new Bounds(this.Center + new Vector3(num, 0f, -num), size2);
		this.childBounds[2] = new Bounds(this.Center + new Vector3(-num, 0f, num), size2);
		this.childBounds[3] = new Bounds(this.Center + new Vector3(num, 0f, num), size2);
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x00087714 File Offset: 0x00085914
	private void SubAdd(T obj, Bounds objBounds)
	{
		if (this.children == null && (this.objects.Count < 4 || this.BaseLength / 2f < this.minSize))
		{
			BoundsQuadtreeNode<T>.QuadtreeObject item = new BoundsQuadtreeNode<T>.QuadtreeObject
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
				BoundsQuadtreeNode<T>.QuadtreeObject quadtreeObject = this.objects[i];
				num = this.BestFitChild(quadtreeObject.Bounds);
				if (BoundsQuadtreeNode<T>.Encapsulates(this.children[num].bounds, quadtreeObject.Bounds))
				{
					this.children[num].SubAdd(quadtreeObject.Obj, quadtreeObject.Bounds);
					this.objects.Remove(quadtreeObject);
				}
			}
		}
		num = this.BestFitChild(objBounds);
		if (BoundsQuadtreeNode<T>.Encapsulates(this.children[num].bounds, objBounds))
		{
			this.children[num].SubAdd(obj, objBounds);
			return;
		}
		BoundsQuadtreeNode<T>.QuadtreeObject item2 = new BoundsQuadtreeNode<T>.QuadtreeObject
		{
			Obj = obj,
			Bounds = objBounds
		};
		this.objects.Add(item2);
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x00087854 File Offset: 0x00085A54
	private void Split()
	{
		float num = this.BaseLength / 4f;
		float baseLengthVal = this.BaseLength / 2f;
		this.children = new BoundsQuadtreeNode<T>[4];
		this.children[0] = new BoundsQuadtreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, 0f, -num));
		this.children[1] = new BoundsQuadtreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, 0f, -num));
		this.children[2] = new BoundsQuadtreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, 0f, num));
		this.children[3] = new BoundsQuadtreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, 0f, num));
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x00087950 File Offset: 0x00085B50
	private void Merge()
	{
		for (int i = 0; i < this.children.Length; i++)
		{
			BoundsQuadtreeNode<T> boundsQuadtreeNode = this.children[i];
			for (int j = boundsQuadtreeNode.objects.Count - 1; j >= 0; j--)
			{
				BoundsQuadtreeNode<T>.QuadtreeObject item = boundsQuadtreeNode.objects[j];
				this.objects.Add(item);
			}
		}
		this.children = null;
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x00014137 File Offset: 0x00012337
	private static bool Encapsulates(Bounds outerBounds, Bounds innerBounds)
	{
		return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000879B1 File Offset: 0x00085BB1
	private int BestFitChild(Bounds objBounds)
	{
		return ((objBounds.center.x <= this.Center.x) ? 0 : 1) + ((objBounds.center.z <= this.Center.z) ? 0 : 2);
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000879F0 File Offset: 0x00085BF0
	private bool ShouldMerge()
	{
		int num = this.objects.Count;
		if (this.children != null)
		{
			foreach (BoundsQuadtreeNode<T> boundsQuadtreeNode in this.children)
			{
				if (boundsQuadtreeNode.children != null)
				{
					return false;
				}
				num += boundsQuadtreeNode.objects.Count;
			}
		}
		return num <= 4;
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x00087A4C File Offset: 0x00085C4C
	private bool HasAnyObjects()
	{
		if (this.objects.Count > 0)
		{
			return true;
		}
		if (this.children != null)
		{
			for (int i = 0; i < this.children.Length; i++)
			{
				if (this.children[i].HasAnyObjects())
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04002273 RID: 8819
	private float looseness;

	// Token: 0x04002274 RID: 8820
	private float minSize;

	// Token: 0x04002275 RID: 8821
	private float adjLength;

	// Token: 0x04002276 RID: 8822
	private Bounds bounds;

	// Token: 0x04002277 RID: 8823
	private readonly List<BoundsQuadtreeNode<T>.QuadtreeObject> objects = new List<BoundsQuadtreeNode<T>.QuadtreeObject>();

	// Token: 0x04002278 RID: 8824
	private BoundsQuadtreeNode<T>[] children;

	// Token: 0x04002279 RID: 8825
	private Bounds[] childBounds;

	// Token: 0x0400227A RID: 8826
	private const int numObjectsAllowed = 4;

	// Token: 0x0400227B RID: 8827
	private const float DRAW_AS_HEIGHT = 1000f;

	// Token: 0x0200068F RID: 1679
	private class QuadtreeObject
	{
		// Token: 0x0400227C RID: 8828
		public T Obj;

		// Token: 0x0400227D RID: 8829
		public Bounds Bounds;
	}
}
