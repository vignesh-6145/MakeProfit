﻿namespace MakeProfits.Backend.Models.Stock
{
    public class Stock : AbstractStock
    {
        public int SecurityID { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}