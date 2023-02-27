namespace Nebula.Database.Helpers;

public class NumberToLetters
{
    private readonly decimal _value;

    public NumberToLetters(decimal value)
    {
        _value = Convert.ToDecimal(value.ToString("N2"));
    }

    public override string ToString()
    {
        var entero = Convert.ToInt64(Math.Truncate(_value));
        var decimales = Convert.ToInt32(Math.Round((_value - entero) * 100, 2));
        return $"{ConvertNumber(Convert.ToDecimal(entero))} CON {decimales:0,0}/100 SOLES";
    }

    private string ConvertNumber(decimal value)
    {
        string num2Text;
        value = Math.Truncate(value);
        if (value == 0) num2Text = "CERO";
        else if (value == 1) num2Text = "UNO";
        else if (value == 2) num2Text = "DOS";
        else if (value == 3) num2Text = "TRES";
        else if (value == 4) num2Text = "CUATRO";
        else if (value == 5) num2Text = "CINCO";
        else if (value == 6) num2Text = "SEIS";
        else if (value == 7) num2Text = "SIETE";
        else if (value == 8) num2Text = "OCHO";
        else if (value == 9) num2Text = "NUEVE";
        else if (value == 10) num2Text = "DIEZ";
        else if (value == 11) num2Text = "ONCE";
        else if (value == 12) num2Text = "DOCE";
        else if (value == 13) num2Text = "TRECE";
        else if (value == 14) num2Text = "CATORCE";
        else if (value == 15) num2Text = "QUINCE";
        else if (value < 20) num2Text = "DIECI" + ConvertNumber(value - 10);
        else if (value == 20) num2Text = "VEINTE";
        else if (value < 30) num2Text = "VEINTI" + ConvertNumber(value - 20);
        else if (value == 30) num2Text = "TREINTA";
        else if (value == 40) num2Text = "CUARENTA";
        else if (value == 50) num2Text = "CINCUENTA";
        else if (value == 60) num2Text = "SESENTA";
        else if (value == 70) num2Text = "SETENTA";
        else if (value == 80) num2Text = "OCHENTA";
        else if (value == 90) num2Text = "NOVENTA";
        else if (value < 100)
            num2Text = ConvertNumber(Math.Truncate(value / 10) * 10) + " Y " + ConvertNumber(value % 10);
        else if (value == 100) num2Text = "CIEN";
        else if (value < 200) num2Text = "CIENTO " + ConvertNumber(value - 100);
        else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800))
            num2Text = ConvertNumber(Math.Truncate(value / 100)) + "CIENTOS";
        else if (value == 500) num2Text = "QUINIENTOS";
        else if (value == 700) num2Text = "SETECIENTOS";
        else if (value == 900) num2Text = "NOVECIENTOS";
        else if (value < 1000)
            num2Text = ConvertNumber(Math.Truncate(value / 100) * 100) + " " + ConvertNumber(value % 100);
        else if (value == 1000) num2Text = "MIL";
        else if (value < 2000) num2Text = "MIL " + ConvertNumber(value % 1000);
        else if (value < 1000000)
        {
            num2Text = ConvertNumber(Math.Truncate(value / 1000)) + " MIL";
            if ((value % 1000) > 0)
            {
                num2Text = num2Text + " " + ConvertNumber(value % 1000);
            }
        }
        else if (value == 1000000)
        {
            num2Text = "UN MILLÓN";
        }
        else if (value < 2000000)
        {
            num2Text = "UN MILLÓN " + ConvertNumber(value % 1000000);
        }
        else if (value < 1000000000000)
        {
            num2Text = ConvertNumber(Math.Truncate(value / 1000000)) + " MILLONES ";
            if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
            {
                num2Text = num2Text + " " + ConvertNumber(value - Math.Truncate(value / 1000000) * 1000000);
            }
        }
        else if (value == 1000000000000) num2Text = "UN BILLÓN";
        else if (value < 2000000000000)
            num2Text = "UN BILLÓN " + ConvertNumber(value - Math.Truncate(value / 1000000000000) * 1000000000000);
        else
        {
            num2Text = ConvertNumber(Math.Truncate(value / 1000000000000)) + " BILLONES";
            if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
            {
                num2Text = num2Text + " " +
                           ConvertNumber(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
        }

        return num2Text;
    }
}
