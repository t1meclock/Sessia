using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Sessia.Models;

namespace Sessia.Services
{
    public class AppContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        /*public DataTable Select(string selectSQL) // функция подключения к базе данных и обработка запросов
        {
            DataTable dataTable = new DataTable("dataBase");                // создаём таблицу в приложении
                                                                            // подключаемся к базе данных
            SqlConnection sqlConnection = new SqlConnection("server=DESKTOP-NBH5JF2;Trusted_Connection=Yes;DataBase=TEST;");
            sqlConnection.Open();                                           // открываем базу данных
            SqlCommand sqlCommand = sqlConnection.CreateCommand();          // создаём команду
            sqlCommand.CommandText = selectSQL;                             // присваиваем команде текст
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // создаём обработчик
            sqlDataAdapter.Fill(dataTable);                                 // возращаем таблицу с результатом
            
            return dataTable;
        }*/

        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(@"Server = DESKTOP-P5OEDRP; Trusted_Connection = Yes; DataBase = Session1; User = Eternal_Silence; PWD = 4855294a;");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return null;
            }
        }

        /*public static NpgsqlConnection GetConnection()
        {
            try
            {
                return new NpgsqlConnection(@"Server = localhost; Port = 5432; User Id = postgres; Password = 123; Database = Lets_Meet_DB;");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return null;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server = localhost; Port = 5432; User Id = postgres; Password = 123; Database = Lets_Meet_DB;");
        }*/
    }
}
