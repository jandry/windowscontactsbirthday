using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsContactsBirthday
{
    using System.Xml;
    using Microsoft.Communications.Contacts;

    /// <summary>
    /// Refresh contact list completed handler.
    /// </summary>
    public delegate void RefreshContactListCompletedHandler();

    /// <summary>
    /// Contact controler.
    /// </summary>
    class ContactControler
    {

        /// <summary>
        /// Contact namespace.
        /// </summary>
        public const String contactNamespace = "http://schemas.microsoft.com/Contact";

        /// <summary>
        /// c prefix.
        /// </summary>
        public const String contactPrefix = "c";

        /// <summary>
        /// Tag labelCollection.
        /// </summary>
        public const String tagLabelCollection = "LabelCollection";

        /// <summary>
        /// Tag label.
        /// </summary>
        public const String tagLabel = "Label";

        /// <summary>
        /// Birthday flag label.
        /// </summary>
        public const String birthdayFlagLabel = "wab:Birthday";

        /// <summary>
        /// Birthdate select query.
        /// </summary>
        public const String birthdaySelectQuery = "{0}:DateCollection/{0}:Date";

        /// <summary>
        /// Refresh contact list completed event.
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
                ContactBirthday tmp = ContactControler.createContactBirthday(contact);
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
                RefreshContactListCompleted();
            System.Console.Out.WriteLine("Refresh contact finished.");
            return null;
        }

        /// <summary>
        /// Create a contact birthday. Return null if no birthdate present.
        /// </summary>
        /// <param name="pContact">Contact</param>
        /// <returns>ContactBirthday for contact</returns>
        public static ContactBirthday createContactBirthday(Contact pContact)
        {
            ContactBirthday result = null;
            if (ContactUtility.hasBirthdate(pContact))
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

        /// <summary>
        /// Get list of all birthdays.
        /// </summary>
        /// <returns>List of all birthdays</returns>
        public static List<ContactBirthday> getBirthdayList()
        {
            return _contactWithBirthdateList;
        }

        /// <summary>
        /// Import note into birthdate.
        /// </summary>
        public static void importNoteToBirthdate()
        {
            foreach (Contact contact in getContactManager().GetContactCollection())
            {
                String note = contact.Notes;
                if (!ContactUtility.hasBirthdate(contact) && note != null && !String.Empty.Equals(note))
                {
                    try
                    {

                        // Add date
                        DateTime date = DateTime.Parse(note);
                        contact.Dates.Add(date);
                        // Empty note
                        //contact.Notes = String.Empty;
                        // Commit changes                       
                        contact.CommitChanges();
                        // Open contact file to mark date as birthdate
                        XmlDocument doc = new XmlDocument();
                        doc.Load(contact.Path);
                        // Create flag birthday
                        XmlNode nodeLabelCollection = doc.CreateNode(XmlNodeType.Element, contactPrefix, tagLabelCollection, contactNamespace);
                        XmlNode nodeLabel = doc.CreateNode(XmlNodeType.Element, contactPrefix, tagLabel, contactNamespace);
                        nodeLabel.InnerText = birthdayFlagLabel;
                        nodeLabelCollection.AppendChild(nodeLabel);
                        // Recherche le tag Date
                        XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
                        mgr.AddNamespace(contactPrefix, contactNamespace);
                        String select = String.Format(birthdaySelectQuery, contactPrefix, tagLabelCollection, tagLabel);
                        XmlNodeList list = doc.DocumentElement.SelectNodes(select, mgr);
                        if (list.Count == 1)
                        {
                            list.Item(0).AppendChild(nodeLabelCollection);
                            doc.Save(contact.Path);
                        }
                    }
                    catch (FormatException)
                    {
                        // Nothing to do, note is not a date
                    }
                }
            }
            System.Console.Out.WriteLine("Import note to birthdate finished.");
        }
    }
}
