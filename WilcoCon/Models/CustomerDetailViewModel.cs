using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WilcoCon.Models
{
    public class CustomerDetailViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}