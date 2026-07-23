using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class EQ_OrbitCamera : MonoBehaviour
{
	// Token: 0x060001A3 RID: 419 RVA: 0x0000C684 File Offset: 0x0000A884
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

	// Token: 0x060001A4 RID: 420 RVA: 0x00003296 File Offset: 0x00001496
	private void Update()
	{
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000C6E8 File Offset: 0x0000A8E8
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

	// Token: 0x060001A6 RID: 422 RVA: 0x0000C70C File Offset: 0x0000A90C
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

	// Token: 0x060001A7 RID: 423 RVA: 0x0000C7CC File Offset: 0x0000A9CC
	private void CalculateDesiredPosition()
	{
		this.Distance = Mathf.SmoothDamp(this.Distance, this.desiredDistance, ref this.velocityDistance, this.DistanceSmooth);
		this.desiredPosition = this.CalculatePosition(this.mouseY, this.mouseX, this.Distance);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000C81C File Offset: 0x0000AA1C
	private Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 point = new Vector3(0f, 0f, -distance);
		Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
		return this.TargetLookAt.position + rotation * point;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000C860 File Offset: 0x0000AA60
	private void UpdatePosition()
	{
		float x = Mathf.SmoothDamp(this.position.x, this.desiredPosition.x, ref this.velX, this.X_Smooth);
		float y = Mathf.SmoothDamp(this.position.y, this.desiredPosition.y, ref this.velY, this.Y_Smooth);
		float z = Mathf.SmoothDamp(this.position.z, this.desiredPosition.z, ref this.velZ, this.X_Smooth);
		this.position = new Vector3(x, y, z);
		base.transform.position = this.position;
		base.transform.LookAt(this.TargetLookAt);
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000C915 File Offset: 0x0000AB15
	private void Reset()
	{
		this.mouseX = 0f;
		this.mouseY = 0f;
		this.Distance = this.startingDistance;
		this.desiredDistance = this.Distance;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000B969 File Offset: 0x00009B69
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

	// Token: 0x040001EF RID: 495
	public Transform TargetLookAt;

	// Token: 0x040001F0 RID: 496
	public float Distance = 10f;

	// Token: 0x040001F1 RID: 497
	public float DistanceMin = 5f;

	// Token: 0x040001F2 RID: 498
	public float DistanceMax = 15f;

	// Token: 0x040001F3 RID: 499
	private float startingDistance;

	// Token: 0x040001F4 RID: 500
	private float desiredDistance;

	// Token: 0x040001F5 RID: 501
	private float mouseX;

	// Token: 0x040001F6 RID: 502
	private float mouseY;

	// Token: 0x040001F7 RID: 503
	public float X_MouseSensitivity = 5f;

	// Token: 0x040001F8 RID: 504
	public float Y_MouseSensitivity = 5f;

	// Token: 0x040001F9 RID: 505
	public float MouseWheelSensitivity = 5f;

	// Token: 0x040001FA RID: 506
	public float Y_MinLimit = 15f;

	// Token: 0x040001FB RID: 507
	public float Y_MaxLimit = 70f;

	// Token: 0x040001FC RID: 508
	public float DistanceSmooth = 0.025f;

	// Token: 0x040001FD RID: 509
	private float velocityDistance;

	// Token: 0x040001FE RID: 510
	private Vector3 desiredPosition = Vector3.zero;

	// Token: 0x040001FF RID: 511
	public float X_Smooth = 0.05f;

	// Token: 0x04000200 RID: 512
	public float Y_Smooth = 0.1f;

	// Token: 0x04000201 RID: 513
	private float velX;

	// Token: 0x04000202 RID: 514
	private float velY;

	// Token: 0x04000203 RID: 515
	private float velZ;

	// Token: 0x04000204 RID: 516
	private Vector3 position = Vector3.zero;
}
