using System;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class LockUpright : MonoBehaviour
{
	// Token: 0x06000BD6 RID: 3030 RVA: 0x000317C8 File Offset: 0x0002F9C8
	public void Awake()
	{
		this.FaceCamera();
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x000317C8 File Offset: 0x0002F9C8
	public void OnRenderObject()
	{
		this.FaceCamera();
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x000317D0 File Offset: 0x0002F9D0
	private void FaceCamera()
	{
		base.transform.rotation = Quaternion.identity;
		base.transform.Rotate(this.xOffset, base.transform.rotation.y, base.transform.rotation.z);
	}

	// Token: 0x04000ABD RID: 2749
	public float xOffset;
}
