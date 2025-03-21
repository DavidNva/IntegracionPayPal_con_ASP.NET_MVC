﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace paypalTest.Models.PaypalOrder
{
    public class PaypalOrder
    {
        public string intent { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public ApplicationContext application_context { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class ApplicationContext
    {
        public string brand_name { get; set; }
        public string landing_page { get; set; }
        public string user_action { get; set; }
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }

    public class PurchaseUnit
    {
        public Amount amount { get; set; }
        public string description { get; set; }
    }

}