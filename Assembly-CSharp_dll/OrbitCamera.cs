using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x0600018D RID: 397 RVA: 0x0000B6A8 File Offset: 0x000098A8
	private void Start()
	{
		this.Distance = Vector3.Distance(this.TargetLookAt.transform.position, base.gameObject.transform.position);
		if (this.Distance > this.DistanceMax)
		{
			this.DistanceMax = this.Distance;
		}
		this.startingDistance = this.Distance;
		this.Reset();
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00003296 File Offset: 0x00001496
	private void Update()
	{
	}

	// Token: 0x0600018F RID: 399 RVA: 0x0000B70C File Offset: 0x0000990C
	private void LateUpdate()
	{
		if (this.TargetLookAt == null)
		{
			return;
		}
		this.HandlePlayerInput();
		this.CalculateDesiredPosition();
		this.UpdatePosition();
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000B730 File Offset: 0x00009930
	private void HandlePlayerInput()
	{
		float num = 0.01f;
		if (Input.GetMouseButton(0))
		{
			this.mouseX += Input.GetAxis("Mouse X") * this.X_MouseSensitivity;
			this.mouseY -= Input.GetAxis("Mouse Y") * this.Y_MouseSensitivity;
		}
		this.mouseY = this.ClampAngle(this.mouseY, this.Y_MinLimit, this.Y_MaxLimit);
		if (Input.GetAxis("Mouse ScrollWheel") < -num || Input.GetAxis("Mouse ScrollWheel") > num)
		{
			this.desiredDistance = Mathf.Clamp(this.Distance - Input.GetAxis("Mouse ScrollWheel") * this.MouseWheelSensitivity, this.DistanceMin, this.DistanceMax);
		}
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000B7F0 File Offset: 0x000099F0
	private void CalculateDesiredPosition()
	{
		this.Distance = Mathf.SmoothDamp(this.Distance, this.desiredDistance, ref this.velocityDistance, this.DistanceSmooth);
		this.desiredPosition = this.CalculatePosition(this.mouseY, this.mouseX, this.Distance);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000B840 File Offset: 0x00009A40
	private Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 point = new Vector3(0f, 0f, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
		return this.TargetLookAt.position + rotation * point;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000B884 File Offset: 0x00009A84
	private void UpdatePosition()
	{
		float x = Mathf.SmoothDamp(this.position.x, this.desiredPosition.x, ref this.velX, this.X_Smooth);
		float y = Mathf.SmoothDamp(this.position.y, this.desiredPosition.y, ref this.velY, this.Y_Smooth);
		float z = Mathf.SmoothDamp(this.position.z, this.desiredPosition.z, ref this.velZ, this.X_Smooth);
		this.position = new Vector3(x, y, z);
		base.transform.position = this.position;
		base.transform.LookAt(this.TargetLookAt);
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000B939 File Offset: 0x00009B39
	private void Reset()
	{
		this.mouseX = 0f;
		this.mouseY = 0f;
		this.Distance = this.startingDistance;
		this.desiredDistance = this.Distance;
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000B969 File Offset: 0x00009B69
	private float ClampAngle(float angle, float min, float max)
	{
		while (angle < -360f || angle > 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x040001BC RID: 444
	public Transform TargetLookAt;

	// Token: 0x040001BD RID: 445
	public float Distance = 10f;

	// Token: 0x040001BE RID: 446
	public float DistanceMin = 5f;

	// Token: 0x040001BF RID: 447
	public float DistanceMax = 15f;

	// Token: 0x040001C0 RID: 448
	private float startingDistance;

	// Token: 0x040001C1 RID: 449
	private float desiredDistance;

	// Token: 0x040001C2 RID: 450
	private float mouseX;

	// Token: 0x040001C3 RID: 451
	private float mouseY;

	// Token: 0x040001C4 RID: 452
	public float X_MouseSensitivity = 5f;

	// Token: 0x040001C5 RID: 453
	public float Y_MouseSensitivity = 5f;

	// Token: 0x040001C6 RID: 454
	public float MouseWheelSensitivity = 5f;

	// Token: 0x040001C7 RID: 455
	public float Y_MinLimit = 15f;

	// Token: 0x040001C8 RID: 456
	public float Y_MaxLimit = 70f;

	// Token: 0x040001C9 RID: 457
	public float DistanceSmooth = 0.025f;

	// Token: 0x040001CA RID: 458
	private float velocityDistance;

	// Token: 0x040001CB RID: 459
	private Vector3 desiredPosition = Vector3.zero;

	// Token: 0x040001CC RID: 460
	public float X_Smooth = 0.05f;

	// Token: 0x040001CD RID: 461
	public float Y_Smooth = 0.1f;

	// Token: 0x040001CE RID: 462
	private float velX;

	// Token: 0x040001CF RID: 463
	private float velY;

	// Token: 0x040001D0 RID: 464
	private float velZ;

	// Token: 0x040001D1 RID: 465
	private Vector3 position = Vector3.zero;
}
