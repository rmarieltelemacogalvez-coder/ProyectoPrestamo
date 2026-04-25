using System;
using Spectre.Console;

namespace Proyectoprestamo
{
    class Program
    {
        static decimal monto;
        static decimal tasaAnual;
        static int plazo;

        static void Main(string[] args)
        {
            MostrarTitulo();

            PedirDatos();

            if (!DatosValidos())
                return;

            decimal tasaMensual = ObtenerTasaMensual();
            decimal cuota = CalcularCuota(tasaMensual);

            var tabla = CrearTabla(tasaMensual, cuota);

            MostrarTabla(tabla);
        }

        static void MostrarTitulo()
        {
            AnsiConsole.Write(
                new FigletText("Prestamo")
                .Color(Color.Green)
                .Centered());
        }

        static void PedirDatos()
        {
            monto = AnsiConsole.Ask<decimal>("\nIngrese el monto del préstamo:");
            tasaAnual = AnsiConsole.Ask<decimal>("Ingrese la tasa anual (%):");
            plazo = AnsiConsole.Ask<int>("Ingrese la cantidad de meses:");
        }

        static bool DatosValidos()
        {
            if (monto <= 0 || tasaAnual <= 0 || plazo <= 0)
            {
                AnsiConsole.MarkupLine("[red]Los valores deben ser mayores que cero.[/]");
                return false;
            }
            return true;
        }

        static decimal ObtenerTasaMensual()
        {
            return (tasaAnual / 100) / 12;
        }

        static decimal CalcularCuota(decimal tasaMensual)
        {
            decimal factor = (decimal)Math.Pow((double)(1 + tasaMensual), plazo);
            return monto * (tasaMensual * factor) / (factor - 1);
        }

        static Table CrearTabla(decimal tasaMensual, decimal cuota)
        {
            decimal saldo = monto;

            var tabla = new Table();
            tabla.Border(TableBorder.Rounded);

            tabla.AddColumn("Mes");
            tabla.AddColumn("Cuota");
            tabla.AddColumn("Interés");
            tabla.AddColumn("Capital");
            tabla.AddColumn("Saldo");

            for (int i = 1; i <= plazo; i++)
            {
                decimal interes = saldo * tasaMensual;
                decimal capital = cuota - interes;

                if (i == plazo)
                {
                    capital = saldo;
                    cuota = interes + capital;
                }

                saldo -= capital;

                if (saldo < 0)
                    saldo = 0;

                tabla.AddRow(
                    i.ToString(),
                    cuota.ToString("C2"),
                    interes.ToString("C2"),
                    capital.ToString("C2"),
                    saldo.ToString("C2")
                );
            }

            return tabla;
        }

        static void MostrarTabla(Table tabla)
        {
            AnsiConsole.Write(tabla);
            AnsiConsole.MarkupLine("\n[green]Proceso completado.[/]");
        }
    }
}