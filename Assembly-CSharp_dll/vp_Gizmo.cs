using System;
using UnityEngine;

// Token: 0x02000839 RID: 2105
public class vp_Gizmo : MonoBehaviour
{
	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06002C2E RID: 11310 RVA: 0x000A6C44 File Offset: 0x000A4E44
	protected Collider Collider
	{
		get
		{
			if (this.m_Collider == null)
			{
				this.m_Collider = base.GetComponent<Collider>();
			}
			return this.m_Collider;
		}
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x000A6C68 File Offset: 0x000A4E68
	public void OnDrawGizmos()
	{
		Vector3 center = this.Collider.bounds.center;
		Vector3 size = this.Collider.bounds.size;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawCube(center, size);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000A6CDC File Offset: 0x000A4EDC
	public void OnDrawGizmosSelected()
	{
		Vector3 center = this.Collider.bounds.center;
		Vector3 size = this.Collider.bounds.size;
		Gizmos.color = this.selectedGizmoColor;
		Gizmos.DrawCube(center, size);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x04002A4F RID: 10831
	public Color gizmoColor = new Color(1f, 1f, 1f, 0.4f);

	// Token: 0x04002A50 RID: 10832
	public Color selectedGizmoColor = new Color(1f, 1f, 1f, 0.4f);

	// Token: 0x04002A51 RID: 10833
	protected Collider m_Collider;
}
