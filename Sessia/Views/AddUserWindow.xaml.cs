using Sessia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        SqlConnection con = Services.AppContext.GetConnection();

        private int _maxId = 0;

        private ObservableCollection<Office> _OfficesList = new ObservableCollection<Office>();

        public AddUserWindow(int maxId)
        {
            this._maxId = maxId;

            con.Open();

            InitializeComponent();

            InitializeLists();
        }

        private void InitializeLists()
        {
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

            OfficeCB.ItemsSource = _OfficesList;
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            bool emailIsValid = IsValidEmail(EmailTB.Text.Trim());

            if(EmailTB.Text == null || EmailTB.Text.Trim().Length < 1 || !emailIsValid)
            {
                MessageBox.Show("Введите корректный Email.");

                return;
            }

            if(FirstNameTB.Text == null || FirstNameTB.Text.Trim().Length < 1)
            {
                MessageBox.Show("Введите корректное имя.");

                return;
            }

            if(LastNameTB.Text == null || LastNameTB.Text.Trim().Length < 1)
            {
                MessageBox.Show("Введите корректную фамилию.");

                return;
            }

            if(BirthdateDP.SelectedDate == null || BirthdateDP.SelectedDate >= DateTime.Now.Date)
            {
                MessageBox.Show("Выберите корректную дату.");

                return;
            } 

            if(PasswordTB.Text == null || PasswordTB.Text.Trim().Length < 3)
            {
                MessageBox.Show("Выберите корректный пароль. От 3-х символов.");

                return;
            }

            Office selOffice = new Office();
            selOffice = (Office)OfficeCB.SelectedValue;

            try
            {
                _maxId++;

                string queryUser = "INSERT INTO Users (ID, RoleID, Email, Password, FirstName, LastName, OfficeID, Birthdate, Active) VALUES (@ID, @RoleID, @Email, @Password, @FirstName, @LastName, @OfficeID, @Birthdate, @Active)";
                using(var cmdUser = new SqlCommand(queryUser, con))
                {
                    cmdUser.CommandType = CommandType.Text;

                    cmdUser.Parameters.Add(new SqlParameter("@ID", _maxId));
                    cmdUser.Parameters.Add(new SqlParameter("@RoleID", 2));
                    cmdUser.Parameters.Add(new SqlParameter("@Email", EmailTB.Text));
                    cmdUser.Parameters.Add(new SqlParameter("@Password", PasswordTB.Text));
                    cmdUser.Parameters.Add(new SqlParameter("@FirstName", FirstNameTB.Text));
                    cmdUser.Parameters.Add(new SqlParameter("@LastName", LastNameTB.Text));
                    cmdUser.Parameters.Add(new SqlParameter("@OfficeID", selOffice.Id));
                    cmdUser.Parameters.Add(new SqlParameter("@Birthdate", BirthdateDP.SelectedDate));
                    cmdUser.Parameters.Add(new SqlParameter("@Active", 1));

                    cmdUser.ExecuteScalar();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

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

        private void CancelAdding_Click(object sender, RoutedEventArgs e)
        {
            con.Dispose();
            con.Close();

            this.Close();
        }
    }
}
