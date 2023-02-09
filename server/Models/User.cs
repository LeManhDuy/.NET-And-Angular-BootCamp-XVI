using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
  public class User
  {
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(256)]
    public string Username { get; set; }
    public byte[] PasswordHashed { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; }
  }
}