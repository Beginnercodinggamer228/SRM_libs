using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class InstrumentDirector : SRBehaviour, InstrumentModel.Participant
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000BA5 RID: 2981 RVA: 0x00031094 File Offset: 0x0002F294
	// (remove) Token: 0x06000BA6 RID: 2982 RVA: 0x000310CC File Offset: 0x0002F2CC
	public event InstrumentDirector.OnInstrumentChangedDelegate onInstrumentChanged = delegate(EchoNoteGameMetadata <p0>)
	{
	};

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000BA7 RID: 2983 RVA: 0x00031104 File Offset: 0x0002F304
	// (remove) Token: 0x06000BA8 RID: 2984 RVA: 0x0003113C File Offset: 0x0002F33C
	public event InstrumentDirector.OnInstrumentUnlockedDelegate onInstrumentUnlocked = delegate()
	{
	};

	// Token: 0x06000BA9 RID: 2985 RVA: 0x00031174 File Offset: 0x0002F374
	private void Awake()
	{
		foreach (EchoNoteGameMetadata echoNoteGameMetadata in this.instruments)
		{
			if (this.instrumentNoteData.ContainsKey(echoNoteGameMetadata.instrument))
			{
				throw new Exception("Duplicate instrument data for instrument type: " + Enum.GetName(typeof(InstrumentModel.Instrument), echoNoteGameMetadata.instrument));
			}
			if (echoNoteGameMetadata.instrument == InstrumentModel.Instrument.NONE)
			{
				throw new Exception("Invalid instrument data - no instrument type set: " + echoNoteGameMetadata);
			}
			this.instrumentNoteData[echoNoteGameMetadata.instrument] = echoNoteGameMetadata;
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x00031203 File Offset: 0x0002F403
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterInstrument(this);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(InstrumentModel model)
	{
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00031215 File Offset: 0x0002F415
	public void SetModel(InstrumentModel model)
	{
		this.model = model;
		if (model.GetCurrentlySelectedInstrument() == InstrumentModel.Instrument.NONE && SRSingleton<SceneContext>.Instance.PediaDirector.IsUnlocked(PediaDirector.Id.ECHO_NOTES))
		{
			model.UnlockInstrument(InstrumentModel.Instrument.MARIMBA);
			model.SelectInstrument(InstrumentModel.Instrument.MARIMBA);
		}
		this.UpdateCurrentEchoNotes();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00031254 File Offset: 0x0002F454
	public void UnlockInstrument(InstrumentModel.Instrument instrument)
	{
		if (this.IsUnlocked(instrument))
		{
			return;
		}
		this.model.UnlockInstrument(instrument);
		if (this.model.GetCurrentlySelectedInstrument() == InstrumentModel.Instrument.NONE)
		{
			this.SelectInstrument(instrument);
		}
		if (this.model.GetUnlockedInstruments().Count > 1)
		{
			PopupDirector popupDirector = SRSingleton<SceneContext>.Instance.PopupDirector;
			popupDirector.QueueForPopup(new InstrumentPopupUI.PopupCreator(this.instrumentNoteData[instrument]));
			popupDirector.MaybePopupNext();
		}
		this.onInstrumentUnlocked();
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x000312D0 File Offset: 0x0002F4D0
	public HashSet<InstrumentModel.Instrument> GetUnlockedInstruments()
	{
		return this.model.GetUnlockedInstruments();
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x000312DD File Offset: 0x0002F4DD
	public bool IsUnlocked(InstrumentModel.Instrument instrument)
	{
		return this.model.GetUnlockedInstruments().Contains(instrument);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x000312F0 File Offset: 0x0002F4F0
	public void UnlockNextInstrument()
	{
		foreach (InstrumentModel.Instrument instrument in this.unlockOrder)
		{
			if (!this.IsUnlocked(instrument))
			{
				this.UnlockInstrument(instrument);
				return;
			}
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00031327 File Offset: 0x0002F527
	public void SelectInstrument(InstrumentModel.Instrument instrument)
	{
		this.model.SelectInstrument(instrument);
		this.UpdateCurrentEchoNotes();
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003133C File Offset: 0x0002F53C
	public void SelectNextInstrument()
	{
		if (this.model.GetCurrentlySelectedInstrument() == InstrumentModel.Instrument.NONE)
		{
			throw new Exception("Trying to select next instrument with no instruments available.");
		}
		List<InstrumentModel.Instrument> list = this.unlockOrder.Where(new Func<InstrumentModel.Instrument, bool>(this.IsUnlocked)).ToList<InstrumentModel.Instrument>();
		list.AddRange(from instrument in this.model.GetUnlockedInstruments()
		where !this.unlockOrder.Contains(instrument)
		select instrument);
		int num = list.IndexOf(this.model.GetCurrentlySelectedInstrument());
		this.SelectInstrument(list[(num + 1) % list.Count]);
		this.UpdateCurrentEchoNotes();
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x000313D0 File Offset: 0x0002F5D0
	private void UpdateCurrentEchoNotes()
	{
		if (this.model.GetCurrentlySelectedInstrument() != InstrumentModel.Instrument.NONE)
		{
			this.currentInstrument = this.instrumentNoteData[this.model.GetCurrentlySelectedInstrument()];
		}
		else
		{
			this.currentInstrument = this.instrumentNoteData[this.unlockOrder[0]];
		}
		this.onInstrumentChanged(this.currentInstrument);
	}

	// Token: 0x04000AA7 RID: 2727
	[Tooltip("Order in which instruments are unlocked.")]
	public InstrumentModel.Instrument[] unlockOrder;

	// Token: 0x04000AA8 RID: 2728
	[Tooltip("Echo note metadata for all available instruments.")]
	public EchoNoteGameMetadata[] instruments;

	// Token: 0x04000AA9 RID: 2729
	[Tooltip("EchoNotes metadata for the currently selected instrument.")]
	public EchoNoteGameMetadata currentInstrument;

	// Token: 0x04000AAA RID: 2730
	public InstrumentPopupUI popupUI;

	// Token: 0x04000AAB RID: 2731
	private readonly Dictionary<InstrumentModel.Instrument, EchoNoteGameMetadata> instrumentNoteData = new Dictionary<InstrumentModel.Instrument, EchoNoteGameMetadata>();

	// Token: 0x04000AAC RID: 2732
	private InstrumentModel model;

	// Token: 0x02000220 RID: 544
	// (Invoke) Token: 0x06000BB7 RID: 2999
	public delegate void OnInstrumentChangedDelegate(EchoNoteGameMetadata instrument);

	// Token: 0x02000221 RID: 545
	// (Invoke) Token: 0x06000BBB RID: 3003
	public delegate void OnInstrumentUnlockedDelegate();
}
