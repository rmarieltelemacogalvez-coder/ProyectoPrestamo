using System;
using System.Collections.Generic;
using Spectre.Console;

namespace ProyectoAmortizacion
{
    class Program
    {
        static void Main(string[] args)
        {
            MostrarTitulo();

            var datos = CapturarDatos();

            if (!ValidarDatos(datos.monto, datos.tasaAnual, datos.plazo))
                return;

            var tabla = GenerarTabla(datos.monto, datos.tasaAnual, datos.plazo);

            MostrarResultados(tabla);
        }

        static void MostrarTitulo()
        {
            AnsiConsole.Write(
                new FigletText("Tabla de Amortizacion")
                .Color(Color.Green)
                .Centered());
        }

        static (decimal monto, decimal tasaAnual, int plazo) CapturarDatos()
        {
            decimal monto = AnsiConsole.Ask<decimal>("\n[yellow]Digite el monto del préstamo:[/]");
            decimal tasaAnual = AnsiConsole.Ask<decimal>("[yellow]Digite la tasa de interés anual (%):[/]");
            int plazo = AnsiConsole.Ask<int>("[yellow]Digite la cantidad de meses a pagar:[/]");

            return (monto, tasaAnual, plazo);
        }

        static bool ValidarDatos(decimal monto, decimal tasaAnual, int plazo)
        {
            if (monto <= 0 || tasaAnual <= 0 || plazo <= 0)
            {
                AnsiConsole.MarkupLine("\n[red]Error: Todos los valores deben ser mayores que cero.[/]");
                return false;
            }
            return true;
        }

        static Table GenerarTabla(decimal monto, decimal tasaAnual, int plazo)
        {
            decimal tasaMensual = (tasaAnual / 100) / 12;
            decimal potencia = (decimal)Math.Pow((double)(1 + tasaMensual), plazo);
            decimal cuota = monto * (tasaMensual * potencia) / (potencia - 1);
            cuota = Math.Round(cuota, 2);

            decimal saldoPendiente = monto;

            var tabla = new Table();
            tabla.Border(TableBorder.Rounded);
            tabla.AddColumn(new TableColumn("[green]Mes[/]").Centered());
            tabla.AddColumn(new TableColumn("[yellow]Cuota[/]").Centered());
            tabla.AddColumn(new TableColumn("[red]Interés[/]").Centered());
            tabla.AddColumn(new TableColumn("[blue]Capital[/]").Centered());
            tabla.AddColumn(new TableColumn("[magenta]Saldo[/]").Centered());

            for (int mes = 1; mes <= plazo; mes++)
            {
                decimal interes = Math.Round(saldoPendiente * tasaMensual, 2);
                decimal pagoCapital = Math.Round(cuota - interes, 2);

                if (mes == plazo)
                {
                    pagoCapital = saldoPendiente;
                    cuota = interes + pagoCapital;
                }

                saldoPendiente = Math.Round(saldoPendiente - pagoCapital, 2);
                if (saldoPendiente < 0)
                    saldoPendiente = 0;

                tabla.AddRow(
                    mes.ToString(),
                    cuota.ToString("C2"),
                    interes.ToString("C2"),
                    pagoCapital.ToString("C2"),
                    saldoPendiente.ToString("C2")
                );
            }

            return tabla;
        }

        static void MostrarResultados(Table tabla)
        {
            AnsiConsole.Write(tabla);
            AnsiConsole.MarkupLine("\n[green]Tabla generada correctamente.[/]");
        }
    }
}
