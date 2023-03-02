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

        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(@"Server = DESKTOP-C8L5Q9N; Trusted_Connection = Yes; DataBase = Session11; User = Ksushia; PWD = 123456seventeen;");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return null;
            }
        }
    }
}
