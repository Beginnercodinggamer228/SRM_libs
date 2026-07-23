using System;
using UnityEngine;

// Token: 0x02000788 RID: 1928
public class SlimenadoMoveSpiral : MonoBehaviour
{
	// Token: 0x06002844 RID: 10308 RVA: 0x00098C9C File Offset: 0x00096E9C
	private void Start()
	{
		this.rot = 90f;
		this.tgtY = base.transform.position.y;
		this.nextGroundCheck = Time.time + this.nextGroundCheck;
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x00098CD4 File Offset: 0x00096ED4
	private void FixedUpdate()
	{
		float y = base.transform.position.y;
		if (this.tgtY > y)
		{
			base.transform.position = new Vector3(base.transform.position.x, Mathf.Min(this.tgtY, y + 5f * Time.fixedDeltaTime), base.transform.position.z);
		}
		else if (this.tgtY < y)
		{
			base.transform.position = new Vector3(base.transform.position.x, Mathf.Max(this.tgtY, y - 5f * Time.fixedDeltaTime), base.transform.position.z);
		}
		base.transform.Translate(base.transform.forward * (3f * Time.fixedDeltaTime));
		base.transform.Rotate(0f, this.rot * Time.fixedDeltaTime, 0f, Space.World);
		this.rot *= Mathf.Pow(0.9f, Time.fixedDeltaTime);
		if (Time.time >= this.nextGroundCheck)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position + Vector3.up * 10f, Vector3.down, out raycastHit, 20f, 268435457))
			{
				this.tgtY = raycastHit.point.y;
			}
			else
			{
				Destroyer.Destroy(base.gameObject, "SlimenadoMoveSpiral.FixedUpdate");
			}
			this.nextGroundCheck = Time.time + 1f;
		}
	}

	// Token: 0x040027E2 RID: 10210
	private float rot;

	// Token: 0x040027E3 RID: 10211
	private float nextGroundCheck;

	// Token: 0x040027E4 RID: 10212
	private float tgtY;

	// Token: 0x040027E5 RID: 10213
	private const float SPEED = 3f;

	// Token: 0x040027E6 RID: 10214
	private const float VERT_SPEED = 5f;

	// Token: 0x040027E7 RID: 10215
	private const float INIT_ROT = 90f;

	// Token: 0x040027E8 RID: 10216
	private const float GROUND_CHECK_PERIOD = 1f;

	// Token: 0x040027E9 RID: 10217
	private const float GROUND_ADJUST_DIST = 10f;

	// Token: 0x040027EA RID: 10218
	private const int GROUND_RAY_MASK = 268435457;
}
