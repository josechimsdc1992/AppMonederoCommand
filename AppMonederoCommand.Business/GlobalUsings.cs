﻿global using AppMonederoCommand.Business.Interfaces;
global using AppMonederoCommand.Business.Interfaces.Catalogos;
global using AppMonederoCommand.Business.Interfaces.Jwt;
global using AppMonederoCommand.Business.Interfaces.Monedero;
global using AppMonederoCommand.Business.Interfaces.Parametro;
global using AppMonederoCommand.Business.Interfaces.Tarifa;
global using AppMonederoCommand.Business.Interfaces.Tarjeta;
global using AppMonederoCommand.Business.Interfaces.Usuarios;
global using AppMonederoCommand.Business.Properties;
global using AppMonederoCommand.Business.Repositories;
global using AppMonederoCommand.Business.Repositories.Catalogos;
global using AppMonederoCommand.Business.Repositories.Jwt;
global using AppMonederoCommand.Business.Repositories.Monedero;
global using AppMonederoCommand.Business.Repositories.Parametro;
global using AppMonederoCommand.Business.Repositories.Tarjetas;
global using AppMonederoCommand.Business.Repositories.TipoTarifa;
global using AppMonederoCommand.Business.Repositories.Usuarios;
global using AppMonederoCommand.Entities;
global using AppMonederoCommand.Entities.Catalogos;
global using AppMonederoCommand.Entities.Enums;
global using AppMonederoCommand.Entities.Lenguajes;
global using AppMonederoCommand.Entities.Monedero;
global using AppMonederoCommand.Entities.Monedero.ConfiguracionEstatus;
global using AppMonederoCommand.Entities.Monedero.Enums;
global using AppMonederoCommand.Entities.Monedero.RequestHTTP;
global using AppMonederoCommand.Entities.Notificaciones;
global using AppMonederoCommand.Entities.Parametro;
global using AppMonederoCommand.Entities.Replicas;
global using AppMonederoCommand.Entities.Sender;
global using AppMonederoCommand.Entities.Tarjetas;
global using AppMonederoCommand.Entities.TarjetaUsuario;
global using AppMonederoCommand.Entities.TarjetaUsuario.ConfiguracionEstatus;
global using AppMonederoCommand.Entities.TarjetaUsuario.Enums;
global using AppMonederoCommand.Entities.TipoOperaciones;
global using AppMonederoCommand.Entities.TipoTarifa;
global using AppMonederoCommand.Entities.Usuarios;
global using AppMonederoCommand.Entities.Usuarios.ActualizaUsuario;
global using AppMonederoCommand.Entities.Usuarios.AzureBlobStorage.Request;
global using AppMonederoCommand.Entities.Usuarios.BusMessage;
global using AppMonederoCommand.Entities.Usuarios.CambioDispositivo;
global using AppMonederoCommand.Entities.Usuarios.CodigoVerificacion;
global using AppMonederoCommand.Entities.Usuarios.Contrasena;
global using AppMonederoCommand.Entities.Usuarios.EliminarCuenta;
global using AppMonederoCommand.Entities.Usuarios.FirebaseToken;
global using AppMonederoCommand.Entities.Usuarios.Http.Request;
global using AppMonederoCommand.Entities.Usuarios.Http.Response;
global using AppMonederoCommand.Entities.Usuarios.JWTEntities;
global using AppMonederoCommand.Entities.Usuarios.Login;
global using AppMonederoCommand.Entities.Usuarios.ReenviarCodigo;
global using AppMonederoCommand.Entities.Usuarios.RefreshToken;
global using AppMonederoCommand.Services.Enums;
global using AppMonederoCommand.Services.Interfaces;
global using AppMonederoCommand.Services.Interfaces.AzureBlobStorage;
global using AppMonederoCommand.Utils;
global using AutoMapper;
global using IMD.Utils;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Newtonsoft.Json.Linq;
global using System.Dynamic;
global using System.Globalization;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
