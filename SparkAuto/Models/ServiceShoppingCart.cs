using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models
{
    public class ServiceShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int CarId { get; set; }
        public int ServiceId { get; set; }

        [ForeignKey("CarId")]
        public  Car Car { get; set; }

        [ForeignKey("ServiceId")]
        public  ServiceType ServiceType { get; set; }
    }
}
