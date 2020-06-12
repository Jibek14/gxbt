using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptocurrency.Models
{
  public  class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double ChangePercent { get; set; }
        public double LastPrice { get; set; }
        public DateTime requestTime { get; set; }
    }
}
