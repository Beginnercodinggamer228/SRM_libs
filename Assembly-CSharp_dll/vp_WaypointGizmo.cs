using System;
using UnityEngine;

// Token: 0x0200084B RID: 2123
public class vp_WaypointGizmo : MonoBehaviour
{
	// Token: 0x06002CB9 RID: 11449 RVA: 0x000A89F8 File Offset: 0x000A6BF8
	public void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = this.m_GizmoColor;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x000A8A5C File Offset: 0x000A6C5C
	public void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = this.m_SelectedGizmoColor;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x04002AA5 RID: 10917
	protected Color m_GizmoColor = new Color(1f, 1f, 1f, 0.4f);

	// Token: 0x04002AA6 RID: 10918
	protected Color m_SelectedGizmoColor = new Color32(160, byte.MaxValue, 100, 100);
}
