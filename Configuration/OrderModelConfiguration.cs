﻿namespace Microsoft.Examples.Configuration
{
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class OrderModelConfiguration : IModelConfiguration
    {
        private static readonly ApiVersion V1 = new ApiVersion(1, 0);

        private EntityTypeConfiguration<Order> ConfigureCurrent(ODataModelBuilder builder)
        {
            var order = builder.EntitySet<Order>("orders").EntityType;

            order.HasKey(p => p.Id);

            return order;
        }

        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            if (routePrefix != "api/v{version:apiVersion}")
            {
                return;
            }

            // note: the EDM for orders is only available in version 1.0
            if (apiVersion == V1)
            {
                ConfigureCurrent(builder);
            }
        }
    }
}