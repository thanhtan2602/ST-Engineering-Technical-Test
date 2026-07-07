// Backing services
global using Shared.DDD;
global using Shared.Data;
global using Shared.Data.Seed;
global using Shared.Extensions;
global using Shared.Pagination;
global using Shared.CQRS;

// Libraries
global using Microsoft.EntityFrameworkCore;
global using Mapster;
global using MediatR;
global using Microsoft.Extensions.Logging;
global using Carter;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.Hosting;
global using FluentValidation;

// Catalog module
global using Catalog.Data;
global using Catalog.Products.Models;
global using Catalog.Products.Exceptions;
global using Catalog.Categories.Models;
global using Catalog.Brands.Models;
global using Catalog.ProductTypes.Models;
global using Catalog.AttributeDefinitions.Models;
