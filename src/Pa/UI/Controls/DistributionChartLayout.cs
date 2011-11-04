using System.Collections.Generic;
using System.Xml.Serialization;
using SIL.Pa.PhoneticSearching;

namespace SIL.Pa.UI.Controls
{
	/// ----------------------------------------------------------------------------------------
	[XmlType("chart")]
	public class DistributionChartLayout
	{
		public const string kFileName = "DistributionCharts.xml";

		private string _name;
		
		/// ------------------------------------------------------------------------------------
		public DistributionChartLayout()
		{
			SearchItems = new List<string>();
			SearchQueries = new List<SearchQuery>();
			ColumnWidths = new List<int>();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a new DistributionChartLayout object from the specified DistributionGrid.
		/// If the grid is null or empty, then null is returned.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static DistributionChartLayout NewFromDistributionGrid(DistributionGrid grid)
		{
			if (grid == null || (grid.Rows.Count <= 1 && grid.Columns.Count <= 1))
				return null;

			var layout = new DistributionChartLayout();
			layout.UpdateFromDistributionGrid(grid);
			return layout;
		}

		/// ------------------------------------------------------------------------------------
		public static string GetFileForProject(string projectPathPrefix)
		{
			return projectPathPrefix + kFileName;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the name of the layout if it's not empty or null or "(none)" if it is.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public string NameOrNone
		{
			get
			{
				return (string.IsNullOrEmpty(Name) ?
					App.GetString("Views.DistributionCharts.EmptyName", "(none)") : Name);
			}
		}

		#region Properties
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("name")]
		public string Name
		{
			get { return (_name == null ? null : _name.Trim()); }
			set { _name = value; }
		}

		/// ------------------------------------------------------------------------------------
		[XmlArray("searchItems"), XmlArrayItem("item")]
		public List<string> SearchItems { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlArray("searchQueries"), XmlArrayItem("query")]
		public List<SearchQuery> SearchQueries { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlArray("columnWidths"), XmlArrayItem("width")]
		public List<int> ColumnWidths { get; set; }

		#endregion

		#region Misc. methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the DistributionChartLayout with the information from the specified grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void UpdateFromDistributionGrid(DistributionGrid grid)
		{
			if (grid == null)
				return;

			Reset();

			// Save the search items.
			for (int i = 1; i < grid.Rows.Count; i++)
			{
				if (i != grid.NewRowIndex)
					SearchItems.Add(grid[0, i].Value as string);
			}

			// Add the search item column.
			ColumnWidths.Add(grid.Columns[0].Width);

			// Save the environments and column widths.
			for (int i = 1; i < grid.Columns.Count - 1; i++)
			{
				ColumnWidths.Add(grid.Columns[i].Width);
				var query = grid.Columns[i].Tag as SearchQuery;
				if (query != null)
					SearchQueries.Add(query);
			}

			// Add the new environment column (this is like the column equivalent of the new row).
			ColumnWidths.Add(grid.Columns[grid.Columns.Count - 1].Width);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a deep copy of the layout.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public DistributionChartLayout Clone()
		{
			var clone = new DistributionChartLayout();

			clone._name = _name;
			
			foreach (var srchItem in SearchItems)
				clone.SearchItems.Add(srchItem);

			foreach (var query in SearchQueries)
				clone.SearchQueries.Add(query.Clone());

			foreach (int width in ColumnWidths)
				clone.ColumnWidths.Add(width);

			return clone;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clear's the internal search items and queries lists. The XYCharLayout's name is
		/// preserved.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Reset()
		{
			SearchItems.Clear();
			SearchQueries.Clear();
			ColumnWidths.Clear();
		}

		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return _name;
		}

		#endregion
	}
}
