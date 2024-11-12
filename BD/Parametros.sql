INSERT INTO parametros ( uIdParametro, sNombre,svalor , dtFechaCreacion,
BACTIVO, bBaja)
VALUES (SYS_GUID(), 'OPCIONPAGOCODI','4', (select SYS_EXTRACT_UTC(SYSTIMESTAMP) from dual),1,0);
