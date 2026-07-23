using System;
using UnityEngine;

// Token: 0x0200087F RID: 2175
public class vp_FPEarthquake : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06002E83 RID: 11907 RVA: 0x000B43D8 File Offset: 0x000B25D8
	private vp_FPPlayerEventHandler FPPlayer
	{
		get
		{
			if (this.m_FPPlayer == null)
			{
				this.m_FPPlayer = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
			}
			return this.m_FPPlayer;
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000B4408 File Offset: 0x000B2608
	protected virtual void OnEnable()
	{
		if (this.FPPlayer != null)
		{
			this.Register(this.FPPlayer);
		}
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000B4424 File Offset: 0x000B2624
	protected virtual void OnDisable()
	{
		if (this.FPPlayer != null)
		{
			this.Unregister(this.FPPlayer);
		}
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000B4440 File Offset: 0x000B2640
	protected void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			this.UpdateEarthQuake();
		}
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000B4454 File Offset: 0x000B2654
	protected void UpdateEarthQuake()
	{
		if (!this.FPPlayer.CameraEarthQuake.Active)
		{
			this.m_CameraEarthQuakeForce = Vector3.zero;
			return;
		}
		this.m_CameraEarthQuakeForce = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(1f), this.m_Magnitude.x * (Vector3.right + Vector3.forward) * Mathf.Min(this.m_Endtime - Time.time, 1f) * Time.timeScale);
		this.m_CameraEarthQuakeForce.y = 0f;
		if (UnityEngine.Random.value < 0.3f * Time.timeScale)
		{
			this.m_CameraEarthQuakeForce.y = UnityEngine.Random.Range(0f, this.m_Magnitude.y * 0.35f) * Mathf.Min(this.m_Endtime - Time.time, 1f);
		}
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000B4538 File Offset: 0x000B2738
	protected virtual void OnStart_CameraEarthQuake()
	{
		Vector3 vector = (Vector3)this.FPPlayer.CameraEarthQuake.Argument;
		this.m_Magnitude.x = vector.x;
		this.m_Magnitude.y = vector.y;
		this.m_Endtime = Time.time + vector.z;
		this.FPPlayer.CameraEarthQuake.AutoDuration = vector.z;
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x000B45A5 File Offset: 0x000B27A5
	protected virtual void OnMessage_CameraBombShake(float impact)
	{
		this.FPPlayer.CameraEarthQuake.TryStart<Vector3>(new Vector3(impact * 0.5f, impact * 0.5f, 1f));
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000B45D0 File Offset: 0x000B27D0
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.RegisterActivity("CameraEarthQuake", new vp_Activity.Callback(this.OnStart_CameraEarthQuake), null, null, null, null, null);
		eventHandler.RegisterValue<Vector3>("CameraEarthQuakeForce", new vp_Value<Vector3>.Getter<Vector3>(this.Get_CameraEarthQuakeForce), new vp_Value<Vector3>.Setter<Vector3>(this.Set_CameraEarthQuakeForce));
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000B4638 File Offset: 0x000B2838
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterMessage<float>("CameraBombShake", new vp_Message<float>.Sender<float>(this.OnMessage_CameraBombShake));
		eventHandler.UnregisterActivity("CameraEarthQuake", new vp_Activity.Callback(this.OnStart_CameraEarthQuake), null, null, null, null, null);
		eventHandler.UnregisterValue<Vector3>("CameraEarthQuakeForce", new vp_Value<Vector3>.Getter<Vector3>(this.Get_CameraEarthQuakeForce), new vp_Value<Vector3>.Setter<Vector3>(this.Set_CameraEarthQuakeForce));
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x000B469F File Offset: 0x000B289F
	protected virtual Vector3 Get_CameraEarthQuakeForce()
	{
		return this.m_CameraEarthQuakeForce;
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x000B46A7 File Offset: 0x000B28A7
	protected virtual void Set_CameraEarthQuakeForce(Vector3 value)
	{
		this.m_CameraEarthQuakeForce = value;
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06002E8E RID: 11918 RVA: 0x000B469F File Offset: 0x000B289F
	// (set) Token: 0x06002E8F RID: 11919 RVA: 0x000B46A7 File Offset: 0x000B28A7
	protected virtual Vector3 OnValue_CameraEarthQuakeForce
	{
		get
		{
			return this.m_CameraEarthQuakeForce;
		}
		set
		{
			this.m_CameraEarthQuakeForce = value;
		}
	}

	// Token: 0x04002C7C RID: 11388
	protected Vector3 m_CameraEarthQuakeForce;

	// Token: 0x04002C7D RID: 11389
	protected float m_Endtime;

	// Token: 0x04002C7E RID: 11390
	protected Vector2 m_Magnitude = Vector2.zero;

	// Token: 0x04002C7F RID: 11391
	private vp_FPPlayerEventHandler m_FPPlayer;
}
