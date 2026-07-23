using System;

// Token: 0x0200037B RID: 891
public class SRSingleton<T> : SRBehaviour where T : SRSingleton<T>
{
	// Token: 0x06001267 RID: 4711 RVA: 0x000492D8 File Offset: 0x000474D8
	public virtual void Awake()
	{
		if (SRSingleton<T>.Nested.instance == this)
		{
			return;
		}
		if (SRSingleton<T>.Nested.instance != null)
		{
			Log.Error("An instance of the singleton " + typeof(T) + " already exists, attempting to create additional.", Array.Empty<object>());
		}
		SRSingleton<T>.Nested.instance = (T)((object)this);
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x00049338 File Offset: 0x00047538
	public virtual void OnDestroy()
	{
		if (SRSingleton<T>.Nested.instance == this)
		{
			SRSingleton<T>.Nested.instance = default(T);
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001269 RID: 4713 RVA: 0x00049357 File Offset: 0x00047557
	public static T Instance
	{
		get
		{
			return SRSingleton<T>.Nested.instance;
		}
	}

	// Token: 0x0200037C RID: 892
	private class Nested
	{
		// Token: 0x040011A1 RID: 4513
		internal static T instance;
	}
}
