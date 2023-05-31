namespace SapSecurity.Model;

public class ApplicationUser : BaseEntity<string>
{
    public bool SecurityIsActive { get; set; }
    public string Address { get; set; }
    public double Lan { get; set; }
    public double Lat { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }

    //np
    public virtual IList<LoginInfo> LoginInfos { get; set; }
    public virtual IList<CameraImage> CameraImages { get; set; }
}