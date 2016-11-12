using System;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.Exts
{
	public static class MyExts
	{

		public static void AddChild(this Grid grid, View view, int row, int column, int rowspan = 1, int columnspan = 1)
		{
			if (row < 0)
				throw new ArgumentOutOfRangeException("row");
			if (column < 0)
				throw new ArgumentOutOfRangeException("column");
			if (rowspan <= 0)
				throw new ArgumentOutOfRangeException("rowspan");
			if (columnspan <= 0)
				throw new ArgumentOutOfRangeException("columnspan");
			if (view == null)
				throw new ArgumentNullException("view");
			Grid.SetRow((BindableObject)view, row);
			Grid.SetRowSpan((BindableObject) view, rowspan);
			Grid.SetColumn((BindableObject) view, column);
			Grid.SetColumnSpan((BindableObject) view, columnspan);
			grid.Children.Add(view);      
		}
	}
}

