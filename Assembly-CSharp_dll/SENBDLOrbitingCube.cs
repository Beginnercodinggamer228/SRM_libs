using System;
using UnityEngine;

// Token: 0x020007E9 RID: 2025
public class SENBDLOrbitingCube : MonoBehaviour
{
	// Token: 0x06002A5D RID: 10845 RVA: 0x0009EAE2 File Offset: 0x0009CCE2
	private Vector3 Vec3(float x)
	{
		return new Vector3(x, x, x);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x0009F124 File Offset: 0x0009D324
	private void Start()
	{
		this.transf = base.transform;
		this.rotationVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		this.rotationVector = Vector3.Normalize(this.rotationVector);
		this.spherePosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		this.spherePosition = Vector3.Normalize(this.spherePosition);
		this.spherePosition *= UnityEngine.Random.Range(16.5f, 18f);
		this.randomSphereRotation = new Vector3(UnityEngine.Random.Range(-1.1f, 1f), UnityEngine.Random.Range(0f, 0.1f), UnityEngine.Random.Range(0.5f, 1f));
		this.randomSphereRotation = Vector3.Normalize(this.randomSphereRotation);
		this.sphereRotationSpeed = UnityEngine.Random.Range(10f, 20f);
		this.rotationSpeed = UnityEngine.Random.Range(1f, 90f);
		this.transf.localScale = this.Vec3(UnityEngine.Random.Range(1f, 2f));
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x0009F284 File Offset: 0x0009D484
	private void Update()
	{
		Quaternion rotation = Quaternion.Euler(this.randomSphereRotation * Time.time * this.sphereRotationSpeed);
		Vector3 vector = rotation * this.spherePosition;
		vector += this.spherePosition * (Mathf.Sin(Time.time - this.spherePosition.magnitude / 10f) * 0.5f + 0.5f);
		vector += rotation * this.spherePosition * (Mathf.Sin(Time.time * 3.1415265f / 4f - this.spherePosition.magnitude / 10f) * 0.5f + 0.5f);
		this.transf.position = vector;
		this.transf.rotation = Quaternion.Euler(this.rotationVector * Time.time * this.rotationSpeed);
	}

	// Token: 0x04002975 RID: 10613
	private Transform transf;

	// Token: 0x04002976 RID: 10614
	private Vector3 rotationVector;

	// Token: 0x04002977 RID: 10615
	private float rotationSpeed;

	// Token: 0x04002978 RID: 10616
	private Vector3 spherePosition;

	// Token: 0x04002979 RID: 10617
	private Vector3 randomSphereRotation;

	// Token: 0x0400297A RID: 10618
	private float sphereRotationSpeed;
}
