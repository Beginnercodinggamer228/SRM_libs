using System;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class VacConeDetector : MonoBehaviour
{
	// Token: 0x0600195F RID: 6495 RVA: 0x00062DD2 File Offset: 0x00060FD2
	public void Awake()
	{
		this.faceAnim = base.GetComponent<GordoFaceAnimator>();
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x00062DE0 File Offset: 0x00060FE0
	public void OnEnable()
	{
		this.vacTriggerCount = 0;
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x00062DE9 File Offset: 0x00060FE9
	public void OnTriggerEnter(Collider col)
	{
		if (col.GetComponentInParent<TrackCollisions>())
		{
			this.vacTriggerCount++;
			if (this.vacTriggerCount == 1)
			{
				this.faceAnim.SetInVac(true);
			}
		}
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x00062E1B File Offset: 0x0006101B
	public void OnTriggerExit(Collider col)
	{
		if (col.GetComponentInParent<TrackCollisions>())
		{
			this.vacTriggerCount--;
			if (this.vacTriggerCount == 0)
			{
				this.faceAnim.SetInVac(false);
			}
		}
	}

	// Token: 0x0400190E RID: 6414
	private GordoFaceAnimator faceAnim;

	// Token: 0x0400190F RID: 6415
	private int vacTriggerCount;
}
