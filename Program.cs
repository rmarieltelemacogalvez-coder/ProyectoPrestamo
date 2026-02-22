using System;
using Spectre.Console;

namespace Proyectoprestamo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Título
            AnsiConsole.Write(
                new FigletText("Tabla de de prestamo")
                .Color(Color.Green)
                .Centered());

            // Solicitar datos
            decimal monto = AnsiConsole.Ask<decimal>("\n[yellow]Digite el monto del préstamo:[/]");
            decimal tasaAnual = AnsiConsole.Ask<decimal>("[yellow]Digite la tasa de interés anual (%):[/]");
            int plazo = AnsiConsole.Ask<int>("[yellow]Digite la cantidad de meses a pagar:[/]");

            // Validación
            if (monto <= 0 || tasaAnual <= 0 || plazo <= 0)
            {
                AnsiConsole.MarkupLine("\n[red]Error: Todos los valores deben ser mayores que cero.[/]");
                return;
            }

            // Convertir tasa anual a mensual
            decimal tasaMensual = (tasaAnual / 100) / 12;

            // Calcular cuota mensual (fórmula correcta)
            decimal potencia = (decimal)Math.Pow((double)(1 + tasaMensual), plazo);
            decimal cuota = monto * (tasaMensual * potencia) / (potencia - 1);

            decimal saldoPendiente = monto;

            // Crear tabla
            var tabla = new Table();
            tabla.Border(TableBorder.Rounded);
            tabla.AddColumn("Mes");
            tabla.AddColumn("Cuota");
            tabla.AddColumn("Interés");
            tabla.AddColumn("Capital");
            tabla.AddColumn("Saldo");

            for (int mes = 1; mes <= plazo; mes++)
            {
                decimal interes = saldoPendiente * tasaMensual;
                decimal pagoCapital = cuota - interes;

                // Ajuste en el último mes
                if (mes == plazo)
                {
                    pagoCapital = saldoPendiente;
                    cuota = interes + pagoCapital;
                }

                saldoPendiente -= pagoCapital;

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

            AnsiConsole.Write(tabla);

            AnsiConsole.MarkupLine("\n[green]Tabla generada correctamente.[/]");
        }
    }
}