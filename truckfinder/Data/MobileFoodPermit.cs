using System;
using CsvHelper.Configuration.Attributes;

namespace TruckFinder.Data
{
    public class MobileFoodPermit
    {
        [Name("Applicant")]
        public string Applicant { get; set; }

        [Name("FacilityType")]
        public string FacilityType { get; set; }

        [Name("Address")]
        public string Address { get; set; }

        [Name("permit")]
        public string Permit { get; set; }

        [Name("Status")]
        public string Status { get; set; }

        [Name("PriorPermit")]
        [BooleanTrueValues("1")]
        [BooleanFalseValues("0")]
        public bool PriorPermit { get; set; }

        [Name("FoodItems")]
        public string FoodItems { get; set; }

        [Name("Latitude")]
        public decimal Latitude { get; set; }

        [Name("Longitude")]
        public decimal Longitude { get; set; }

        [Name("dayshours")]
        public string DaysHours { get; set; }

        [Name("Approved")]
        [Optional]
        public DateTime? Approved { get; set; }

        [Name("ExpirationDate")]
        [Optional]
        public DateTime? ExpirationDate { get; set; }
    }
}