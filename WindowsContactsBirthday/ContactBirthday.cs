using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsContactsBirthday
{

    using Microsoft.Communications.Contacts;

    /// <summary>
    /// Contact with a birthday.
    /// </summary>
    class ContactBirthday
    {
        /// <summary>
        /// Internal contact.
        /// </summary>
        public Contact contact { get; set; }

        /// <summary>
        /// Birthdate.
        /// </summary>
        public DateTime birthDate { get; set; }

        /// <summary>
        /// Day left before birthday.
        /// </summary>
        public int dayLeft { get; set; }

        /// <summary>
        /// Age.
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// Display name.
        /// </summary>
        public String displayName { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pContact">Contact</param>
        public ContactBirthday(Contact pContact)
        {
            contact = pContact;
            birthDate = ContactUtility.getBirthdate(pContact);
            birthDate = new DateTime(birthDate.Year, birthDate.Month, birthDate.Day);
            age = DateTime.Now.Year - birthDate.Year;
            displayName = ContactUtility.getDisplayName(pContact);
            // Compute day left
            DateTime tmp = new DateTime(DateTime.Now.Year, birthDate.Month, birthDate.Day);
            TimeSpan tmpSpan = tmp.Subtract(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            dayLeft = tmpSpan.Days;
        }
    }
}
