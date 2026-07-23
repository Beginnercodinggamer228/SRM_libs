using System;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class DroneProgramSource_PathGenerationFailure : SRBehaviour
{
	// Token: 0x06000873 RID: 2163 RVA: 0x00027959 File Offset: 0x00025B59
	public void Awake()
	{
		this.startPosition = base.transform.position;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0002796C File Offset: 0x00025B6C
	public void OnDestroy()
	{
		DroneProgramSource.BLACKLIST.Remove(base.gameObject);
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00027980 File Offset: 0x00025B80
	public void Update()
	{
		if ((base.transform.position - this.startPosition).sqrMagnitude >= 9f)
		{
			Destroyer.Destroy(this, "DroneProgramSource_PathGenerationFailure.Update");
		}
	}

	// Token: 0x0400075B RID: 1883
	private const float MIN_DISTANCE = 3f;

	// Token: 0x0400075C RID: 1884
	private const float MIN_DISTANCE_SQR = 9f;

	// Token: 0x0400075D RID: 1885
	private Vector3 startPosition;
}
