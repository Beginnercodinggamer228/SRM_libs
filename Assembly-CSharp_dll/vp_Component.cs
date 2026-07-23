using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x020007EE RID: 2030
public class vp_Component : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x06002A73 RID: 10867 RVA: 0x0009FCDD File Offset: 0x0009DEDD
	protected virtual StateManager GetStateManager()
	{
		return new EmptyStateManager<vp_Component>(this);
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06002A74 RID: 10868 RVA: 0x0009FCE5 File Offset: 0x0009DEE5
	public vp_EventHandler EventHandler
	{
		get
		{
			if (this.m_EventHandler == null)
			{
				this.m_EventHandler = (vp_EventHandler)this.Transform.root.GetComponentInChildren(typeof(vp_EventHandler));
			}
			return this.m_EventHandler;
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06002A75 RID: 10869 RVA: 0x0009FD20 File Offset: 0x0009DF20
	public Type Type
	{
		get
		{
			if (this.m_Type == null)
			{
				this.m_Type = base.GetType();
			}
			return this.m_Type;
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06002A76 RID: 10870 RVA: 0x0009FD42 File Offset: 0x0009DF42
	public StateManager StateManager
	{
		get
		{
			if (this.m_StateManager == null)
			{
				this.m_StateManager = this.GetStateManager();
			}
			return this.m_StateManager;
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06002A77 RID: 10871 RVA: 0x0009FD5E File Offset: 0x0009DF5E
	public vp_State DefaultState
	{
		get
		{
			return this.m_DefaultState;
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06002A78 RID: 10872 RVA: 0x0009FD66 File Offset: 0x0009DF66
	public float Delta
	{
		get
		{
			return Time.deltaTime * 60f;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06002A79 RID: 10873 RVA: 0x0009FD73 File Offset: 0x0009DF73
	public float SDelta
	{
		get
		{
			return Time.smoothDeltaTime * 60f;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06002A7A RID: 10874 RVA: 0x0009FD80 File Offset: 0x0009DF80
	public Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06002A7B RID: 10875 RVA: 0x0009FDA2 File Offset: 0x0009DFA2
	public Transform Parent
	{
		get
		{
			if (this.m_Parent == null)
			{
				this.m_Parent = base.transform.parent;
			}
			return this.m_Parent;
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06002A7C RID: 10876 RVA: 0x0009FDC9 File Offset: 0x0009DFC9
	public Transform Root
	{
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = base.transform.root;
			}
			return this.m_Root;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06002A7D RID: 10877 RVA: 0x0009FDF0 File Offset: 0x0009DFF0
	public AudioSource Audio
	{
		get
		{
			if (this.m_Audio == null)
			{
				this.m_Audio = base.GetComponent<AudioSource>();
			}
			return this.m_Audio;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06002A7E RID: 10878 RVA: 0x0009FE12 File Offset: 0x0009E012
	public Collider Collider
	{
		get
		{
			if (this.m_Collider == null)
			{
				this.m_Collider = base.GetComponent<Collider>();
			}
			return this.m_Collider;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06002A7F RID: 10879 RVA: 0x0009FE34 File Offset: 0x0009E034
	// (set) Token: 0x06002A80 RID: 10880 RVA: 0x0009FE58 File Offset: 0x0009E058
	public bool Rendering
	{
		get
		{
			return this.Renderers.Count > 0 && this.Renderers[0].enabled;
		}
		set
		{
			foreach (Renderer renderer in this.Renderers)
			{
				if (!(renderer == null))
				{
					renderer.enabled = value;
				}
			}
		}
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x0009FEB4 File Offset: 0x0009E0B4
	protected virtual void Awake()
	{
		this.CacheChildren();
		this.CacheSiblings();
		this.CacheFamily();
		this.CacheRenderers();
		this.CacheAudioSources();
		this.StateManager.SetState("Default", base.enabled);
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x0009FEEA File Offset: 0x0009E0EA
	protected virtual void Start()
	{
		this.ResetState();
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void Init()
	{
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x0009FEF2 File Offset: 0x0009E0F2
	protected virtual void OnEnable()
	{
		if (this.EventHandler != null)
		{
			this.Register(this.EventHandler);
		}
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x0009FF0E File Offset: 0x0009E10E
	protected virtual void OnDisable()
	{
		if (this.EventHandler != null)
		{
			this.Unregister(this.EventHandler);
		}
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x0009FF2A File Offset: 0x0009E12A
	protected virtual void Update()
	{
		if (!this.m_Initialized)
		{
			this.Init();
			this.m_Initialized = true;
		}
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void FixedUpdate()
	{
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void LateUpdate()
	{
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x0009FF44 File Offset: 0x0009E144
	public void SetState(string state, bool enabled = true, bool recursive = false, bool includeDisabled = false)
	{
		this.StateManager.SetState(state, enabled);
		if (recursive)
		{
			foreach (vp_Component vp_Component in this.Children)
			{
				if (includeDisabled || (vp_Utility.IsActive(vp_Component.gameObject) && vp_Component.enabled))
				{
					vp_Component.SetState(state, enabled, true, includeDisabled);
				}
			}
		}
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x0009FFC4 File Offset: 0x0009E1C4
	public void ActivateGameObject(bool setActive = true)
	{
		if (setActive)
		{
			this.Activate();
			foreach (vp_Component vp_Component in this.Siblings)
			{
				vp_Component.Activate();
			}
			this.VerifyRenderers();
			return;
		}
		this.DeactivateWhenSilent();
		foreach (vp_Component vp_Component2 in this.Siblings)
		{
			vp_Component2.DeactivateWhenSilent();
		}
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000A006C File Offset: 0x0009E26C
	public void ResetState()
	{
		this.StateManager.Reset();
		this.Refresh();
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000A007F File Offset: 0x0009E27F
	public bool StateEnabled(string stateName)
	{
		return this.StateManager.IsEnabled(stateName);
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x000A0090 File Offset: 0x0009E290
	public void RefreshDefaultState()
	{
		vp_State vp_State = null;
		if (this.States.Count == 0)
		{
			vp_State = new vp_State(this.Type.Name, "Default", null, null);
			this.States.Add(vp_State);
		}
		else
		{
			for (int i = this.States.Count - 1; i > -1; i--)
			{
				if (this.States[i].Name == "Default")
				{
					vp_State = this.States[i];
					this.States.Remove(vp_State);
					this.States.Add(vp_State);
				}
			}
			if (vp_State == null)
			{
				vp_State = new vp_State(this.Type.Name, "Default", null, null);
				this.States.Add(vp_State);
			}
		}
		if (vp_State.Preset == null || vp_State.Preset.ComponentType == null)
		{
			vp_State.Preset = new vp_ComponentPreset();
		}
		if (vp_State.TextAsset == null)
		{
			vp_State.Preset.InitFromComponent(this);
		}
		vp_State.Enabled = true;
		this.m_DefaultState = vp_State;
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x000A01A5 File Offset: 0x0009E3A5
	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		this.RefreshDefaultState();
		this.Refresh();
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000A01BB File Offset: 0x0009E3BB
	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromResources(this, path);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000A01D0 File Offset: 0x0009E3D0
	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000A01E8 File Offset: 0x0009E3E8
	public void CacheChildren()
	{
		this.Children.Clear();
		foreach (vp_Component vp_Component in base.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component.transform.parent == base.transform)
			{
				this.Children.Add(vp_Component);
			}
		}
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000A0240 File Offset: 0x0009E440
	public void CacheSiblings()
	{
		this.Siblings.Clear();
		foreach (vp_Component vp_Component in base.GetComponents<vp_Component>())
		{
			if (vp_Component != this)
			{
				this.Siblings.Add(vp_Component);
			}
		}
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000A0288 File Offset: 0x0009E488
	public void CacheFamily()
	{
		this.Family.Clear();
		foreach (vp_Component vp_Component in base.transform.root.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component != this)
			{
				this.Family.Add(vp_Component);
			}
		}
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x000A02DC File Offset: 0x0009E4DC
	public void CacheRenderers()
	{
		this.Renderers.Clear();
		foreach (Renderer item in base.GetComponentsInChildren<Renderer>(true))
		{
			this.Renderers.Add(item);
		}
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x000A031C File Offset: 0x0009E51C
	protected void VerifyRenderers()
	{
		if (this.Renderers.Count == 0)
		{
			return;
		}
		if (this.Renderers[0] == null || !vp_Utility.IsDescendant(this.Renderers[0].transform, this.Transform))
		{
			this.Renderers.Clear();
			this.CacheRenderers();
		}
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x000A037C File Offset: 0x0009E57C
	public void CacheAudioSources()
	{
		this.AudioSources.Clear();
		foreach (AudioSource item in base.GetComponentsInChildren<AudioSource>(true))
		{
			this.AudioSources.Add(item);
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000A03BA File Offset: 0x0009E5BA
	public virtual void Activate()
	{
		this.m_DeactivationTimer.Cancel();
		vp_Utility.Activate(base.gameObject, true);
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x000A03D3 File Offset: 0x0009E5D3
	public virtual void Deactivate()
	{
		vp_Utility.Activate(base.gameObject, false);
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x000A03E4 File Offset: 0x0009E5E4
	public void DeactivateWhenSilent()
	{
		if (this == null)
		{
			return;
		}
		if (vp_Utility.IsActive(base.gameObject))
		{
			foreach (AudioSource audioSource in this.AudioSources)
			{
				if (audioSource.isPlaying && !audioSource.loop)
				{
					this.Rendering = false;
					vp_Timer.In(0.1f, delegate()
					{
						this.DeactivateWhenSilent();
					}, this.m_DeactivationTimer);
					return;
				}
			}
		}
		this.Deactivate();
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Refresh()
	{
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Register(vp_EventHandler eventHandler)
	{
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Unregister(vp_EventHandler eventHandler)
	{
	}

	// Token: 0x0400299F RID: 10655
	public bool Persist;

	// Token: 0x040029A0 RID: 10656
	protected StateManager m_StateManager;

	// Token: 0x040029A1 RID: 10657
	protected vp_EventHandler m_EventHandler;

	// Token: 0x040029A2 RID: 10658
	[NonSerialized]
	protected vp_State m_DefaultState;

	// Token: 0x040029A3 RID: 10659
	protected bool m_Initialized;

	// Token: 0x040029A4 RID: 10660
	protected Transform m_Transform;

	// Token: 0x040029A5 RID: 10661
	protected Transform m_Parent;

	// Token: 0x040029A6 RID: 10662
	protected Transform m_Root;

	// Token: 0x040029A7 RID: 10663
	protected AudioSource m_Audio;

	// Token: 0x040029A8 RID: 10664
	protected Collider m_Collider;

	// Token: 0x040029A9 RID: 10665
	public List<vp_State> States = new List<vp_State>();

	// Token: 0x040029AA RID: 10666
	public List<vp_Component> Children = new List<vp_Component>();

	// Token: 0x040029AB RID: 10667
	public List<vp_Component> Siblings = new List<vp_Component>();

	// Token: 0x040029AC RID: 10668
	public List<vp_Component> Family = new List<vp_Component>();

	// Token: 0x040029AD RID: 10669
	public List<Renderer> Renderers = new List<Renderer>();

	// Token: 0x040029AE RID: 10670
	public List<AudioSource> AudioSources = new List<AudioSource>();

	// Token: 0x040029AF RID: 10671
	protected Type m_Type;

	// Token: 0x040029B0 RID: 10672
	protected FieldInfo[] m_Fields;

	// Token: 0x040029B1 RID: 10673
	protected vp_Timer.Handle m_DeactivationTimer = new vp_Timer.Handle();
}
