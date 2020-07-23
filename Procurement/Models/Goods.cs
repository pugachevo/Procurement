using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Models
{
    public class Goods
    {
        [Key]
        public int GoodID { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Сost { get; set; }
        
        public int Quantity { get; set; }
       
        public int OrdersID { get; set; }

        public virtual Orders Orders { get; set; }


    }
}