using ClosedXML.Excel;
using Nebula.Database.Models.Common;
using Nebula.Database.Models.Sales;

namespace Nebula.Database.Dto.Sales;

public class ExportarReporteMensual
{
    private readonly List<InvoiceSerie> invoiceSeries;
    private readonly List<InvoiceSale> invoiceSales;
    private readonly List<CreditNote> creditNotes;

    public ExportarReporteMensual(List<InvoiceSerie> invoiceSeries,
        List<InvoiceSale> invoiceSales, List<CreditNote> creditNotes)
    {
        this.invoiceSeries = invoiceSeries;
        this.invoiceSales = invoiceSales;
        this.creditNotes = creditNotes;
    }

    private List<InvoiceSale> GetBoletas(string invoiceSerieId)
    {
        return invoiceSales.Where(x => x.DocType == "BOLETA"
        && x.InvoiceSerieId == invoiceSerieId).OrderBy(x => x.Number).ToList();
    }

    private List<InvoiceSale> GetFacturas(string invoiceSerieId)
    {
        return invoiceSales.Where(x => x.DocType == "FACTURA"
        && x.InvoiceSerieId == invoiceSerieId).OrderBy(x => x.Number).ToList();
    }

    public string GuardarCambios()
    {
        string fileName = Guid.NewGuid().ToString();
        string filePath = Path.Combine(Path.GetTempPath(), $"{fileName}.xlsx");
        using (var workbook = new XLWorkbook())
        {
            invoiceSeries.ForEach(serieComprobante =>
            {
                var worksheet = workbook.Worksheets.Add(serieComprobante.Name);
                // generar cabecera documento.
                worksheet.Cell("A1").Value = "FECHA";
                worksheet.Cell("B1").Value = "N° BOLETA";
                worksheet.Cell("C1").Value = "IMPORTE TOTAL";
                worksheet.Cell("D1").Value = "ESTADO SUNAT";
                worksheet.Cell("E1").Value = "ANULADO";
                // F1 - empty.
                worksheet.Cell("G1").Value = "FECHA";
                worksheet.Cell("H1").Value = "N° FACTURA";
                worksheet.Cell("I1").Value = "IMPORTE TOTAL";
                worksheet.Cell("J1").Value = "ESTADO SUNAT";
                worksheet.Cell("K1").Value = "ANULADO";
                // formato cabecera documento.
                var cabeceraBoletas = worksheet.Range("A1:E1");
                cabeceraBoletas.Style.Font.Bold = true;
                cabeceraBoletas.Style.Font.FontColor = XLColor.White;
                cabeceraBoletas.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                var cabeceraFacturas = worksheet.Range("G1:K1");
                cabeceraFacturas.Style.Font.Bold = true;
                cabeceraFacturas.Style.Font.FontColor = XLColor.White;
                cabeceraFacturas.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                // generar registro de boletas.            
                int contador = 2;
                List<InvoiceSale> boletas = GetBoletas(serieComprobante.Id);
                foreach (var item in boletas)
                {
                    worksheet.Cell(contador, 1).Value = item.FecEmision;
                    worksheet.Cell(contador, 2).Value = $"{item.Serie}-{item.Number}";
                    worksheet.Cell(contador, 3).Value = item.SumImpVenta;
                    worksheet.Cell(contador, 4).Value = item.SituacionFacturador;
                    worksheet.Cell(contador, 5).Value = item.Anulada ? "SI" : "NO";
                    contador++;
                }
                #region NOTA_CRÉDITO!
                contador = contador + 2;
                worksheet.Cell(contador, 1).Value = "FECHA";
                worksheet.Cell(contador, 2).Value = "N° NOTA CRÉDITO";
                worksheet.Cell(contador, 3).Value = "IMPORTE TOTAL";
                worksheet.Cell(contador, 4).Value = "ESTADO SUNAT";
                worksheet.Cell(contador, 5).Value = "DOC. AFECTADO";
                var cabeceraNotaBoletas = worksheet.Range($"A{contador}:E{contador}");
                cabeceraNotaBoletas.Style.Font.Bold = true;
                cabeceraNotaBoletas.Style.Font.FontColor = XLColor.White;
                cabeceraNotaBoletas.Style.Fill.BackgroundColor = XLColor.DarkRed;

                #endregion
                // generar registro de facturas.
                contador = 2;
                List<InvoiceSale> facturas = GetFacturas(serieComprobante.Id);
                foreach (var item in facturas)
                {
                    worksheet.Cell(contador, 7).Value = item.FecEmision;
                    worksheet.Cell(contador, 8).Value = $"{item.Serie}-{item.Number}";
                    worksheet.Cell(contador, 9).Value = item.SumImpVenta;
                    worksheet.Cell(contador, 10).Value = item.SituacionFacturador;
                    worksheet.Cell(contador, 11).Value = item.Anulada ? "SI" : "NO";
                    contador++;
                }
                #region NOTA_CRÉDITO!
                contador = contador + 2;
                worksheet.Cell(contador, 7).Value = "FECHA";
                worksheet.Cell(contador, 8).Value = "N° NOTA CRÉDITO";
                worksheet.Cell(contador, 9).Value = "IMPORTE TOTAL";
                worksheet.Cell(contador, 10).Value = "ESTADO SUNAT";
                worksheet.Cell(contador, 11).Value = "DOC. AFECTADO";
                var cabeceraNotaFacturas = worksheet.Range($"G{contador}:K{contador}");
                cabeceraNotaFacturas.Style.Font.Bold = true;
                cabeceraNotaFacturas.Style.Font.FontColor = XLColor.White;
                cabeceraNotaFacturas.Style.Fill.BackgroundColor = XLColor.DarkRed;

                #endregion
                // ajustar ancho de las columnas para que se muestren todo el contenido.
                worksheet.Columns(1, 5).AdjustToContents();
                worksheet.Columns(7, 11).AdjustToContents();
            });
            workbook.SaveAs(filePath);
        }
        return filePath;
    }
}
