using System;

// Token: 0x02000721 RID: 1825
public abstract class IdHandler : SRBehaviour
{
	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06002625 RID: 9765 RVA: 0x000923E5 File Offset: 0x000905E5
	public string id
	{
		get
		{
			if (this.director == null)
			{
				this.director = base.GetRequiredComponentInParent<IdDirector>(false);
			}
			return this.director.GetPersistenceIdentifier(this);
		}
	}

	// Token: 0x06002626 RID: 9766
	protected abstract string IdPrefix();

	// Token: 0x04002588 RID: 9608
	private IdDirector director;
}
