using System;
using UnityEngine;

// Token: 0x02000834 RID: 2100
public class vp_Spring
{
	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06002C03 RID: 11267 RVA: 0x000A600E File Offset: 0x000A420E
	// (set) Token: 0x06002C04 RID: 11268 RVA: 0x000A6016 File Offset: 0x000A4216
	public Transform Transform
	{
		get
		{
			return this.m_Transform;
		}
		set
		{
			this.m_Transform = value;
			this.RefreshUpdateMode();
		}
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x000A6028 File Offset: 0x000A4228
	public vp_Spring(Transform transform, vp_Spring.UpdateMode mode, bool autoUpdate = true)
	{
		this.Mode = mode;
		this.Transform = transform;
		this.m_AutoUpdate = autoUpdate;
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x000A6110 File Offset: 0x000A4310
	public void FixedUpdate()
	{
		if (this.m_VelocityFadeInEndTime > Time.time)
		{
			this.m_VelocityFadeInCap = Mathf.Clamp01(1f - (this.m_VelocityFadeInEndTime - Time.time) / this.m_VelocityFadeInLength);
		}
		else
		{
			this.m_VelocityFadeInCap = 1f;
		}
		if (this.m_SoftForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SoftForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SoftForceFrame[i] = ((i < 119) ? this.m_SoftForceFrame[i + 1] : Vector3.zero);
				if (this.m_SoftForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.Calculate();
		this.m_UpdateFunc();
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x000A61E1 File Offset: 0x000A43E1
	private void Position()
	{
		this.m_Transform.localPosition = this.State;
	}

	// Token: 0x06002C08 RID: 11272 RVA: 0x000A61F4 File Offset: 0x000A43F4
	private void Rotation()
	{
		this.m_Transform.localEulerAngles = this.State;
	}

	// Token: 0x06002C09 RID: 11273 RVA: 0x000A6207 File Offset: 0x000A4407
	private void Scale()
	{
		this.m_Transform.localScale = this.State;
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x000A621A File Offset: 0x000A441A
	private void PositionAdditiveLocal()
	{
		this.m_Transform.localPosition += this.State;
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x000A6238 File Offset: 0x000A4438
	private void PositionAdditiveGlobal()
	{
		this.m_Transform.position += this.State;
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x000A6256 File Offset: 0x000A4456
	private void PositionAdditiveSelf()
	{
		this.m_Transform.Translate(this.State, this.m_Transform);
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x000A626F File Offset: 0x000A446F
	private void RotationAdditiveLocal()
	{
		this.m_Transform.localEulerAngles += this.State;
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000A628D File Offset: 0x000A448D
	private void RotationAdditiveGlobal()
	{
		this.m_Transform.eulerAngles += this.State;
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x000A62AB File Offset: 0x000A44AB
	private void ScaleAdditiveLocal()
	{
		this.m_Transform.localScale += this.State;
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x00003296 File Offset: 0x00001496
	private void None()
	{
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x000A62CC File Offset: 0x000A44CC
	protected void RefreshUpdateMode()
	{
		this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.None);
		switch (this.Mode)
		{
		case vp_Spring.UpdateMode.Position:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Position);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveLocal:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveLocal);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveGlobal:
			this.State = this.m_Transform.position;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveGlobal);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveSelf:
			this.State = this.m_Transform.position;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveSelf);
			}
			break;
		case vp_Spring.UpdateMode.Rotation:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Rotation);
			}
			break;
		case vp_Spring.UpdateMode.RotationAdditiveLocal:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.RotationAdditiveLocal);
			}
			break;
		case vp_Spring.UpdateMode.RotationAdditiveGlobal:
			this.State = this.m_Transform.eulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.RotationAdditiveGlobal);
			}
			break;
		case vp_Spring.UpdateMode.Scale:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Scale);
			}
			break;
		case vp_Spring.UpdateMode.ScaleAdditiveLocal:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.ScaleAdditiveLocal);
			}
			break;
		}
		this.RestState = this.State;
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x000A64E4 File Offset: 0x000A46E4
	protected void Calculate()
	{
		if (this.State == this.RestState)
		{
			return;
		}
		this.m_Velocity += Vector3.Scale(this.RestState - this.State, this.Stiffness);
		this.m_Velocity = Vector3.Scale(this.m_Velocity, this.Damping);
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		if (this.m_Velocity.sqrMagnitude > this.MinVelocity * this.MinVelocity)
		{
			this.Move();
			return;
		}
		this.Reset();
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x000A6587 File Offset: 0x000A4787
	private void AddForceInternal(Vector3 force)
	{
		force *= this.m_VelocityFadeInCap;
		this.m_Velocity += force;
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		this.Move();
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x000A65C6 File Offset: 0x000A47C6
	public void AddForce(Vector3 force)
	{
		if (Time.timeScale < 1f)
		{
			this.AddSoftForce(force, 1f);
			return;
		}
		this.AddForceInternal(force);
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000A65E8 File Offset: 0x000A47E8
	public void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			this.m_SoftForceFrame[i] += force / frames;
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x000A6658 File Offset: 0x000A4858
	protected void Move()
	{
		this.State += this.m_Velocity * Time.timeScale;
		this.State.x = Mathf.Clamp(this.State.x, this.MinState.x, this.MaxState.x);
		this.State.y = Mathf.Clamp(this.State.y, this.MinState.y, this.MaxState.y);
		this.State.z = Mathf.Clamp(this.State.z, this.MinState.z, this.MaxState.z);
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x000A6719 File Offset: 0x000A4919
	public void Reset()
	{
		this.m_Velocity = Vector3.zero;
		this.State = this.RestState;
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000A6732 File Offset: 0x000A4932
	public void Stop(bool includeSoftForce = false)
	{
		this.m_Velocity = Vector3.zero;
		if (includeSoftForce)
		{
			this.StopSoftForce();
		}
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000A6748 File Offset: 0x000A4948
	public void StopSoftForce()
	{
		for (int i = 0; i < 120; i++)
		{
			this.m_SoftForceFrame[i] = Vector3.zero;
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000A6773 File Offset: 0x000A4973
	public void ForceVelocityFadeIn(float seconds)
	{
		this.m_VelocityFadeInLength = seconds;
		this.m_VelocityFadeInEndTime = Time.time + seconds;
		this.m_VelocityFadeInCap = 0f;
	}

	// Token: 0x04002A34 RID: 10804
	protected vp_Spring.UpdateMode Mode;

	// Token: 0x04002A35 RID: 10805
	protected bool m_AutoUpdate = true;

	// Token: 0x04002A36 RID: 10806
	protected vp_Spring.UpdateDelegate m_UpdateFunc;

	// Token: 0x04002A37 RID: 10807
	public Vector3 State = Vector3.zero;

	// Token: 0x04002A38 RID: 10808
	protected Vector3 m_Velocity = Vector3.zero;

	// Token: 0x04002A39 RID: 10809
	public Vector3 RestState = Vector3.zero;

	// Token: 0x04002A3A RID: 10810
	public Vector3 Stiffness = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x04002A3B RID: 10811
	public Vector3 Damping = new Vector3(0.75f, 0.75f, 0.75f);

	// Token: 0x04002A3C RID: 10812
	protected float m_VelocityFadeInCap = 1f;

	// Token: 0x04002A3D RID: 10813
	protected float m_VelocityFadeInEndTime;

	// Token: 0x04002A3E RID: 10814
	protected float m_VelocityFadeInLength;

	// Token: 0x04002A3F RID: 10815
	protected Vector3[] m_SoftForceFrame = new Vector3[120];

	// Token: 0x04002A40 RID: 10816
	public float MaxVelocity = 10000f;

	// Token: 0x04002A41 RID: 10817
	public float MinVelocity = 1E-07f;

	// Token: 0x04002A42 RID: 10818
	public Vector3 MaxState = new Vector3(10000f, 10000f, 10000f);

	// Token: 0x04002A43 RID: 10819
	public Vector3 MinState = new Vector3(-10000f, -10000f, -10000f);

	// Token: 0x04002A44 RID: 10820
	protected Transform m_Transform;

	// Token: 0x02000835 RID: 2101
	public enum UpdateMode
	{
		// Token: 0x04002A46 RID: 10822
		Position,
		// Token: 0x04002A47 RID: 10823
		PositionAdditiveLocal,
		// Token: 0x04002A48 RID: 10824
		PositionAdditiveGlobal,
		// Token: 0x04002A49 RID: 10825
		PositionAdditiveSelf,
		// Token: 0x04002A4A RID: 10826
		Rotation,
		// Token: 0x04002A4B RID: 10827
		RotationAdditiveLocal,
		// Token: 0x04002A4C RID: 10828
		RotationAdditiveGlobal,
		// Token: 0x04002A4D RID: 10829
		Scale,
		// Token: 0x04002A4E RID: 10830
		ScaleAdditiveLocal
	}

	// Token: 0x02000836 RID: 2102
	// (Invoke) Token: 0x06002C1C RID: 11292
	protected delegate void UpdateDelegate();
}
