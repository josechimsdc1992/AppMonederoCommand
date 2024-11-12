CREATE TABLE TipoMovimiento
(
    uIdTipoMovimiento RAW(16) NOT NULL,
    sNombre VARCHAR(50) NOT NULL,
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT Monedero_PK PRIMARY KEY (uIdTipoMovimiento)
)

CREATE TABLE Monedero
(
    iIdMonedero RAW(16),
    dSaldo NUMERIC(16, 2),
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT Monedero_PK PRIMARY KEY (iIdMonedero)
)

CREATE TABLE Transferencia
(
    uIdTransferencia RAW(16) NOT NULL,
    uIdMonederoOrigen RAW(16) NOT NULL,
    uIdMonederoDestino RAW(16) NOT NULL,
    dImporte NUMERIC(16, 2) NOT NULL,
    fLatitud FLOAT NULL,
    fLongitud FLOAT NULL,
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT Transferencia_PK PRIMARY KEY (uIdTipoMovimiento)
)

CREATE TABLE Movimientos
(
    uIdMovimento RAW(16) NOT NULL,
    uIdMonedero RAW(16) NOT NULL,
    uIdTipoMovimiento RAW(16) NOT NULL,
    sTipoMovimiento VARCHAR(50) NULL,
    dSaldoAnterior NUMERIC(16, 2) NOT NULL,
    dImporte NUMERIC(16, 2) NOT NULL,
    dSaldoActual NUMERIC(16, 2) NOT NULL,
    dtFechaOperacion DATETIME NOT NULL,
    uIdReferencia RAW(16) NULL,
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT Movimientos_PK PRIMARY KEY (uIdMovimiento)
)

CREATE TABLE PerfilTarjeta
(
    uIdPerfil RAW(16) NOT NULL,
    sNombre VARCHAR(50) NOT NULL,
    sDescirpcion VARCHAR(100) NOT NULL,
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT PerfilTarjeta_PK PRIMARY KEY (uIdPerfil),
)

CREATE TABLE TarjetaUsuario
(
    uIdTarjeta RAW(16) NOT NULL,
    uIdMonedero RAW(16) NULL,
    uIdUsuario RAW(16) NULL,
    sNumeroTarjeta VARCHAR(100 BYTE) NOT NULL,
    sNombreTitular VARCHAR(200 BYTE) NULL,
    dSaldo NUMERIC(16, 2) DEFAULT 0 NOT NULL,
    iIdEstadoTarjeta INT NOT NULL,
    sNoMonedero VARCHAR(200 BYTE) NULL,
    sNumSerie VARCHAR(100 BYTE) NULL,
    uIdPerfil RAW(16) NOT NULL,
    dtFechaVigenciaUnica DATETIME NULL,
    dtFechaVigenciaUnicaFin DATETIME NULL,
    dtFechaCreacion DATETIME NOT NULL,
    dtFechaModificacion DATETIME NULL,
    dtFechaBaja DATETIME NULL,
    bActivo NUMBER(1,0),
    uIdUsuarioCreacion RAW(16) NOT NULL,
    uIdUsuarioModificacion RAW(16) NULL,
    uIdUsuarioBaja RAW(16) NULL,
    CONSTRAINT fk_Monedero
        FOREIGN KEY (uIdMonedero)
        REFERENCES Monedero(uIdMonedero),
    CONSTRAINT fk_Usuario
        FOREIGN KEY (uIdUsuario)
        REFERENCES Usuario(uIdUsuario),
    CONSTRAINT fk_TipoPerfil
        FOREIGN KEY (uIdPerfil)
        REFERENCES TipoPerfil(uIdPerfil),
    CONSTRAINT TarjetaUsuario_PK PRIMARY KEY (uId),
)
comment on column TarjetaUsuario.uId is 'Identificador de la tabla'
comment on column TarjetaUsuario.uIdMonedero is 'Este campo es referencia al identificador de Monedero'
comment on column TarjetaUsuario.uIdUsuario is 'Este campo es referencia al identificador de Usuario'
comment on column TarjetaUsuario.sNumeroTarjeta is 'Número de tarjeta'
comment on column TarjetaUsuario.sNombreTitular is 'Nombre del titular de la tarjeta'
comment on column TarjetaUsuario.dSaldo is 'Saldo de la tarjeta'
comment on column TarjetaUsuario.iIdEstadoTarjeta is 'Estado de la tarjeta'
comment on column TarjetaUsuario.sNoMonedero is 'Número de Monedero'
comment on column TarjetaUsuario.sNumSerie is 'Número de serie de la tarjeta'
comment on column TarjetaUsuario.uIdPerfil is 'Este campo es ferefencia al identificador de Perfil'
comment on column TarjetaUsuario.dtFechaVigencia is 'Fecha inicio de vigencia'
comment on column TarjetaUsuario.dtFechaVigenciaUnicaFin is 'Fecha fin de vigencia'
comment on column TarjetaUsuario.dtFechaCreacion is 'Fecha de creación de la tarjeta'
comment on column TarjetaUsuario.dtFechaModificacion is 'Fecha de modificaión de la tarjeta'
comment on column TarjetaUsuario.dtFechaBaja is 'Fecha de baja de la tarjeta'
comment on column TarjetaUsuario.bActivo is 'Indica si la tarjeta esta activa'
comment on column TarjetaUsuario.uIdUsuarioCreacion is 'Este campo es referencia al Usuario que creo'
comment on column TarjetaUsuario.uIdUsuarioModificacion is 'Este campo es referencia al Usuario que modifico'
comment on column TarjetaUsuario.uIdUsuarioBaja is 'Este campo es referencia al Usuario que dio de baja'