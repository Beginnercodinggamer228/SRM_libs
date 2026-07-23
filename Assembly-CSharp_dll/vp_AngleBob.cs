using System;
using UnityEngine;

// Token: 0x0200082C RID: 2092
public class vp_AngleBob : MonoBehaviour
{
	// Token: 0x06002BDD RID: 11229 RVA: 0x000A4E96 File Offset: 0x000A3096
	protected virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_InitialRotation = this.m_Transform.eulerAngles;
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x000A4EB5 File Offset: 0x000A30B5
	protected virtual void OnEnable()
	{
		this.m_Transform.eulerAngles = this.m_InitialRotation;
		if (this.RandomizeBobOffset)
		{
			this.YOffset = UnityEngine.Random.value;
		}
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x000A4EDC File Offset: 0x000A30DC
	protected virtual void Update()
	{
		if (this.BobRate.x != 0f && this.BobAmp.x != 0f)
		{
			this.m_Offset.x = vp_MathUtility.Sinus(this.BobRate.x, this.BobAmp.x, 0f);
		}
		if (this.BobRate.y != 0f && this.BobAmp.y != 0f)
		{
			this.m_Offset.y = vp_MathUtility.Sinus(this.BobRate.y, this.BobAmp.y, 0f);
		}
		if (this.BobRate.z != 0f && this.BobAmp.z != 0f)
		{
			this.m_Offset.z = vp_MathUtility.Sinus(this.BobRate.z, this.BobAmp.z, 0f);
		}
		if (this.LocalMotion)
		{
			this.m_Transform.eulerAngles = this.m_InitialRotation + Vector3.up * this.YOffset;
			this.m_Transform.localEulerAngles += this.m_Transform.TransformDirection(this.m_Offset);
			return;
		}
		if (this.FadeToTarget)
		{
			this.m_Transform.rotation = Quaternion.Lerp(this.m_Transform.rotation, Quaternion.Euler(this.m_InitialRotation + this.m_Offset + Vector3.up * this.YOffset), Time.deltaTime);
			return;
		}
		this.m_Transform.eulerAngles = this.m_InitialRotation + this.m_Offset + Vector3.up * this.YOffset;
	}

	// Token: 0x04002A0D RID: 10765
	public Vector3 BobAmp = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04002A0E RID: 10766
	public Vector3 BobRate = new Vector3(0f, 4f, 0f);

	// Token: 0x04002A0F RID: 10767
	public float YOffset;

	// Token: 0x04002A10 RID: 10768
	public bool RandomizeBobOffset;

	// Token: 0x04002A11 RID: 10769
	public bool LocalMotion;

	// Token: 0x04002A12 RID: 10770
	public bool FadeToTarget;

	// Token: 0x04002A13 RID: 10771
	protected Transform m_Transform;

	// Token: 0x04002A14 RID: 10772
	protected Vector3 m_InitialRotation;

	// Token: 0x04002A15 RID: 10773
	protected Vector3 m_Offset;
}
