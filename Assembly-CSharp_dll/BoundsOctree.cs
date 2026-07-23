using System;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class BoundsOctree<T>
{
	// Token: 0x17000234 RID: 564
	// (get) Token: 0x060022AC RID: 8876 RVA: 0x00085CF9 File Offset: 0x00083EF9
	// (set) Token: 0x060022AD RID: 8877 RVA: 0x00085D01 File Offset: 0x00083F01
	public int Count { get; private set; }

	// Token: 0x060022AE RID: 8878 RVA: 0x00085D0C File Offset: 0x00083F0C
	public BoundsOctree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize, float loosenessVal)
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
		this.rootNode = new BoundsOctreeNode<T>(this.initialSize, this.minSize, loosenessVal, initialWorldPos);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x00085DA4 File Offset: 0x00083FA4
	public void Add(T obj, Bounds objBounds)
	{
		int num = 0;
		while (!this.rootNode.Add(obj, objBounds))
		{
			this.Grow(objBounds.center - this.rootNode.Center);
			if (++num > 20)
			{
				Log.Error("Aborted Add operation as it seemed to be going on forever (" + (num - 1) + ") attempts at growing the octree.", Array.Empty<object>());
				return;
			}
		}
		int count = this.Count;
		this.Count = count + 1;
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x00085E1C File Offset: 0x0008401C
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

	// Token: 0x060022B1 RID: 8881 RVA: 0x00085E4E File Offset: 0x0008404E
	public bool IsColliding(Bounds checkBounds)
	{
		return this.rootNode.IsColliding(checkBounds);
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x00085E5C File Offset: 0x0008405C
	public T[] GetColliding(Bounds checkBounds)
	{
		return this.rootNode.GetColliding(checkBounds);
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x00085E6A File Offset: 0x0008406A
	public void DrawAllBounds()
	{
		this.rootNode.DrawAllBounds(0f);
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x00085E7C File Offset: 0x0008407C
	public void DrawAllObjects()
	{
		this.rootNode.DrawAllObjects();
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x00085E8C File Offset: 0x0008408C
	private void Grow(Vector3 direction)
	{
		int num = (direction.x >= 0f) ? 1 : -1;
		int num2 = (direction.y >= 0f) ? 1 : -1;
		int num3 = (direction.z >= 0f) ? 1 : -1;
		BoundsOctreeNode<T> boundsOctreeNode = this.rootNode;
		float num4 = this.rootNode.BaseLength / 2f;
		float baseLengthVal = this.rootNode.BaseLength * 2f;
		Vector3 vector = this.rootNode.Center + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4);
		this.rootNode = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, vector);
		int rootPosIndex = BoundsOctree<T>.GetRootPosIndex(num, num2, num3);
		BoundsOctreeNode<T>[] array = new BoundsOctreeNode<T>[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == rootPosIndex)
			{
				array[i] = boundsOctreeNode;
			}
			else
			{
				num = ((i % 2 == 0) ? -1 : 1);
				num2 = ((i > 3) ? -1 : 1);
				num3 = ((i < 2 || (i > 3 && i < 6)) ? -1 : 1);
				array[i] = new BoundsOctreeNode<T>(this.rootNode.BaseLength, this.minSize, this.looseness, vector + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4));
			}
		}
		this.rootNode.SetChildren(array);
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x00085FDF File Offset: 0x000841DF
	private void Shrink()
	{
		this.rootNode = this.rootNode.ShrinkIfPossible(this.initialSize);
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x00085FF8 File Offset: 0x000841F8
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

	// Token: 0x0400225C RID: 8796
	private BoundsOctreeNode<T> rootNode;

	// Token: 0x0400225D RID: 8797
	private readonly float looseness;

	// Token: 0x0400225E RID: 8798
	private readonly float initialSize;

	// Token: 0x0400225F RID: 8799
	private readonly float minSize;
}
