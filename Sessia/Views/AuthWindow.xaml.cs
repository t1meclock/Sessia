using Sessia.DataSet1TableAdapters;
using Sessia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sessia.Views
{
    public partial class AuthWindow : Window
    {
        SqlConnection con = Services.AppContext.GetConnection();

        private int _ID = 0, _roleID = 0;

        public AuthWindow()
        {
            con.Open();

            if(con != null)
            {
                MessageBox.Show("Подключено.");
            }

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int tableIsEmpty = 0;

            try
            {
                string query = "SELECT COUNT(*) FROM Users";

                using(var cmd = new SqlCommand(query, con))
                {
                    tableIsEmpty = int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if(tableIsEmpty > 0)
            {
                return;
            }

            ObservableCollection<Role> roleList = new ObservableCollection<Role>();
            ObservableCollection<Office> officeList = new ObservableCollection<Office>();

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

                            roleList.Add(role);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

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

                            officeList.Add(office);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            int id = 1;

            using(var streamReader = new StreamReader(@"..\UserData.csv"))
            {
                while(!streamReader.EndOfStream)
                {
                    string splitLine = streamReader.ReadLine();

                    if(!String.IsNullOrWhiteSpace(splitLine))
                    {
                        User newUser = new User();

                        string[] values = splitLine.Split(',');

                        newUser.Id = id;
                        foreach(var role in roleList)
                        {
                            if(role.Title == values[0])
                            {
                                newUser.RoleId = role.Id;

                                break;
                            }
                        }
                        newUser.Email = values[1];
                        newUser.Password = values[2];
                        newUser.FirstName = values[3];
                        newUser.LastName = values[4];
                        foreach(var office in officeList)
                        {
                            if(office.Title == values[5])
                            {
                                newUser.OfficeId = office.Id;

                                break;
                            }
                        }

                        string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy" };
                        newUser.Birthdate = DateTime.ParseExact(values[6], formats, new CultureInfo("en-US"),
                            DateTimeStyles.None);
                        newUser.Active = Convert.ToBoolean(Int32.Parse(values[7]));

                        try
                        {
                            string queryUser = "INSERT INTO Users (ID, RoleID, Email, Password, FirstName, LastName, OfficeID, Birthdate, Active) VALUES (@ID, @RoleID, @Email, @Password, @FirstName, @LastName, @OfficeID, @Birthdate, @Active)";
                            using(var cmdUser = new SqlCommand(queryUser, con))
                            {
                                cmdUser.CommandType = CommandType.Text;

                                cmdUser.Parameters.Add(new SqlParameter("@ID", newUser.Id));
                                cmdUser.Parameters.Add(new SqlParameter("@RoleID", newUser.RoleId));
                                cmdUser.Parameters.Add(new SqlParameter("@Email", newUser.Email));
                                cmdUser.Parameters.Add(new SqlParameter("@Password", newUser.Password));
                                cmdUser.Parameters.Add(new SqlParameter("@FirstName", newUser.FirstName));
                                cmdUser.Parameters.Add(new SqlParameter("@LastName", newUser.LastName));
                                cmdUser.Parameters.Add(new SqlParameter("@OfficeID", newUser.OfficeId));
                                cmdUser.Parameters.Add(new SqlParameter("@Birthdate", newUser.Birthdate));
                                cmdUser.Parameters.Add(new SqlParameter("@Active", Convert.ToInt32(newUser.Active)));

                                cmdUser.ExecuteScalar();
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        id++;
                    }
                }
            }
        }

        private void MoveWindow(object sender, RoutedEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            string checkLogin = String.Empty, checkPassword = String.Empty;

            if(LoginTB.Text == null || PassTB.Text == null)
            {
                MessageBox.Show("Неверный логин или пароль.");

                return;
            }
            else
            {
                try
                {
                    string querySignIn = "SELECT * FROM Users WHERE Email = '" + LoginTB.Text + "' AND Password = '" + PassTB.Text + "' AND Active = '" + 1 + "'";
                    using(var cmd = new SqlCommand(querySignIn, con))
                    {
                        cmd.Parameters.AddWithValue("Email", LoginTB.Text);
                        cmd.Parameters.AddWithValue("Password", PassTB.Text);

                        using(var rdr = cmd.ExecuteReader())
                        {
                            if(rdr.Read())
                            {
                                _ID = rdr.GetInt32(0);
                                _roleID = rdr.GetInt32(1);
                                checkLogin = rdr.GetString(2);
                                checkPassword = rdr.GetString(3);
                            }
                        }
                    }

                    if(LoginTB.Text == checkLogin && PassTB.Text == checkPassword)
                    {
                        if(_roleID == 1)
                        {
                            AdmMenu admMenu = new AdmMenu(_ID);

                            admMenu.Show();

                            con.Dispose();
                            con.Close();
                            this.Close();
                        }
                        else if(_roleID == 2)
                        {
                            MainWindow mainWindow = new MainWindow(_ID);

                            mainWindow.Show();

                            con.Dispose();
                            con.Close();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неизвестный пользователь.");

                            LoginTB.Text = "";
                            PassTB.Text = "";

                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль.");

                        LoginTB.Text = "";
                        PassTB.Text = "";

                        return;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            con.Close();
            this.Close();
        }
    }
}