using System;
using System.Collections.Generic;
using System.Linq;
using TesteCandidatoTriangulo.Interfaces;

namespace TesteCandidatoTriangulo
{
    public class Triangulo : ITriangulo
    {
        #region Regras
        /// <summary>
        ///    6
        ///   3 5
        ///  9 7 1
        /// 4 6 8 4
        /// Um elemento somente pode ser somado com um dos dois elementos da próxima linha. Como o elemento 5 na Linha 2 pode ser somado com 7 e 1, mas não com o 9.
        /// Neste triangulo o total máximo é 6 + 5 + 7 + 8 = 26
        /// 
        /// Seu código deverá receber uma matriz (multidimensional) como entrada. O triângulo acima seria: [[6],[3,5],[9,7,1],[4,6,8,4]]
        /// </summary>
        /// <param name="dadosTriangulo"></param>
        /// <returns>Retorna o resultado do calculo conforme regra acima</returns>
        #endregion

        public int ResultadoTriangulo(string dadosTriangulo)
        {
            string[] linhasString = dadosTriangulo.Split(';');

            List<List<int>> triangulo = new List<List<int>>(); //para versão c# 9 ou acima pode usar o new();

            foreach (string linha in linhasString)
            {
                List<int> numerosLinha = new List<int>();
                foreach (var numStr in linha.Trim().Split(' ', (char)StringSplitOptions.RemoveEmptyEntries))
                {
                    numerosLinha.Add(int.Parse(numStr));
                }

                triangulo.Add(numerosLinha);
            }

            // -- Explicação do funcionamento -- 
            //Começar na penúltima linha
            //somar os que estão na última 
            //subir + 1 com a soma feita entre a última e penúltima e assim por diante

            //Exemplo no console

            for (int linhaAtual = triangulo.Count - 2; linhaAtual >= 0; linhaAtual--) 
            {
                Console.WriteLine(
                     $"Analisando linha [{string.Join(", ", triangulo[linhaAtual])}] e as somadas abaixo [{string.Join(", ", triangulo[linhaAtual + 1])}]"
                );

                for (int posicaoNumeroAbaixo = 0; posicaoNumeroAbaixo < triangulo[linhaAtual].Count; posicaoNumeroAbaixo++)
                {
                    int abaixoEsquerda = triangulo[linhaAtual + 1][posicaoNumeroAbaixo];
                    int abaixoDireita = triangulo[linhaAtual + 1][posicaoNumeroAbaixo + 1];

                    Console.WriteLine(
                        $"O maior número abaixo (entre esquerda e direita) " +
                        $"é {Math.Max(abaixoEsquerda, abaixoDireita)}");

                    triangulo[linhaAtual][posicaoNumeroAbaixo] += Math.Max(abaixoEsquerda, abaixoDireita);
                }

                Console.WriteLine();
            }

            return triangulo[0][0];
        }

        public List<List<int>> LerTrianguloDoUsuario()
        {
            var triangulo = new List<List<int>>();
            Console.WriteLine("Digite o triângulo, linha por linha. Pressione Enter em branco para finalizar:");

            while (true)
            {
                string linha = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(linha))
                    break;

                var numeros = linha
                    .Trim()
                    .Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                triangulo.Add(numeros);
            }

            return triangulo;
        }
    }
}
