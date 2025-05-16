using System.Collections.Generic;

namespace TesteCandidatoTriangulo.Interfaces
{
    public interface ITriangulo
    {
        int ResultadoTriangulo(string dadosTriangulo);
        List<List<int>> LerTrianguloDoUsuario();
    }
}
