using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x02000C2D RID: 3117
	public class ForceShieldShootBall : MonoBehaviour
	{
		// Token: 0x06005803 RID: 22531 RVA: 0x0010988C File Offset: 0x00107A8C
		private void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distance);
				vector = Camera.main.ScreenToWorldPoint(vector);
				Rigidbody rigidbody = UnityEngine.Object.Instantiate<Rigidbody>(this.bullet, base.transform.position, Quaternion.identity);
				rigidbody.transform.LookAt(vector);
				rigidbody.AddForce(rigidbody.transform.forward * this.speed);
			}
		}

		// Token: 0x04004380 RID: 17280
		public Rigidbody bullet;

		// Token: 0x04004381 RID: 17281
		public Transform origshoot;

		// Token: 0x04004382 RID: 17282
		public float speed = 1000f;

		// Token: 0x04004383 RID: 17283
		private float distance = 10f;
	}
}
