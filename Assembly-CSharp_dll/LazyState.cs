using System;

// Token: 0x02000682 RID: 1666
public class LazyState<T> where T : struct
{
	// Token: 0x0600226E RID: 8814 RVA: 0x00085081 File Offset: 0x00083281
	public static implicit operator T(LazyState<T> instance)
	{
		if (instance.state == null)
		{
			throw new InvalidOperationException();
		}
		return instance.state.Value;
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000850A1 File Offset: 0x000832A1
	public LazyState()
	{
		this.state = null;
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000850B5 File Offset: 0x000832B5
	public LazyState(T initialValue)
	{
		this.state = new T?(initialValue);
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000850C9 File Offset: 0x000832C9
	public bool Update(T current)
	{
		if (!current.Equals(this.state))
		{
			this.state = new T?(current);
			return true;
		}
		return false;
	}

	// Token: 0x0400223D RID: 8765
	private T? state;
}
