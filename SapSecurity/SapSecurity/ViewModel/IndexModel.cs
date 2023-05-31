using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapSecurity.ViewModel
{
    public class IndexModel
    {
        public IndexModel(int sensorId, int zoneId, int groupId, string userId, int indexValue, DateTime createDate)
        {
            SensorId = sensorId;
            ZoneId = zoneId;
            GroupId = groupId;
            UserId = userId;
            IndexValue = indexValue;
            CreateDate = createDate;
        }

        public int SensorId { get; set; }
        public int ZoneId { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public int IndexValue { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
