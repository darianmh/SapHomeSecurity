using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapSecurity.Model
{
    /// <summary>
    /// sensor groups like gas or IR ...
    /// </summary>
    public class SensorGroup : BaseEntity
    {
        /// <summary>
        /// Name of sensor
        /// </summary>
        public string Title { get; set; }

        public string ImagePath { get; set; }
        public string? ImagePathDanger { get; set; }
        public string? SafeImagePath { get; set; }
        /// <summary>
        /// define sensor is digital or analog
        /// </summary>
        public bool IsDigital { get; set; }
        /// <summary>
        /// value when sensor is working Properly
        /// </summary>
        public int? NeutralValue { get; set; }
        /// <summary>
        /// type of group
        /// </summary>
        public SensorGroupType SensorGroupType { get; set; }

        public int Weight { get; set; }
        /// <summary>
        /// weight affect per second
        /// if 100 means each un neutral value fills weight
        /// if 50 mean each un neutral value fill 500 percent of weight
        /// </summary>
        public int? WeightPercent { get; set; }

        //np
        public IList<SensorDetail> SensorDetails { get; set; }
    }
}
