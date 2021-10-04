using System;

namespace Research_Repository_Utility
{
    public class DateHelper
    {
        public static bool CheckDate(String date)

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

    }
}
