using System;
using System.Linq;
using System.Data;

namespace SweetTreats
{
    class Program
    {
        static void Main(string[] args)
        {
            DataSet courierDS = CreateDataSet();  //Get Dataset with tables of all 3 Courier Services

            TimeSpan timeOfService = new TimeSpan(0, 0, 0);
            float distance = 0;
            bool refridgerationRequired = false;

            // Get user inputs and validation done on those inputs 
            bool isInputValid = GetUserInputAndValidate(courierDS, ref timeOfService, ref distance, ref refridgerationRequired);

            DataRow opRow = null;

            if (isInputValid) // Get cheapest Courier Service if all inputs are valid
                opRow = GetCheapestCourier(courierDS, timeOfService, distance, refridgerationRequired);

            if (opRow != null)
            {
                Console.WriteLine("Cheapiest Courier Service is : {0}", opRow.ItemArray[1].ToString());
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No courier service is available with given criteria");
                Console.ReadLine();
            }
        }

        // Method creates a Dataset with 3 given Courier Services
        public static DataSet CreateDataSet()
        {
            DataSet courierDS = new DataSet();
            DataTable courierTable = courierDS.Tables.Add("Courier");
            DataColumn workCol = courierTable.Columns.Add("CourierID", typeof(int));
            workCol.AllowDBNull = false;
            workCol.Unique = true;
            courierTable.Columns.Add("CourierName", typeof(String));
            courierTable.Columns.Add("StartTime", typeof(TimeSpan));
            courierTable.Columns.Add("EndTime", typeof(TimeSpan));
            courierTable.Columns.Add("Miles", typeof(int));
            courierTable.Columns.Add("Price", typeof(Double));
            courierTable.Columns.Add("IsRefrigirated", typeof(bool));

            courierTable.Rows.Add(1, "Bobby", new TimeSpan(9, 0, 0), new TimeSpan(13, 0, 0), 5, 1.75, true);
            courierTable.Rows.Add(2, "Martin", new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 3, 1.50, false);
            courierTable.Rows.Add(3, "Geoff", new TimeSpan(10, 0, 0), new TimeSpan(16, 0, 0), 4, 2.00, true);

            return courierDS;
        }

        // Method validates user inputs, return true/false based on validity 
        // and sets those inputs in reference variables
        public static bool GetUserInputAndValidate(DataSet courierDS, ref TimeSpan timeOfService, ref float distance, ref bool refridgerationRequired)
        {
            bool isValueValid = false;
            Console.WriteLine("Please enter the time in Format (hh:mm) : ");
            string strTime = Console.ReadLine();

            while (!isValueValid)
            {
                isValueValid = TimeSpan.TryParse(strTime, out timeOfService);
                if (!isValueValid)
                {
                    Console.WriteLine("Please enter the time in Format (hh:mm) : ");
                    strTime = Console.ReadLine();
                }
            }

            isValueValid = false;
            Console.WriteLine("Please enter the distance: ");
            string strDistance = Console.ReadLine();

            while (!isValueValid)
            {
                isValueValid = float.TryParse(strDistance, out distance);
                if (!isValueValid)
                {
                    Console.WriteLine("Please enter a numeric value: ");
                    strDistance = Console.ReadLine();
                }
            }

            isValueValid = false;
            Console.WriteLine("Please enter if refrigeration required (y/n): ");
            string strRefridgerationReq = Console.ReadLine();

            while (!isValueValid)
            {
                if (!(strRefridgerationReq.ToUpper() == "Y" || strRefridgerationReq.ToUpper() == "N"))
                {
                    Console.WriteLine("Please enter y/n: ");
                    strRefridgerationReq = Console.ReadLine();
                    isValueValid = false;
                }
                else
                {
                    isValueValid = true;
                    if ((strRefridgerationReq.ToUpper() == "Y"))
                        refridgerationRequired = true;
                    else
                        refridgerationRequired = false;
                }
            }

            if (isValueValid)
                return true;
            else
                return false;
        }

        // Method returns the cheapest Courier based on the user inputs using a LINQ query
        public static DataRow GetCheapestCourier(DataSet CourierDS, TimeSpan timeOfService, float distance, bool isRefrigerationRequired)
        {
            var result = CourierDS.Tables["Courier"].AsEnumerable()
                .Where(resultRow => resultRow.Field<int>("Miles") >= distance
                && resultRow.Field<bool>("IsRefrigirated") == isRefrigerationRequired
                && resultRow.Field<TimeSpan>("StartTime") <= timeOfService
                && resultRow.Field<TimeSpan>("EndTime") >= timeOfService)
                .OrderBy(resultRow => resultRow.Field<double>("Price")).FirstOrDefault();

            return result;
        }
    }
}
