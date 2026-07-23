using System;
using UnityEngine;

// Token: 0x02000833 RID: 2099
public class vp_Spin : MonoBehaviour
{
	// Token: 0x06002C00 RID: 11264 RVA: 0x000A5FC1 File Offset: 0x000A41C1
	protected virtual void Start()
	{
		this.m_Transform = base.transform;
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x000A5FCF File Offset: 0x000A41CF
	protected virtual void Update()
	{
		this.m_Transform.Rotate(this.RotationSpeed * Time.deltaTime);
	}

	// Token: 0x04002A32 RID: 10802
	public Vector3 RotationSpeed = new Vector3(0f, 90f, 0f);

	// Token: 0x04002A33 RID: 10803
	private Transform m_Transform;
}
