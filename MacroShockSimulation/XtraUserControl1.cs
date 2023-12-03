using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace MacroShockSimulation
{
    public partial class XtraUserControl1 : DevExpress.XtraEditors.XtraUserControl
    {
        string[] columns = {
            "t",
            "Монетарный шок",
            "Технологический шок",
            "Производственный разрыв",
            "Инфляция",
            "Номинальная процентная ставка",
            "Реальная процентная ставка",
            "ВВП на душу населения",
            "Темп прироста денежной массы",
        };
        public enum columnsPosition
        {
            t,
            monetary_shock,
            technological_shock,
            output_gap,
            inflation,
            nominal_interest_rate,
            real_interest_rate,
            gdp_per_capita,
            money_supply_growth_rate,
        }
        public XtraUserControl1()
        {
            InitializeComponent();

            gridView1.CustomDrawCell += GridView_CustomDrawCell;
        }

        private void ComboBoxDouble_EditValueChanging(object sender, ChangingEventArgs e)
        {
            string newText = e.NewValue.ToString();
            var comboBox = sender as DevExpress.XtraEditors.TextEdit;
            if (!IsValidDouble(newText))
            {
                // Некорректное значение, устанавливаем красный цвет текста
                comboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
                calculationButton.Enabled = false;
            }
            else
            {
                // Корректное значение, устанавливаем цвет текста по умолчанию
                comboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
                calculationButton.Enabled = true;
            }
        }

        private void ComboBoxInt_EditValueChanging(object sender, ChangingEventArgs e)
        {
            string newText = e.NewValue.ToString();
            var comboBox = sender as DevExpress.XtraEditors.TextEdit;
            if (!IsValidInt(newText))
            {
                // Некорректное значение, устанавливаем красный цвет текста
                comboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
                calculationButton.Enabled = false;
            }
            else
            {
                // Корректное значение, устанавливаем цвет текста по умолчанию
                comboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
                calculationButton.Enabled = true;
            }
        }
        private bool IsValidDouble(string value)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private bool IsValidInt(string value)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private void calculationButton_Click(object sender, EventArgs e)
        {
            // 1. Получаем значения из наших TextEdit
            int period = int.Parse(periodTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double beta = double.Parse(betaTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double sigma = double.Parse(sigmaTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double phi = double.Parse(phiTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double alpha = double.Parse(alphaTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double epsilon = double.Parse(epsilonTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double eta = double.Parse(etaTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double theta = double.Parse(thetaTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double psi_pi = double.Parse(psiPiTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double psi_y = double.Parse(psiYTtextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double rho_nu = double.Parse(rhoNuTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            double rho_a = double.Parse(rhoAtextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            ////
            // 2. Запишем вспомогательные переменные
            // 7 страница в pdf и 47 в учебнике
            double _lambda = ((1.0 - beta * theta) * (1.0 - alpha) / 
                (1.0 - alpha + alpha * epsilon) * (1.0 - theta)) / theta;
            double gamma = Math.Log(epsilon / (epsilon - 1));
            // 9 страница в pdf и 49 в учебнике
            double kappa = _lambda * (sigma + (phi + alpha) / (1.0 - alpha));
            double rho = -Math.Log(beta);
            double y_t_n = (Math.Log(1 - alpha) - gamma) / (sigma + ((alpha + phi) / (1 - alpha)));
            // 11 страница в pdf и 51 в учебнике
            double capitalLambda_nu = 1 / ((sigma * (1 - rho_nu) + psi_y) * (1 - beta * rho_nu) + 
                                           (psi_pi - rho_nu) * kappa);
            double y_t_wave = -(1 - beta * rho_nu) * capitalLambda_nu;
            double pi_t = kappa * capitalLambda_nu;
            ////
            // 3. Найдём d(m_t)/d(eps_t^nu)
            // 12 страница в pdf и 52 в учебнике
            double derivative = -capitalLambda_nu * (1 - beta * rho_nu) *
                                (1 + eta * sigma * (1 - rho_nu)) +
                                kappa * (1 - eta * rho_nu);
            ////
            // 4. Построим матрицу At
            // 10 страница в pdf и 50 в учебнике
             var At = new double[2, 2];
            At[0, 0] = sigma / (sigma + psi_y + psi_pi * kappa);
            At[0, 1] = (1.0 - beta * psi_pi) / (sigma + psi_y + psi_pi * kappa);
            At[1, 0] = sigma * kappa / (sigma + psi_y + psi_pi * kappa);
            At[1, 1] = (kappa + beta * (sigma + psi_y) / (sigma + psi_y + psi_pi * kappa));
            ////
            // 5. Создаём таблицу
            var dataTable = new DataTable();

            // столбцы
            foreach (var col in columns)
            {
                dataTable.Columns.Add(new DataColumn(col));
            }
            ////
            // 6. Заполняем начальные параметры
            var rowData = new object[columns.Length];
            rowData[(int)columnsPosition.t] = 1;
            rowData[(int)columnsPosition.technological_shock] = 0.25;
            rowData[(int)columnsPosition.monetary_shock] = 0 + (double)rowData[2];
            rowData[(int)columnsPosition.output_gap] = -(1 - beta * rho_nu) * capitalLambda_nu *
                                              (double)rowData[(int)columnsPosition.monetary_shock];
            rowData[(int)columnsPosition.inflation] = -kappa * capitalLambda_nu * eta *
                                              (double)rowData[(int)columnsPosition.monetary_shock];
            rowData[(int)columnsPosition.nominal_interest_rate] = (sigma * (1 - rho_nu) *
                                   (1 - beta * rho_nu) - rho_nu * kappa) * capitalLambda_nu * eta *
                                              (double)rowData[(int)columnsPosition.monetary_shock];
            rowData[(int)columnsPosition.real_interest_rate] = sigma * (1 - rho_nu) *
                                   (1 - beta * rho_nu) * capitalLambda_nu * eta *
                                              (double)rowData[(int)columnsPosition.monetary_shock];
            rowData[(int)columnsPosition.gdp_per_capita] =
                (double)rowData[(int)columnsPosition.output_gap] + y_t_n;
            rowData[(int)columnsPosition.money_supply_growth_rate] =
                (double)rowData[(int)columnsPosition.gdp_per_capita] -
                eta * (double)rowData[(int)columnsPosition.nominal_interest_rate] +
                (double)rowData[(int)columnsPosition.monetary_shock] * derivative;
            dataTable.Rows.Add(rowData);
            // 7. Заполняем по периодам
            for (int t = 1; t < period; t++)
            {
                rowData = new object[columns.Length];
                var prevData = dataTable.Rows[t - 1].ItemArray;
                rowData[(int)columnsPosition.t] = t + 1;
                rowData[(int)columnsPosition.technological_shock] = 0.0;
                rowData[(int)columnsPosition.monetary_shock] =
                    rho_nu * double.Parse((string)prevData[(int)columnsPosition.monetary_shock]) +
                                          (double)rowData[(int)columnsPosition.technological_shock];
                rowData[(int)columnsPosition.output_gap] =
                    -(1 - beta * rho_nu) * capitalLambda_nu * (double)rowData[(int)columnsPosition.monetary_shock];
                rowData[(int)columnsPosition.inflation] = -kappa * capitalLambda_nu * 
                                                          (double)rowData[(int)columnsPosition.monetary_shock] * 4;
                rowData[(int)columnsPosition.nominal_interest_rate] = (sigma * (1 - rho_nu) * 
                                                                          (1 - beta * rho_nu) - rho_nu * kappa) *
                capitalLambda_nu * (double)rowData[(int)columnsPosition.monetary_shock] * 4;
                rowData[(int)columnsPosition.real_interest_rate] = sigma * (1 - rho_nu) * (1 - beta * rho_nu) *
                capitalLambda_nu * (double)rowData[(int)columnsPosition.monetary_shock] * 4;
                rowData[(int)columnsPosition.gdp_per_capita] = (double)rowData[(int)columnsPosition.output_gap] + y_t_n;
                if (t == 1)
                {
                    rowData[(int)columnsPosition.money_supply_growth_rate] = 
                        (double)rowData[(int)columnsPosition.gdp_per_capita] -
                    eta * (double)rowData[(int)columnsPosition.nominal_interest_rate] -
                        double.Parse((string)prevData[(int)columnsPosition.money_supply_growth_rate]);
                }
                else
                {

                    rowData[(int)columnsPosition.money_supply_growth_rate] = 
                        (double)rowData[(int)columnsPosition.gdp_per_capita] - 
                        eta * (double)rowData[(int)columnsPosition.nominal_interest_rate] - 
                        (double.Parse((string)prevData[(int)columnsPosition.gdp_per_capita]) -
                         eta * double.Parse((string)prevData[(int)columnsPosition.nominal_interest_rate]));

                }
                dataTable.Rows.Add(rowData);
            }


            gridControl.DataSource = dataTable;
            SetGridViewColumnsWidth(gridView1, columnsWidth: 150);
            gridView1.IndicatorWidth = 30;
            xtraTabPage2.Visible = true;
            xtraTabPage2.PageVisible = true;

            // Отрисовка графика
            var tList = new List<int>();
            var anotherList = new List<double>();
            Series series = new Series("Монетарный шок от T", ViewType.Line);
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object cellValue = gridView1.GetRowCellValue(i, "t");
                tList.Add(int.Parse((string)cellValue));
                cellValue = gridView1.GetRowCellValue(i, "Монетарный шок");
                anotherList.Add(double.Parse((string)cellValue));
                series.Points.Add(new SeriesPoint(tList[i], anotherList[i]));
            }
            chartControl.Series.Clear();
            chartControl.Series.Add(series);
        }

        private void SetGridViewColumnsWidth(GridView gridView, int columnsWidth)
        {
            int columnsCount = gridView.Columns.Count;
            gridView.Columns[0].Width = 200;
            gridView.Columns[1].Width = 220;
            for (int i = 2; i < columnsCount; i++)
            {
                gridView.Columns[i].Width = columnsWidth;
            }
            gridView.Columns[0].AppearanceCell.Font
                = new System.Drawing.Font(gridView.Columns[0].AppearanceCell.Font, System.Drawing.FontStyle.Bold);
        }

        private void GridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // Проверка, что текущая ячейка не является заголовком столбца
            if (e.RowHandle >= 0 && e.Column.AbsoluteIndex > 0)
            {
                // Проверка, что значение ячейки является числом
                if (e.DisplayText != null && double.TryParse(e.DisplayText, out double cellValue))
                {
                    // Округление числа до 8 знаков после запятой
                    string roundedValue = Math.Round(cellValue, 8).ToString("0.########");

                    // Установка округленного значения в ячейку
                    e.DisplayText = roundedValue;
                }
            }
        }

        private void XtraUserControl1_Load(object sender, EventArgs e)
        {
            YcomboBoxEdit.Properties.Items.AddRange(columns);
            XcomboBoxEdit.Properties.Items.AddRange(columns);
            YcomboBoxEdit.SelectedItem = columns[(int)columnsPosition.monetary_shock];
            XcomboBoxEdit.SelectedItem = columns[(int)columnsPosition.t];
        }

        private void AxisComboBoxEdit_SelectedValueChanged(object sender, EventArgs e)
        {
            chartControl.Series.Clear();
            var xValues = new List<double>();
            var yValues = new List<double>();
            Series series = new Series((string)YcomboBoxEdit.SelectedItem + " от " + (string)XcomboBoxEdit.SelectedItem, ViewType.Line);
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                object cellValue = gridView1.GetRowCellValue(i, (string)XcomboBoxEdit.SelectedItem);
                xValues.Add(double.Parse((string)cellValue));
                cellValue = gridView1.GetRowCellValue(i, (string)YcomboBoxEdit.SelectedItem);
                yValues.Add(double.Parse((string)cellValue));
                series.Points.Add(new SeriesPoint(xValues[i], yValues[i]));
            }
            chartControl.Series.Add(series);
        }
    }
}
