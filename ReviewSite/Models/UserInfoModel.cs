﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ReviewSite.Controllers
{

    public class UserInfoModel
    {

        public string ALL_HTTP { get; set; }
        public string ALL_RAW { get; set; }
        public string APPL_MD_PATH { get; set; }
        public string APPL_PHYSICAL_PATH { get; set; }
        public string AUTH_TYPE { get; set; }
        public string AUTH_USER { get; set; }
        public string AUTH_PASSWORD { get; set; }
        public string LOGON_USER { get; set; }
        public string REMOTE_USER { get; set; }
        public string CERT_COOKIE { get; set; }
        public string CERT_FLAGS { get; set; }
        public string CERT_ISSUER { get; set; }
        public string CERT_KEYSIZE { get; set; }
        public string CERT_SECRETKEYSIZE { get; set; }
        public string CERT_SERIALNUMBER { get; set; }
        public string CERT_SERVER_ISSUER { get; set; }
        public string CERT_SERVER_SUBJECT { get; set; }
        public string CERT_SUBJECT { get; set; }
        public string CONTENT_LENGTH { get; set; }
        public string CONTENT_TYPE { get; set; }
        public string GATEWAY_INTERFACE { get; set; }
        public string HTTPS { get; set; }
        public string HTTPS_KEYSIZE { get; set; }
        public string HTTPS_SECRETKEYSIZE { get; set; }
        public string HTTPS_SERVER_ISSUER { get; set; }
        public string HTTPS_SERVER_SUBJECT { get; set; }
        public string INSTANCE_ID { get; set; }
        public string INSTANCE_META_PATH { get; set; }
        public string LOCAL_ADDR { get; set; }
        public string PATH_INFO { get; set; }
        public string PATH_TRANSLATED { get; set; }
        public string QUERY_STRING { get; set; }
        public string REMOTE_ADDR { get; set; }
        public string REMOTE_HOST { get; set; }
        public string REMOTE_PORT { get; set; }
        public string REQUEST_METHOD { get; set; }
        public string SCRIPT_NAME { get; set; }
        public string SERVER_NAME { get; set; }
        public string SERVER_PORT { get; set; }
        public string SERVER_PORT_SECURE { get; set; }
        public string SERVER_PROTOCOL { get; set; }
        public string SERVER_SOFTWARE { get; set; }
        public string URL { get; set; }
        public string HTTP_CACHE_CONTROL { get; set; }
        public string HTTP_CONNECTION { get; set; }
        public string HTTP_ACCEPT { get; set; }
        public string HTTP_ACCEPT_ENCODING { get; set; }
        public string HTTP_ACCEPT_LANGUAGE { get; set; }
        public string HTTP_HOST { get; set; }
        public string HTTP_USER_AGENT { get; set; }
        public string HTTP_UPGRADE_INSECURE_REQUESTS { get; set; }

        public Dictionary<String, String> userProperties = new Dictionary<string, string>();

        public UserInfoModel()
        {
            Type type = this.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = type.GetProperties(flags);
            foreach (var prop in properties)
            {
                if (prop.Name != "Item")
                    userProperties.Add(prop.Name, "");
            }
        }

        public string this[string index]
        {
            get
            {
                if (userProperties.ContainsKey(index))
                    return userProperties[index];
                else
                    return null;
            }

            set
            {
                if (userProperties.ContainsKey(index))
                    userProperties[index] = value;
            }
        }


    }
}