using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class vp_SwimmingTrigger : MonoBehaviour
{
	// Token: 0x06000065 RID: 101 RVA: 0x0000500E File Offset: 0x0000320E
	protected virtual void Start()
	{
		base.GetComponent<Collider>().isTrigger = true;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000501C File Offset: 0x0000321C
	protected virtual void OnTriggerEnter(Collider col)
	{
		Dictionary<string, object> dataForCollider = this.GetDataForCollider(col);
		if (!(bool)dataForCollider[vp_SwimmingTrigger.m_IsPlayerKey])
		{
			return;
		}
		vp_FPPlayerEventHandler vp_FPPlayerEventHandler = (vp_FPPlayerEventHandler)dataForCollider[vp_SwimmingTrigger.m_PlayerKey];
		vp_FPPlayerEventHandler.SetState(this.StateName, true, true, false);
		vp_FPPlayerEventHandler.MotorThrottle.Set(Vector3.zero);
		vp_FPPlayerEventHandler.Jump.TryStop(true);
		Vector3 force = new Vector3(0f, vp_FPPlayerEventHandler.Velocity.Get().normalized.y * 0.25f, 0f);
		vp_FPPlayerEventHandler.Stop.Send();
		vp_FPPlayerEventHandler.GetComponent<vp_FPController>().AddSoftForce(force, 10f);
	}

	// Token: 0x06000067 RID: 103 RVA: 0x000050DC File Offset: 0x000032DC
	protected virtual void OnTriggerExit(Collider col)
	{
		Dictionary<string, object> dataForCollider = this.GetDataForCollider(col);
		if (!(bool)dataForCollider[vp_SwimmingTrigger.m_IsPlayerKey])
		{
			return;
		}
		((vp_FPPlayerEventHandler)dataForCollider[vp_SwimmingTrigger.m_PlayerKey]).SetState(this.StateName, false, true, false);
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00005124 File Offset: 0x00003324
	protected virtual Dictionary<string, object> GetDataForCollider(Collider col)
	{
		if ((this.LayerMask.value & 1 << col.gameObject.layer) == 0)
		{
			return new Dictionary<string, object>
			{
				{
					vp_SwimmingTrigger.m_IsPlayerKey,
					false
				}
			};
		}
		vp_FPPlayerEventHandler component = col.gameObject.GetComponent<vp_FPPlayerEventHandler>();
		if (component == null)
		{
			return new Dictionary<string, object>
			{
				{
					vp_SwimmingTrigger.m_IsPlayerKey,
					false
				}
			};
		}
		return new Dictionary<string, object>
		{
			{
				vp_SwimmingTrigger.m_IsPlayerKey,
				true
			},
			{
				vp_SwimmingTrigger.m_PlayerKey,
				component
			}
		};
	}

	// Token: 0x040000AB RID: 171
	public LayerMask LayerMask = 256;

	// Token: 0x040000AC RID: 172
	public string StateName = "Swimming";

	// Token: 0x040000AD RID: 173
	protected static string m_IsPlayerKey = "Is Player";

	// Token: 0x040000AE RID: 174
	protected static string m_PlayerKey = "Player";
}
