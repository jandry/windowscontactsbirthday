using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsContactsBirthday
{
    using Microsoft.Communications.Contacts;

    // A delegate type for hooking up change notifications.
    public delegate void RefreshContactListCompletedHandler(object sender, EventArgs e);

    /// <summary>
    /// Contact utility.
    /// </summary>
    class ContactUtility
    {
        /// <summary>
        /// Refrash contact list completed event.
        /// </summary>
        public static event RefreshContactListCompletedHandler RefreshContactListCompleted;

        /// <summary>
        /// Contact manager.
        /// </summary>
        private static ContactManager _contactManager = new ContactManager();

        /// <summary>
        /// List of contact with birthday.
        /// </summary>
        private static List<ContactBirthday> _contactWithBirthdateList = new List<ContactBirthday>();

        /// <summary>
        /// List of contact without a birthday.
        /// </summary>
        private static List<Contact> _contactWithoutBirthdateList = new List<Contact>();

        /// <summary>
        /// List of contact without a birthday today.
        /// </summary>
        private static List<ContactBirthday> _todayBirthdayList = new List<ContactBirthday>();

        /// <summary>
        /// List of contact without a birthday within the week.
        /// </summary>
        private static List<ContactBirthday> _weekBirthdayList = new List<ContactBirthday>();

        /// <summary>
        /// Get contact manager.
        /// </summary>
        /// <returns>Contact manager</returns>
        private static ContactManager getContactManager()
        {
            if (_contactManager == null)
            {
                _contactManager = new ContactManager();
                _contactManager.CollectionChanged += (sender, e) => RefreshContactList();
            }
            return _contactManager;
        }

        /// <summary>
        /// Refresh contact list.
        /// </summary>
        /// <returns></returns>
        public static object RefreshContactList()
        {
            _contactWithBirthdateList = null;
            _contactWithBirthdateList = new List<ContactBirthday>();
            _contactWithoutBirthdateList = null;
            _contactWithoutBirthdateList = new List<Contact>();
            _todayBirthdayList = null;
            _todayBirthdayList = new List<ContactBirthday>();
            _weekBirthdayList = null;
            _weekBirthdayList = new List<ContactBirthday>();

            foreach (Contact contact in getContactManager().GetContactCollection())
            {
                ContactBirthday tmp = ContactUtility.createContactBirthday(contact);
                if (tmp == null)
                {
                    _contactWithoutBirthdateList.Add(contact);
                }
                else
                {
                    _contactWithBirthdateList.Add(tmp);                     
                }
            }
            if (RefreshContactListCompleted != null)
                RefreshContactListCompleted(Console.Out, EventArgs.Empty);
            System.Console.Out.WriteLine("Refresh contact finished.");
            return null;
        }

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
                catch (SchemaException e)
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

        /// <summary>
        /// Create a contact birthday. Return null if no birthdate present.
        /// </summary>
        /// <param name="pContact">Contact</param>
        /// <returns>ContactBirthday for contact</returns>
        public static ContactBirthday createContactBirthday(Contact pContact)
        {
            ContactBirthday result = null;
            if (hasBirthdate(pContact))
            {
                result = new ContactBirthday(pContact);
                if (0==result.dayLeft)
                {
                    _todayBirthdayList.Add(result);
                }
                if ((7 > result.dayLeft) && (0 < result.dayLeft))
                {
                    _weekBirthdayList.Add(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Get list of today birthdays.
        /// </summary>
        /// <returns>List of today birthdays</returns>
        public static List<ContactBirthday> getTodayBirthdayList()
        {
            return _todayBirthdayList;
        }

        /// <summary>
        /// Get list of week birthdays.
        /// </summary>
        /// <returns>List of week birthdays</returns>
        public static List<ContactBirthday> getWeekBirthdayList()
        {
            return _weekBirthdayList;
        }
        
    }
}
