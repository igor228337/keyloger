using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KeyLoger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {
            List<UserData> list = new List<UserData>();
            SQLiteManager sQLiteManager = new SQLiteManager($"./db.db");
            var data = sQLiteManager.ExecuteCustomQuery("SELECT word, password FROM Keywords");
            foreach (Dictionary<string, object> item in data)
            {
                string login = item["word"] as string;
                string password = item["password"] as string;

                list.Add(new UserData { Login = login, Password = password });
            }
            MainGrid.ItemsSource = list;
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CopyBTN_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem is UserData selectedUser)
            {
                Clipboard.SetText(selectedUser.Password);

                MessageBox.Show($"Значение '{selectedUser.Password}' скопировано в буфер обмена.");
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку.");
            }
        }

        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = MainGrid.SelectedItem as UserData;
            if (selectedUser != null)
            {
                var sQLiteManager = new SQLiteManager("./db.db");
                sQLiteManager.DeleteItem("Keywords", "word", selectedUser.Login);

                // Удаление выбранного элемента из списка
                var list = MainGrid.ItemsSource as List<UserData>;
                if (list != null)
                {
                    list.Remove(selectedUser);
                    MainGrid.ItemsSource = null;
                    MainGrid.ItemsSource = list;

                    MessageBox.Show("Выбранная строка и соответствующая запись в базе данных успешно удалены.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.");
            }
        }
    }
}
