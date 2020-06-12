using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace Cryptocurrency.Models
{
  public  class CourseContext:DbContext
    {
        public CourseContext() : base("DefaultConnection")
        {
        }
        public DbSet<Course> Course { get; set; }
    }
}
