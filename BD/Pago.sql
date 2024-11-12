CREATE TABLE PAGO 
(
  UIDUSUARIO RAW(15) NOT NULL 
, DMONTO NUMBER(16,2) NOT NULL 
, DTFECHAPAGO DATE NOT NULL 
, SMETODOPAGO VARCHAR2(20) NOT NULL 
, DTFECHACREACION DATE NOT NULL 
, DTFECHAMODIFICACION DATE 
, DTFECHABAJA DATE 
, BACTIVO NUMBER(1,0) NOT NULL 
, BBAJA NUMBER(1,0) 
, UIDUSUARIOCREACION RAW(16) NOT NULL 
, UIDUSUARIOMODIFICACION RAW(16) 
, UIDUSUARIOBAJA RAW(16) 
);

COMMENT ON COLUMN PAGO.UIDUSUARIO IS 'Identificador �nico de la Tarjeta';

COMMENT ON COLUMN PAGO.DMONTO IS 'Monto de cobro';

COMMENT ON COLUMN PAGO.DTFECHAPAGO IS 'Fecha de registro de Pago';

COMMENT ON COLUMN PAGO.SMETODOPAGO IS 'M�todo de pago seleccionado ';

COMMENT ON COLUMN PAGO.DTFECHACREACION IS 'Fecha de creaci�n';

COMMENT ON COLUMN PAGO.DTFECHAMODIFICACION IS 'Fecha en que se modifico';

COMMENT ON COLUMN PAGO.DTFECHABAJA IS 'Fecha de baja';

COMMENT ON COLUMN PAGO.BACTIVO IS 'Esta activo';

COMMENT ON COLUMN PAGO.BBAJA IS 'Es baja';

COMMENT ON COLUMN PAGO.UIDUSUARIOCREACION IS 'Usuario creador';

COMMENT ON COLUMN PAGO.UIDUSUARIOMODIFICACION IS 'Usuario que modifico';

COMMENT ON COLUMN PAGO.UIDUSUARIOBAJA IS 'Usuario que dio de baja';
