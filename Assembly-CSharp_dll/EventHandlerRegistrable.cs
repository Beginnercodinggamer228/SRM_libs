using System;

// Token: 0x020007F4 RID: 2036
public interface EventHandlerRegistrable
{
	// Token: 0x06002AD7 RID: 10967
	void Register(vp_EventHandler eventHandler);

	// Token: 0x06002AD8 RID: 10968
	void Unregister(vp_EventHandler eventHandler);
}
