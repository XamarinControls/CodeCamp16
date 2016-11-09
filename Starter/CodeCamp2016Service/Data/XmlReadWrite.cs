using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace CodeCamp2016Service.Data
{
    public static class XmlReadWrite
    {
        static ReaderWriterLock rwl = new ReaderWriterLock();
        private static readonly string XMLPath;

        static XmlReadWrite()
        {
            var folder = HostingEnvironment.MapPath("~/App_Data");
            XMLPath = Path.Combine(folder, "ApiData.xml");
        }

        public static bool FindUser(string username, string password)
        {
            try
            {
                rwl.AcquireReaderLock(TimeSpan.FromSeconds(2).Milliseconds);
                try
                {
                    return (from x in GetXMl().Descendants("user")
                                where x.Value != null && x.Value == username && x.Attribute("password").Value == password
                                select x).FirstOrDefault() != null;
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch(ApplicationException)
            {
                // reader timeout
                return false;
            }
        }

        public static bool FindUser(string username)
        {
            try
            {
                rwl.AcquireReaderLock(TimeSpan.FromSeconds(2).Milliseconds);
                try
                {
                    return (from x in GetXMl().Descendants()
                            where x.Element("user") != null && x.Element("user").Value == username
                            select x).FirstOrDefault() != null;
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // reader timeout                
            }
            return false;
        }

        public static bool Register(string username, string password)
        {
            try
            {
                rwl.AcquireWriterLock(TimeSpan.FromSeconds(2).Milliseconds);
                try
                {
                    var doc = GetXMl();

                    if (FindUser(username) == false)
                    {
                        doc.Element("users").Add(new XElement("user",
                                                    new XAttribute("password", password), username));
                        doc.Save(XMLPath);
                        return true;
                    }
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // reader timeout
                return false;
            }
            return false;
        }

        private static XDocument GetXMl()
        {
            var doc = XDocument.Load(XMLPath);
            return doc;
        }
    }
}