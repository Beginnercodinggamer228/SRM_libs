using System;
using UnityEngine;

// Token: 0x02000832 RID: 2098
[RequireComponent(typeof(Rigidbody))]
public class vp_RigidbodyImpulse : MonoBehaviour
{
	// Token: 0x06002BFD RID: 11261 RVA: 0x000A5F13 File Offset: 0x000A4113
	protected virtual void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x000A5F24 File Offset: 0x000A4124
	protected virtual void OnEnable()
	{
		if (this.m_Rigidbody == null)
		{
			return;
		}
		if (this.RigidbodyForce != Vector3.zero)
		{
			this.m_Rigidbody.AddForce(this.RigidbodyForce, ForceMode.Impulse);
		}
		if (this.RigidbodySpin != 0f)
		{
			this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * this.RigidbodySpin);
		}
	}

	// Token: 0x04002A2F RID: 10799
	protected Rigidbody m_Rigidbody;

	// Token: 0x04002A30 RID: 10800
	public Vector3 RigidbodyForce = new Vector3(0f, 5f, 0f);

	// Token: 0x04002A31 RID: 10801
	public float RigidbodySpin = 0.2f;
}
