using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class Smear : MonoBehaviour
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600003B RID: 59 RVA: 0x00002FA3 File Offset: 0x000011A3
	// (set) Token: 0x0600003C RID: 60 RVA: 0x00002FAB File Offset: 0x000011AB
	private Material InstancedMaterial
	{
		get
		{
			return this.m_instancedMaterial;
		}
		set
		{
			this.m_instancedMaterial = value;
		}
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002FB4 File Offset: 0x000011B4
	private void Start()
	{
		this.InstancedMaterial = this.Renderer.material;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002FC8 File Offset: 0x000011C8
	private void LateUpdate()
	{
		if (this.m_recentPositions.Count > this.FramesBufferSize)
		{
			this.InstancedMaterial.SetVector("_PrevPosition", this.m_recentPositions.Dequeue());
		}
		this.InstancedMaterial.SetVector("_Position", base.transform.position);
		this.m_recentPositions.Enqueue(base.transform.position);
	}

	// Token: 0x04000025 RID: 37
	private Queue<Vector3> m_recentPositions = new Queue<Vector3>();

	// Token: 0x04000026 RID: 38
	public int FramesBufferSize;

	// Token: 0x04000027 RID: 39
	public Renderer Renderer;

	// Token: 0x04000028 RID: 40
	private Material m_instancedMaterial;
}
