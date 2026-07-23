using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000843 RID: 2115
public class vp_Timer : MonoBehaviour
{
	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06002C5A RID: 11354 RVA: 0x000A77A9 File Offset: 0x000A59A9
	public bool WasAddedCorrectly
	{
		get
		{
			return Application.isPlaying && !(base.gameObject != vp_Timer.m_MainObject);
		}
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000A77C9 File Offset: 0x000A59C9
	private void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		if (!this.WasAddedCorrectly)
		{
			Destroyer.Destroy(this, "vp_Timer.Awake");
			return;
		}
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x000A77F0 File Offset: 0x000A59F0
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			if (vp_Timer.m_Active[i].CancelOnLoad)
			{
				vp_Timer.m_Active[i].Id = 0;
			}
		}
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x000A7838 File Offset: 0x000A5A38
	private void Update()
	{
		vp_Timer.m_EventBatch = 0;
		while (vp_Timer.m_Active.Count > 0 && vp_Timer.m_EventBatch < vp_Timer.MaxEventsPerFrame)
		{
			if (vp_Timer.m_EventIterator < 0)
			{
				vp_Timer.m_EventIterator = vp_Timer.m_Active.Count - 1;
				return;
			}
			if (vp_Timer.m_EventIterator > vp_Timer.m_Active.Count - 1)
			{
				vp_Timer.m_EventIterator = vp_Timer.m_Active.Count - 1;
			}
			if (Time.time >= vp_Timer.m_Active[vp_Timer.m_EventIterator].DueTime || vp_Timer.m_Active[vp_Timer.m_EventIterator].Id == 0)
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].Execute();
			}
			else if (vp_Timer.m_Active[vp_Timer.m_EventIterator].Paused)
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].DueTime += Time.deltaTime;
			}
			else
			{
				vp_Timer.m_Active[vp_Timer.m_EventIterator].LifeTime += Time.deltaTime;
			}
			vp_Timer.m_EventIterator--;
			vp_Timer.m_EventBatch++;
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000A7961 File Offset: 0x000A5B61
	public static void In(float delay, vp_Timer.Callback callback, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, 1, -1f);
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000A7973 File Offset: 0x000A5B73
	public static void In(float delay, vp_Timer.Callback callback, int iterations, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, iterations, -1f);
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000A7985 File Offset: 0x000A5B85
	public static void In(float delay, vp_Timer.Callback callback, int iterations, float interval, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, callback, null, null, timerHandle, iterations, interval);
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000A7994 File Offset: 0x000A5B94
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, 1, -1f);
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x000A79A6 File Offset: 0x000A5BA6
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, int iterations, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, iterations, -1f);
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x000A79B9 File Offset: 0x000A5BB9
	public static void In(float delay, vp_Timer.ArgCallback callback, object arguments, int iterations, float interval, vp_Timer.Handle timerHandle = null)
	{
		vp_Timer.Schedule(delay, null, callback, arguments, timerHandle, iterations, interval);
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x000A79C9 File Offset: 0x000A5BC9
	public static void Start(vp_Timer.Handle timerHandle)
	{
		vp_Timer.Schedule(315360000f, delegate
		{
		}, null, null, timerHandle, 1, -1f);
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x000A7A00 File Offset: 0x000A5C00
	private static void Schedule(float time, vp_Timer.Callback func, vp_Timer.ArgCallback argFunc, object args, vp_Timer.Handle timerHandle, int iterations, float interval)
	{
		if (func == null && argFunc == null)
		{
			Debug.LogError("Error: (vp_Timer) Aborted event because function is null.");
			return;
		}
		if (vp_Timer.m_MainObject == null)
		{
			vp_Timer.m_MainObject = new GameObject("Timers");
			vp_Timer.m_MainObject.AddComponent<vp_Timer>();
			UnityEngine.Object.DontDestroyOnLoad(vp_Timer.m_MainObject);
		}
		time = Mathf.Max(0f, time);
		iterations = Mathf.Max(0, iterations);
		interval = ((interval == -1f) ? time : Mathf.Max(0f, interval));
		vp_Timer.m_NewEvent = null;
		if (vp_Timer.m_Pool.Count > 0)
		{
			vp_Timer.m_NewEvent = vp_Timer.m_Pool[0];
			vp_Timer.m_Pool.Remove(vp_Timer.m_NewEvent);
		}
		else
		{
			vp_Timer.m_NewEvent = new vp_Timer.Event();
		}
		vp_Timer.m_EventCount++;
		vp_Timer.m_NewEvent.Id = vp_Timer.m_EventCount;
		if (func != null)
		{
			vp_Timer.m_NewEvent.Function = func;
		}
		else if (argFunc != null)
		{
			vp_Timer.m_NewEvent.ArgFunction = argFunc;
			vp_Timer.m_NewEvent.Arguments = args;
		}
		vp_Timer.m_NewEvent.StartTime = Time.time;
		vp_Timer.m_NewEvent.DueTime = Time.time + time;
		vp_Timer.m_NewEvent.Iterations = iterations;
		vp_Timer.m_NewEvent.Interval = interval;
		vp_Timer.m_NewEvent.LifeTime = 0f;
		vp_Timer.m_NewEvent.Paused = false;
		vp_Timer.m_Active.Add(vp_Timer.m_NewEvent);
		if (timerHandle != null)
		{
			if (timerHandle.Active)
			{
				timerHandle.Cancel();
			}
			timerHandle.Id = vp_Timer.m_NewEvent.Id;
		}
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x000A7B89 File Offset: 0x000A5D89
	private static void Cancel(vp_Timer.Handle handle)
	{
		if (handle == null)
		{
			return;
		}
		if (handle.Active)
		{
			handle.Id = 0;
			return;
		}
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x000A7BA0 File Offset: 0x000A5DA0
	public static void CancelAll()
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			vp_Timer.m_Active[i].Id = 0;
		}
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x000A7BD8 File Offset: 0x000A5DD8
	public static void CancelAll(string methodName)
	{
		for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
		{
			if (vp_Timer.m_Active[i].MethodName == methodName)
			{
				vp_Timer.m_Active[i].Id = 0;
			}
		}
	}

	// Token: 0x06002C69 RID: 11369 RVA: 0x000A7C25 File Offset: 0x000A5E25
	public static void DestroyAll()
	{
		vp_Timer.m_Active.Clear();
		vp_Timer.m_Pool.Clear();
	}

	// Token: 0x06002C6A RID: 11370 RVA: 0x000A7C3C File Offset: 0x000A5E3C
	public static vp_Timer.Stats EditorGetStats()
	{
		vp_Timer.Stats result;
		result.Created = vp_Timer.m_Active.Count + vp_Timer.m_Pool.Count;
		result.Inactive = vp_Timer.m_Pool.Count;
		result.Active = vp_Timer.m_Active.Count;
		return result;
	}

	// Token: 0x06002C6B RID: 11371 RVA: 0x000A7C88 File Offset: 0x000A5E88
	public static string EditorGetMethodInfo(int eventIndex)
	{
		if (eventIndex < 0 || eventIndex > vp_Timer.m_Active.Count - 1)
		{
			return "Argument out of range.";
		}
		return vp_Timer.m_Active[eventIndex].MethodInfo;
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x000A7CB3 File Offset: 0x000A5EB3
	public static int EditorGetMethodId(int eventIndex)
	{
		if (eventIndex < 0 || eventIndex > vp_Timer.m_Active.Count - 1)
		{
			return 0;
		}
		return vp_Timer.m_Active[eventIndex].Id;
	}

	// Token: 0x04002A86 RID: 10886
	private static GameObject m_MainObject = null;

	// Token: 0x04002A87 RID: 10887
	private static List<vp_Timer.Event> m_Active = new List<vp_Timer.Event>();

	// Token: 0x04002A88 RID: 10888
	private static List<vp_Timer.Event> m_Pool = new List<vp_Timer.Event>();

	// Token: 0x04002A89 RID: 10889
	private static vp_Timer.Event m_NewEvent = null;

	// Token: 0x04002A8A RID: 10890
	private static int m_EventCount = 0;

	// Token: 0x04002A8B RID: 10891
	private static int m_EventBatch = 0;

	// Token: 0x04002A8C RID: 10892
	private static int m_EventIterator = 0;

	// Token: 0x04002A8D RID: 10893
	public static int MaxEventsPerFrame = 500;

	// Token: 0x02000844 RID: 2116
	// (Invoke) Token: 0x06002C70 RID: 11376
	public delegate void Callback();

	// Token: 0x02000845 RID: 2117
	// (Invoke) Token: 0x06002C74 RID: 11380
	public delegate void ArgCallback(object args);

	// Token: 0x02000846 RID: 2118
	public struct Stats
	{
		// Token: 0x04002A8E RID: 10894
		public int Created;

		// Token: 0x04002A8F RID: 10895
		public int Inactive;

		// Token: 0x04002A90 RID: 10896
		public int Active;
	}

	// Token: 0x02000847 RID: 2119
	private class Event
	{
		// Token: 0x06002C77 RID: 11383 RVA: 0x000A7D18 File Offset: 0x000A5F18
		public void Execute()
		{
			if (this.Id == 0 || this.DueTime == 0f)
			{
				this.Recycle();
				return;
			}
			if (this.Function != null)
			{
				this.Function();
			}
			else
			{
				if (this.ArgFunction == null)
				{
					this.Error("Aborted event because function is null.");
					this.Recycle();
					return;
				}
				this.ArgFunction(this.Arguments);
			}
			if (this.Iterations > 0)
			{
				this.Iterations--;
				if (this.Iterations < 1)
				{
					this.Recycle();
					return;
				}
			}
			this.DueTime = Time.time + this.Interval;
		}

		// Token: 0x06002C78 RID: 11384 RVA: 0x000A7DBC File Offset: 0x000A5FBC
		private void Recycle()
		{
			this.Id = 0;
			this.DueTime = 0f;
			this.StartTime = 0f;
			this.CancelOnLoad = true;
			this.Function = null;
			this.ArgFunction = null;
			this.Arguments = null;
			if (vp_Timer.m_Active.Remove(this))
			{
				vp_Timer.m_Pool.Add(this);
			}
		}

		// Token: 0x06002C79 RID: 11385 RVA: 0x000A7E1A File Offset: 0x000A601A
		private void Destroy()
		{
			vp_Timer.m_Active.Remove(this);
			vp_Timer.m_Pool.Remove(this);
		}

		// Token: 0x06002C7A RID: 11386 RVA: 0x000A7E34 File Offset: 0x000A6034
		private void Error(string message)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") ",
				message
			}));
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06002C7B RID: 11387 RVA: 0x000A7E60 File Offset: 0x000A6060
		public string MethodName
		{
			get
			{
				if (this.Function != null)
				{
					if (this.Function.Method != null)
					{
						if (this.Function.Method.Name[0] == '<')
						{
							return "delegate";
						}
						return this.Function.Method.Name;
					}
				}
				else if (this.ArgFunction != null && this.ArgFunction.Method != null)
				{
					if (this.ArgFunction.Method.Name[0] == '<')
					{
						return "delegate";
					}
					return this.ArgFunction.Method.Name;
				}
				return null;
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000A7F08 File Offset: 0x000A6108
		public string MethodInfo
		{
			get
			{
				string text = this.MethodName;
				if (!string.IsNullOrEmpty(text))
				{
					text += "(";
					if (this.Arguments != null)
					{
						if (this.Arguments.GetType().IsArray)
						{
							object[] array = (object[])this.Arguments;
							foreach (object obj in array)
							{
								text += obj.ToString();
								if (Array.IndexOf<object>(array, obj) < array.Length - 1)
								{
									text += ", ";
								}
							}
						}
						else
						{
							text += this.Arguments;
						}
					}
					text += ")";
				}
				else
				{
					text = "(function = null)";
				}
				return text;
			}
		}

		// Token: 0x04002A91 RID: 10897
		public int Id;

		// Token: 0x04002A92 RID: 10898
		public vp_Timer.Callback Function;

		// Token: 0x04002A93 RID: 10899
		public vp_Timer.ArgCallback ArgFunction;

		// Token: 0x04002A94 RID: 10900
		public object Arguments;

		// Token: 0x04002A95 RID: 10901
		public int Iterations = 1;

		// Token: 0x04002A96 RID: 10902
		public float Interval = -1f;

		// Token: 0x04002A97 RID: 10903
		public float DueTime;

		// Token: 0x04002A98 RID: 10904
		public float StartTime;

		// Token: 0x04002A99 RID: 10905
		public float LifeTime;

		// Token: 0x04002A9A RID: 10906
		public bool Paused;

		// Token: 0x04002A9B RID: 10907
		public bool CancelOnLoad = true;
	}

	// Token: 0x02000848 RID: 2120
	public class Handle
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06002C7E RID: 11390 RVA: 0x000A7FDC File Offset: 0x000A61DC
		// (set) Token: 0x06002C7F RID: 11391 RVA: 0x000A7FF3 File Offset: 0x000A61F3
		public bool Paused
		{
			get
			{
				return this.Active && this.m_Event.Paused;
			}
			set
			{
				if (this.Active)
				{
					this.m_Event.Paused = value;
				}
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06002C80 RID: 11392 RVA: 0x000A8009 File Offset: 0x000A6209
		public float TimeOfInitiation
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.StartTime;
				}
				return 0f;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06002C81 RID: 11393 RVA: 0x000A8024 File Offset: 0x000A6224
		public float TimeOfFirstIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_FirstDueTime;
				}
				return 0f;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06002C82 RID: 11394 RVA: 0x000A803A File Offset: 0x000A623A
		public float TimeOfNextIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.DueTime;
				}
				return 0f;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06002C83 RID: 11395 RVA: 0x000A8055 File Offset: 0x000A6255
		public float TimeOfLastIteration
		{
			get
			{
				if (this.Active)
				{
					return Time.time + this.DurationLeft;
				}
				return 0f;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06002C84 RID: 11396 RVA: 0x000A8071 File Offset: 0x000A6271
		public float Delay
		{
			get
			{
				return Mathf.Round((this.m_FirstDueTime - this.TimeOfInitiation) * 1000f) / 1000f;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06002C85 RID: 11397 RVA: 0x000A8091 File Offset: 0x000A6291
		public float Interval
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.Interval;
				}
				return 0f;
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06002C86 RID: 11398 RVA: 0x000A80AC File Offset: 0x000A62AC
		public float TimeUntilNextIteration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.DueTime - Time.time;
				}
				return 0f;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06002C87 RID: 11399 RVA: 0x000A80CD File Offset: 0x000A62CD
		public float DurationLeft
		{
			get
			{
				if (this.Active)
				{
					return this.TimeUntilNextIteration + (float)(this.m_Event.Iterations - 1) * this.m_Event.Interval;
				}
				return 0f;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06002C88 RID: 11400 RVA: 0x000A80FE File Offset: 0x000A62FE
		public float DurationTotal
		{
			get
			{
				if (this.Active)
				{
					return this.Delay + (float)this.m_StartIterations * ((this.m_StartIterations > 1) ? this.Interval : 0f);
				}
				return 0f;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06002C89 RID: 11401 RVA: 0x000A8133 File Offset: 0x000A6333
		public float Duration
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.LifeTime;
				}
				return 0f;
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06002C8A RID: 11402 RVA: 0x000A814E File Offset: 0x000A634E
		public int IterationsTotal
		{
			get
			{
				return this.m_StartIterations;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06002C8B RID: 11403 RVA: 0x000A8156 File Offset: 0x000A6356
		public int IterationsLeft
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.Iterations;
				}
				return 0;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06002C8C RID: 11404 RVA: 0x000A816D File Offset: 0x000A636D
		// (set) Token: 0x06002C8D RID: 11405 RVA: 0x000A8178 File Offset: 0x000A6378
		public int Id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
				if (this.m_Id == 0)
				{
					this.m_Event.DueTime = 0f;
					return;
				}
				this.m_Event = null;
				for (int i = vp_Timer.m_Active.Count - 1; i > -1; i--)
				{
					if (vp_Timer.m_Active[i].Id == this.m_Id)
					{
						this.m_Event = vp_Timer.m_Active[i];
						break;
					}
				}
				if (this.m_Event == null)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Error: (",
						this,
						") Failed to assign event with Id '",
						this.m_Id,
						"'."
					}));
				}
				this.m_StartIterations = this.m_Event.Iterations;
				this.m_FirstDueTime = this.m_Event.DueTime;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06002C8E RID: 11406 RVA: 0x000A8252 File Offset: 0x000A6452
		public bool Active
		{
			get
			{
				return this.m_Event != null && this.Id != 0 && this.m_Event.Id != 0 && this.m_Event.Id == this.Id;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06002C8F RID: 11407 RVA: 0x000A8286 File Offset: 0x000A6486
		public string MethodName
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.MethodName;
				}
				return "";
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06002C90 RID: 11408 RVA: 0x000A82A1 File Offset: 0x000A64A1
		public string MethodInfo
		{
			get
			{
				if (this.Active)
				{
					return this.m_Event.MethodInfo;
				}
				return "";
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06002C91 RID: 11409 RVA: 0x000A82BC File Offset: 0x000A64BC
		// (set) Token: 0x06002C92 RID: 11410 RVA: 0x000A82D3 File Offset: 0x000A64D3
		public bool CancelOnLoad
		{
			get
			{
				return !this.Active || this.m_Event.CancelOnLoad;
			}
			set
			{
				if (this.Active)
				{
					this.m_Event.CancelOnLoad = value;
					return;
				}
				Debug.LogWarning("Warning: (" + this + ") Tried to set CancelOnLoad on inactive timer handle.");
			}
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x000A82FF File Offset: 0x000A64FF
		public void Cancel()
		{
			vp_Timer.Cancel(this);
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000A8307 File Offset: 0x000A6507
		public void Execute()
		{
			this.m_Event.DueTime = Time.time;
		}

		// Token: 0x04002A9C RID: 10908
		private vp_Timer.Event m_Event;

		// Token: 0x04002A9D RID: 10909
		private int m_Id;

		// Token: 0x04002A9E RID: 10910
		private int m_StartIterations = 1;

		// Token: 0x04002A9F RID: 10911
		private float m_FirstDueTime;
	}
}
