using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public class BoundsQuadtree<T>
{
	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060022CE RID: 8910 RVA: 0x00086B17 File Offset: 0x00084D17
	// (set) Token: 0x060022CF RID: 8911 RVA: 0x00086B1F File Offset: 0x00084D1F
	public int Count { get; private set; }

	// Token: 0x060022D0 RID: 8912 RVA: 0x00086B28 File Offset: 0x00084D28
	public BoundsQuadtree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize, float loosenessVal)
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
		this.looseness = Mathf.Clamp(loosenessVal, 1f, 2f);
		this.rootNode = new BoundsQuadtreeNode<T>(this.initialSize, this.minSize, loosenessVal, initialWorldPos);
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x00086BC0 File Offset: 0x00084DC0
	public void Add(T obj, Bounds objBounds)
	{
		int num = 0;
		while (!this.rootNode.Add(obj, objBounds))
		{
			this.Grow(objBounds.center - this.rootNode.Center);
			if (++num > 20)
			{
				Log.Error("Aborted Add operation as it seemed to be going on forever (" + (num - 1) + ") attempts at growing the quadtree.", Array.Empty<object>());
				return;
			}
		}
		int count = this.Count;
		this.Count = count + 1;
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x00086C38 File Offset: 0x00084E38
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

	// Token: 0x060022D3 RID: 8915 RVA: 0x00086C6A File Offset: 0x00084E6A
	public bool IsColliding(Bounds checkBounds)
	{
		return this.rootNode.IsColliding(checkBounds);
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x00086C78 File Offset: 0x00084E78
	public List<T> GetColliding(Bounds checkBounds, ref List<T> result)
	{
		if (this.rootNode.IntersectsBounds(checkBounds))
		{
			this.rootNode.GetColliding(checkBounds, ref result);
		}
		return result;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x00086C97 File Offset: 0x00084E97
	public List<T> GetColliding(Vector3 checkPoint, ref List<T> result)
	{
		if (this.rootNode.ContainsPoint(checkPoint))
		{
			this.rootNode.GetColliding(checkPoint, ref result);
		}
		return result;
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x00086CB6 File Offset: 0x00084EB6
	public List<T> GetAll(ref List<T> result)
	{
		return this.GetColliding(new Bounds(Vector3.zero, Vector3.one * float.PositiveInfinity), ref result);
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x00086CD8 File Offset: 0x00084ED8
	public void DrawAllBounds()
	{
		this.rootNode.DrawAllBounds(0f);
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x00086CEA File Offset: 0x00084EEA
	public void DrawAllObjects()
	{
		this.rootNode.DrawAllObjects();
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x00086CF8 File Offset: 0x00084EF8
	private void Grow(Vector3 direction)
	{
		int num = (direction.x >= 0f) ? 1 : -1;
		int num2 = (direction.z >= 0f) ? 1 : -1;
		BoundsQuadtreeNode<T> boundsQuadtreeNode = this.rootNode;
		float num3 = this.rootNode.BaseLength / 2f;
		float baseLengthVal = this.rootNode.BaseLength * 2f;
		Vector3 vector = this.rootNode.Center + new Vector3((float)num * num3, 0f, (float)num2 * num3);
		this.rootNode = new BoundsQuadtreeNode<T>(baseLengthVal, this.minSize, this.looseness, vector);
		int rootPosIndex = BoundsQuadtree<T>.GetRootPosIndex(num, num2);
		BoundsQuadtreeNode<T>[] array = new BoundsQuadtreeNode<T>[4];
		for (int i = 0; i < 4; i++)
		{
			if (i == rootPosIndex)
			{
				array[i] = boundsQuadtreeNode;
			}
			else
			{
				num = ((i % 2 == 0) ? -1 : 1);
				num2 = ((i >= 2) ? 1 : -1);
				array[i] = new BoundsQuadtreeNode<T>(this.rootNode.BaseLength, this.minSize, this.looseness, vector + new Vector3((float)num * num3, this.rootNode.Center.y, (float)num2 * num3));
			}
		}
		this.rootNode.SetChildren(array);
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x00086E2A File Offset: 0x0008502A
	private void Shrink()
	{
		this.rootNode = this.rootNode.ShrinkIfPossible(this.initialSize);
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x00086E44 File Offset: 0x00085044
	private static int GetRootPosIndex(int xDir, int zDir)
	{
		int num = (xDir > 0) ? 1 : 0;
		if (zDir > 0)
		{
			num += 2;
		}
		return num;
	}

	// Token: 0x0400226D RID: 8813
	private BoundsQuadtreeNode<T> rootNode;

	// Token: 0x0400226E RID: 8814
	private readonly float looseness;

	// Token: 0x0400226F RID: 8815
	private readonly float initialSize;

	// Token: 0x04002270 RID: 8816
	private readonly float minSize;
}
