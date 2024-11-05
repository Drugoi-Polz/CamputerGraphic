using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Interpolyation
{
	public partial class Form1 : Form
	{
		private double[] xArray = new double[25];
		private double[] yArray = new double[25];
		private double step = 5.0 / 24;
		public Form1()
		{
			InitializeComponent();

			for (int i = 0; i < 25; i++)
			{
				xArray[i] = i * step;
				yArray[i] = Math.Cos(xArray[i]) * Math.Cos(xArray[i]);
			}

		}
		public double LagrangeInterpolation(double[] x, double[] y, double targetX)
		{
			int n = x.Length;
			double result = 0;

			for (int i = 0; i < n; i++)
			{
				double term = y[i];
				for (int j = 0; j < n; j++)
				{
					if (j != i)
					{
						term *= (targetX - x[j]) / (x[i] - x[j]);
					}
				}
				result += term;
			}

			return result;
		}
		public double[,] DividedDifferenceTable(double[] x, double[] y)
		{
			int n = x.Length;
			double[,] table = new double[n, n];

			for (int i = 0; i < n; i++)
				table[i, 0] = y[i];

			for (int j = 1; j < n; j++)
			{
				for (int i = 0; i < n - j; i++)
				{
					table[i, j] = (table[i + 1, j - 1] - table[i, j - 1]) / (x[i + j] - x[i]);
				}
			}

			return table;
		}
		public double NewtonInterpolation(double[] x, double[] y, double targetX)
		{
			int n = x.Length;
			double[,] table = DividedDifferenceTable(x, y);
			double result = table[0, 0];
			double product = 1.0;

			for (int i = 1; i < n; i++)
			{
				product *= (targetX - x[i - 1]);
				result += table[0, i] * product;
			}

			return result;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			chart1.Series.Clear();
			chart1.Size = new System.Drawing.Size(1000, 1000);

			chart1.Series.Add("f(x) = cos^2(x)");
			chart1.Series[0].ChartType = SeriesChartType.Line;

			for (double x = 0; x <= 5; x += step)
			{
				double y = Math.Cos(x) * Math.Cos(x);
				chart1.Series[0].Points.AddXY(x, y);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			chart1.Series.Clear();
			chart1.Size = new System.Drawing.Size(1000, 1000);
			
			// Серия для интерполяции Лагранжа
			chart1.Series.Add("Интерполяции Лагранжа");
			chart1.Series[0].ChartType = SeriesChartType.Line;
			chart1.Series[0].Color = Color.Red;

			for (double x = 0; x <= 5; x += step)
			{
				double y = LagrangeInterpolation(xArray, yArray, x);
				chart1.Series[0].Points.AddXY(x, y);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			chart1.Series.Clear();
			chart1.Size = new System.Drawing.Size(1000, 1000);

			// Серия для интерполяции Ньютона
			chart1.Series.Add("Интерполяции Ньютона");
			chart1.Series[0].ChartType = SeriesChartType.Line;
			chart1.Series[0].Color = Color.Magenta;
			for (double x = 0; x <= 5; x += step)
			{
				double y = NewtonInterpolation(xArray, yArray, x);
				chart1.Series[0].Points.AddXY(x, y);
			}
		}
	}
}
