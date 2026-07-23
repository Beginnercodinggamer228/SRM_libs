using System;
using UnityEngine;

// Token: 0x0200082D RID: 2093
public class vp_Billboard : MonoBehaviour
{
	// Token: 0x06002BE1 RID: 11233 RVA: 0x000A50F0 File Offset: 0x000A32F0
	protected virtual void Start()
	{
		this.m_Transform = base.transform;
		this.m_Renderer = base.GetComponent<Renderer>();
		if (this.m_CameraTransform == null)
		{
			this.m_CameraTransform = Camera.main.transform;
		}
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000A5128 File Offset: 0x000A3328
	protected virtual void Update()
	{
		if (this.m_Renderer != null && this.m_Renderer.isVisible && this.m_CameraTransform != null)
		{
			this.m_Transform.eulerAngles = this.m_CameraTransform.eulerAngles;
		}
	}

	// Token: 0x04002A16 RID: 10774
	public Transform m_CameraTransform;

	// Token: 0x04002A17 RID: 10775
	private Transform m_Transform;

	// Token: 0x04002A18 RID: 10776
	private Renderer m_Renderer;
}
