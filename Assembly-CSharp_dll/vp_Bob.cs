using System;
using UnityEngine;

// Token: 0x0200082E RID: 2094
public class vp_Bob : MonoBehaviour
{
	// Token: 0x06002BE4 RID: 11236 RVA: 0x000A5174 File Offset: 0x000A3374
	protected virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_InitialPosition = this.m_Transform.position;
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x000A5193 File Offset: 0x000A3393
	protected virtual void OnEnable()
	{
		this.m_Transform.position = this.m_InitialPosition;
		if (this.RandomizeBobOffset)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x000A51BC File Offset: 0x000A33BC
	protected virtual void Update()
	{
		if (this.BobRate.x != 0f && this.BobAmp.x != 0f)
		{
			this.m_Offset.x = vp_MathUtility.Sinus(this.BobRate.x, this.BobAmp.x, this.BobOffset);
		}
		if (this.BobRate.y != 0f && this.BobAmp.y != 0f)
		{
			this.m_Offset.y = vp_MathUtility.Sinus(this.BobRate.y, this.BobAmp.y, this.BobOffset);
		}
		if (this.BobRate.z != 0f && this.BobAmp.z != 0f)
		{
			this.m_Offset.z = vp_MathUtility.Sinus(this.BobRate.z, this.BobAmp.z, this.BobOffset);
		}
		if (!this.LocalMotion)
		{
			this.m_Transform.position = this.m_InitialPosition + this.m_Offset + Vector3.up * this.GroundOffset;
			return;
		}
		this.m_Transform.position = this.m_InitialPosition + Vector3.up * this.GroundOffset;
		this.m_Transform.localPosition += this.m_Transform.TransformDirection(this.m_Offset);
	}

	// Token: 0x04002A19 RID: 10777
	public Vector3 BobAmp = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04002A1A RID: 10778
	public Vector3 BobRate = new Vector3(0f, 4f, 0f);

	// Token: 0x04002A1B RID: 10779
	public float BobOffset;

	// Token: 0x04002A1C RID: 10780
	public float GroundOffset;

	// Token: 0x04002A1D RID: 10781
	public bool RandomizeBobOffset;

	// Token: 0x04002A1E RID: 10782
	public bool LocalMotion;

	// Token: 0x04002A1F RID: 10783
	protected Transform m_Transform;

	// Token: 0x04002A20 RID: 10784
	protected Vector3 m_InitialPosition;

	// Token: 0x04002A21 RID: 10785
	protected Vector3 m_Offset;
}
