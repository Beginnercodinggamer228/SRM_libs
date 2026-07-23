using System;

// Token: 0x0200001B RID: 27
public interface StateManager
{
	// Token: 0x06000061 RID: 97
	void SetState(string stateName, bool setEnabled = true);

	// Token: 0x06000062 RID: 98
	void Reset();

	// Token: 0x06000063 RID: 99
	void CombineStates();

	// Token: 0x06000064 RID: 100
	bool IsEnabled(string stateName);
}
