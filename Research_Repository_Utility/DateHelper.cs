using System;

namespace Research_Repository_Utility
{
    public class DateHelper
    {
        public static bool CheckDateFormat(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckDateRange(string startDate, string endDate)
        {
            try
            {
                if ((DateTime.Parse(endDate) - DateTime.Parse(startDate)).Days > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                } 
            }
            catch
            {
                return false;
            }

        }
    }
}
