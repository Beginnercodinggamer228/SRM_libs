using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class EQ_ParticleMoveSample : MonoBehaviour
{
	// Token: 0x060001AD RID: 429 RVA: 0x00003296 File Offset: 0x00001496
	private void Start()
	{
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000C9EC File Offset: 0x0000ABEC
	private void Update()
	{
		if (this.m_MoveMethod != this.m_MoveMethodOld || this.m_Range != this.m_RangeOld)
		{
			this.m_MoveMethodOld = this.m_MoveMethod;
			this.ResetPosition();
		}
		switch (this.m_MoveMethod)
		{
		case eMoveMethod.LeftRight:
			this.UpdateLeftRight();
			return;
		case eMoveMethod.UpDown:
			this.UpdateUpDown();
			return;
		case eMoveMethod.CircularXY_Clockwise:
			this.UpdateCircularXY_Clockwise();
			return;
		case eMoveMethod.CircularXY_CounterClockwise:
			this.UpdateCircularXY_CounterClockwise();
			return;
		case eMoveMethod.CircularXZ_Clockwise:
			this.UpdateCircularXZ_Clockwise();
			return;
		case eMoveMethod.CircularXZ_CounterClockwise:
			this.UpdateCircularXZ_CounterClockwise();
			return;
		case eMoveMethod.CircularYZ_Clockwise:
			this.UpdateCircularYZ_Clockwise();
			return;
		case eMoveMethod.CircularYZ_CounterClockwise:
			this.UpdateCircularYZ_CounterClockwise();
			return;
		default:
			return;
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000CA8C File Offset: 0x0000AC8C
	private void ResetPosition()
	{
		switch (this.m_MoveMethod)
		{
		case eMoveMethod.LeftRight:
		case eMoveMethod.UpDown:
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			break;
		case eMoveMethod.CircularXY_Clockwise:
		case eMoveMethod.CircularXY_CounterClockwise:
		case eMoveMethod.CircularXZ_Clockwise:
		case eMoveMethod.CircularXZ_CounterClockwise:
			base.transform.localPosition = new Vector3(this.m_Range, 0f, 0f);
			break;
		case eMoveMethod.CircularYZ_Clockwise:
		case eMoveMethod.CircularYZ_CounterClockwise:
			base.transform.localPosition = new Vector3(0f, 0f, this.m_Range);
			break;
		}
		this.m_RangeOld = this.m_Range;
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000CB38 File Offset: 0x0000AD38
	private void UpdateLeftRight()
	{
		if (!this.m_ToggleDirectionFlag)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x - this.m_Speed * Time.deltaTime, 0f, 0f);
			if (base.transform.localPosition.x <= -this.m_Range)
			{
				this.m_ToggleDirectionFlag = true;
				return;
			}
		}
		else
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x + this.m_Speed * Time.deltaTime, 0f, 0f);
			if (base.transform.localPosition.x >= this.m_Range)
			{
				this.m_ToggleDirectionFlag = false;
			}
		}
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000CBFC File Offset: 0x0000ADFC
	private void UpdateUpDown()
	{
		if (!this.m_ToggleDirectionFlag)
		{
			base.transform.localPosition = new Vector3(0f, base.transform.localPosition.y + this.m_Speed * Time.deltaTime, 0f);
			if (base.transform.localPosition.y >= this.m_Range)
			{
				this.m_ToggleDirectionFlag = true;
				return;
			}
		}
		else
		{
			base.transform.localPosition = new Vector3(0f, base.transform.localPosition.y - this.m_Speed * Time.deltaTime, 0f);
			if (base.transform.localPosition.y <= -this.m_Range)
			{
				this.m_ToggleDirectionFlag = false;
			}
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
	private void UpdateCircularXY_Clockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float x = base.transform.localPosition.x;
		float y = base.transform.localPosition.y;
		float x2 = num + ((x - num) * Mathf.Cos(this.m_Speed / 360f) - (y - num2) * Mathf.Sin(this.m_Speed / 360f));
		float y2 = num2 + ((x - num) * Mathf.Sin(this.m_Speed / 360f) + (y - num2) * Mathf.Cos(this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(x2, y2, 0f);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000CD74 File Offset: 0x0000AF74
	private void UpdateCircularXY_CounterClockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float x = base.transform.localPosition.x;
		float y = base.transform.localPosition.y;
		float x2 = num + ((x - num) * Mathf.Cos(-this.m_Speed / 360f) - (y - num2) * Mathf.Sin(-this.m_Speed / 360f));
		float y2 = num2 + ((x - num) * Mathf.Sin(-this.m_Speed / 360f) + (y - num2) * Mathf.Cos(-this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(x2, y2, 0f);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000CE2C File Offset: 0x0000B02C
	private void UpdateCircularXZ_Clockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float x = base.transform.localPosition.x;
		float z = base.transform.localPosition.z;
		float x2 = num + ((x - num) * Mathf.Cos(this.m_Speed / 360f) - (z - num2) * Mathf.Sin(this.m_Speed / 360f));
		float z2 = num2 + ((x - num) * Mathf.Sin(this.m_Speed / 360f) + (z - num2) * Mathf.Cos(this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(x2, 0f, z2);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000CEE0 File Offset: 0x0000B0E0
	private void UpdateCircularXZ_CounterClockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float x = base.transform.localPosition.x;
		float z = base.transform.localPosition.z;
		float x2 = num + ((x - num) * Mathf.Cos(-this.m_Speed / 360f) - (z - num2) * Mathf.Sin(-this.m_Speed / 360f));
		float z2 = num2 + ((x - num) * Mathf.Sin(-this.m_Speed / 360f) + (z - num2) * Mathf.Cos(-this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(x2, 0f, z2);
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000CF98 File Offset: 0x0000B198
	private void UpdateCircularYZ_Clockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float y = base.transform.localPosition.y;
		float z = base.transform.localPosition.z;
		float y2 = num + ((y - num) * Mathf.Cos(this.m_Speed / 360f) - (z - num2) * Mathf.Sin(this.m_Speed / 360f));
		float z2 = num2 + ((y - num) * Mathf.Sin(this.m_Speed / 360f) + (z - num2) * Mathf.Cos(this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(0f, y2, z2);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000D04C File Offset: 0x0000B24C
	private void UpdateCircularYZ_CounterClockwise()
	{
		float num = 0f;
		float num2 = 0f;
		float y = base.transform.localPosition.y;
		float z = base.transform.localPosition.z;
		float y2 = num + ((y - num) * Mathf.Cos(-this.m_Speed / 360f) - (z - num2) * Mathf.Sin(-this.m_Speed / 360f));
		float z2 = num2 + ((y - num) * Mathf.Sin(-this.m_Speed / 360f) + (z - num2) * Mathf.Cos(-this.m_Speed / 360f));
		base.transform.localPosition = new Vector3(0f, y2, z2);
	}

	// Token: 0x0400020E RID: 526
	public eMoveMethod m_MoveMethod;

	// Token: 0x0400020F RID: 527
	private eMoveMethod m_MoveMethodOld;

	// Token: 0x04000210 RID: 528
	public float m_Range = 5f;

	// Token: 0x04000211 RID: 529
	private float m_RangeOld = 5f;

	// Token: 0x04000212 RID: 530
	public float m_Speed = 2f;

	// Token: 0x04000213 RID: 531
	private bool m_ToggleDirectionFlag;
}
