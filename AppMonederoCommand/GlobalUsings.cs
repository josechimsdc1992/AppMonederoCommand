global using AppMonederoCommand.Api;
global using AppMonederoCommand.Api.Authorization;
global using AppMonederoCommand.Business;
global using AppMonederoCommand.Business.BusSugerencia;
global using AppMonederoCommand.Business.BusUsuarios;
global using AppMonederoCommand.Business.Catalogos;
global using AppMonederoCommand.Business.Interfaces;
global using AppMonederoCommand.Business.Interfaces.Catalogos;
global using AppMonederoCommand.Business.Interfaces.Jwt;
global using AppMonederoCommand.Business.Interfaces.Monedero;
global using AppMonederoCommand.Business.Interfaces.Parametro;
global using AppMonederoCommand.Business.Interfaces.Sugerencia;
global using AppMonederoCommand.Business.Interfaces.Tarifa;
global using AppMonederoCommand.Business.Interfaces.Tarjeta;
global using AppMonederoCommand.Business.Interfaces.Usuarios;
global using AppMonederoCommand.Business.JWTService;
global using AppMonederoCommand.Business.Lenguaje;
global using AppMonederoCommand.Business.Mapping;
global using AppMonederoCommand.Business.Monedero;
global using AppMonederoCommand.Business.Parametro;
global using AppMonederoCommand.Business.Repositories;
global using AppMonederoCommand.Business.Repositories.Catalogos;
global using AppMonederoCommand.Business.Repositories.Jwt;
global using AppMonederoCommand.Business.Repositories.Monedero;
global using AppMonederoCommand.Business.Repositories.Parametro;
global using AppMonederoCommand.Business.Repositories.Sugerencia;
global using AppMonederoCommand.Business.Repositories.Tarjetas;
global using AppMonederoCommand.Business.Repositories.TipoTarifa;
global using AppMonederoCommand.Business.Repositories.Usuarios;
global using AppMonederoCommand.Business.Tarifa;
global using AppMonederoCommand.Business.Tarjetas;
global using AppMonederoCommand.Data;
global using AppMonederoCommand.Data.Mapping;
global using AppMonederoCommand.Data.Queries;
global using AppMonederoCommand.Data.Queries.Catalogos;
global using AppMonederoCommand.Data.Queries.Jwt;
global using AppMonederoCommand.Data.Queries.Monedero;
global using AppMonederoCommand.Data.Queries.Monedero.Folio;
global using AppMonederoCommand.Data.Queries.Parametro;
global using AppMonederoCommand.Data.Queries.Sugerencia;
global using AppMonederoCommand.Data.Queries.TarjetaUsuario;
global using AppMonederoCommand.Data.Queries.TipoTarifa;
global using AppMonederoCommand.Data.Queries.Usuarios;
global using AppMonederoCommand.Entities;
global using AppMonederoCommand.Entities.Catalogos;
global using AppMonederoCommand.Entities.Enums;
global using AppMonederoCommand.Entities.Monedero;
global using AppMonederoCommand.Entities.Notificaciones;
global using AppMonederoCommand.Entities.Parametro;
global using AppMonederoCommand.Entities.Replicas;
global using AppMonederoCommand.Entities.Sugerencia;
global using AppMonederoCommand.Entities.Tarjetas;
global using AppMonederoCommand.Entities.TarjetaUsuario;
global using AppMonederoCommand.Entities.TipoOperaciones;
global using AppMonederoCommand.Entities.Usuarios;
global using AppMonederoCommand.Entities.Usuarios.ActualizaUsuario;
global using AppMonederoCommand.Entities.Usuarios.AzureBlobStorage.Request;
global using AppMonederoCommand.Entities.Usuarios.BusMessage;
global using AppMonederoCommand.Entities.Usuarios.CambioDispositivo;
global using AppMonederoCommand.Entities.Usuarios.CodigoVerificacion;
global using AppMonederoCommand.Entities.Usuarios.Contrasena;
global using AppMonederoCommand.Entities.Usuarios.EliminarCuenta;
global using AppMonederoCommand.Entities.Usuarios.FirebaseToken;
global using AppMonederoCommand.Entities.Usuarios.JWTEntities;
global using AppMonederoCommand.Entities.Usuarios.Login;
global using AppMonederoCommand.Entities.Usuarios.RecuperarCuenta;
global using AppMonederoCommand.Entities.Usuarios.ReenviarCodigo;
global using AppMonederoCommand.Services;
global using AppMonederoCommand.Services.AzureBlobStorage;
global using AppMonederoCommand.Services.Interfaces;
global using AppMonederoCommand.Services.Interfaces.AzureBlobStorage;
global using AutoMapper;
global using IMD.Utils;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Cors;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Newtonsoft.Json.Serialization;
global using Oracle.ManagedDataAccess.Client;
global using Swashbuckle.AspNetCore.Annotations;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;
