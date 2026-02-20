namespace ApiMoniEquipamentosPesados.Exceptions;

public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string message) : base(message) { }
}

public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string message) : base(message) { }
}