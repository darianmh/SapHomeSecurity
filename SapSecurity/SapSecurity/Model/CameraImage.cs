using System.ComponentModel.DataAnnotations.Schema;

namespace SapSecurity.Model;

public class CameraImage : BaseEntity
{
    public string Path { get; set; }
    public DateTime DateTimeUtc { get; set; }
    public string UserId { get; set; }

    //np
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}