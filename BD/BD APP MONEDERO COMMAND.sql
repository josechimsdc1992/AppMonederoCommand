CREATE SEQUENCE APPMONEDEROCOMMAND.SECUENCIA_FOLIOS INCREMENT BY 1 MINVALUE 1306 MAXVALUE 9999999999999999999999999999 NOCYCLE NOCACHE NOORDER;

CREATE SEQUENCE APPMONEDEROCOMMAND.SECUENCIA_FOLIOS_RECARGAS INCREMENT BY 1 MINVALUE 657 MAXVALUE 9999999999999999999999999999 NOCYCLE NOCACHE NOORDER;

CREATE TABLE "APPMONEDEROCOMMAND"."PARAMETROS" 
   (	"SNOMBRE" VARCHAR2(100) NOT NULL ENABLE, 
	"SVALOR" VARCHAR2(150) NOT NULL ENABLE, 
	"DTFECHACREACION" DATE NOT NULL ENABLE, 
	"DTFECHAMODIFICACION" DATE, 
	"DTFECHABAJA" DATE, 
	"BACTIVO" NUMBER(1,0), 
	"BBAJA" NUMBER(1,0), 
	"UIDUSUARIOCREACION" VARCHAR2(50), 
	"UIDUSUARIOBAJA" VARCHAR2(50), 
	"UIDPARAMETRO" VARCHAR2(50), 
	"UIDUSUARIOMODIFICACION" VARCHAR2(50), 
	"BENCRIPTADO" NUMBER(1,0), 
	"SDESCRIPCION" VARCHAR2(300), 
	 CONSTRAINT "PARAMETROS_PK" PRIMARY KEY ("UIDPARAMETRO")
   ) 


CREATE TABLE "APPMONEDEROCOMMAND"."TIPOSTARIFA" 
   (	"UIDTIPOTARIFA" VARCHAR2(50), 
	"STIPOTARIFA" VARCHAR2(100), 
	"SCLAVETIPOTARIFA" VARCHAR2(10), 
	"ITIPOTARJETA" NUMBER DEFAULT 0, 
	 CONSTRAINT "SYS_C008470" CHECK ("UIDTIPOTARIFA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C008471" CHECK ("STIPOTARIFA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "TARIFAS_PK" PRIMARY KEY ("UIDTIPOTARIFA")
   ) 

CREATE TABLE "APPMONEDEROCOMMAND"."TIPOOPERACIONES" 
   (	"UIDTIPOOPERACION" VARCHAR2(50) NOT NULL ENABLE, 
	"SNOMBRE" VARCHAR2(100) NOT NULL ENABLE, 
	"SCLAVE" VARCHAR2(100) NOT NULL ENABLE, 
	"IMODULO" NUMBER NOT NULL ENABLE, 
	"BACTIVO" NUMBER(1,0) NOT NULL ENABLE, 
	"BBAJA" NUMBER(1,0) NOT NULL ENABLE
   ) 
   
   
   CREATE TABLE "APPMONEDEROCOMMAND"."MOTIVOS" 
   (	"UIDMOTIVO" VARCHAR2(50), 
	"SMOTIVO" VARCHAR2(100), 
	"BACTIVO" NUMBER(1,0) DEFAULT 1, 
	"BBAJA" NUMBER(1,0) DEFAULT 0, 
	"SDESCRIPCION" VARCHAR2(100), 
	"ITIPO" NUMBER(10,0) DEFAULT 0, 
	"BPERMITIROPERACIONES" NUMBER(1,0) DEFAULT 1, 
	"BPERMITIRREACTIVAR" NUMBER(1,0) DEFAULT 1, 
	"BPERMITIREDITAR" NUMBER(1,0) DEFAULT 0, 
	 CONSTRAINT "SYS_C0012547" CHECK ("ITIPO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C0012548" CHECK ("BPERMITIROPERACIONES" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C0012549" CHECK ("BPERMITIRREACTIVAR" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C0012603" CHECK ("BPERMITIREDITAR" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009057" CHECK ("UIDMOTIVO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009058" CHECK ("SMOTIVO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009060" CHECK ("BACTIVO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009061" CHECK ("BBAJA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "MOTIVOS_PK" PRIMARY KEY ("UIDMOTIVO")
   ) 
   

CREATE TABLE APPMONEDEROCOMMAND.USUARIO (
	UIDUSUARIOTT RAW(16) NULL,
	SNOMBRE VARCHAR2(200) NOT NULL,
	SAPELLIDOPATERNO VARCHAR2(200) NOT NULL,
	SAPELLIDOMATERNO VARCHAR2(200) NULL,
	STELEFONO VARCHAR2(10) NULL,
	SCORREO VARCHAR2(250) NULL,
	SCONTRASENA VARCHAR2(250) NULL,
	SCODIGOVERIFICACION VARCHAR2(100) NULL,
	BCUENTAVERIFICADA NUMBER(1,0) DEFAULT 0  NOT NULL,
	DTFECHANACIMIENTO DATE NULL,
	SCURP VARCHAR2(18) NULL,
	CGENERO VARCHAR2(20) NOT NULL,
	UIDREDSOCIALGOOGLE VARCHAR2(100) NULL,
	SREDSOCIALGOOGLE VARCHAR2(20) NULL,
	UIDREDSOCIALFACEBOOK VARCHAR2(100) NULL,
	SREDSOCIALFACEBOOK VARCHAR2(20) NULL,
	UIDREDSOCIALAPPLE VARCHAR2(100) NULL,
	SREDSOCIALAPPLE VARCHAR2(20) NULL,
	SFOTOGRAFIA CLOB NULL,
	BMIGRADO NUMBER(1,0) DEFAULT 1  NOT NULL,
	DTFECHAVENCIMIENTOCONTRASENA TIMESTAMP NULL,
	DTFECHACREACION TIMESTAMP NOT NULL,
	DTFECHAMODIFICACION TIMESTAMP NULL,
	DTFECHABAJA TIMESTAMP NULL,
	UIDMONEDEROTT RAW(16) NULL,
	BACTIVO NUMBER(1,0) DEFAULT 1  NOT NULL,
	BBAJA NUMBER(1,0) DEFAULT 0   NOT NULL,
	UIDUSUARIOCREACIONTT RAW(16) NULL,
	UIDUSUARIOMODIFICACIONTT RAW(16) NULL,
	UIDUSUARIOBAJATT RAW(16) NULL,
	UIDUSUARIO VARCHAR2(50) NOT NULL,
	UIDMONEDERO VARCHAR2(50) NULL,
	UIDUSUARIOCREACION VARCHAR2(50) NULL,
	UIDUSUARIOMODIFICACION VARCHAR2(50) NULL,
	UIDUSUARIOBAJA VARCHAR2(50) NULL,
	SLADA VARCHAR2(10) NULL,
	SIDAPLICACION VARCHAR2(50) NULL,
	IESTATUSCUENTA NUMBER(2,0) DEFAULT 1  NOT NULL,
	CONSTRAINT SYS_C0010981 CHECK ("SNOMBRE" IS NOT NULL),
	CONSTRAINT SYS_C0010982 CHECK ("SAPELLIDOPATERNO" IS NOT NULL),
	CONSTRAINT SYS_C0010987 CHECK ("DTFECHACREACION" IS NOT NULL),
	CONSTRAINT SYS_C0011093 CHECK ("BCUENTAVERIFICADA" IS NOT NULL),
	CONSTRAINT SYS_C0011094 CHECK ("BMIGRADO" IS NOT NULL),
	CONSTRAINT SYS_C0011095 CHECK ("BACTIVO" IS NOT NULL),
	CONSTRAINT SYS_C0011463 CHECK ("BBAJA" IS NOT NULL),
	CONSTRAINT SYS_C0017026 CHECK ("CGENERO" IS NOT NULL),
	CONSTRAINT SYS_C0017938 CHECK ("IESTATUSCUENTA" IS NOT NULL),
	CONSTRAINT USUARIO_PK PRIMARY KEY (UIDUSUARIO)
);

-- APP.FIREBASETOKEN definition

CREATE TABLE "APPMONEDEROCOMMAND"."FIREBASETOKEN" 
   (	"UIDFIREBASETOKENTT" RAW(16), 
	"UIDUSUARIOTT" RAW(16), 
	"SFCMTOKEN" VARCHAR2(200) NOT NULL ENABLE, 
	"DTFECHACREACION" TIMESTAMP (6) NOT NULL ENABLE, 
	"DTFECHAMODIFICACION" TIMESTAMP (6), 
	"DTFECHABAJA" TIMESTAMP (6), 
	"BACTIVO" NUMBER(1,0), 
	"BBAJA" NUMBER(1,0), 
	"SINFOAPPOS" VARCHAR2(700), 
	"UIDFIREBASETOKEN" VARCHAR2(50) NOT NULL ENABLE, 
	"UIDUSUARIO" VARCHAR2(50), 
	"UIDUSUARIOCREACION" VARCHAR2(50), 
	"UIDUSUARIOMODIFICACION" VARCHAR2(50), 
	"UIDUSUARIOBAJA" VARCHAR2(50), 
	"SIDAPLICACION" VARCHAR2(50), 
	 CONSTRAINT "FIREBASETOKEN_PK" PRIMARY KEY ("UIDFIREBASETOKEN")
  
   ) 

-- APP.ESTADODECUENTA definition

CREATE TABLE "APPMONEDEROCOMMAND"."ESTADODECUENTA" 
   (	"UIDESTADODECUENTA" VARCHAR2(50), 
	"UIDMONEDERO" VARCHAR2(50), 
	"UIDTIPOTARIFA" VARCHAR2(50), 
	"UIDULTIMAOPERACION" VARCHAR2(50), 
	"UIDESTATUS" VARCHAR2(50), 
	"INUMMONEDERO" NUMBER(16,0), 
	"DSALDO" NUMBER(18,2), 
	"STIPOTARIFA" VARCHAR2(20), 
	"STELEFONO" VARCHAR2(20), 
	"SESTATUS" VARCHAR2(20), 
	"BACTIVO" NUMBER(1,0), 
	"BBAJA" NUMBER(1,0), 
	"DTFECHAULTIMAOPERACION" TIMESTAMP (6), 
	"DTFECHAULTIMOABONO" TIMESTAMP (6), 
	"DTFECHACREACION" DATE, 
	"DTFECHABAJA" TIMESTAMP (6), 
	"SNOMBRE" VARCHAR2(200), 
	"SAPELLIDOPATERNO" VARCHAR2(200), 
	"SAPELLIDOMATERNO" VARCHAR2(200), 
	"SCORREO" VARCHAR2(200), 
	"SFECHAVIGENCIA" VARCHAR2(20), 
	"DTFECHANACIMIENTO" TIMESTAMP (6), 
	"UIDTIPOMONEDERO" VARCHAR2(50), 
	"STIPOMONEDERO" VARCHAR2(20), 
	"INUMTARJETA" NUMBER(16,0), 
	"UIDMOTIVO" VARCHAR2(50), 
	"SPANHASH" VARCHAR2(100), 
	 CONSTRAINT "SYS_C009971" CHECK ("UIDESTADODECUENTA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009972" CHECK ("UIDMONEDERO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009973" CHECK ("UIDTIPOTARIFA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009974" CHECK ("UIDULTIMAOPERACION" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009975" CHECK ("UIDESTATUS" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009976" CHECK ("INUMMONEDERO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009977" CHECK ("DSALDO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009978" CHECK ("STIPOTARIFA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009980" CHECK ("SESTATUS" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009981" CHECK ("BACTIVO" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009982" CHECK ("BBAJA" IS NOT NULL) ENABLE, 
	 CONSTRAINT "SYS_C009984" CHECK ("DTFECHACREACION" IS NOT NULL) ENABLE, 
	 CONSTRAINT "ESTADODECUENTA_PK" PRIMARY KEY ("UIDESTADODECUENTA")
  
   ) 
   
   -- CREDENCIALIZACION.TARJETAS definition

CREATE TABLE "APPMONEDEROCOMMAND"."TARJETAS" 
   (	"UIDTARJETA" VARCHAR2(50) DEFAULT SYS_GUID(), 
	"INUMEROMONEDERO" NUMBER(16,0) NOT NULL ENABLE, 
	"INUMEROTARJETA" NUMBER(16,0) NOT NULL ENABLE, 
	"DTFECHAFABRICACION" DATE, 
	"SVIGENCIA" VARCHAR2(20), 
	"UIDESTATUSTARJETA" VARCHAR2(50) NOT NULL ENABLE, 
	"SNOMBREPROVEEDOR" VARCHAR2(100), 
	"BVENDIDA" NUMBER(1,0) NOT NULL ENABLE, 
	"BASOCIADA" NUMBER(1,0) NOT NULL ENABLE, 
	"UIDTIPOTARIFA" VARCHAR2(50) NOT NULL ENABLE, 
	"DTFECHAMODIFICACION" DATE, 
	"DTFECHABAJA" DATE, 
	"BACTIVO" NUMBER(1,0), 
	"BBAJA" NUMBER(1,0), 
	"UIDUSUARIOCREACION" VARCHAR2(50) NOT NULL ENABLE, 
	"UIDUSUARIOMODIFICACION" VARCHAR2(50), 
	"UIDUSUARIOBAJA" VARCHAR2(50) NOT NULL ENABLE, 
	"DTFECHACREACION" DATE DEFAULT SYSDATE NOT NULL ENABLE, 
	"UIDSOLICITUD" VARCHAR2(50) NOT NULL ENABLE, 
	"SCCV" VARCHAR2(3) NOT NULL ENABLE, 
	"BINICIALIZADA" NUMBER(1,0), 
	"STELEFONO" VARCHAR2(50), 
	"UIDMONEDERO" VARCHAR2(50), 
	"SFOLIO" VARCHAR2(14), 
	"INUMEROPROVEEDOR" NUMBER(10,0) NOT NULL ENABLE, 
	"UIDMOTIVO" VARCHAR2(50), 
	"SSKU" VARCHAR2(16), 
	"UIDUSUARIOTARJETA" VARCHAR2(50), 
	"SPANHASH" VARCHAR2(100), 
	"DTFECHAVALIDEZ" DATE, 
	"UIDDETALLESOLICITUD" VARCHAR2(50), 
	 CONSTRAINT "TARJETAS_PK" PRIMARY KEY ("UIDTARJETA"), 
	 CONSTRAINT "PANHASH_UNIQUE" UNIQUE ("SPANHASH")
   ) 

COMMENT ON COLUMN APPMONEDEROCOMMAND.TARJETAS.DTFECHAVALIDEZ IS 'Fecha de la vigencia de tarjeta';

