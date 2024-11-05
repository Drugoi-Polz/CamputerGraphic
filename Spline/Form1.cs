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

namespace Spline
{
	public partial class Form1 : Form
	{
		private double[] xArray = new double[25];
		private double[] yArray = new double[25];
		private double step = 5.0 / 24.0;

		public Form1()
		{
			InitializeComponent();

			// Инициализация значений x и y
			for (int i = 0; i < 25; i++)
			{
				xArray[i] = i * step;
				yArray[i] = Math.Cos(xArray[i]) * Math.Cos(xArray[i]);
			}

			DrawGraphic();
		}

		public class Spline
		{
			public double A, B, C, D, X;
		}

		// Упрощенный метод создания сплайнов, без прогонки
		public Spline[] CreateBasicSplines(double[] x, double[] y)
		{
			int n = x.Length - 1;
			Spline[] splines = new Spline[n];

			for (int i = 0; i < n; i++)
			{
				splines[i] = new Spline
				{
					X = x[i],
					A = y[i],
					B = (y[i + 1] - y[i]) / (x[i + 1] - x[i])  // Простейший линейный коэффициент
				};
				splines[i].C = splines[i].D = 0; // Обнулим C и D, чтобы проверить только линейный вариант
			}

			return splines;
		}

		// Интерполяция значения для базового сплайна
		public double BasicSplineInterpolation(Spline[] splines, double targetX)
		{
			int n = splines.Length;
			Spline spline = splines[0];

			// Поиск подходящего интервала
			if (targetX <= splines[0].X)
				spline = splines[0];
			else if (targetX >= splines[n - 1].X)
				spline = splines[n - 1];
			else
			{
				for (int i = 0; i < n - 1; i++)
				{
					if (targetX >= splines[i].X && targetX <= splines[i + 1].X)
					{
						spline = splines[i];
						break;
					}
				}
			}

			// Интерполяция с использованием только A и B
			double dx = targetX - spline.X;
			return spline.A + spline.B * dx;
		}

		private void DrawGraphic()
		{
			chart1.Series.Clear();

			// Исходная функция f(x) = cos^2(x)
			chart1.Series.Add("f(x) = cos^2(x)");
			chart1.Series[0].ChartType = SeriesChartType.Line;
			chart1.Series[0].BorderWidth = 2;
			chart1.Size = new Size(1000, 1000);
			for (double x = 0; x <= 5; x += 0.1)
			{
				double y = Math.Cos(x) * Math.Cos(x);
				chart1.Series[0].Points.AddXY(x, y);
			}

			// Простейшая линейная интерполяция сплайнами
			var splines = CreateBasicSplines(xArray, yArray);
			if (splines == null)
			{
				MessageBox.Show("Ошибка при создании сплайнов.");
				return;
			}

			chart1.Series.Add("Linear Spline Interpolation");
			chart1.Series[1].ChartType = SeriesChartType.Line;
			chart1.Series[1].BorderWidth = 2;
			for (double x = 0; x <= 5; x += 0.1)
			{
				double y = BasicSplineInterpolation(splines, x);
				if (double.IsNaN(y) || double.IsInfinity(y))
				{
					MessageBox.Show($"Некорректное значение y при x = {x}: {y}");
					continue;
				}
				chart1.Series[1].Points.AddXY(x, y);
			}
		}
	}
}
