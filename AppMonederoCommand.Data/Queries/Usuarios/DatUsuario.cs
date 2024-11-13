using BusMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Data.Entities.Usuarios.Usuario, AppMonederoCommand.Entities.Usuarios.EntUsuario>;
using DbMapper = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Usuarios.EntUsuario, AppMonederoCommand.Data.Entities.Usuarios.Usuario>;
using DbMapperFirebase = IMD.Utils.IMDAutoMapper<AppMonederoCommand.Entities.Usuarios.FirebaseToken.EntFirebaseToken, AppMonederoCommand.Data.Entities.FirebaseTokens.FirebaseToken>;

namespace AppMonederoCommand.Data.Queries.Usuarios
{
    /* IMASD S.A.DE C.V
   =========================================================================================
   * Descripción: 
   * Historial de cambios:
   * ---------------------------------------------------------------------------------------
   *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
   * ---------------------------------------------------------------------------------------
   *      1        | 21/07/2023 | L.I. Oscar Luna       | Creación
   *      2        | 22/08/2023 | Neftalí Rodríguez     | Update y Delete usuario
   *      2        | 22/09/2023 | L.I. Oscar Luna       | Update contraseña
   * ---------------------------------------------------------------------------------------
   */
    public class DatUsuario : IDatUsuario
    {
        protected TransporteContext _dbContext { get; }
        private readonly ILogger<DatUsuario> _logger;
        private readonly IMapper _mapper;
        private readonly ExchangeConfig _exchangeConfig;
        private readonly IMDRabbitNotifications _rabbitNotifications;

        public DatUsuario(TransporteContext dbContext, ILogger<DatUsuario> logger, IMapper mapper, 
            IServiceProvider serviceProvider, ExchangeConfig exchangeConfig)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _rabbitNotifications = serviceProvider.GetRequiredService<IMDRabbitNotifications>();
            _exchangeConfig = exchangeConfig;
        }

        #region Métodos Service Default

        [IMDMetodo(67823463616300, 67823463615523)]
        public async Task<IMDResponse<EntUsuario>> DGet(Guid uIdUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario);
                response.SetSuccess(BusMapper.MapEntity(query));
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }
        #endregion

        #region Métodos Service DatUsuario
        //Guarda un nuevo usuario en la BD
        public async Task<IMDResponse<EntUsuario>> DSave(EntUsuario nuevoUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DSave);
            _logger.LogInformation(IMDSerializer.Serialize(67823461789573, $"Inicia {metodo}(EntUsuario nuevoUsuario)", nuevoUsuario, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));

            try
            {
                var usuario = DbMapper.MapEntity(nuevoUsuario);
                usuario.uIdUsuario = Guid.NewGuid();
                _dbContext.Usuario.Add(usuario);

                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {

                    return default;
                }

                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == usuario.uIdUsuario);
                response.SetSuccess(BusMapper.MapEntity(query));
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461790350;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461790350, $"Error en {metodo}(EntUsuario nuevoUsuario): {ex.Message}", nuevoUsuario, ex, response));
            }

            _logger.LogInformation(IMDSerializer.Serialize(67823461789573, $"Termina {metodo}(EntUsuario nuevoUsuario)", nuevoUsuario, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")));
            return response;
        }

        [IMDMetodo(67823466027331, 67823466028108)]
        public async Task<IMDResponse<bool>> DSaveFirebaseToken(EntFirebaseToken firebaseToken, EntUsuario entUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", firebaseToken, entUsuario));

            try
            {
                int i = 0;
                var tokenUsuario = await _dbContext.FirebaseToken.FirstOrDefaultAsync(w => w.uIdUsuario == firebaseToken.uIdUsuario && w.sIdAplicacion == firebaseToken.sIdAplicacion);
                
                EntReplicaFirebaseToken entReplicaFirebaseToken = new EntReplicaFirebaseToken
                {
                    uIdUsuario = firebaseToken.uIdUsuario,
                    sFcmToken = firebaseToken.sFcmToken,
                    dtFechaCreacion = firebaseToken.dtFechaCreacion,
                    sIdAplicacion = firebaseToken.sIdAplicacion,
                };

                if (tokenUsuario == null)
                {
                    //Se agrega el registro...
                    var tokenfire = DbMapperFirebase.MapEntity(firebaseToken);

                    tokenfire.bActivo = true;
                    tokenfire.uIdFirebaseToken = Guid.NewGuid();
                    _dbContext.FirebaseToken.Add(tokenfire);
                    i = await _dbContext.SaveChangesAsync();

                    if (entUsuario != null)
                    {
                        entReplicaFirebaseToken.uIdFirebaseToken = tokenfire.uIdFirebaseToken;
                        entReplicaFirebaseToken.sNombre = entUsuario.sNombre;
                        entReplicaFirebaseToken.sApellidoPaterno = entUsuario.sApellidoPaterno;
                        entReplicaFirebaseToken.sApellidoMaterno = entUsuario.sApellidoMaterno;
                        entReplicaFirebaseToken.sCorreo = entUsuario.sCorreo;
                        entReplicaFirebaseToken.sTelefono = entUsuario.sTelefono;
                        entReplicaFirebaseToken.uIdMonedero = entUsuario.uIdMonedero;
                    }

                    ///Envío réplica FirebaseToken
                    await _rabbitNotifications.SendAsync(RoutingKeys.FirebaseTokenCreacion.GetDescription(), _exchangeConfig, new QueueMessage<EntReplicaFirebaseToken>
                    {
                        Content = entReplicaFirebaseToken
                    });
                }
                else
                {
                    //Se actualiza el registro...
                    dynamic update = new ExpandoObject();
                    update.sFcmToken = firebaseToken.sFcmToken;
                    update.dtFechaModificacion = DateTime.Now;
                    if (firebaseToken.sInfoAppOS != null)
                    {
                        update.sInfoAppOS = firebaseToken.sInfoAppOS;
                    }

                    _dbContext.Entry(tokenUsuario).CurrentValues.SetValues(update);
                    i = await _dbContext.SaveChangesAsync();

                    if (entUsuario != null)
                    {
                        entReplicaFirebaseToken.uIdFirebaseToken = tokenUsuario.uIdFirebaseToken;
                        entReplicaFirebaseToken.dtFechaModificacion = update.dtFechaModificacion;
                        entReplicaFirebaseToken.sFcmToken = update.sFcmToken;
                        entReplicaFirebaseToken.uIdMonedero = entUsuario.uIdMonedero;
                    }

                    ///Envío réplica FirebaseToken
                    await _rabbitNotifications.SendAsync(RoutingKeys.FirebaseTokenModificacion.GetDescription(), _exchangeConfig, new QueueMessage<EntReplicaFirebaseToken>
                    {
                        Content = entReplicaFirebaseToken
                    });
                }

                if (i != 0)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetError("Error desconocido, no se pudo guardar el FirebaseToken ");
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", firebaseToken, entUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetCorreo(string pEmail)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetCorreo);
            _logger.LogInformation(IMDSerializer.Serialize(67823461783357, $"Inicia {metodo}(string pEmail)", pEmail));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == pEmail);
                response.SetSuccess(BusMapper.MapEntity(query));
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461784134;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461784134, $"Error en {metodo}(string pEmail): {ex.Message}", pEmail, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465662918, 67823465662141)]
        public async Task<IMDResponse<EntUsuario>> DGetByRedSocial(string sIdRedSocial, string sRedSocial)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sIdRedSocial, string sRedSocial)", sIdRedSocial, sRedSocial));

            try
            {
                EntRedSocial redesSociales = new EntRedSocial();
                EntUsuario? usuario = null;
                if (sRedSocial == redesSociales.sRedSocialGoogle)
                {
                    usuario = BusMapper.MapEntity(await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdRedSocialGoogle == sIdRedSocial));
                }
                else if (sRedSocial == redesSociales.sRedSocialFacebook)
                {
                    usuario = BusMapper.MapEntity(await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdRedSocialFaceBook == sIdRedSocial));
                }
                else if (sRedSocial == redesSociales.sRedSocialApple)
                {
                    usuario = BusMapper.MapEntity(await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdRedSocialApple == sIdRedSocial));
                }

                response.SetSuccess(usuario);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sIdRedSocial, string sRedSocial): {ex.Message}", sIdRedSocial, sRedSocial, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetTelefono(string pTelefono)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetTelefono);
            _logger.LogInformation(IMDSerializer.Serialize(67823461786465, $"Inicia {metodo}(string pTelefono)", pTelefono));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sTelefono == pTelefono && u.bCuentaVerificada == true);
                response.SetSuccess(BusMapper.MapEntity(query));
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461787242;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461787242, $"Error en {metodo}(string pTelefono): {ex.Message}", pTelefono, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetCURP(string pCURP)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetCURP);
            _logger.LogInformation(IMDSerializer.Serialize(67823461788019, $"Inicia {metodo}(string pCURP)", pCURP));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCURP == pCURP);
                response.SetSuccess(BusMapper.MapEntity(query));
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823461788796;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823461788796, $"Error en {metodo}(string pCURP): {ex.Message}", pCURP, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetUsuario(EntLogin pUsuario, bool pTipoLogin, bool? bCuentaVerificada = true, bool? bActivo = true, bool? bBaja = false)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462022673, $"Inicia {metodo}(EntLogin pUsuario, bool pTipoLogin, bool bCuentaVerificada = true, bool bActivo = true, bool bBaja = false)", pUsuario, pTipoLogin));

            try
            {

                if (pTipoLogin == false)//Login normal
                {
                    string PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
                    string PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";
                    pUsuario.sPassword = IMDSecurity.BEncrypt(pUsuario.sPassword, PCKEY, PCIV);
                    var query = await _dbContext.Usuario.SingleOrDefaultAsync(u =>
                    u.sCorreo == pUsuario.sUsername
                    && u.sContrasena == pUsuario.sPassword
                    && (!bCuentaVerificada.HasValue || u.bCuentaVerificada == bCuentaVerificada.Value)
                    && (!bActivo.HasValue || u.bActivo == bActivo.Value)
                    && (!bBaja.HasValue || u.bBaja == bBaja.Value));
                    response.SetSuccess(BusMapper.MapEntity(query));
                }

                if (pTipoLogin == true)//Login por red social
                {
                    EntRedSocial redesSociales = new EntRedSocial();


                    if (redesSociales.sRedSocialGoogle.Equals(pUsuario.sNewtwork))
                    {

                        var query = await _dbContext.Usuario.SingleOrDefaultAsync(u =>
                        u.sCorreo == pUsuario.sUsername
                        && u.uIdRedSocialGoogle == pUsuario.uIdNetwork
                        && (!bCuentaVerificada.HasValue || u.bCuentaVerificada == bCuentaVerificada.Value)
                        && (!bActivo.HasValue || u.bActivo == bActivo.Value)
                        && (!bBaja.HasValue || u.bBaja == bBaja.Value));
                        response.SetSuccess(BusMapper.MapEntity(query));

                    }

                    if (redesSociales.sRedSocialFacebook.Equals(pUsuario.sNewtwork))
                    {
                        var query = await _dbContext.Usuario.SingleOrDefaultAsync(u =>
                        u.sCorreo == pUsuario.sUsername
                        && u.uIdRedSocialFaceBook == pUsuario.uIdNetwork
                        && (!bCuentaVerificada.HasValue || u.bCuentaVerificada == bCuentaVerificada.Value)
                        && (!bActivo.HasValue || u.bActivo == bActivo.Value)
                        && (!bBaja.HasValue || u.bBaja == bBaja.Value));
                        response.SetSuccess(BusMapper.MapEntity(query));
                    }

                    if (redesSociales.sRedSocialApple.Equals(pUsuario.sNewtwork))
                    {
                        if (string.IsNullOrEmpty(pUsuario.sUsername))
                        {
                            var query = await _dbContext.Usuario.SingleOrDefaultAsync(u =>
                            u.uIdRedSocialApple == pUsuario.uIdNetwork
                            && (!bCuentaVerificada.HasValue || u.bCuentaVerificada == bCuentaVerificada.Value)
                            && (!bActivo.HasValue || u.bActivo == bActivo.Value)
                            && (!bBaja.HasValue || u.bBaja == bBaja.Value));
                            response.SetSuccess(BusMapper.MapEntity(query));
                        }
                        else
                        {
                            var query = await _dbContext.Usuario.SingleOrDefaultAsync(u =>
                            u.sCorreo == pUsuario.sUsername
                            && u.uIdRedSocialApple == pUsuario.uIdNetwork
                            && (!bCuentaVerificada.HasValue || u.bCuentaVerificada == bCuentaVerificada.Value)
                            && (!bActivo.HasValue || u.bActivo == bActivo.Value)
                            && (!bBaja.HasValue || u.bBaja == bBaja.Value));
                            response.SetSuccess(BusMapper.MapEntity(query));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462023450;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462023450, $"Error en {metodo}(EntLogin pUsuario, bool pTipoLogin, bool bCuentaVerificada = true, bool bActivo = true, bool bBaja = false): {ex.Message}", pUsuario, pTipoLogin, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetUsuarioMigrado(EntLogin pUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetUsuarioMigrado);
            _logger.LogInformation(IMDSerializer.Serialize(67823462776363, $"Inicia {metodo}(EntLogin pUsuario)", pUsuario));

            try
            {

                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == pUsuario.sUsername && u.bActivo == true);

                if (query != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(query));
                }
                else
                {
                    response.SetNotFound(new EntUsuario(), Menssages.DatVerifyEmail);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462777140;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462777140, $"Error en {metodo}(EntLogin pUsuario): {ex.Message}", pUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DGetValidaUsuario(EntCodigoVerificacion codigo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DGetValidaUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462145439, $"Inicia {metodo}(EntCodigoVerificacion codigo)", codigo));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == codigo.sCorreo && u.bCuentaVerificada == false);

                if (query != null)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetSuccess(false);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462146216;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462146216, $"Error en {metodo}(EntCodigoVerificacion codigo): {ex.Message}", codigo, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DGetCodigoValido(EntCodigoVerificacion codigo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DGetCodigoValido);
            _logger.LogInformation(IMDSerializer.Serialize(67823462146993, $"Inicia {metodo}(EntCodigoVerificacion codigo)", codigo));

            try
            {
                string PCKEY = Environment.GetEnvironmentVariable("PCKEY") ?? "";
                string PCIV = Environment.GetEnvironmentVariable("PCIV") ?? "";
                var codigoValida = IMDSecurity.BEncrypt(codigo.sClaveVerificacion, PCKEY, PCIV);
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == codigo.sCorreo && u.sCodigoVerificacion == codigoValida);

                if (query != null)
                {
                    response.SetSuccess(true);
                }
                else
                {
                    response.SetNoContent();
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462147770;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462147770, $"Error en {metodo}(EntCodigoVerificacion codigo): {ex.Message}", codigo, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DUpdateUsuarioVerificado(EntCodigoVerificacion codigo)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DUpdateUsuarioVerificado);
            _logger.LogInformation(IMDSerializer.Serialize(67823462148547, $"Inicia {metodo}(EntCodigoVerificacion codigo)", codigo));

            try
            {

                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == codigo.sCorreo && u.bCuentaVerificada == false);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserNoExist);
                    return response;
                }
                query.bCuentaVerificada = true;
                query.bMigrado = true;
                query.dtFechaModificacion = DateTime.UtcNow;

                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {
                    response.SetError(Menssages.DatNoUseStatusAccount);
                }
                else
                {

                    var queryEnt = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == codigo.sCorreo && u.bCuentaVerificada == true);

                    if (queryEnt != null)
                    {
                        response.SetSuccess(BusMapper.MapEntity(queryEnt));
                    }
                    else
                    {
                        response.SetError(Menssages.DatNoExistUserVerificated);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462149324;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462149324, $"Error en {metodo}(EntCodigoVerificacion codigo): {ex.Message}", codigo, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateCodigoVerificacion(EntReenviaCodigo solicitud, string codgioEncrypt)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateCodigoVerificacion);
            _logger.LogInformation(IMDSerializer.Serialize(67823462159425, $"Inicia {metodo}(EntReenviaCodigo solicitud, string codgioEncrypt)", solicitud, codgioEncrypt));

            try
            {
                //Update
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.sCorreo == solicitud.sCorreo);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
                else
                {
                    query.sCodigoVerificacion = codgioEncrypt;
                    query.dtFechaModificacion = DateTime.Now;

                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatNoUpdateCodeVerification); ;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462160202;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462160202, $"Error en {metodo}(EntReenviaCodigo solicitud, string codgioEncrypt): {ex.Message}", solicitud, codgioEncrypt, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateContrasena(Guid uIdUsuario, EntNuevaContrasena contrasena)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateContrasena);
            _logger.LogInformation(IMDSerializer.Serialize(67823462185843, $"Inicia {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena)", uIdUsuario, contrasena));

            try
            {

                //Update
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserSolicitadeNoAviable);

                }
                else
                {
                    query.uIdUsuarioModificacion = uIdUsuario;
                    query.sContrasena = contrasena.sContrasenia;
                    query.bMigrado = true;
                    query.dtFechaModificacion = DateTime.UtcNow;


                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatPasswordDiferent);
                        //Termina update
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462186620;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462186620, $"Error en {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena): {ex.Message}", uIdUsuario, contrasena, ex, response));
            }
            return response;
        }

       
        public async Task<IMDResponse<bool>> DUpdateContrasena(Guid uIdUsuario, EntNuevaContrasena contrasena, string token)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateContrasena);
            _logger.LogInformation(IMDSerializer.Serialize(67823462795011, $"Inicia {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena, string token)", uIdUsuario, contrasena, token));

            try
            {

                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario && u.bActivo == true);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserSolicitadeNoAviable);

                }
                else
                {
                    query.uIdUsuarioModificacion = uIdUsuario;
                    query.sContrasena = contrasena.sContrasenia;
                    query.bCuentaVerificada = true;
                    query.bMigrado = true;
                    query.dtFechaModificacion = DateTime.UtcNow;


                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatPasswordDiferent);
                        //Termina update
                    }

                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462795788;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462795788, $"Error en {metodo}(Guid uIdUsuario, EntNuevaContrasena contrasena, string token): {ex.Message}", uIdUsuario, contrasena, token, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsuario entUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462583667, $"Inicia {metodo}(EntUpdateUsuario entUsuario)", entUsuario));

            try
            {
                var usuario = await _dbContext.Usuario.FindAsync(entUsuario.uIdUsuario);
                if (usuario != null)
                {
                    _dbContext.Entry(usuario).CurrentValues.SetValues(entUsuario);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i > 0)
                    {
                        response.SetSuccess(true, Menssages.DatCompleteSucces);
                    }
                    else
                    {
                        response.SetSuccess(false, Menssages.DatCompleteFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoUpdateExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462584444;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(67823462584444, $"Error en {metodo}(EntUpdateUsuario entUsuario): {ex.Message}", entUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsuarioRedSocial entUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462812105, $"Inicia {metodo}(EntUpdateUsuarioRedSocial entUsuario)", entUsuario));

            try
            {
                var usuario = await _dbContext.Usuario.FindAsync(entUsuario.uIdUsuario);
                if (usuario != null)
                {
                    _dbContext.Entry(usuario).CurrentValues.SetValues(entUsuario);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i > 0)
                    {
                        response.SetSuccess(true, Menssages.DatCompleteSucces);
                    }
                    else
                    {
                        response.SetSuccess(false, Menssages.DatCompleteFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoUpdateExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462812882;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462812882, $"Error en {metodo}(EntUpdateUsuarioRedSocial entUsuario): {ex.Message}", entUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateUsuario(EntUsuario nuevoUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462813659, $"Inicia {metodo}(EntUsuario nuevoUsuario)", nuevoUsuario));

            try
            {
                var usuario = await _dbContext.Usuario.FindAsync(nuevoUsuario.uIdUsuario);
                if (usuario != null)
                {
                    _dbContext.Entry(usuario).CurrentValues.SetValues(nuevoUsuario);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i > 0)
                    {
                        response.SetSuccess(true, Menssages.DatCompleteSucces);
                    }
                    else
                    {
                        response.SetSuccess(false, Menssages.DatCompleteFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoUpdateExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462814436;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462814436, $"Error en {metodo}(EntUsuario nuevoUsuario): {ex.Message}", nuevoUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsarioActivo usuarioActivo)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462824537, $"Inicia {metodo}(EntUpdateUsarioActivo usuarioActivo)", usuarioActivo));

            try
            {
                var usuario = await _dbContext.Usuario.FindAsync(usuarioActivo.uIdUsuario);
                if (usuario != null)
                {
                    _dbContext.Entry(usuario).CurrentValues.SetValues(usuarioActivo);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i > 0)
                    {
                        response.SetSuccess(true, Menssages.DatCompleteSucces);
                    }
                    else
                    {
                        response.SetSuccess(false, Menssages.DatCompleteFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoUpdateExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462825314;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462825314, $"Error en {metodo}(EntUpdateUsarioActivo usuarioActivo): {ex.Message}", usuarioActivo, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateContrasenaTemporal(Guid uIdUsuario, string contrasenaAleatoria)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateContrasenaTemporal);
            _logger.LogInformation(IMDSerializer.Serialize(67823462779471, $"Inicia {metodo}(Guid uIdUsuario, string contrasenaAleatoria)", uIdUsuario, contrasenaAleatoria));

            try
            {
                //Update
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserSolicitadeNoAviable);

                }
                else
                {
                    query.uIdUsuarioModificacion = uIdUsuario;
                    query.sContrasena = contrasenaAleatoria;
                    query.dtFechaVencimientoContrasena = DateTime.UtcNow.AddHours(48);
                    query.dtFechaModificacion = DateTime.UtcNow;


                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatPasswordRandom);
                        //Termina update
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462780248;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462780248, $"Error en {metodo}(Guid uIdUsuario, string contrasenaAleatoria): {ex.Message}", uIdUsuario, contrasenaAleatoria, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateMonederoUsuario(Guid uIdUsuario, Guid uIdMonedero)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateMonederoUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823463359113, $"Inicia {metodo}(Guid uIdUsuario, Guid uIdMonedero)", uIdUsuario, uIdMonedero));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserSolicitadeNoAviable);

                }
                else
                {
                    query.uIdMonedero = uIdMonedero;


                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatPurseNoChanges);
                        //Termina update
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463359890;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463359890, $"Error en {metodo}(Guid uIdUsuario, Guid uIdMonedero): {ex.Message}", uIdUsuario, uIdMonedero, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetUsuario(Guid uIdUsuario)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462586775, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var usuario = await _dbContext.Usuario.Where(w => w.uIdUsuario == uIdUsuario).SingleOrDefaultAsync();
                if (usuario != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(usuario));
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462587552;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(67823462587552, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DEliminarUsuario(EntEliminarUsuario entUsuario)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DEliminarUsuario);
            _logger.LogInformation(IMDSerializer.Serialize(67823462596099, $"Inicia {metodo}(EntEliminarUsuario entUsuario)", entUsuario));

            try
            {
                var usuario = await _dbContext.Usuario.FindAsync(entUsuario.uIdUsuario);
                if (usuario != null)
                {
                    _dbContext.Entry(usuario).CurrentValues.SetValues(entUsuario);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i > 0)
                    {
                        response.SetSuccess(true, Menssages.DatCompleteSucces);
                    }
                    else
                    {
                        response.SetSuccess(false, Menssages.DatCompleteFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatNoExistRegisterDelete);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462596876;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(67823462596876, $"Error en {metodo}(EntEliminarUsuario entUsuario): {ex.Message}", entUsuario, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<EntUsuario>> DGetExisteCuenta(string correo)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetExisteCuenta);
            _logger.LogInformation(IMDSerializer.Serialize(67823462673799, $"Inicia {metodo}(string correo)", correo));

            try
            {
                var usuario = await _dbContext.Usuario.Where(w => w.sCorreo == correo && w.bActivo == true && w.bBaja == false).SingleOrDefaultAsync();
                if (usuario != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(usuario));
                }
                else
                {

                    response.SetNotFound(new EntUsuario(), Menssages.DatAccountSolicitadeNoExits);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462674576;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462674576, $"Error en {metodo}(string correo): {ex.Message}", correo, ex, response));
            }
            return response;
        }

        //Si es por vía red social recupera la cuenta aunque este eliminada
        public async Task<IMDResponse<EntUsuario>> DGetExisteCuenta(string correo, bool isSocialNetwork)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            string metodo = nameof(this.DGetExisteCuenta);
            _logger.LogInformation(IMDSerializer.Serialize(67823462802781, $"Inicia {metodo}(string correo, bool isSocialNetwork)", correo, isSocialNetwork));

            try
            {
                var usuario = await _dbContext.Usuario.Where(w => w.sCorreo == correo && w.bActivo == true).SingleOrDefaultAsync();
                if (usuario != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(usuario));
                }
                else
                {

                    response.SetNotFound(new EntUsuario(), Menssages.DatAccountSolicitadeNoExits);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823462803558;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823462803558, $"Error en {metodo}(string correo, bool isSocialNetwork): {ex.Message}", correo, isSocialNetwork, ex, response));
            }
            return response;
        }


        [IMDMetodo(67823462788018, 67823462787241)]
        public async Task<IMDResponse<EntUsuario>> DGetByAppleId(string sAppleId)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(string sAppleId)", sAppleId));

            try
            {
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdRedSocialApple == sAppleId && u.bBaja == false);
                if (query != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(query));
                }
                else
                {
                    response.SetNotFound(new EntUsuario(), Menssages.DatAccountNoExits);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(string sAppleId): {ex.Message}", sAppleId, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463468670, 67823463467893)]
        public async Task<IMDResponse<EntUsuario>> DUpdateUsuarioDatosAdicionales(Guid uIdUsuario, string sTelefono, string sCURP)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario, string sTelefono, string sCURP)", uIdUsuario, sTelefono, sCURP));

            try
            {

                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(u =>
                u.uIdUsuario == uIdUsuario);
                if (usuario == null)
                {
                    response.SetError(Menssages.DatUserNoExist);
                    return response;
                }
                usuario.sTelefono = sTelefono;
                usuario.sCURP = sCURP;
                usuario.dtFechaModificacion = DateTime.UtcNow;
                int i = await _dbContext.SaveChangesAsync();

                if (i == 0)
                {
                    response.SetError(Menssages.DatOcurredError);
                }
                else
                {
                    response.SetSuccess(BusMapper.MapEntity(usuario), Menssages.DatCompleteSucces);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario, string sTelefono, string sCURP): {ex.Message}", uIdUsuario, sTelefono, sCURP, ex, response));
            }
            return response;
        }

        public async Task<IMDResponse<bool>> DUpdateFotografia(Guid uIdUsuario, string sFotografia)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            string metodo = nameof(this.DUpdateFotografia);
            _logger.LogInformation(IMDSerializer.Serialize(67823463534715, $"Inicia {metodo}(Guid uIdUsuario, string sFotografia)", uIdUsuario, sFotografia));

            try
            {
                //Update
                var query = await _dbContext.Usuario.SingleOrDefaultAsync(u => u.uIdUsuario == uIdUsuario);

                if (query == null)
                {
                    response.SetError(Menssages.DatUserSolicitadeNoAviable);

                }
                else
                {
                    query.uIdUsuarioModificacion = uIdUsuario;
                    query.sFotografia = sFotografia;
                    query.dtFechaModificacion = DateTime.UtcNow;


                    int i = await _dbContext.SaveChangesAsync();

                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetNoContent();//La imagane se guardo con el mismo nombre

                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 67823463535492;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(67823463535492, $"Error en {metodo}(Guid uIdUsuario, string sFotografia): {ex.Message}", uIdUsuario, sFotografia, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823463684676, 67823463683899)]
        public async Task<IMDResponse<List<EntFirebaseToken>>> DGetFirebaseToken(Guid uIdUsuario, int iTop = 0, string? sIdAplicacion = null)
        {
            IMDResponse<List<EntFirebaseToken>> response = new IMDResponse<List<EntFirebaseToken>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                var query = _dbContext.FirebaseToken.Where(f => f.uIdUsuario == uIdUsuario && f.bActivo);

                if (sIdAplicacion != null)
                    query = query.Where(f => f.sIdAplicacion == sIdAplicacion);

                // Agrupar primero
                //var groupedQuery = query.GroupBy(g => new { g.uIdUsuario, g.sFcmToken });

                // Ordenar dentro de cada grupo
                /*var orderedQuery = groupedQuery
                    .Select(group => new
                    {
                        Group = group,
                        MaxDate = group.Max(o => o.dtFechaModificacion > o.dtFechaCreacion ? o.dtFechaModificacion : o.dtFechaCreacion)
                    })
                    .OrderByDescending(o => o.MaxDate)
                    .SelectMany(o => o.Group);

                if (iTop > 0)
                {
                    orderedQuery = orderedQuery.Take(iTop);
                }*/

                var tmp = query.AsEnumerable()
                   .Select(group => new
                   {
                       Group = group,
                       MaxDate = group.dtFechaModificacion is null ? group.dtFechaCreacion : group.dtFechaModificacion
                   })
                   .OrderByDescending(o => o.MaxDate)
                   .ToList();

                List<FirebaseToken> lstGroupFirebaseToken = tmp.Select(x => x.Group).ToList();

                //Filtrar del listado los que tengan el mismo token y dar prioridad al que tenga sIdAplicacion
                List<FirebaseToken> lsEliminar = new List<FirebaseToken>();
                List<string> lsConservar = new List<string>();
                lstGroupFirebaseToken.ForEach(x =>
                {
                    if (!lsConservar.Contains(x.sFcmToken))
                        lsConservar.Add(x.sFcmToken);
                    else
                        lsEliminar.Add(x);
                });

                lstGroupFirebaseToken.RemoveAll(x => lsEliminar.Contains(x));

                if (iTop > 0)
                {
                    lstGroupFirebaseToken = lstGroupFirebaseToken.Take(iTop).ToList();
                }


                //var lstGroupFirebaseToken = await orderedQuery.ToListAsync();
                if (lstGroupFirebaseToken.Any())
                {
                    var result = _mapper.Map<List<EntFirebaseToken>>(lstGroupFirebaseToken);
                    response.SetSuccess(result, Menssages.DatTokensSucess);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823464913890, 67823464913113)]
        public async Task<IMDResponse<EntUsuario>> DGetByMonedero(Guid uIdMonedero)
        {
            IMDResponse<EntUsuario> response = new IMDResponse<EntUsuario>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdMonedero)", uIdMonedero));

            try
            {
                var usuario = await _dbContext.Usuario.Where(w => w.uIdMonedero == uIdMonedero).FirstOrDefaultAsync();
                if (usuario != null)
                {
                    response.SetSuccess(BusMapper.MapEntity(usuario));
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdMonedero): {ex.Message}", uIdMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465561908, 67823465561131)]
        public async Task<IMDResponse<EntPagination<EntUsuarioApp>>> DGetUsuariosApp(EntFiltersUsuario pEntity)
        {
            IMDResponse<EntPagination<EntUsuarioApp>> response = new IMDResponse<EntPagination<EntUsuarioApp>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntFiltersUsuario pEntity)", pEntity));

            try
            {
                var ListUsuarios = _dbContext.Usuario.OrderByDescending(i => i.dtFechaCreacion).AsNoTracking().AsQueryable();

                if (pEntity.sNombreCompleto != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => (i.sNombre.ToLower() + " " + i.sApellidoPaterno.ToLower() + " " + i.sApellidoMaterno.ToLower()).Contains(pEntity.sNombreCompleto.ToLower()));
                }
                if (pEntity.sTelefono != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.sTelefono.Contains(pEntity.sTelefono));
                }
                if (pEntity.sCorreo != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.sCorreo.ToLower().Contains(pEntity.sCorreo.ToLower()));
                }
                if (pEntity.sCURP != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.sCURP.ToUpper().Contains(pEntity.sCURP.ToUpper()));
                }
                if (pEntity.uIdMonedero != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.uIdMonedero == pEntity.uIdMonedero);
                }
                if (pEntity.uIdUsuario != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.uIdUsuario == pEntity.uIdUsuario);
                }
                if (pEntity.bMonedero != null)
                {
                    if (pEntity.bMonedero == true)
                    {
                        ListUsuarios = ListUsuarios.Where(i => i.uIdMonedero.HasValue);
                    }
                    else
                    {
                        ListUsuarios = ListUsuarios.Where(i => !i.uIdMonedero.HasValue);
                    }
                }
                if (pEntity.bMigrado != null)
                {
                    ListUsuarios = ListUsuarios.Where(i => i.bMigrado == pEntity.bMigrado);
                }

                ListUsuarios = ListUsuarios.Where(i => i.bActivo == true && i.bBaja == false);

                var totalReg = ListUsuarios.Count();

                if (totalReg == 0)
                {
                    response.SetSuccess(new(pEntity.iPage, pEntity.iRegistros, 0), Menssages.DatNoExistRegister);
                    return response;
                }

                if (totalReg <= pEntity.iRegistros)
                    pEntity.iPage = 1;

                var DataTemp = !pEntity.bExportar ? IMDPagination.ApplyPagination(ListUsuarios, pEntity.iPage, pEntity.iRegistros) : ListUsuarios;

                var paginator = new EntPagination<EntUsuarioApp>(pEntity.iPage, pEntity.iRegistros, ListUsuarios.Count());

                var query = from User in DataTemp
                            select new EntUsuarioApp
                            {
                                sNombreCompleto = User.sNombre + " " + User.sApellidoPaterno + " " + User.sApellidoMaterno,
                                Nombre = new Nombre
                                {
                                    sNombre = User.sNombre,
                                    sApellidoPaterno = User.sApellidoPaterno,
                                    sApellidoMaterno = User.sApellidoMaterno
                                },
                                sLada = string.IsNullOrEmpty(User.sLada) ? "52" : User.sLada,
                                sTelefono = User.sTelefono,
                                sCorreo = User.sCorreo,
                                bCuentaVerificada = User.bCuentaVerificada,
                                dtFechaNacimiento = User.dtFechaNacimiento,
                                sCURP = User.sCURP,
                                cGenero = User.cGenero,
                                bEsGoogle = string.IsNullOrEmpty(User.uIdRedSocialGoogle) ? false : true,
                                bEsFacebook = string.IsNullOrEmpty(User.uIdRedSocialFaceBook) ? false : true,
                                bEsApple = string.IsNullOrEmpty(User.uIdRedSocialApple) ? false : true,
                                sFotografia = User.sFotografia,
                                dtFechaCreacion = User.dtFechaCreacion,
                                dtFechaModificacion = User.dtFechaModificacion,
                                dtFechaBaja = User.dtFechaBaja,
                                bActivo = User.bActivo,
                                bBaja = User.bBaja,
                                uIdUsuario = User.uIdUsuario,
                                bMonedero = string.IsNullOrEmpty(User.uIdMonedero.ToString()) ? false : true,
                                uIdMonedero = User.uIdMonedero,
                                bMigrado = User.bMigrado.HasValue ? User.bMigrado.Value : false,
                                sIdAplicacion = User.sIdAplicacion,
                                sEstatusCuenta = User.iEstatusCuenta.ToString()
                            };

                var usuarios = query.ToList();

                paginator.Datos = usuarios;

                response.SetSuccess(paginator);
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntFiltersUsuario pEntity): {ex.Message}", pEntity, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465596096, 67823465595319)]
        public async Task<IMDResponse<List<EntUsuarioAppInfo>>> DGetUsuarioAppInfo(Guid uIdUsuario)
        {
            IMDResponse<List<EntUsuarioAppInfo>> response = new IMDResponse<List<EntUsuarioAppInfo>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(Guid uIdUsuario)", uIdUsuario));

            try
            {
                List<EntUsuarioAppInfo> listEntUsuarioAppInfo = new List<EntUsuarioAppInfo>();

                var lstGroupFirebaseToken = await _dbContext.FirebaseToken.Where(f => f.uIdUsuario == uIdUsuario && f.bActivo)
                    .GroupBy(g => new { g.uIdUsuario, g.sFcmToken }).ToListAsync();

                if (lstGroupFirebaseToken.Any())
                {
                    foreach (var grupo in lstGroupFirebaseToken)
                    {
                        EntUsuarioAppInfo usuarioAppInfo = new EntUsuarioAppInfo();

                        usuarioAppInfo.uIdFirebaseToken = grupo.Select(x => x.uIdFirebaseToken).FirstOrDefault();
                        usuarioAppInfo.sFcmToken = grupo.Select(x => x.sFcmToken).FirstOrDefault();
                        usuarioAppInfo.sInfoAppOS = grupo.Select(x => x.sInfoAppOS).FirstOrDefault();
                        usuarioAppInfo.dtFechaCreacion = grupo.Select(x => x.dtFechaCreacion).FirstOrDefault();
                        usuarioAppInfo.dtFechaModificacion = grupo.Select(x => x.dtFechaModificacion).FirstOrDefault();

                        listEntUsuarioAppInfo.Add(usuarioAppInfo);
                    }

                    response.SetSuccess(listEntUsuarioAppInfo, Menssages.DatTokensSucess);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(Guid uIdUsuario): {ex.Message}", uIdUsuario, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465633392, 67823465632615)]
        public async Task<IMDResponse<bool>> DUpdateUsuarioByMonedero(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo}(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero)", entUpdateUsuarioByMonedero));

            try
            {
                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(user => user.uIdMonedero == entUpdateUsuarioByMonedero.uIdMonedero);
                if (usuario != null)
                {
                    dynamic userUpdate = new ExpandoObject();
                    userUpdate.sNombre = entUpdateUsuarioByMonedero.sNombre;
                    userUpdate.sApellidoPaterno = entUpdateUsuarioByMonedero.sApellidoPaterno;
                    userUpdate.sApellidoMaterno = entUpdateUsuarioByMonedero.sApellidoMaterno;
                    userUpdate.dtFechaNacimiento = entUpdateUsuarioByMonedero.dtFechaNacimiento;
                    userUpdate.sCURP = entUpdateUsuarioByMonedero.sCURP;
                    userUpdate.cGenero = entUpdateUsuarioByMonedero.cGenero;
                    userUpdate.dtFechaModificacion = DateTime.Now;
                    userUpdate.uIdUsuarioModificacion = entUpdateUsuarioByMonedero.uIdUsuario;
                    userUpdate.iEstatusCuenta = entUpdateUsuarioByMonedero.iEstatusCuentaApp;

                    _dbContext.Entry(usuario).CurrentValues.SetValues(userUpdate);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatUpdateFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo}(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero): {ex.Message}", entUpdateUsuarioByMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466293065, 67823466293842)]
        public async Task<IMDResponse<bool>> DActualizaEstatusCuenta(EntEstatusCuentaUpdate entEstatusCuentaUpdate)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entEstatusCuentaUpdate));
            try
            {
                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(user => user.uIdUsuario == entEstatusCuentaUpdate.uIdUsuario);
                if (usuario != null)
                {
                    dynamic userUpdate = new ExpandoObject();
                    userUpdate.dtFechaModificacion = DateTime.Now;
                    userUpdate.uIdUsuarioModificacion = entEstatusCuentaUpdate.uIdUsuario;
                    userUpdate.iEstatusCuenta = entEstatusCuentaUpdate.iEstatusCuenta;

                    _dbContext.Entry(usuario).CurrentValues.SetValues(userUpdate);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatUpdateFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entEstatusCuentaUpdate, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466303943, 67823466304720)]
        public async Task<IMDResponse<bool>> DActualizaDispositivoCuenta(EntDispositivoCuentaUpdate entDispositivoCuentaUpdate)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entDispositivoCuentaUpdate));
            try
            {
                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(user => user.uIdUsuario == entDispositivoCuentaUpdate.uIdUsuario);
                if (usuario != null)
                {
                    dynamic userUpdate = new ExpandoObject();
                    userUpdate.dtFechaModificacion = DateTime.Now;
                    userUpdate.uIdUsuarioModificacion = entDispositivoCuentaUpdate.uIdUsuario;
                    userUpdate.sIdAplicacion = entDispositivoCuentaUpdate.sIdAplicacion;
                    userUpdate.iEstatusCuenta = entDispositivoCuentaUpdate.iEstatusCuenta;

                    _dbContext.Entry(usuario).CurrentValues.SetValues(userUpdate);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatUpdateFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entDispositivoCuentaUpdate, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466305497, 67823466306274)]
        public async Task<IMDResponse<List<EntFirebaseToken>>> DGetListFirebaseToken(Guid uIdUsuario, string sIdAplicacion)
        {
            IMDResponse<List<EntFirebaseToken>> response = new IMDResponse<List<EntFirebaseToken>>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", uIdUsuario, sIdAplicacion));

            try
            {
                var query = _dbContext.FirebaseToken.Where(f => f.uIdUsuario == uIdUsuario && f.bActivo && f.sIdAplicacion != sIdAplicacion);

                var tmp = query.AsEnumerable()
                   .Select(group => new
                   {
                       Group = group,
                       MaxDate = group.dtFechaModificacion is null ? group.dtFechaCreacion : group.dtFechaModificacion
                   })
                   .OrderByDescending(o => o.MaxDate)
                   .ToList();

                List<FirebaseToken> lstGroupFirebaseToken = tmp.Select(x => x.Group).ToList();

                List<FirebaseToken> lsEliminar = new List<FirebaseToken>();
                List<string> lsConservar = new List<string>();
                lstGroupFirebaseToken.ForEach(x =>
                {
                    if (!lsConservar.Contains(x.sFcmToken))
                        lsConservar.Add(x.sFcmToken);
                    else
                        lsEliminar.Add(x);
                });

                lstGroupFirebaseToken.RemoveAll(x => lsEliminar.Contains(x));

                if (lstGroupFirebaseToken.Any())
                {
                    var result = _mapper.Map<List<EntFirebaseToken>>(lstGroupFirebaseToken);
                    response.SetSuccess(result, Menssages.DatTokensSucess);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = metodo.iCodigoError;
                response.SetError(ex.Message);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", uIdUsuario, sIdAplicacion, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466361441, 67823466362218)]
        public async Task<IMDResponse<bool>> DUpdateEstatusCuentaByMonedero(EntUpdateEstatusCuentaByMonedero entUpdateEstatusCuentaByMonedero)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entUpdateEstatusCuentaByMonedero));

            try
            {
                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(user => user.uIdMonedero == entUpdateEstatusCuentaByMonedero.uIdMonedero);
                if (usuario != null)
                {
                    dynamic userUpdate = new ExpandoObject();
                    userUpdate.dtFechaModificacion = DateTime.Now;
                    userUpdate.uIdUsuarioModificacion = entUpdateEstatusCuentaByMonedero.uIdUsuario;
                    userUpdate.iEstatusCuenta = entUpdateEstatusCuentaByMonedero.iEstatusCuentaApp;

                    _dbContext.Entry(usuario).CurrentValues.SetValues(userUpdate);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatUpdateFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entUpdateEstatusCuentaByMonedero, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466383197, 67823466383974)]
        public async Task<IMDResponse<bool>> DActualizarEstatusCuenta(EntActualizarEstatusCuenta entActualizarEstatusCuenta)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod()!.GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entActualizarEstatusCuenta));
            try
            {
                var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(user => user.uIdUsuario == entActualizarEstatusCuenta.uIdUsuario);
                if (usuario != null)
                {
                    dynamic userUpdate = new ExpandoObject();
                    userUpdate.dtFechaModificacion = DateTime.Now;
                    userUpdate.uIdUsuarioModificacion = entActualizarEstatusCuenta.uIdUsuario;
                    userUpdate.iEstatusCuenta = entActualizarEstatusCuenta.iEstatusCuenta;

                    _dbContext.Entry(usuario).CurrentValues.SetValues(userUpdate);
                    int i = await _dbContext.SaveChangesAsync();
                    if (i != 0)
                    {
                        response.SetSuccess(true);
                    }
                    else
                    {
                        response.SetError(Menssages.DatUpdateFailed);
                    }
                }
                else
                {
                    response.SetError(Menssages.DatUserNoExist);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);

                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entActualizarEstatusCuenta, ex, response));
            }
            return response;
        }
        #endregion
    }
}
