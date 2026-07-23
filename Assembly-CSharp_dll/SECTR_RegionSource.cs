using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Region Source")]
public class SECTR_RegionSource : SECTR_PointSource
{
	// Token: 0x060002FE RID: 766 RVA: 0x00013168 File Offset: 0x00011368
	private void Update()
	{
		if (this.instance)
		{
			Vector3 position = SECTR_AudioSystem.Listener.position;
			Vector3 position2 = base.transform.position;
			Collider component = base.GetComponent<Collider>();
			if (this.Raycast && component)
			{
				Vector3 vector = base.transform.position - position;
				float magnitude = vector.magnitude;
				vector /= magnitude;
				RaycastHit raycastHit;
				if (component.Raycast(new Ray(position, vector), out raycastHit, magnitude))
				{
					position2 = raycastHit.point;
				}
				else
				{
					position2 = position;
				}
			}
			else if (component)
			{
				if (component.bounds.Contains(position))
				{
					position2 = position;
				}
				else
				{
					position2 = component.ClosestPointOnBounds(position);
				}
			}
			this.instance.Position = position2;
		}
	}

	// Token: 0x04000323 RID: 803
	[SECTR_ToolTip("Determine the closest point by raycast instead of bounding box. More accurate but more expensive.")]
	public bool Raycast;
}
