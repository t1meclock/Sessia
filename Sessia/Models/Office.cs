using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sessia.Models
{
    public class Office
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public string Title { get; set; }

        public string Phone { get; set; }

        public string Contact { get; set; }
    }
}
