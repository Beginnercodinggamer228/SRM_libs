using System;
using UnityEngine;

// Token: 0x02000690 RID: 1680
public class PointOctree<T> where T : class
{
	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060022F5 RID: 8949 RVA: 0x00087A96 File Offset: 0x00085C96
	// (set) Token: 0x060022F6 RID: 8950 RVA: 0x00087A9E File Offset: 0x00085C9E
	public int Count { get; private set; }

	// Token: 0x060022F7 RID: 8951 RVA: 0x00087AA8 File Offset: 0x00085CA8
	public PointOctree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize)
	{
		if (minNodeSize > initialWorldSize)
		{
			Log.Warning(string.Concat(new object[]
			{
				"Minimum node size must be at least as big as the initial world size. Was: ",
				minNodeSize,
				" Adjusted to: ",
				initialWorldSize
			}), Array.Empty<object>());
			minNodeSize = initialWorldSize;
		}
		this.Count = 0;
		this.initialSize = initialWorldSize;
		this.minSize = minNodeSize;
		this.rootNode = new PointOctreeNode<T>(this.initialSize, this.minSize, initialWorldPos);
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x00087B28 File Offset: 0x00085D28
	public void Add(T obj, Vector3 objPos)
	{
		int num = 0;
		while (!this.rootNode.Add(obj, objPos))
		{
			this.Grow(objPos - this.rootNode.Center);
			if (++num > 20)
			{
				Log.Error("Aborted Add operation as it seemed to be going on forever (" + (num - 1) + ") attempts at growing the octree.", Array.Empty<object>());
				return;
			}
		}
		int count = this.Count;
		this.Count = count + 1;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x00087B9C File Offset: 0x00085D9C
	public bool Remove(T obj)
	{
		bool flag = this.rootNode.Remove(obj);
		if (flag)
		{
			int count = this.Count;
			this.Count = count - 1;
			this.Shrink();
		}
		return flag;
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x00087BCE File Offset: 0x00085DCE
	public T[] GetNearby(Ray ray, float maxDistance)
	{
		return this.rootNode.GetNearby(ray, maxDistance);
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x00087BDD File Offset: 0x00085DDD
	public void DrawAllBounds()
	{
		this.rootNode.DrawAllBounds(0f);
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x00087BEF File Offset: 0x00085DEF
	public void DrawAllObjects()
	{
		this.rootNode.DrawAllObjects();
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x00087BFC File Offset: 0x00085DFC
	private void Grow(Vector3 direction)
	{
		int num = (direction.x >= 0f) ? 1 : -1;
		int num2 = (direction.y >= 0f) ? 1 : -1;
		int num3 = (direction.z >= 0f) ? 1 : -1;
		PointOctreeNode<T> pointOctreeNode = this.rootNode;
		float num4 = this.rootNode.SideLength / 2f;
		float baseLengthVal = this.rootNode.SideLength * 2f;
		Vector3 vector = this.rootNode.Center + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4);
		this.rootNode = new PointOctreeNode<T>(baseLengthVal, this.minSize, vector);
		int rootPosIndex = PointOctree<T>.GetRootPosIndex(num, num2, num3);
		PointOctreeNode<T>[] array = new PointOctreeNode<T>[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == rootPosIndex)
			{
				array[i] = pointOctreeNode;
			}
			else
			{
				num = ((i % 2 == 0) ? -1 : 1);
				num2 = ((i > 3) ? -1 : 1);
				num3 = ((i < 2 || (i > 3 && i < 6)) ? -1 : 1);
				array[i] = new PointOctreeNode<T>(this.rootNode.SideLength, this.minSize, vector + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4));
			}
		}
		this.rootNode.SetChildren(array);
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x00087D43 File Offset: 0x00085F43
	private void Shrink()
	{
		this.rootNode = this.rootNode.ShrinkIfPossible(this.initialSize);
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x00087D5C File Offset: 0x00085F5C
	private static int GetRootPosIndex(int xDir, int yDir, int zDir)
	{
		int num = (xDir > 0) ? 1 : 0;
		if (yDir < 0)
		{
			num += 4;
		}
		if (zDir > 0)
		{
			num += 2;
		}
		return num;
	}

	// Token: 0x0400227F RID: 8831
	private PointOctreeNode<T> rootNode;

	// Token: 0x04002280 RID: 8832
	private readonly float initialSize;

	// Token: 0x04002281 RID: 8833
	private readonly float minSize;
}
