using System;
using UnityEngine.UI;

// Token: 0x02000549 RID: 1353
public class BugReportUI : BaseUI
{
	// Token: 0x06001C3C RID: 7228 RVA: 0x0006BB51 File Offset: 0x00069D51
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.ignoreCallback = true;
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x0006BB60 File Offset: 0x00069D60
	public void Submit()
	{
		string text = this.summaryField.text;
		if (text.Length <= 0)
		{
			base.Error("e.require_summary", false);
			return;
		}
		this.submitButton.interactable = false;
		base.Status("m.sending_report");
		SentrySdk.CaptureFeedback(text, this.descField.text, new SentrySdk.OnCaptureCompleteDelegate(this.OnBugReportComplete));
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x0006BBC3 File Offset: 0x00069DC3
	private void OnBugReportComplete(Exception exception)
	{
		if (this.ignoreCallback)
		{
			return;
		}
		if (exception != null)
		{
			this.submitButton.interactable = true;
			base.Error(exception.Message, false);
			return;
		}
		this.Close();
	}

	// Token: 0x04001B3A RID: 6970
	public InputField summaryField;

	// Token: 0x04001B3B RID: 6971
	public InputField descField;

	// Token: 0x04001B3C RID: 6972
	public Button submitButton;

	// Token: 0x04001B3D RID: 6973
	private const string ERR_REQUIRE_SUMMARY = "e.require_summary";

	// Token: 0x04001B3E RID: 6974
	private const string MSG_SENDING_REPORT = "m.sending_report";

	// Token: 0x04001B3F RID: 6975
	private bool ignoreCallback;
}
