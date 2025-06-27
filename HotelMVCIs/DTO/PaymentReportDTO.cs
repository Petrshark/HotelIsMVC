using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.DTOs
{
    public class PaymentReportDTO
    {
        [DataType(DataType.Date)]
        [Display(Name = "Datum od")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Datum do")]
        public DateTime EndDate { get; set; }

        public List<ReportEntryDTO> Entries { get; set; } = new List<ReportEntryDTO>();

        [Display(Name = "Celkem za období")]
        public decimal GrandTotal { get; set; }
    }
}