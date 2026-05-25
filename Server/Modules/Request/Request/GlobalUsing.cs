global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

global using Carter;
global using MediatR;

global using Request.Data;
global using Request.Data.Repository.Read;
global using Request.Data.Repository.Write;
global using Request.Dto;
global using Request.Requests.Model;
global using Request.Service.CommandHandlerService;

global using System.Reflection;

global using Shared.CQRS;
global using Shared.DDD;
global using Shared.Data.Extensions;
global using Shared.Data.UnitOfWork;
global using Shared.Pagination;
