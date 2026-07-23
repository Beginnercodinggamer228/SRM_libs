using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000568 RID: 1384
public class DecorizerUI : BaseUI
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06001CD3 RID: 7379 RVA: 0x0006DC72 File Offset: 0x0006BE72
	// (set) Token: 0x06001CD4 RID: 7380 RVA: 0x0006DC7A File Offset: 0x0006BE7A
	public DecorizerStorage storage { get; set; }

	// Token: 0x06001CD5 RID: 7381 RVA: 0x0006DC83 File Offset: 0x0006BE83
	public void OnEnable()
	{
		SECTR_AudioSystem.Play(this.onEnableCue, Vector3.zero, false);
		this.states = new Stack<DecorizerUI.State>();
		this.states.Push(DecorizerUI.State.DEFAULT);
		this.Refresh();
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x0006DCB4 File Offset: 0x0006BEB4
	public void OnDisable()
	{
		SECTR_AudioSystem.Play(this.onDisableCue, Vector3.zero, false);
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x0006DCC8 File Offset: 0x0006BEC8
	private void Refresh()
	{
		DecorizerUI.State state = this.states.Peek();
		this.defaultParent.SetActive(state == DecorizerUI.State.DEFAULT);
		this.cleanupParent.SetActive(state == DecorizerUI.State.CLEANUP);
		this.retrieveCategoriesParent.SetActive(state == DecorizerUI.State.RETRIEVE_CATEGORIES);
		this.retrieveParent.SetActive(false);
		if (state - DecorizerUI.State.RETRIEVE_ECHOES <= 2)
		{
			this.retrieveParent.SetActive(true);
			List<Identifiable.Id> list;
			if (state == DecorizerUI.State.RETRIEVE_ECHOES)
			{
				list = Identifiable.ECHO_CLASS.ToList<Identifiable.Id>();
			}
			else if (state == DecorizerUI.State.RETRIEVE_ECHO_NOTES)
			{
				list = Identifiable.ECHO_NOTE_CLASS.ToList<Identifiable.Id>();
			}
			else
			{
				if (state != DecorizerUI.State.RETRIEVE_ORNAMENTS)
				{
					throw new InvalidOperationException(string.Format("Failed to get decorizer retrieve entries. [state={0}]", state));
				}
				list = Identifiable.ORNAMENT_CLASS.ToList<Identifiable.Id>();
			}
			list.Sort((Identifiable.Id a, Identifiable.Id b) => Identifiable.GetName(a, true).CompareTo(Identifiable.GetName(b, true)));
			for (int i = 0; i < this.retrieveContentsGrid.transform.childCount; i++)
			{
				Destroyer.Destroy(this.retrieveContentsGrid.transform.GetChild(i).gameObject, "DecorizerUI.Refresh");
			}
			bool flag = false;
			using (List<Identifiable.Id>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Identifiable.Id id = enumerator.Current;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.retrieveContentsEntry, this.retrieveContentsGrid.transform);
					DecorizerUIEntry component = gameObject.GetComponent<DecorizerUIEntry>();
					component.name.text = Identifiable.GetName(id, false);
					component.image.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(id);
					component.button.onClick.AddListener(delegate()
					{
						this.OnButtonPressed_Retrieve_Entry(id);
					});
					int count = SRSingleton<SceneContext>.Instance.GameModel.GetDecorizerModel().GetCount(id);
					component.count.text = ((count > 999) ? string.Format("{0}+", 999) : count.ToString());
					if (!flag)
					{
						gameObject.GetRequiredComponentInChildren(false).gameObject.AddComponent<InitSelected>();
						flag = true;
					}
				}
			}
		}
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0006DF18 File Offset: 0x0006C118
	protected override void OnCancelPressed()
	{
		this.states.Pop();
		if (this.states.Count == 0)
		{
			this.Close();
			return;
		}
		this.Refresh();
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0006DF40 File Offset: 0x0006C140
	public void OnButtonPressed_Retrieve()
	{
		SECTR_AudioSystem.Play(this.onButtonRetrieveCue, Vector3.zero, false);
		this.states.Push(DecorizerUI.State.RETRIEVE_CATEGORIES);
		this.Refresh();
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x0006DF66 File Offset: 0x0006C166
	public void OnButtonPressed_Retrieve_Category_Echoes()
	{
		this.OnButtonPressed_Retrieve_Category(DecorizerUI.State.RETRIEVE_ECHOES);
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x0006DF6F File Offset: 0x0006C16F
	public void OnButtonPressed_Retrieve_Category_EchoNotes()
	{
		this.OnButtonPressed_Retrieve_Category(DecorizerUI.State.RETRIEVE_ECHO_NOTES);
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x0006DF78 File Offset: 0x0006C178
	public void OnButtonPressed_Retrieve_Category_Ornaments()
	{
		this.OnButtonPressed_Retrieve_Category(DecorizerUI.State.RETRIEVE_ORNAMENTS);
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0006DF81 File Offset: 0x0006C181
	public void OnButtonPressed_Cleanup()
	{
		SECTR_AudioSystem.Play(this.onButtonCleanupCue, Vector3.zero, false);
		this.states.Push(DecorizerUI.State.CLEANUP);
		this.Refresh();
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0006DFA7 File Offset: 0x0006C1A7
	public void OnButtonPressed_Cleanup_All()
	{
		this.OnButtonPressed_Cleanup(DecorizerModel.ITEM_CLASSES.SelectMany((HashSet<Identifiable.Id> c) => c));
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x0006DFD8 File Offset: 0x0006C1D8
	public void OnButtonPressed_Cleanup_Echoes()
	{
		this.OnButtonPressed_Cleanup(Identifiable.ECHO_CLASS);
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x0006DFE5 File Offset: 0x0006C1E5
	public void OnButtonPressed_Cleanup_EchoNotes()
	{
		this.OnButtonPressed_Cleanup(Identifiable.ECHO_NOTE_CLASS);
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x0006DFF2 File Offset: 0x0006C1F2
	public void OnButtonPressed_Cleanup_Ornaments()
	{
		this.OnButtonPressed_Cleanup(Identifiable.ORNAMENT_CLASS);
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x0006DFFF File Offset: 0x0006C1FF
	private void OnButtonPressed_Retrieve_Category(DecorizerUI.State state)
	{
		SECTR_AudioSystem.Play(this.onButtonRetrieveCategoryCue, Vector3.zero, false);
		this.states.Push(state);
		this.Refresh();
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x0006E025 File Offset: 0x0006C225
	private void OnButtonPressed_Cleanup(IEnumerable<Identifiable.Id> ids)
	{
		SECTR_AudioSystem.Play(this.onButtonCleanupEntryCue, Vector3.zero, false);
		this.storage.Cleanup(ids);
		this.Close();
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x0006E04B File Offset: 0x0006C24B
	private void OnButtonPressed_Retrieve_Entry(Identifiable.Id id)
	{
		if (SRSingleton<SceneContext>.Instance.GameModel.GetDecorizerModel().GetCount(id) > 0)
		{
			SECTR_AudioSystem.Play(this.onButtonRetrieveEntryCue, Vector3.zero, false);
			this.storage.selected = id;
			this.Close();
		}
	}

	// Token: 0x04001BE7 RID: 7143
	[Tooltip("Main menu parent panel.")]
	public GameObject defaultParent;

	// Token: 0x04001BE8 RID: 7144
	[Tooltip("Parent panel containing the retrieve categories ui.")]
	public GameObject retrieveCategoriesParent;

	// Token: 0x04001BE9 RID: 7145
	[Tooltip("Parent panel containing the retrieve items ui.")]
	public GameObject retrieveParent;

	// Token: 0x04001BEA RID: 7146
	[Tooltip("Decorizer shared retrieve items grid parent.")]
	public GameObject retrieveContentsGrid;

	// Token: 0x04001BEB RID: 7147
	[Tooltip("Prefab of a Decorizer shared content item.")]
	public GameObject retrieveContentsEntry;

	// Token: 0x04001BEC RID: 7148
	[Tooltip("Parent panel containing the cleanup ui.")]
	public GameObject cleanupParent;

	// Token: 0x04001BED RID: 7149
	[Header("SFX")]
	[Tooltip("SFX played when the UI is enabled. (optional)")]
	public SECTR_AudioCue onEnableCue;

	// Token: 0x04001BEE RID: 7150
	[Tooltip("SFX played when the UI is disabled. (optional)")]
	public SECTR_AudioCue onDisableCue;

	// Token: 0x04001BEF RID: 7151
	[Tooltip("SFX played when the \"retrieve\" button is pressed. (optional)")]
	public SECTR_AudioCue onButtonRetrieveCue;

	// Token: 0x04001BF0 RID: 7152
	[Tooltip("SFX played when a \"retrieve\" category button is pressed. (optional)")]
	public SECTR_AudioCue onButtonRetrieveCategoryCue;

	// Token: 0x04001BF1 RID: 7153
	[Tooltip("SFX played when a \"retrieve\" item button is pressed. (optional)")]
	public SECTR_AudioCue onButtonRetrieveEntryCue;

	// Token: 0x04001BF2 RID: 7154
	[Tooltip("SFX played when the \"cleanup\" button is pressed. (optional)")]
	public SECTR_AudioCue onButtonCleanupCue;

	// Token: 0x04001BF3 RID: 7155
	[Tooltip("SFX played when a \"cleanup\" item button is pressed. (optional)")]
	public SECTR_AudioCue onButtonCleanupEntryCue;

	// Token: 0x04001BF5 RID: 7157
	private const int MAX_DISPLAY_COUNT = 999;

	// Token: 0x04001BF6 RID: 7158
	private Stack<DecorizerUI.State> states;

	// Token: 0x02000569 RID: 1385
	private enum State
	{
		// Token: 0x04001BF8 RID: 7160
		DEFAULT,
		// Token: 0x04001BF9 RID: 7161
		RETRIEVE_CATEGORIES,
		// Token: 0x04001BFA RID: 7162
		RETRIEVE_ECHOES,
		// Token: 0x04001BFB RID: 7163
		RETRIEVE_ECHO_NOTES,
		// Token: 0x04001BFC RID: 7164
		RETRIEVE_ORNAMENTS,
		// Token: 0x04001BFD RID: 7165
		CLEANUP
	}
}
