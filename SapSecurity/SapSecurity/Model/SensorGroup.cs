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
        public double? NeutralValue { get; set; }
        /// <summary>
        /// define sensor is critical
        /// </summary>
        public bool IsCritical { get; set; }
        /// <summary>
        /// define sensor is restricted in zone
        /// </summary>
        public bool IsZoneRestricted { get; set; }

        public int Weight { get; set; }
        //np
        public IList<SensorDetail> SensorDetails { get; set; }
    }
}
