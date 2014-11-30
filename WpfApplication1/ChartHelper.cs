using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace WpfApplication1
{
    class ChartHelper
    {
        public static Series CreateSeries(string seriesName, SeriesChartType chartType, System.Collections.IEnumerable xData, System.Collections.IEnumerable yData, string chartAreaName)
        {
            Series series = new Series(seriesName);
            series.ChartType = chartType;
            series.Points.DataBindXY(xData, yData);
            series.ChartArea = chartAreaName;

            return series;
        }
    }
}
