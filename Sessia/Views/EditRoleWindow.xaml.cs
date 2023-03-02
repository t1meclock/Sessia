using Sessia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Sessia.Views
{
    /// <summary>
    /// Interaction logic for EditRoleWindow.xaml
    /// </summary>
    public partial class EditRoleWindow : Window
    {
        SqlConnection con = Services.AppContext.GetConnection();

        private int _ID = 0;

        private ObservableCollection<Role> _RolesList = new ObservableCollection<Role>();
        private ObservableCollection<Office> _OfficesList = new ObservableCollection<Office>();

        private User _userForEdit = new User();

        public EditRoleWindow(int ID)
        {
            this._ID = ID;

            con.Open();

            InitializeComponent();

            InitializeLists();
        }

        private void InitializeLists()
        {
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

            // Считываем текущего выбранного пользователя
            try
            {
                string query = "SELECT * FROM Users WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    using(var rdr = cmd.ExecuteReader())
                    {
                        while(rdr.Read())
                        {
                            _userForEdit.Id = rdr.GetInt32(0);
                            _userForEdit.RoleId = rdr.GetInt32(1);
                            _userForEdit.Email = rdr.GetString(2);
                            _userForEdit.Password = rdr.GetString(3);
                            _userForEdit.FirstName = rdr.GetString(4);
                            _userForEdit.LastName = rdr.GetString(5);
                            _userForEdit.OfficeId = rdr.GetInt32(6);
                            _userForEdit.Birthdate = rdr.GetDateTime(7);
                            _userForEdit.Active = rdr.GetBoolean(8);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            NewRoleCB.ItemsSource = _RolesList;
            NewOfficeCB.ItemsSource = _OfficesList;
        }

        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            // Меняем имейл, если поле не пустое
            if (NewEmailTB.Text != null && NewEmailTB.Text.Trim().Length > 0)
            {
                bool emailIsValid = IsValidEmail(NewEmailTB.Text.Trim());

                if(!emailIsValid)
                {
                    MessageBox.Show("Введите корректный Email.");

                    return;
                }

                string query = "UPDATE Users SET Email = '" + NewEmailTB.Text.Trim() + "' WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Меняем имя ...
            if(NewFirstNameTB.Text != null && NewFirstNameTB.Text.Trim().Length > 0)
            {
                if(NewFirstNameTB.Text.Trim().Contains(' '))
                {
                    MessageBox.Show("Введите корректное имя.");

                    return;
                }

                string query = "UPDATE Users SET FirstName = '" + NewFirstNameTB.Text.Trim() + "' WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Меняем фамилию ...
            if(NewLastNameTB.Text != null && NewLastNameTB.Text.Trim().Length > 0)
            {
                if(NewLastNameTB.Text.Trim().Contains(' '))
                {
                    MessageBox.Show("Введите корректную фамилию.");

                    return;
                }

                string query = "UPDATE Users SET LastName = '" + NewLastNameTB.Text.Trim() + "' WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Меняем офис
            Office selOffice = new Office();
            selOffice = (Office)NewOfficeCB.SelectedValue;

            if(selOffice.Id != _userForEdit.OfficeId)
            {
                string query = "UPDATE Users SET OfficeID = '" + selOffice.Id + "' WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Меняем роль
            Role selRole = new Role();
            selRole = (Role)NewRoleCB.SelectedValue;

            if(selRole.Id != _userForEdit.RoleId)
            {
                string query = "UPDATE Users SET RoleID = '" + selRole.Id + "' WHERE ID = '" + this._ID + "'";
                using(var cmd = new SqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Данные пользователя успешно изменены.");

            ((AdmMenu)this.Owner).ReadAllUsers();

            con.Dispose();
            con.Close();
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            if(new EmailAddressAttribute().IsValid(email))
                return true;

            return false;
        }

        private void CancelChanges_Click(object sender, RoutedEventArgs e)
        {
            con.Dispose();
            con.Close();

            this.Close();
        }
    }
}
