﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayPal.Api;
using System;
using System.Net;

namespace PayPal.Testing
{
    [TestClass]
    public class SaleTest : BaseTest
    {
        public static readonly string SaleJson =
            "{\"amount\":" + AmountTest.AmountJson + "," +
            "\"parent_payment\":\"103\"," +
            "\"state\":\"Approved\"," +
            "\"create_time\":\"2013-01-17T18:12:02.347Z\"," +
            "\"links\":[" + LinksTest.LinksJson + "]}";

        public static Sale GetSale()
        {
            return JsonFormatter.ConvertFromJson<Sale>(SaleJson);
        }

        [TestMethod, TestCategory("Unit")]
        public void SaleObjectTest()
        {
            var sale = GetSale();
            Assert.AreEqual("103", sale.parent_payment);
            Assert.AreEqual("Approved", sale.state);
            Assert.AreEqual("2013-01-17T18:12:02.347Z", sale.create_time);
            Assert.IsNotNull(sale.amount);
            Assert.IsNotNull(sale.links);
        }

        [TestMethod, TestCategory("Unit")]
        public void SaleNullIdTest()
        {
            TestingUtil.AssertThrownException<System.ArgumentNullException>(() => Sale.Get(new APIContext("token"), null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SaleConvertToJsonTest()
        {
            Assert.IsFalse(GetSale().ConvertToJson().Length == 0);
        }

        [TestMethod, TestCategory("Unit")]
        public void SaleToStringTest()
        {
            Assert.IsFalse(GetSale().ToString().Length == 0);
        }

        [TestMethod, TestCategory("Functional")]
        public void SaleGetTest()
        {
            try
            {
                var saleId = "4V7971043K262623A";
                var sale = Sale.Get(TestingUtil.GetApiContext(), saleId);
                this.RecordConnectionDetails();

                Assert.IsNotNull(sale);
                Assert.AreEqual(saleId, sale.id);
            }
            catch(ConnectionException)
            {
                this.RecordConnectionDetails(false);
            }
        }

        [TestMethod, TestCategory("Functional")]
        public void SaleRefundTest()
        {
            try
            {
                var apiContext = TestingUtil.GetApiContext();
                this.RecordConnectionDetails();

                // Create a credit card sale payment
                var payment = PaymentTest.CreatePaymentForSale(apiContext);
                this.RecordConnectionDetails();

                // Get the sale resource
                var sale = payment.transactions[0].related_resources[0].sale;

                var refund = new Refund
                {
                    amount = new Amount
                    {
                        currency = "USD",
                        total = "0.01"
                    }
                };

                var response = sale.Refund(apiContext, refund);
                this.RecordConnectionDetails();

                Assert.IsNotNull(response);
                Assert.AreEqual("completed", response.state);
            }
            catch(ConnectionException)
            {
                this.RecordConnectionDetails(false);
            }
        }
    }
}
