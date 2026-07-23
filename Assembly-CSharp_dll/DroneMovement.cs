using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
[RequireComponent(typeof(Drone))]
[RequireComponent(typeof(Rigidbody))]
public class DroneMovement : MonoBehaviour
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000725 RID: 1829 RVA: 0x000249E5 File Offset: 0x00022BE5
	// (set) Token: 0x06000726 RID: 1830 RVA: 0x000249ED File Offset: 0x00022BED
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000727 RID: 1831 RVA: 0x000249F6 File Offset: 0x00022BF6
	// (set) Token: 0x06000728 RID: 1832 RVA: 0x000249FE File Offset: 0x00022BFE
	public Drone drone { get; private set; }

	// Token: 0x06000729 RID: 1833 RVA: 0x00024A07 File Offset: 0x00022C07
	public void Awake()
	{
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.drone = base.GetComponent<Drone>();
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00024A24 File Offset: 0x00022C24
	public bool MoveTowards(Vector3 position)
	{
		return DroneMovement.ApproximatelyEquals(position, this.rigidbody.position = Vector3.MoveTowards(this.rigidbody.position, position, Time.fixedDeltaTime * 0.25f * this.rigidbody.mass), 0.05f);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00024A74 File Offset: 0x00022C74
	public bool RotateTowards(Quaternion rotation)
	{
		return DroneMovement.ApproximatelyEquals(rotation, this.rigidbody.rotation = Quaternion.RotateTowards(this.rigidbody.rotation, rotation, Time.fixedDeltaTime * 20f * this.rigidbody.mass), 0.5f);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00024AC4 File Offset: 0x00022CC4
	public static bool ApproximatelyEquals(Vector3 v1, Vector3 v2, float range)
	{
		return (v2 - v1).sqrMagnitude <= range * range;
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00024AE8 File Offset: 0x00022CE8
	public static bool ApproximatelyEquals(Quaternion q1, Quaternion q2, float range)
	{
		return Quaternion.Angle(q1, q2) < range;
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00024AF4 File Offset: 0x00022CF4
	public void PathTowards(Vector3 position)
	{
		Vector3 forceMoveTowards = this.GetForceMoveTowards(position);
		Vector3 normalized = (forceMoveTowards + this.GetForceAvoidance()).normalized;
		this.rigidbody.AddTorque(Vector3.Cross(Quaternion.AngleAxis(this.rigidbody.angularVelocity.magnitude * 57.29578f * this.rotationFacingStability * 0.1f / this.rotationFacingSpeed, this.rigidbody.angularVelocity) * base.transform.forward, forceMoveTowards) * this.rotationFacingSpeed * this.rotationFacingSpeed * this.rigidbody.mass);
		this.rigidbody.AddForce(normalized * this.movementSpeed * Time.fixedDeltaTime * this.rigidbody.mass);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00024BD4 File Offset: 0x00022DD4
	private Vector3 GetForceMoveTowards(Vector3 target)
	{
		return (target - this.rigidbody.position).normalized;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00024BFC File Offset: 0x00022DFC
	private Vector3 GetForceAvoidance()
	{
		Vector3 vector = Vector3.zero;
		if (!this.drone.noClip.enabled)
		{
			Vector3 forward = base.transform.forward;
			Quaternion rotation = base.transform.rotation;
			float num = Mathf.Max(0.6f, this.rigidbody.velocity.sqrMagnitude * 0.4f);
			float num2 = num * 2f;
			Vector3 position = base.transform.position;
			float radius = 0.5f;
			RaycastHit raycastHit;
			if (Physics.SphereCast(position - forward * 0.1f, radius, forward, out raycastHit, num, -537968901))
			{
				Ray ray = new Ray(position + base.transform.right * 0.5f - base.transform.up * 0.25f, forward + base.transform.right * 0.1f);
				Ray ray2 = new Ray(position - base.transform.right * 0.5f - base.transform.up * 0.25f, forward - base.transform.right * 0.1f);
				Ray ray3 = new Ray(position + base.transform.up * 0.5f, forward + base.transform.up);
				RaycastHit raycastHit2;
				Physics.Raycast(ray, out raycastHit2, num2, -537968901);
				RaycastHit raycastHit3;
				Physics.Raycast(ray2, out raycastHit3, num2, -537968901);
				RaycastHit raycastHit4;
				Physics.Raycast(ray3, out raycastHit4, num2 * DroneMovement.SQRT_TWO, -537968901);
				float num3 = (raycastHit3.collider == null) ? float.PositiveInfinity : raycastHit3.distance;
				float num4 = (raycastHit2.collider == null) ? float.PositiveInfinity : raycastHit2.distance;
				float num5 = (raycastHit4.collider == null) ? float.PositiveInfinity : raycastHit4.distance;
				if (num3 < num && num4 < num && num5 > num)
				{
					vector = base.transform.up;
				}
				else if (num3 > num4)
				{
					vector = -base.transform.right;
				}
				else
				{
					vector = base.transform.right;
				}
			}
		}
		return vector.normalized;
	}

	// Token: 0x040006AC RID: 1708
	[Tooltip("Movement: speed")]
	public float movementSpeed;

	// Token: 0x040006AD RID: 1709
	[Tooltip("Rotation: facing speed")]
	public float rotationFacingSpeed;

	// Token: 0x040006AE RID: 1710
	[Tooltip("Rotation: facing stability")]
	public float rotationFacingStability;

	// Token: 0x040006AF RID: 1711
	[Tooltip("Avoidance: min/max strength of normal adjustment to collision")]
	public Vector2 avoidanceStrength;

	// Token: 0x040006B0 RID: 1712
	public const int AVOIDANCE_MASK = -537968901;

	// Token: 0x040006B3 RID: 1715
	private static readonly float SQRT_TWO = Mathf.Sqrt(2f);
}
