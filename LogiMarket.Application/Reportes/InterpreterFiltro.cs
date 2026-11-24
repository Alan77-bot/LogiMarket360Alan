using LogiMarket.Domain.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Patron Interpreter: construye expresiones para filtros con AND, OR y parentesis; no concatena SQL.
// Interpreter: descenso recursivo; AND tiene prioridad sobre OR.
public sealed class InterpreterFiltro
{
    private readonly List<string> tokens = [];
    private int posicion;
    public IExpresionFiltro Analizar(string texto)
    {
        tokens.Clear();
        posicion = 0;
        var patron = "\\G\\s*(>=|=|\\(|\\)|\"[^\"]*\"|[\\p{L}_][\\p{L}\\p{N}_]*|[0-9]+(?:\\.[0-9]+)?)";
        var offset = 0;
        while (offset < texto.Length && !string.IsNullOrWhiteSpace(texto[offset..]))
        {
            var match = new Regex(patron, RegexOptions.None, TimeSpan.FromSeconds(1)).Match(texto, offset);
            if (!match.Success)
                throw new FormatException($"Simbolo invalido en posicion {offset}.");
            tokens.Add(match.Groups[1].Value);
            offset = match.Index + match.Length;
        }

        var arbol = LeerOr();
        if (posicion != tokens.Count)
            throw new FormatException("Sobran elementos en el filtro.");
        return arbol;
    }

    private IExpresionFiltro LeerOr()
    {
        var nodo = LeerAnd();
        while (Aceptar("OR"))
            nodo = new Disyuncion(nodo, LeerAnd());
        return nodo;
    }

    private IExpresionFiltro LeerAnd()
    {
        var nodo = LeerAtomo();
        while (Aceptar("AND"))
            nodo = new Conjuncion(nodo, LeerAtomo());
        return nodo;
    }

    private IExpresionFiltro LeerAtomo()
    {
        if (Aceptar("("))
        {
            var nodo = LeerOr();
            Exigir(")");
            return nodo;
        }

        var campo = Tomar().ToUpperInvariant();
        if (campo == "TOTAL")
        {
            Exigir(">=");
            if (!decimal.TryParse(Tomar(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var n))
                throw new FormatException("Importe invalido.");
            return new TotalExpresion(n);
        }

        Exigir("=");
        var valor = Tomar().Trim('"');
        if (campo == "CLIENTE")
            return new ClienteExpresion(valor);
        if (campo == "ESTADO" && Enum.TryParse<EstadoPedido>(valor, true, out var estado) && Enum.IsDefined(estado))
            return new EstadoExpresion(estado);
        throw new FormatException("Campo o estado desconocido.");
    }

    private string Tomar() => posicion < tokens.Count ? tokens[posicion++] : throw new FormatException("Filtro incompleto.");
    private bool Aceptar(string valor)
    {
        if (posicion >= tokens.Count || !tokens[posicion].Equals(valor, StringComparison.OrdinalIgnoreCase))
            return false;
        posicion++;
        return true;
    }

    private void Exigir(string valor)
    {
        if (!Aceptar(valor))
            throw new FormatException($"Se esperaba {valor}.");
    }
}
