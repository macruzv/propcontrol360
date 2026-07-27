namespace propcontrol360.Models
{
    public enum PropertyCategory
    {
        Terreno,
        Lote,
        Casa,
        Apartamento,
        BloqueCompleto
    }

    public enum PropertyStatus
    {
        Disponible,
        Preventa,
        Reservado,
        Vendido,
        Alquilado
    }

    public enum ClientCategory
    {
        Comprador,
        Inquilino,
        Inversionista,
        Propietario
    }

    public enum ContractType
    {
        Venta,
        Reserva,
        Alquiler
    }

    public enum ContractStatus
    {
        Activo,
        Completado,
        Cancelado
    }
}
