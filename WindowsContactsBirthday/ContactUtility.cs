using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsContactsBirthday
{
    using Microsoft.Communications.Contacts;

    /// <summary>
    /// Contact utility.
    /// </summary>
    class ContactUtility
    {

        /// <summary>
        /// Has birthdate.
        /// </summary>
        /// <param name="pContact">Contact</param>
        /// <returns>True</returns>
        public static Boolean hasBirthdate(Contact pContact)
        {
            Boolean result = false;
            // Retrieve birth date (first of in date list)
            if (pContact.Dates.Count > 0)
            {
                try
                {
                    DateTime? birthDate = pContact.Dates[0];
                    result = birthDate.HasValue;
                }
                catch (SchemaException)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Get birth date.
        /// </summary>
        /// <param name="pContact">Contact</param>
        /// <returns>Birth date</returns>
        public static DateTime getBirthdate(Contact pContact)
        {
            // Retrieve birth date (first of in date list)
            DateTime? birthDate = pContact.Dates[0];
            DateTime result = DateTime.MinValue;
            if (birthDate.HasValue)
            {
                result = birthDate.Value;
            }
            return result;
        }

        /// <summary>
        /// Get display name.
        /// </summary>
        /// <param name="pContact">Contact</param>
        /// <returns>Display name</returns>
        public static String getDisplayName(Contact pContact)
        {
            String result = pContact.Names.Default.FormattedName;
            return result;
        }
    }
}
