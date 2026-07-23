using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class VacJointBreaker : MonoBehaviour
{
	// Token: 0x06000FC3 RID: 4035 RVA: 0x0003E2E4 File Offset: 0x0003C4E4
	public void OnJointBreak(float breakForce)
	{
		Vacuumable component = base.GetComponent<Joint>().connectedBody.GetComponent<Vacuumable>();
		component.release();
		component.gameObject.GetComponent<Rigidbody>().velocity = component.gameObject.GetComponent<Rigidbody>().velocity * 0.5f;
		Destroyer.Destroy(base.gameObject, "VacJointBreaker.OnJointBreak");
	}

	// Token: 0x04000E86 RID: 3718
	private const float FLING_REDUCTION_FACTOR = 0.5f;
}
