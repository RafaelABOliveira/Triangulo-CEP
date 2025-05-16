using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TesteCandidatoTriangulo.Interfaces;

namespace TesteCandidatoTriangulo
{
    class Program
    {
        static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<ITriangulo, Triangulo>()
                .BuildServiceProvider();

            var triangulo = serviceProvider.GetRequiredService<ITriangulo>();

            var trianguloPadrao = new List<List<int>>
            {
                new List<int>{6},
                new List<int>{3, 5},
                new List<int>{9, 7, 1},
                new List<int>{4, 6, 8, 4}
            };

            Console.WriteLine("Deseja usar o valor padrão de triângulo(dadosTriangulo = \"[[6], [3, 5], [9, 7, 1], [4, 6, 8, 4]]\";)? ");
            string entrada = Console.ReadLine();

            if (entrada == "Sim")
            {
                entrada = FormatarTrianguloParaString(trianguloPadrao);
            }
            else
            {
                entrada = FormatarTrianguloParaString(triangulo.LerTrianguloDoUsuario());
            }

            int resultado = triangulo.ResultadoTriangulo(entrada);
            Console.WriteLine($"Resultado: {resultado}");

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        private static string FormatarTrianguloParaString(List<List<int>> triangulo)
        {
            return string.Join(";", triangulo.Select(linha => string.Join(" ", linha)));
        }
    }
}
