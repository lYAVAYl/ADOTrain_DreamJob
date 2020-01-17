using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls.Primitives;

namespace DreamJob
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Worker> workers = new List<Worker>();

        public MainWindow()
        {
            InitializeComponent();
        }
               
        private void StartInput(object sender, RoutedEventArgs e)
        {
            if (inputWorker.Text == "Введите имя сотрудника") inputWorker.Text = "";
            inputWorker.Foreground = Brushes.Black;
        }

        private void StopInput(object sender, RoutedEventArgs e)
        {
            if (inputWorker.Text.Trim() == "")
            {
                var brush = new BrushConverter();
                inputWorker.Foreground = (Brush)brush.ConvertFrom("#FF969696");

                inputWorker.Text = "Введите имя сотрудника";
            }
        }

        private void CheckLetter(object sender, TextCompositionEventArgs e)
        {
            if (Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void btnShowResults_Click(object sender, RoutedEventArgs e)
        {
            // Создания строки подключения с помощью объекта-построителя
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "DreamJob",
                DataSource = "DESKTOP-5D0552Q",
                ConnectTimeout = 30,
                IntegratedSecurity = true
            };

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = cnStringBuilder.ConnectionString;

                connection.Open();

                // Проверка условия вывода уволенных работников
                string showDissmised = "";
                if (ShowDissmised.IsChecked != true)
                    showDissmised = "WHERE Staff.Active = 'True'";

                string sql = $"SELECT Staff.WorkerName, {(CashFilter.Text == "Средняя за месяц" ? "AVG" : "MAX")}(Paymants.Salary) AS 'Зарплата'\n" +
                            "FROM Staff\n" +
                            "LEFT OUTER JOIN Paymants ON Staff.WorkerID = Paymants.WorkerID\n" +
                            $"{showDissmised}\n" +
                            "GROUP BY Staff.WorkerName, MONTH(Paymants.CashDate)\n" +
                            "ORDER BY MONTH(Paymants.CashDate)";
                SqlCommand myCommand = new SqlCommand(sql, connection);

                // Получить объект чтения данных с помощью ExecuteReader()
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    workers.Clear();

                    while (myDataReader.Read())
                    {
                        string name = myDataReader.GetValue(0).ToString();
                        var salary = myDataReader.GetValue(1) == DBNull.Value ? 0 : myDataReader.GetValue(1);
                        workers.Add(new Worker(name, (double)Math.Round(Convert.ToDecimal(salary), 2)));
                    }

                    workers = workers.OrderByDescending(w => w.Salary).ToList();
                    ResultList.Items.Clear();
                    // Добавление элементов списка в ListView
                    for (int i = 0; i < workers.Count; i++)
                    {
                        ResultList.Items.Add(workers[i]);

                        Highlighting(workers[i]);
                    }
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Поиск сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ResultList.SelectedItems.Clear();
            ResultList.Items.Refresh();
            
            if (inputWorker.Text != "Введите имя сотрудника")
                foreach (var item in ResultList.Items)
                {
                    if (((Worker)item).Name.ToLower().Contains(inputWorker.Text.ToLower()))
                    {
                        ResultList.SelectedItems.Add(item);
                    }
                }
        }


        /// <summary>
        /// Выделение работника с зп менее 20000
        /// </summary>
        /// <param name="worker"></param>
        void Highlighting(Worker worker)
        {
            if (worker.Salary < 20000)
            {
                ResultList.UpdateLayout(); // Обновление визуальных элементов
                                           // Получение контейнера элемента, который нужно выделить
                var itemContainer = ResultList.ItemContainerGenerator.ContainerFromItem(worker) as ListViewItem;
                if (itemContainer != null)
                {
                    var brush = new BrushConverter();
                    // Выделение
                    itemContainer.Background = (Brush)brush.ConvertFrom("#FFFFF1F1");
                }
            }
        }

    }
}
