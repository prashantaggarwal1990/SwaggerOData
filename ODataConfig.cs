using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using SwaggerOData.Models;

namespace SwaggerOData
{
    public class ODataConfig : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {
            var employeesCollection = builder.EntitySet<Employee>("Employees").EntityType.Collection;
        }
    }
}
