using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Examples.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Examples
{
    public static partial class SwaggerExtensions
    {
        public class ODataQueryOptionsSlimFilter : IOperationFilter
        {
            private static readonly Type _queryOptionsGenericDefinition
                = typeof( ODataQueryOptionsSlim<> ).GetGenericTypeDefinition();

            private static IReadOnlyList<string> OptionsRootProperties { get; }
                = ClassInfo<ODataQueryOptionsSlim<object>>.RootPropertyPaths
                    .ToList();

            private static OpenApiParameter[] ODataParameters { get; } =
            {
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.Filter,
                    In = ParameterLocation.Query,
                    Description = "OData formatted filter string.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                },
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.Select,
                    In = ParameterLocation.Query,
                    Description = "Comma separated list of fields to return.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                },
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.OrderBy,
                    In = ParameterLocation.Query,
                    Description = "Field name and direction by which to order records.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                },
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.Top,
                    In = ParameterLocation.Query,
                    Description = "Number of records to return.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "integer" }
                },
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.Skip,
                    In = ParameterLocation.Query,
                    Description = "Number of records that needs to be skipped.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "integer" }
                },
                new OpenApiParameter
                {
                    Name = ODataQueryOptionName.Count,
                    In = ParameterLocation.Query,
                    Description = "Flag indicating whether total count of records should be calculated.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "boolean" }
                }
            };

            public void Apply( OpenApiOperation operation, OperationFilterContext context )
            {
                if ( !HasODataQueryParameter( context.MethodInfo ) )
                {
                    return;
                }

                var correctedParameters = operation.Parameters
                    .Where( x => !IsAutoExpanded( x ) )
                    .Concat( ODataParameters );

                operation.Parameters = correctedParameters.ToList();

                if ( IsObjectResponse( context ) )
                {
                    operation.Responses["200"] =
                        new OpenApiResponse
                        {
                            Description = "Success",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                [MediaTypeNames.Application.Json] =
                                    new OpenApiMediaType
                                    {
                                        Schema = GetODataSchema( context )
                                    }
                            }
                        };
                }
            }

            private bool IsObjectResponse( OperationFilterContext context ) =>
                context.MethodInfo.ReturnType != typeof( void ) &&
                context.MethodInfo.ReturnType != typeof( Task ) &&
                context.MethodInfo.ReturnType != typeof( IActionResult );

            private OpenApiSchema GetODataSchema( OperationFilterContext context )
            {
                var type = context.MethodInfo
                    .ReturnType
                    .UnwrapTaskIfNeeded()
                    .UnwrapCollectionTypeIfNeeded();

                type = typeof( ODataCollectionResponse<> ).MakeGenericType( type );

                return context.SchemaGenerator.GenerateSchema(
                    type,
                    context.SchemaRepository );
            }

            private bool HasODataQueryParameter( MethodInfo actionMethod )
                =>
                    actionMethod.GetCustomAttribute<EnableQueryAttribute>() != null ||
                    actionMethod
                        .GetParameters()
                        .Any(
                            x =>
                            {
                                var parameterType = x.ParameterType;

                                return parameterType.IsConstructedGenericType &&
                                       parameterType.GetGenericTypeDefinition() == _queryOptionsGenericDefinition;
                            } );

            private bool IsAutoExpanded( OpenApiParameter parameter )
                => OptionsRootProperties.Any( x => parameter.Name == x || parameter.Name.StartsWith( $"{x}." ) );
        }
    }
}