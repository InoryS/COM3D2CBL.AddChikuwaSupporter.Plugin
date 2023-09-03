using System;

namespace UnityObsoleteGui
{
	// Token: 0x02000015 RID: 21
	public class ElementEventArgs : EventArgs
	{
		// Token: 0x06000136 RID: 310 RVA: 0x0000F4D5 File Offset: 0x0000E4D5
		public ElementEventArgs(string name, bool sizeChanged, bool visibleChanged)
		{
			this.Name = name;
			this.SizeChanged = sizeChanged;
			this.VisibleChanged = visibleChanged;
		}

		// Token: 0x04000115 RID: 277
		public string Name;

		// Token: 0x04000116 RID: 278
		public bool SizeChanged;

		// Token: 0x04000117 RID: 279
		public bool VisibleChanged;
	}
}
