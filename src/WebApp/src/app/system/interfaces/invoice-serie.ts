export class InvoiceSerie {
  constructor(
    public id: any = undefined,
    public name: string = '',
    public warehouse: any = undefined,
    public factura: string = '',
    public counterFactura: number = 0,
    public boleta: string = '',
    public counterBoleta: number = 0,
    public notaDeVenta: string = '',
    public counterNotaDeVenta: number = 0,
    public creditNote: string = '',
    public counterCreditNote: number = 0,
    public debitNote: string = '',
    public counterDebitNote: number = 0) {
  }
}
