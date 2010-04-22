using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SIL.Pa.Properties;
using SIL.Pa.UI.Controls;
using SilUtils;

namespace SIL.Pa.Processing
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// This class provides a way to export a vowel or consonant chart to an XML format that
	/// is transformed into an html file with an accompanying cascading style sheet.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class DistributionChartExporter : ExporterBase
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool ToHtml(PaProject project, string outputFileName,
			XYGrid distChartGrid, bool openAfterExport)
		{
			return Process(project, outputFileName, OutputFormat.XHTML, distChartGrid, openAfterExport,
				Pipeline.ProcessType.ExportToXHTML, Settings.Default.AppThatOpensHtml);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool ToWordXml(PaProject project, string outputFileName,
			XYGrid distChartGrid, bool openAfterExport)
		{
			return Process(project, outputFileName, OutputFormat.WordXml, distChartGrid, openAfterExport,
				Pipeline.ProcessType.ExportToWord, Settings.Default.AppThatOpensWordXml);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static bool Process(PaProject project, string outputFileName,
			OutputFormat outputFormat, DataGridView grid, bool openAfterExport,
			Pipeline.ProcessType finalPipeline, string appToOpenOutput)
		{
			var exporter = new DistributionChartExporter(project, outputFileName, outputFormat, grid);

			var result = exporter.InternalProcess(Settings.Default.KeepTempDistributionChartExportFile,
				Pipeline.ProcessType.ExportDistributionChart, finalPipeline);

			if (result && openAfterExport)
				CallAppToOpenWordXML(appToOpenOutput, outputFileName);

			return result;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private DistributionChartExporter(PaProject project, string outputFileName,
			OutputFormat outputFormat, DataGridView distChartGrid)
			: base(project, outputFileName, outputFormat, distChartGrid)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override string Title
		{
			get
			{
				return (string.IsNullOrEmpty(((XYGrid)m_grid).ChartName) ?
					"Distribution Chart" : ((XYGrid)m_grid).ChartName);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override string Name
		{
			get { return ((XYGrid)m_grid).ChartName; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override string View
		{
			get { return "Distribution Chart"; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override string TableClass
		{
			get { return "distribution chart"; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override string NumberOfRecords
		{
			get { return App.WordCache.Count.ToString(); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override IEnumerable<KeyValuePair<string, Font>> GetFormattingFieldInfo()
		{
			yield return new KeyValuePair<string, Font>("Phonetic", FontHelper.PhoneticFont);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override IEnumerable<DataGridViewRow> GetGridRows()
		{
			// Skip the first row because in a distribution chart, it is like a heading row.
			return from x in m_grid.Rows.Cast<DataGridViewRow>()
				   where x.Visible && x.Index > 0 && x.Index != m_grid.NewRowIndex
				   select x;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override IEnumerable<DataGridViewColumn> GetGridColumns()
		{
			// Skip the first column because, in a distribution chart, it is like a row heading.
			return from x in m_grid.Columns.Cast<DataGridViewColumn>()
				   orderby x.DisplayIndex
				   where x.Visible && x.Index > 0 && x.Index < m_grid.ColumnCount - 1
				   select x;
		}
	
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void WriteTableHeadingColumnGroups()
		{
			// Write group for far left column containing search item.
			ProcessHelper.WriteColumnGroup(m_writer, 1);

			// Write group for each column after the search item column, but not including
			// the last column which is always empty to provide a space for the user to
			// add another search pattern column.
			ProcessHelper.WriteColumnGroup(m_writer, GetGridColumns().Count());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void WriteTableHeadingContent()
		{
			// Write an empty element for the first column, because it is like a row heading.
			ProcessHelper.WriteEmptyElement(m_writer, "th");

			foreach (var col in GetGridColumns())
			{
				ProcessHelper.WriteStartElementWithAttrib(m_writer, "th", "class", "Phonetic");
				m_writer.WriteAttributeString("scope", "col");
				m_writer.WriteString(m_grid[col.Index, 0].Value as string);
				m_writer.WriteEndElement();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void WriteTableRowContent(DataGridViewRow row)
		{
			ProcessHelper.WriteStartElementWithAttrib(m_writer, "th", "class", "Phonetic");
			m_writer.WriteAttributeString("scope", "row");
			m_writer.WriteString(row.Cells[0].Value as string);
			m_writer.WriteEndElement();

			foreach (var col in GetGridColumns())
			{
				var value = row.Cells[col.Index].Value;

				if (value is XYChartException)
				{
					ProcessHelper.WriteStartElementWithAttrib(m_writer, "td", "class", "error");

					var msg = ((XYChartException)value).QueryErrorMessage;
					msg = msg.Replace(System.Environment.NewLine, " ");
					m_writer.WriteAttributeString("title", msg);
					m_writer.WriteEndElement();
				}
				else if (value == null)
					ProcessHelper.WriteEmptyElement(m_writer, "td");
				else
				{
					if (value.GetType() == typeof(int))
						m_writer.WriteElementString("td", ((int)value).ToString());
					else if (value is string)
						m_writer.WriteElementString("td", value as string);
				}
			}
		}
	}
}