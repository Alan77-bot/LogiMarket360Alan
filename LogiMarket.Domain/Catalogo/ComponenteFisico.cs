namespace LogiMarket.Domain.Catalogo;
// Responsabilidad de ComponenteFisico: colaborar desde LogiMarket.Domain.Catalogo sin mezclar reglas, persistencia y pantallas.
public sealed record ComponenteFisico(string Sku, int Cantidad, decimal Precio, decimal Peso);
