using System;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x02000014 RID: 20
	public class SelectButton : Element
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600012F RID: 303 RVA: 0x0000F3D8 File Offset: 0x0000E3D8
		public int SelectedIndex
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000130 RID: 304 RVA: 0x0000F3F0 File Offset: 0x0000E3F0
		public string Value
		{
			get
			{
				return this.buttonNames[this.selected];
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000131 RID: 305 RVA: 0x0000F40F File Offset: 0x0000E40F
		// (remove) Token: 0x06000132 RID: 306 RVA: 0x0000F428 File Offset: 0x0000E428
		public event EventHandler<SelectEventArgs> OnSelect;

		// Token: 0x06000133 RID: 307 RVA: 0x0000F441 File Offset: 0x0000E441
		public SelectButton(string name, Rect rect, string[] buttonNames, EventHandler<SelectEventArgs> _onSelect) : base(name, rect)
		{
			this.buttonNames = buttonNames;
			this.OnSelect = (EventHandler<SelectEventArgs>)Delegate.Combine(this.OnSelect, _onSelect);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000F474 File Offset: 0x0000E474
		public override void Draw(Rect outRect)
		{
			this.onSelect(GUI.Toolbar(outRect, this.selected, this.buttonNames));
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000F490 File Offset: 0x0000E490
		private void onSelect(int newSelected)
		{
			if (this.selected != newSelected)
			{
				this.OnSelect(this, new SelectEventArgs(this.name, newSelected, this.buttonNames[newSelected]));
				this.selected = newSelected;
			}
		}

		// Token: 0x04000112 RID: 274
		private string[] buttonNames;

		// Token: 0x04000113 RID: 275
		private int selected = 0;
	}
}
