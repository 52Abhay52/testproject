using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using RahulDev.Xrm.Plugins;
using Plugins;
using FakeXrmEasy.Abstractions.Plugins;
using FakeXrmEasy.Plugins;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.FakeMessageExecutors;
using System.Reflection;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Abstractions.Enums;

namespace RahulDev.Xrm.Plugins.Tests
{
    [TestClass]
    public class PortalContactTest_FakeContext
    {
        protected readonly IOrganizationService _service;
        protected readonly IXrmFakedContext _fakeContext;

        [TestMethod]
        public void TestPortalContact()
        {
            //var _fakeContext = new XrmFakedContext();

            var _fakeContext = MiddlewareBuilder.New().AddCrud().
                AddFakeMessageExecutors(Assembly.GetAssembly(typeof(AddListMembersListRequestExecutor)))
                .UseCrud().UseMessages().SetLicense(FakeXrmEasyLicense.RPL_1_5).Build();

            var _service = _fakeContext.GetOrganizationService();

            //Create Target entity
            var targetEntity = new Entity("contact");
            var targetID = Guid.NewGuid();
            targetEntity.Id = targetID;
            targetEntity["crac0_nhsnumber"] = "SJ314765B";

            //Create Input Parameter
            var inputParameter = new ParameterCollection();
            inputParameter.Add("Target", targetEntity);

            //Create plugin execution context
            var fakePluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                UserId = Guid.NewGuid(),
                PrimaryEntityName = "contact",
                PrimaryEntityId = targetID,
                InputParameters = inputParameter
            };

            //Create NHS number dummy record
            var contact = new Entity("Contact");

            contact.Attributes.Add("crac0_nhsnumber", "SJ314765B");
            _service.Create(contact);

            //Execute the plugin code
            _fakeContext.ExecutePluginWith<PortalContact>(fakePluginExecutionContext);

            Assert.AreEqual(targetEntity.GetAttributeValue<string>("crac0_nhsnumber"), "SJ314765B");
        }
    }
}
