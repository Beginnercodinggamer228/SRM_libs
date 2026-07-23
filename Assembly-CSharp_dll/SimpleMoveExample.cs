using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class SimpleMoveExample : MonoBehaviour
{
	// Token: 0x06000038 RID: 56 RVA: 0x00002DE5 File Offset: 0x00000FE5
	private void Start()
	{
		this.m_originalPosition = base.transform.position;
		this.m_previous = base.transform.position;
		this.m_target = base.transform.position;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002E1C File Offset: 0x0000101C
	private void Update()
	{
		base.transform.position = Vector3.Slerp(this.m_previous, this.m_target, Time.deltaTime * this.Speed);
		this.m_previous = base.transform.position;
		if (Vector3.Distance(this.m_target, base.transform.position) < 0.1f)
		{
			this.m_target = base.transform.position + UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.7f, 4f);
			this.m_target.Set(Mathf.Clamp(this.m_target.x, this.m_originalPosition.x - this.BoundingVolume.x, this.m_originalPosition.x + this.BoundingVolume.x), Mathf.Clamp(this.m_target.y, this.m_originalPosition.y - this.BoundingVolume.y, this.m_originalPosition.y + this.BoundingVolume.y), Mathf.Clamp(this.m_target.z, this.m_originalPosition.z - this.BoundingVolume.z, this.m_originalPosition.z + this.BoundingVolume.z));
		}
	}

	// Token: 0x04000020 RID: 32
	private Vector3 m_previous;

	// Token: 0x04000021 RID: 33
	private Vector3 m_target;

	// Token: 0x04000022 RID: 34
	private Vector3 m_originalPosition;

	// Token: 0x04000023 RID: 35
	public Vector3 BoundingVolume = new Vector3(3f, 1f, 3f);

	// Token: 0x04000024 RID: 36
	public float Speed = 10f;
}
