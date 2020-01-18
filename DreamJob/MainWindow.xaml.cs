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
        
        public MainWindow()
        {
            InitializeComponent();
            ShowAllStaff();
        }

        // Создания строки подключения с помощью объекта-построителя
        private SqlConnectionStringBuilder cnStringBuilder = new SqlConnectionStringBuilder
        {
            InitialCatalog = "DreamJob",
            DataSource = "DESKTOP-5D0552Q",
            ConnectTimeout = 30,
            IntegratedSecurity = true
        };

        private void StartInput(object sender, RoutedEventArgs e)
        {
            if (findWorkerInput.Text == "Введите имя сотрудника") findWorkerInput.Text = "";
            findWorkerInput.Foreground = Brushes.Black;
        }

        private void StopInput(object sender, RoutedEventArgs e)
        {
            if (findWorkerInput.Text.Trim() == "")
            {
                var brush = new BrushConverter();
                findWorkerInput.Foreground = (Brush)brush.ConvertFrom("#FF969696");

                findWorkerInput.Text = "Введите имя сотрудника";
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
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = cnStringBuilder.ConnectionString;

                connection.Open();

                // Проверка условия вывода уволенных работников
                string showDissmised = "";
                if (ShowDissmised.IsChecked != true)
                    showDissmised = "WHERE Staff.Active = 'True'";

                // Фильтр отображения зп (средняя или максимальная)
                string filter = (CashFilter.Text == "Средняя за месяц" ? "AVG" : "MAX");

                // Запрос выборки к БД
                string sql = $"SELECT Staff.WorkerName, {filter}(Paymants.Salary)\n" +
                            "FROM Staff\n" +
                            "LEFT OUTER JOIN Paymants ON Staff.WorkerID = Paymants.WorkerID\n" +
                            $"{showDissmised}\n" +
                            "GROUP BY Staff.WorkerName, MONTH(Paymants.CashDate)\n" +
                            $"ORDER BY  {filter}(Paymants.Salary) DESC";
                SqlCommand myCommand = new SqlCommand(sql, connection);

                // Получить объект чтения данных с помощью ExecuteReader()
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    ResultList.Items.Clear();

                    while (myDataReader.Read())
                    {
                        // Получить имя сотрутника
                        string name = myDataReader.GetValue(0).ToString();

                        // Проверка наличия выплат зп в текущем месяце
                        // если NULL, то 0
                        // иначе перевести в double и округлить до 2 знаков после запятой
                        double salary = myDataReader.GetValue(1) == DBNull.Value ? 0 : Math.Round(Convert.ToDouble(myDataReader.GetValue(1)),2);
                        
                        ResultList.Items.Add(new Worker(name, salary));

                        if (salary < 20000)
                            Highlighting((Worker)ResultList.Items[ResultList.Items.Count-1], "#FFF68080");

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
            // Очистить список выбранных элементов
            ResultList.SelectedItems.Clear();
            // Сбросить выделения
            ResultList.Items.Refresh();
            
            if (findWorkerInput.Text != "Введите имя сотрудника")
                foreach (var item in ResultList.Items)
                {
                    if (((Worker)item).Salary < 20000) Highlighting((Worker)item, "#FFF68080");

                    if (((Worker)item).Name.ToLower().Contains(findWorkerInput.Text.ToLower()))
                    {
                        ResultList.SelectedItems.Add(item);
                        Highlighting((Worker)item, "#FF99B4D1");
                    }
                }

        }


        /// <summary>
        /// Выделение элемента списка
        /// </summary>
        /// <param name="worker">Элемент ListView, который надо выделить</param>
        /// <param name="color">Цвет выделения</param>
        void Highlighting(Worker worker, string color)
        {
            ResultList.UpdateLayout(); // Обновление визуальных элементов

            // Получение контейнера элемента, который нужно выделить
            var itemContainer = ResultList.ItemContainerGenerator.ContainerFromItem(worker) as ListViewItem;
            if (itemContainer != null)
            {
                var brush = new BrushConverter();
                // Выделение 
                itemContainer.Background = (Brush)brush.ConvertFrom(color);
            }
        }

        private void findWorkerInput_Focus(object sender, RoutedEventArgs e)
        {
            if (findWorkerInput.Text == "Добавить сотрудника") findWorkerInput.Text = "";
            findWorkerInput.Foreground = Brushes.Black;
        }

        private void findWorkerInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (findWorkerInput.Text == "") findWorkerInput.Text = "Добавить сотрудника";
            findWorkerInput.Foreground = Brushes.Gray;
        }


        private void ShowAllStaff()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = cnStringBuilder.ConnectionString;

                connection.Open();

                // Запрос выборки к БД
                string sql = $"SELECT * FROM Staff";
                SqlCommand myCommand = new SqlCommand(sql, connection);

                // Получить объект чтения данных с помощью ExecuteReader()
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        // Получить имя сотрутника
                        int id = Convert.ToInt32(myDataReader.GetValue(0).ToString());
                        string name = myDataReader.GetValue(1).ToString();
                        bool active = myDataReader.GetValue(2).ToString() == "True" ? true : false;

                        
                        StaffList.Items.Add(new Staff(id, name, active));
                    }

                }

                connection.Close();
            }

        }
    }
}
