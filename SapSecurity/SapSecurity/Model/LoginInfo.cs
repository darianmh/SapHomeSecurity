using System.ComponentModel.DataAnnotations.Schema;

namespace SapSecurity.Model;

public class LoginInfo : BaseEntity
{
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string UserId { get; set; }

    //np
    [ForeignKey("UserId")]
    public virtual ApplicationUser ApplicationUser { get; set; }

}