using Sessia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Sessia.Views
{
    /// <summary>
    /// Interaction logic for AdmMenu.xaml
    /// </summary>
    public partial class AdmMenu : Window
    {
        SqlConnection con = Services.AppContext.GetConnection();
        
        private int _ID = 0;

        private ObservableCollection<User> _UsersList = new ObservableCollection<User>();
        private ObservableCollection<Role> _RolesList = new ObservableCollection<Role>();
        private ObservableCollection<Office> _OfficesList = new ObservableCollection<Office>();

        public AdmMenu(int ID)
        {
            this._ID = ID;

            con.Open();

            InitializeComponent();

            InitializeLists();
        }

        private void InitializeLists()
        {
            Office def = new Office();
            def.Title = "All offices";
            _OfficesList.Add(def);

            // Получаем из БД список ролей
            try
            {
                string query = "SELECT * FROM Roles";
                using(var cmd = new SqlCommand(query, con))
                {
                    using(var rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            Role role = new Role();

                            role.Id = rdr.GetInt32(0);
                            role.Title = rdr.GetString(1);

                            _RolesList.Add(role);
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            // Получаем из БД список офисов
            try
            {
                string query = "SELECT * FROM Offices";
                using(var cmd = new SqlCommand(query, con))
                {
                    using(var rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            Office office = new Office();

                            office.Id = rdr.GetInt32(0);
                            office.CountryId = rdr.GetInt32(1);
                            office.Title = rdr.GetString(2);
                            office.Phone = rdr.GetString(3);
                            office.Contact = rdr.GetString(4);

                            _OfficesList.Add(office);
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            OfficesListCB.ItemsSource = _OfficesList;
        }

        private void OfficesListCB_SelectedChanged(object sender, EventArgs e)
        {
            ReadUsersByFilter();
        }

        public void ReadAllUsers()
        {
            _UsersList.Clear();
            UsersListDG.ItemsSource = null;
            UsersListDG.Items.Clear();
            UsersListDG.Items.Refresh();

            try
            {
                string query = "SELECT * FROM Users WHERE ID != '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    using(var rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            User user = new User();

                            user.Id = rdr.GetInt32(0);
                            user.RoleId = rdr.GetInt32(1);
                            user.Email = rdr.GetString(2);
                            user.Password = rdr.GetString(3);
                            user.FirstName = rdr.GetString(4);
                            user.LastName = rdr.GetString(5);
                            user.OfficeId = rdr.GetInt32(6);
                            user.Birthdate = rdr.GetDateTime(7);
                            user.Active = rdr.GetBoolean(8);

                            _UsersList.Add(user);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            // Обновляем инфу в списке юзеров (добавляем название роли, название офиса, возраст)
            foreach(var user in _UsersList)
            {
                foreach(var role in _RolesList)
                {
                    if(user.RoleId == role.Id)
                    {
                        user.RoleName = role.Title;

                        break;
                    }
                }

                foreach(var office in _OfficesList)
                {
                    if(user.OfficeId == office.Id)
                    {
                        user.OfficeName = office.Title;

                        break;
                    }
                }

                var today = DateTime.Today;
                user.Age = today.Year - user.Birthdate.Year;
            }

            UsersListDG.ItemsSource = _UsersList;
        }

        private void ReadUsersByFilter()
        {
            _UsersList.Clear();
            UsersListDG.ItemsSource = null;
            UsersListDG.Items.Clear();
            UsersListDG.Items.Refresh();

            if (OfficesListCB.SelectedIndex > 0)
            {
                Office selectedOffice = (Office)OfficesListCB.SelectedValue;

                try
                {
                    string query = "SELECT * FROM Users WHERE ID != '" + this._ID + "' AND OfficeID = '" + selectedOffice.Id + "'";
                    using(var cmd = new SqlCommand(query, con))
                    {
                        using(var rdr = cmd.ExecuteReader())
                        {
                            while(rdr.Read())
                            {
                                User user = new User();

                                user.Id = rdr.GetInt32(0);
                                user.RoleId = rdr.GetInt32(1);
                                user.Email = rdr.GetString(2);
                                user.Password = rdr.GetString(3);
                                user.FirstName = rdr.GetString(4);
                                user.LastName = rdr.GetString(5);
                                user.OfficeId = rdr.GetInt32(6);
                                user.Birthdate = rdr.GetDateTime(7);
                                user.Active = rdr.GetBoolean(8);

                                _UsersList.Add(user);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                // Обновляем инфу в списке юзеров (добавляем название роли, название офиса, возраст)
                foreach(var user in _UsersList)
                {
                    foreach(var role in _RolesList)
                    {
                        if(user.RoleId == role.Id)
                        {
                            user.RoleName = role.Title;

                            break;
                        }
                    }

                    foreach(var office in _OfficesList)
                    {
                        if(user.OfficeId == office.Id)
                        {
                            user.OfficeName = office.Title;

                            break;
                        }
                    }

                    var today = DateTime.Today;
                    user.Age = today.Year - user.Birthdate.Year;
                }

                UsersListDG.ItemsSource = _UsersList;
            }
            else
            {
                ReadAllUsers();
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow(_UsersList.Last().Id);

            addUserWindow.Owner = this;
            addUserWindow.ShowDialog();
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            con.Dispose();
            con.Close();
            this.Close();
        }

        private void ChangeUserRole_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = new User();
            selectedUser = (User)UsersListDG.SelectedValue;

            if(selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.");

                return;
            }

            EditRoleWindow editRoleWindow = new EditRoleWindow(selectedUser.Id);

            editRoleWindow.Owner = this;
            editRoleWindow.ShowDialog();
        }

        private void DisableOrEnableUser_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = new User();
            selectedUser = (User)UsersListDG.SelectedValue;

            bool isActive = false;

            if (selectedUser != null)
            {
                if (!selectedUser.Active)
                    isActive = true;

                string query = "UPDATE Users SET Active = '" + isActive + "' WHERE ID = '" + selectedUser.Id + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Данные пользователя успешно изменены.");

                ReadAllUsers();

                return;
            }

            MessageBox.Show("Выберите пользователя.");
        }
    }
}
