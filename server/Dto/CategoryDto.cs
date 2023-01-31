using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Dto
{
  public class CategoryDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Hidden { get; set; }
  }
}