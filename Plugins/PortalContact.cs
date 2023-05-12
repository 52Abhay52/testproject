using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugins
{
    public class PortalContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
            serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
            context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName == "contact")
                {
                    if (entity.Attributes.Contains("crac0_nhsnumber") == true)
                    {
                      var NHSNumber =  entity.Attributes["crac0_nhsnumber"].ToString();

                        var qe = new QueryExpression();
                        //qe.EntityName = "crac0_hartlinkcontacts";
                        qe.EntityName = "contact";
                        qe.ColumnSet = new ColumnSet();
                        qe.ColumnSet.Columns.Add("crac0_nhsnumber");

                        EntityCollection ec = service.RetrieveMultiple(qe);
                        foreach(var entityRecord in ec.Entities)
                        {
                            if (!NHSNumber.Equals(entityRecord["crac0_nhsnumber"].ToString()))
                            {
                                throw new InvalidPluginExecutionException("NHS Number is not exist");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("The NHS Number is not present.");
                    }
                }

            }
        }
    }
}
