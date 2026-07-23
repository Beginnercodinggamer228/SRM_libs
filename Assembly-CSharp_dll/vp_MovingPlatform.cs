using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000855 RID: 2133
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class vp_MovingPlatform : MonoBehaviour
{
	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06002D0B RID: 11531 RVA: 0x000AA902 File Offset: 0x000A8B02
	public int TargetedWaypoint
	{
		get
		{
			return this.m_TargetedWayPoint;
		}
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x000AA90A File Offset: 0x000A8B0A
	private void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x000AA91D File Offset: 0x000A8B1D
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		vp_MovingPlatform.m_KnownPlayers.Clear();
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000AA92C File Offset: 0x000A8B2C
	private void Start()
	{
		this.m_Transform = base.transform;
		this.m_Collider = base.GetComponentInChildren<Collider>();
		this.m_RigidBody = base.GetComponent<Rigidbody>();
		this.m_RigidBody.useGravity = false;
		this.m_RigidBody.isKinematic = true;
		this.m_NextWaypoint = 0;
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Audio.loop = true;
		this.m_Audio.clip = this.SoundMove;
		if (this.PathWaypoints == null)
		{
			return;
		}
		base.gameObject.layer = 28;
		foreach (object obj in this.PathWaypoints.transform)
		{
			Transform transform = (Transform)obj;
			if (vp_Utility.IsActive(transform.gameObject))
			{
				this.m_Waypoints.Add(transform);
				transform.gameObject.layer = 28;
			}
			if (transform.GetComponent<Renderer>() != null)
			{
				transform.GetComponent<Renderer>().enabled = false;
			}
			if (transform.GetComponent<Collider>() != null)
			{
				transform.GetComponent<Collider>().enabled = false;
			}
		}
		IComparer @object = new vp_MovingPlatform.WaypointComparer();
		this.m_Waypoints.Sort(new Comparison<Transform>(@object.Compare));
		if (this.m_Waypoints.Count > 0)
		{
			this.m_CurrentTargetPosition = this.m_Waypoints[this.m_NextWaypoint].position;
			this.m_CurrentTargetAngle = this.m_Waypoints[this.m_NextWaypoint].eulerAngles;
			this.m_Transform.position = this.m_CurrentTargetPosition;
			this.m_Transform.eulerAngles = this.m_CurrentTargetAngle;
			if (this.MoveAutoStartTarget > this.m_Waypoints.Count - 1)
			{
				this.MoveAutoStartTarget = this.m_Waypoints.Count - 1;
			}
		}
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x000AAB18 File Offset: 0x000A8D18
	private void FixedUpdate()
	{
		this.UpdatePath();
		this.UpdateMovement();
		this.UpdateRotation();
		this.UpdateVelocity();
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x000AAB34 File Offset: 0x000A8D34
	protected void UpdatePath()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		if (this.GetDistanceLeft() < 0.01f && Time.time >= this.m_NextAllowedMoveTime)
		{
			switch (this.PathType)
			{
			case vp_MovingPlatform.PathMoveType.PingPong:
				if (this.PathDirection == vp_MovingPlatform.Direction.Backwards)
				{
					if (this.m_NextWaypoint == 0)
					{
						this.PathDirection = vp_MovingPlatform.Direction.Forward;
					}
				}
				else if (this.m_NextWaypoint == this.m_Waypoints.Count - 1)
				{
					this.PathDirection = vp_MovingPlatform.Direction.Backwards;
				}
				this.OnArriveAtWaypoint();
				this.GoToNextWaypoint();
				break;
			case vp_MovingPlatform.PathMoveType.Loop:
				this.OnArriveAtWaypoint();
				this.GoToNextWaypoint();
				return;
			case vp_MovingPlatform.PathMoveType.Target:
				if (this.m_NextWaypoint != this.m_TargetedWayPoint)
				{
					if (this.m_Moving)
					{
						if (this.m_PhysicsCurrentMoveVelocity == 0f)
						{
							this.OnStart();
						}
						else
						{
							this.OnArriveAtWaypoint();
						}
					}
					this.GoToNextWaypoint();
					return;
				}
				if (this.m_Moving)
				{
					this.OnStop();
					return;
				}
				if (this.m_NextWaypoint != 0)
				{
					this.OnArriveAtDestination();
					return;
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x000AAC32 File Offset: 0x000A8E32
	protected void OnStart()
	{
		if (this.SoundStart != null)
		{
			this.m_Audio.PlayOneShot(this.SoundStart);
		}
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x000AAC53 File Offset: 0x000A8E53
	protected void OnArriveAtWaypoint()
	{
		if (this.SoundWaypoint != null)
		{
			this.m_Audio.PlayOneShot(this.SoundWaypoint);
		}
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000AAC74 File Offset: 0x000A8E74
	protected void OnArriveAtDestination()
	{
		if (this.MoveReturnDelay > 0f && !this.m_ReturnDelayTimer.Active)
		{
			vp_Timer.In(this.MoveReturnDelay, delegate()
			{
				this.GoTo(0);
			}, this.m_ReturnDelayTimer);
		}
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000AACB0 File Offset: 0x000A8EB0
	protected void OnStop()
	{
		this.m_Audio.Stop();
		if (this.SoundStop != null)
		{
			this.m_Audio.PlayOneShot(this.SoundStop);
		}
		this.m_Transform.position = this.m_CurrentTargetPosition;
		this.m_Transform.eulerAngles = this.m_CurrentTargetAngle;
		this.m_Moving = false;
		if (this.m_NextWaypoint == 0)
		{
			this.m_NextAllowedMoveTime = Time.time + this.MoveCooldown;
		}
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000AAD2C File Offset: 0x000A8F2C
	protected void UpdateMovement()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		switch (this.MoveInterpolationMode)
		{
		case vp_MovingPlatform.MovementInterpolationMode.EaseInOut:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_EaseInOutCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			return;
		case vp_MovingPlatform.MovementInterpolationMode.EaseIn:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.MoveTowards(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_MoveTime), default(Vector3));
			return;
		case vp_MovingPlatform.MovementInterpolationMode.EaseOut:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_LinearCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			return;
		case vp_MovingPlatform.MovementInterpolationMode.EaseOut2:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.MoveSpeed * 0.25f), default(Vector3));
			return;
		case vp_MovingPlatform.MovementInterpolationMode.Slerp:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Slerp(this.m_Transform.position, this.m_CurrentTargetPosition, this.m_LinearCurve.Evaluate(this.m_MoveTime)), default(Vector3));
			return;
		case vp_MovingPlatform.MovementInterpolationMode.Lerp:
			this.m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.MoveTowards(this.m_Transform.position, this.m_CurrentTargetPosition, this.MoveSpeed), default(Vector3));
			return;
		default:
			return;
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x000AAED8 File Offset: 0x000A90D8
	protected void UpdateRotation()
	{
		switch (this.RotationInterpolationMode)
		{
		case vp_MovingPlatform.RotateInterpolationMode.SyncToMovement:
			if (this.m_Moving)
			{
				this.m_Transform.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_OriginalAngle.x, this.m_CurrentTargetAngle.x, 1f - this.GetDistanceLeft() / this.m_TravelDistance), Mathf.LerpAngle(this.m_OriginalAngle.y, this.m_CurrentTargetAngle.y, 1f - this.GetDistanceLeft() / this.m_TravelDistance), Mathf.LerpAngle(this.m_OriginalAngle.z, this.m_CurrentTargetAngle.z, 1f - this.GetDistanceLeft() / this.m_TravelDistance)), default(Vector3));
				return;
			}
			break;
		case vp_MovingPlatform.RotateInterpolationMode.EaseOut:
			this.m_Transform.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_Transform.eulerAngles.x, this.m_CurrentTargetAngle.x, this.m_LinearCurve.Evaluate(this.m_MoveTime)), Mathf.LerpAngle(this.m_Transform.eulerAngles.y, this.m_CurrentTargetAngle.y, this.m_LinearCurve.Evaluate(this.m_MoveTime)), Mathf.LerpAngle(this.m_Transform.eulerAngles.z, this.m_CurrentTargetAngle.z, this.m_LinearCurve.Evaluate(this.m_MoveTime))), default(Vector3));
			return;
		case vp_MovingPlatform.RotateInterpolationMode.CustomEaseOut:
			this.m_Transform.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(this.m_Transform.eulerAngles.x, this.m_CurrentTargetAngle.x, this.RotationEaseAmount), Mathf.LerpAngle(this.m_Transform.eulerAngles.y, this.m_CurrentTargetAngle.y, this.RotationEaseAmount), Mathf.LerpAngle(this.m_Transform.eulerAngles.z, this.m_CurrentTargetAngle.z, this.RotationEaseAmount)), default(Vector3));
			return;
		case vp_MovingPlatform.RotateInterpolationMode.CustomRotate:
			this.m_Transform.Rotate(this.RotationSpeed);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000AB10C File Offset: 0x000A930C
	protected void UpdateVelocity()
	{
		this.m_MoveTime += this.MoveSpeed * 0.01f * vp_TimeUtility.AdjustedTimeScale;
		this.m_PhysicsCurrentMoveVelocity = (this.m_Transform.position - this.m_PrevPos).magnitude;
		this.m_PhysicsCurrentRotationVelocity = (this.m_Transform.eulerAngles - this.m_PrevAngle).magnitude;
		this.m_PrevPos = this.m_Transform.position;
		this.m_PrevAngle = this.m_Transform.eulerAngles;
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000AB1A4 File Offset: 0x000A93A4
	public void GoTo(int targetWayPoint)
	{
		if (!vp_Gameplay.isMaster)
		{
			return;
		}
		if (Time.time < this.m_NextAllowedMoveTime)
		{
			return;
		}
		if (this.PathType != vp_MovingPlatform.PathMoveType.Target)
		{
			return;
		}
		this.m_TargetedWayPoint = this.GetValidWaypoint(targetWayPoint);
		if (targetWayPoint > this.m_NextWaypoint)
		{
			if (this.PathDirection != vp_MovingPlatform.Direction.Direct)
			{
				this.PathDirection = vp_MovingPlatform.Direction.Forward;
			}
		}
		else if (this.PathDirection != vp_MovingPlatform.Direction.Direct)
		{
			this.PathDirection = vp_MovingPlatform.Direction.Backwards;
		}
		this.m_Moving = true;
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000AB210 File Offset: 0x000A9410
	protected float GetDistanceLeft()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return 0f;
		}
		return Vector3.Distance(this.m_Transform.position, this.m_Waypoints[this.m_NextWaypoint].position);
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000AB24C File Offset: 0x000A944C
	protected void GoToNextWaypoint()
	{
		if (this.m_Waypoints.Count < 2)
		{
			return;
		}
		this.m_MoveTime = 0f;
		if (!this.m_Audio.isPlaying)
		{
			this.m_Audio.Play();
		}
		this.m_CurrentWaypoint = this.m_NextWaypoint;
		switch (this.PathDirection)
		{
		case vp_MovingPlatform.Direction.Forward:
			this.m_NextWaypoint = this.GetValidWaypoint(this.m_NextWaypoint + 1);
			break;
		case vp_MovingPlatform.Direction.Backwards:
			this.m_NextWaypoint = this.GetValidWaypoint(this.m_NextWaypoint - 1);
			break;
		case vp_MovingPlatform.Direction.Direct:
			this.m_NextWaypoint = this.m_TargetedWayPoint;
			break;
		}
		this.m_OriginalAngle = this.m_CurrentTargetAngle;
		this.m_CurrentTargetPosition = this.m_Waypoints[this.m_NextWaypoint].position;
		this.m_CurrentTargetAngle = this.m_Waypoints[this.m_NextWaypoint].eulerAngles;
		this.m_TravelDistance = this.GetDistanceLeft();
		this.m_Moving = true;
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000AB341 File Offset: 0x000A9541
	protected int GetValidWaypoint(int wayPoint)
	{
		if (wayPoint < 0)
		{
			return this.m_Waypoints.Count - 1;
		}
		if (wayPoint > this.m_Waypoints.Count - 1)
		{
			return 0;
		}
		return wayPoint;
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000AB368 File Offset: 0x000A9568
	protected void OnTriggerEnter(Collider col)
	{
		if (!this.GetPlayer(col))
		{
			return;
		}
		this.TryPushPlayer();
		this.TryAutoStart();
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000AB380 File Offset: 0x000A9580
	protected void OnTriggerStay(Collider col)
	{
		if (!this.PhysicsSnapPlayerToTopOnIntersect)
		{
			return;
		}
		if (!this.GetPlayer(col))
		{
			return;
		}
		this.TrySnapPlayerToTop();
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000AB39C File Offset: 0x000A959C
	public bool GetPlayer(Collider col)
	{
		if (col.gameObject.layer != 8)
		{
			return false;
		}
		if (!vp_MovingPlatform.m_KnownPlayers.ContainsKey(col))
		{
			vp_PlayerEventHandler component = col.transform.root.GetComponent<vp_PlayerEventHandler>();
			if (component == null)
			{
				return false;
			}
			vp_MovingPlatform.m_KnownPlayers.Add(col, component);
		}
		if (!vp_MovingPlatform.m_KnownPlayers.TryGetValue(col, out this.m_PlayerToPush))
		{
			return false;
		}
		this.m_PlayerCollider = col;
		return true;
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000AB40C File Offset: 0x000A960C
	protected void TryPushPlayer()
	{
		if (this.m_PlayerToPush == null || this.m_PlayerToPush.Platform == null)
		{
			return;
		}
		if (this.m_PlayerToPush.Position.Get().y > this.m_Collider.bounds.max.y)
		{
			return;
		}
		if (this.m_PlayerToPush.Platform.Get() == this.m_Transform)
		{
			return;
		}
		float num = this.m_PhysicsCurrentMoveVelocity;
		if (num == 0f)
		{
			num = this.m_PhysicsCurrentRotationVelocity * 0.1f;
		}
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000AB4B4 File Offset: 0x000A96B4
	protected void TrySnapPlayerToTop()
	{
		if (this.m_PlayerToPush == null || this.m_PlayerToPush.Platform == null)
		{
			return;
		}
		if (this.m_PlayerToPush.Position.Get().y > this.m_Collider.bounds.max.y)
		{
			return;
		}
		if (this.m_PlayerToPush.Platform.Get() == this.m_Transform)
		{
			return;
		}
		if (this.RotationSpeed.x != 0f || this.RotationSpeed.z != 0f || this.m_CurrentTargetAngle.x != 0f || this.m_CurrentTargetAngle.z != 0f)
		{
			return;
		}
		if (this.m_Collider.bounds.max.x < this.m_PlayerCollider.bounds.max.x || this.m_Collider.bounds.max.z < this.m_PlayerCollider.bounds.max.z || this.m_Collider.bounds.min.x > this.m_PlayerCollider.bounds.min.x || this.m_Collider.bounds.min.z > this.m_PlayerCollider.bounds.min.z)
		{
			return;
		}
		Vector3 o = this.m_PlayerToPush.Position.Get();
		o.y = this.m_Collider.bounds.max.y - 0.1f;
		this.m_PlayerToPush.Position.Set(o);
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x000AB69B File Offset: 0x000A989B
	public void TryAutoStart()
	{
		if (!vp_Gameplay.isMaster)
		{
			return;
		}
		if (this.MoveAutoStartTarget == 0)
		{
			return;
		}
		if (this.m_PhysicsCurrentMoveVelocity > 0f || this.m_Moving)
		{
			return;
		}
		this.GoTo(this.MoveAutoStartTarget);
	}

	// Token: 0x04002AF7 RID: 10999
	protected Transform m_Transform;

	// Token: 0x04002AF8 RID: 11000
	public vp_MovingPlatform.PathMoveType PathType;

	// Token: 0x04002AF9 RID: 11001
	public GameObject PathWaypoints;

	// Token: 0x04002AFA RID: 11002
	public vp_MovingPlatform.Direction PathDirection;

	// Token: 0x04002AFB RID: 11003
	public int MoveAutoStartTarget = 1000;

	// Token: 0x04002AFC RID: 11004
	protected List<Transform> m_Waypoints = new List<Transform>();

	// Token: 0x04002AFD RID: 11005
	protected int m_NextWaypoint;

	// Token: 0x04002AFE RID: 11006
	protected Vector3 m_CurrentTargetPosition = Vector3.zero;

	// Token: 0x04002AFF RID: 11007
	protected Vector3 m_CurrentTargetAngle = Vector3.zero;

	// Token: 0x04002B00 RID: 11008
	protected int m_TargetedWayPoint;

	// Token: 0x04002B01 RID: 11009
	protected float m_TravelDistance;

	// Token: 0x04002B02 RID: 11010
	protected Vector3 m_OriginalAngle = Vector3.zero;

	// Token: 0x04002B03 RID: 11011
	protected int m_CurrentWaypoint;

	// Token: 0x04002B04 RID: 11012
	public float MoveSpeed = 0.1f;

	// Token: 0x04002B05 RID: 11013
	public float MoveReturnDelay;

	// Token: 0x04002B06 RID: 11014
	public float MoveCooldown;

	// Token: 0x04002B07 RID: 11015
	public vp_MovingPlatform.MovementInterpolationMode MoveInterpolationMode;

	// Token: 0x04002B08 RID: 11016
	protected bool m_Moving;

	// Token: 0x04002B09 RID: 11017
	protected float m_NextAllowedMoveTime;

	// Token: 0x04002B0A RID: 11018
	protected float m_MoveTime;

	// Token: 0x04002B0B RID: 11019
	protected vp_Timer.Handle m_ReturnDelayTimer = new vp_Timer.Handle();

	// Token: 0x04002B0C RID: 11020
	protected Vector3 m_PrevPos = Vector3.zero;

	// Token: 0x04002B0D RID: 11021
	protected AnimationCurve m_EaseInOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04002B0E RID: 11022
	protected AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002B0F RID: 11023
	public float RotationEaseAmount = 0.1f;

	// Token: 0x04002B10 RID: 11024
	public Vector3 RotationSpeed = Vector3.zero;

	// Token: 0x04002B11 RID: 11025
	public vp_MovingPlatform.RotateInterpolationMode RotationInterpolationMode;

	// Token: 0x04002B12 RID: 11026
	protected Vector3 m_PrevAngle = Vector3.zero;

	// Token: 0x04002B13 RID: 11027
	public AudioClip SoundStart;

	// Token: 0x04002B14 RID: 11028
	public AudioClip SoundStop;

	// Token: 0x04002B15 RID: 11029
	public AudioClip SoundMove;

	// Token: 0x04002B16 RID: 11030
	public AudioClip SoundWaypoint;

	// Token: 0x04002B17 RID: 11031
	protected AudioSource m_Audio;

	// Token: 0x04002B18 RID: 11032
	public bool PhysicsSnapPlayerToTopOnIntersect = true;

	// Token: 0x04002B19 RID: 11033
	public float m_PhysicsPushForce = 2f;

	// Token: 0x04002B1A RID: 11034
	protected Rigidbody m_RigidBody;

	// Token: 0x04002B1B RID: 11035
	protected Collider m_Collider;

	// Token: 0x04002B1C RID: 11036
	protected Collider m_PlayerCollider;

	// Token: 0x04002B1D RID: 11037
	protected vp_PlayerEventHandler m_PlayerToPush;

	// Token: 0x04002B1E RID: 11038
	protected float m_PhysicsCurrentMoveVelocity;

	// Token: 0x04002B1F RID: 11039
	protected float m_PhysicsCurrentRotationVelocity;

	// Token: 0x04002B20 RID: 11040
	protected static Dictionary<Collider, vp_PlayerEventHandler> m_KnownPlayers = new Dictionary<Collider, vp_PlayerEventHandler>();

	// Token: 0x02000856 RID: 2134
	protected class WaypointComparer : IComparer
	{
		// Token: 0x06002D25 RID: 11557 RVA: 0x000AB7C1 File Offset: 0x000A99C1
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((Transform)x).name, ((Transform)y).name);
		}
	}

	// Token: 0x02000857 RID: 2135
	public enum PathMoveType
	{
		// Token: 0x04002B22 RID: 11042
		PingPong,
		// Token: 0x04002B23 RID: 11043
		Loop,
		// Token: 0x04002B24 RID: 11044
		Target
	}

	// Token: 0x02000858 RID: 2136
	public enum Direction
	{
		// Token: 0x04002B26 RID: 11046
		Forward,
		// Token: 0x04002B27 RID: 11047
		Backwards,
		// Token: 0x04002B28 RID: 11048
		Direct
	}

	// Token: 0x02000859 RID: 2137
	public enum MovementInterpolationMode
	{
		// Token: 0x04002B2A RID: 11050
		EaseInOut,
		// Token: 0x04002B2B RID: 11051
		EaseIn,
		// Token: 0x04002B2C RID: 11052
		EaseOut,
		// Token: 0x04002B2D RID: 11053
		EaseOut2,
		// Token: 0x04002B2E RID: 11054
		Slerp,
		// Token: 0x04002B2F RID: 11055
		Lerp
	}

	// Token: 0x0200085A RID: 2138
	public enum RotateInterpolationMode
	{
		// Token: 0x04002B31 RID: 11057
		SyncToMovement,
		// Token: 0x04002B32 RID: 11058
		EaseOut,
		// Token: 0x04002B33 RID: 11059
		CustomEaseOut,
		// Token: 0x04002B34 RID: 11060
		CustomRotate
	}
}
