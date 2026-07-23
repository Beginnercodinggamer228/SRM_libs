using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x020007F5 RID: 2037
public class vp_Activity : vp_Event
{
	// Token: 0x06002AD9 RID: 10969 RVA: 0x00003296 File Offset: 0x00001496
	protected static void Empty()
	{
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected static bool AlwaysOK()
	{
		return true;
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x000A1BB0 File Offset: 0x0009FDB0
	public vp_Activity(string name) : base(name, null, null)
	{
		this.EventType = vp_EventType.Activity;
		this.InitFields();
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06002ADC RID: 10972 RVA: 0x000A1BDE File Offset: 0x0009FDDE
	// (set) Token: 0x06002ADD RID: 10973 RVA: 0x000A1BE6 File Offset: 0x0009FDE6
	public float MinPause
	{
		get
		{
			return this.m_MinPause;
		}
		set
		{
			this.m_MinPause = Mathf.Max(0f, value);
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06002ADE RID: 10974 RVA: 0x000A1BF9 File Offset: 0x0009FDF9
	// (set) Token: 0x06002ADF RID: 10975 RVA: 0x000A1C04 File Offset: 0x0009FE04
	public float MinDuration
	{
		get
		{
			return this.m_MinDuration;
		}
		set
		{
			this.m_MinDuration = Mathf.Max(0.001f, value);
			if (this.m_MaxDuration == -1f)
			{
				return;
			}
			if (this.m_MinDuration > this.m_MaxDuration)
			{
				this.m_MinDuration = this.m_MaxDuration;
				Debug.LogWarning("Warning: (vp_Activity) Tried to set MinDuration longer than MaxDuration for '" + base.EventName + "'. Capping at MaxDuration.");
			}
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06002AE0 RID: 10976 RVA: 0x000A1C64 File Offset: 0x0009FE64
	// (set) Token: 0x06002AE1 RID: 10977 RVA: 0x000A1C6C File Offset: 0x0009FE6C
	public float AutoDuration
	{
		get
		{
			return this.m_MaxDuration;
		}
		set
		{
			if (value == -1f)
			{
				this.m_MaxDuration = value;
				return;
			}
			this.m_MaxDuration = Mathf.Max(0.001f, value);
			if (this.m_MaxDuration < this.m_MinDuration)
			{
				this.m_MaxDuration = this.m_MinDuration;
				Debug.LogWarning("Warning: (vp_Activity) Tried to set MaxDuration shorter than MinDuration for '" + base.EventName + "'. Capping at MinDuration.");
			}
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06002AE2 RID: 10978 RVA: 0x000A1CD0 File Offset: 0x0009FED0
	// (set) Token: 0x06002AE3 RID: 10979 RVA: 0x000A1D28 File Offset: 0x0009FF28
	public object Argument
	{
		get
		{
			if (this.m_ArgumentType == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Error: (",
					this,
					") Tried to fetch argument from '",
					base.EventName,
					"' but this activity takes no parameters."
				}));
				return null;
			}
			return this.m_Argument;
		}
		set
		{
			if (this.m_ArgumentType == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Error: (",
					this,
					") Tried to set argument for '",
					base.EventName,
					"' but this activity takes no parameters."
				}));
				return;
			}
			this.m_Argument = value;
		}
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x000A1D80 File Offset: 0x0009FF80
	protected override void InitFields()
	{
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Condition),
			typeof(vp_Activity.Condition),
			typeof(vp_Activity.Callback),
			typeof(vp_Activity.Callback)
		};
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("StartCallbacks"),
			base.GetType().GetField("StopCallbacks"),
			base.GetType().GetField("StartConditions"),
			base.GetType().GetField("StopConditions"),
			base.GetType().GetField("FailStartCallbacks"),
			base.GetType().GetField("FailStopCallbacks")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("AlwaysOK"),
			base.GetType().GetMethod("AlwaysOK"),
			base.GetType().GetMethod("Empty"),
			base.GetType().GetMethod("Empty")
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnStart_",
				0
			},
			{
				"OnStop_",
				1
			},
			{
				"CanStart_",
				2
			},
			{
				"CanStop_",
				3
			},
			{
				"OnFailStart_",
				4
			},
			{
				"OnFailStop_",
				5
			}
		};
		this.StartCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.StopCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.StartConditions = new vp_Activity.Condition(vp_Activity.AlwaysOK);
		this.StopConditions = new vp_Activity.Condition(vp_Activity.AlwaysOK);
		this.FailStartCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
		this.FailStopCallbacks = new vp_Activity.Callback(vp_Activity.Empty);
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000A1FA8 File Offset: 0x000A01A8
	public bool TryStart(bool startIfAllowed = true)
	{
		if (this.m_Active)
		{
			return false;
		}
		if (Time.time < this.NextAllowedStartTime)
		{
			this.m_Argument = null;
			return false;
		}
		Delegate[] invocationList = this.StartConditions.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (!((vp_Activity.Condition)invocationList[i])())
			{
				this.m_Argument = null;
				if (startIfAllowed)
				{
					this.FailStartCallbacks();
				}
				return false;
			}
		}
		if (startIfAllowed)
		{
			this.Active = true;
		}
		return true;
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000A2020 File Offset: 0x000A0220
	public bool TryStop(bool stopIfAllowed = true)
	{
		if (!this.m_Active)
		{
			return false;
		}
		if (Time.time < this.NextAllowedStopTime)
		{
			return false;
		}
		Delegate[] invocationList = this.StopConditions.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (!((vp_Activity.Condition)invocationList[i])())
			{
				if (stopIfAllowed)
				{
					this.FailStopCallbacks();
				}
				return false;
			}
		}
		if (stopIfAllowed)
		{
			this.Active = false;
		}
		return true;
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x06002AE8 RID: 10984 RVA: 0x000A2129 File Offset: 0x000A0329
	// (set) Token: 0x06002AE7 RID: 10983 RVA: 0x000A208C File Offset: 0x000A028C
	public bool Active
	{
		get
		{
			return this.m_Active;
		}
		set
		{
			if (value && !this.m_Active)
			{
				this.m_Active = true;
				this.StartCallbacks();
				this.NextAllowedStopTime = Time.time + this.m_MinDuration;
				if (this.m_MaxDuration > 0f)
				{
					vp_Timer.In(this.m_MaxDuration, delegate()
					{
						this.Stop(0f);
					}, this.m_ForceStopTimer);
					return;
				}
			}
			else if (!value && this.m_Active)
			{
				this.m_Active = false;
				this.StopCallbacks();
				this.NextAllowedStartTime = Time.time + this.m_MinPause;
				this.m_Argument = null;
			}
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000A2131 File Offset: 0x000A0331
	public void Start(float forcedActiveDuration = 0f)
	{
		this.Active = true;
		if (forcedActiveDuration > 0f)
		{
			this.NextAllowedStopTime = Time.time + forcedActiveDuration;
		}
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x000A214F File Offset: 0x000A034F
	public void Stop(float forcedPauseDuration = 0f)
	{
		this.Active = false;
		if (forcedPauseDuration > 0f)
		{
			this.NextAllowedStartTime = Time.time + forcedPauseDuration;
		}
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000A216D File Offset: 0x000A036D
	public void Disallow(float duration)
	{
		this.NextAllowedStartTime = Time.time + duration;
	}

	// Token: 0x040029CC RID: 10700
	public vp_Activity.Callback StartCallbacks;

	// Token: 0x040029CD RID: 10701
	public vp_Activity.Callback StopCallbacks;

	// Token: 0x040029CE RID: 10702
	public vp_Activity.Condition StartConditions;

	// Token: 0x040029CF RID: 10703
	public vp_Activity.Condition StopConditions;

	// Token: 0x040029D0 RID: 10704
	public vp_Activity.Callback FailStartCallbacks;

	// Token: 0x040029D1 RID: 10705
	public vp_Activity.Callback FailStopCallbacks;

	// Token: 0x040029D2 RID: 10706
	protected vp_Timer.Handle m_ForceStopTimer = new vp_Timer.Handle();

	// Token: 0x040029D3 RID: 10707
	protected object m_Argument;

	// Token: 0x040029D4 RID: 10708
	protected bool m_Active;

	// Token: 0x040029D5 RID: 10709
	public float NextAllowedStartTime;

	// Token: 0x040029D6 RID: 10710
	public float NextAllowedStopTime;

	// Token: 0x040029D7 RID: 10711
	private float m_MinPause;

	// Token: 0x040029D8 RID: 10712
	private float m_MinDuration;

	// Token: 0x040029D9 RID: 10713
	private float m_MaxDuration = -1f;

	// Token: 0x020007F6 RID: 2038
	// (Invoke) Token: 0x06002AEE RID: 10990
	public delegate void Callback();

	// Token: 0x020007F7 RID: 2039
	// (Invoke) Token: 0x06002AF2 RID: 10994
	public delegate bool Condition();
}
