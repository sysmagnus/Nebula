import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {faBars, faTrashAlt} from '@fortawesome/free-solid-svg-icons';
import {faWhatsapp} from '@fortawesome/free-brands-svg-icons';
import {faCheckSquare} from '@fortawesome/free-regular-svg-icons';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {environment} from 'src/environments/environment';
import {FacturadorService, InvoiceService} from 'src/app/invoice/services';
import {confirmTask} from 'src/app/global/interfaces';
import {Sale} from '../../interfaces';

@Component({
  selector: 'app-cobrar-modal',
  templateUrl: './cobrar-modal.component.html',
  styleUrls: ['./cobrar-modal.component.scss']
})
export class CobrarModalComponent implements OnInit {
  faBars = faBars;
  faCheckSquare = faCheckSquare;
  faTrashAlt = faTrashAlt;
  faWhatsapp = faWhatsapp;
  // ====================================================================================================
  private appURL: string = environment.applicationUrl;
  @Input()
  cajaDiariaId: number = 0;
  @Input()
  sale: Sale = new Sale();
  @Output()
  responseData = new EventEmitter<boolean>();
  cobrarForm: FormGroup = this.fb.group({
    paymentMethod: ['0', [Validators.required]],
    docType: ['BOLETA', [Validators.required]],
    montoTotal: [null, [Validators.required, Validators.min(0)]],
    remark: ['']
  });
  formReg: boolean = true;
  urlPdf: string = '';
  statusProgress: boolean = false;

  constructor(
    private fb: FormBuilder,
    private invoiceService: InvoiceService,
    private facturadorService: FacturadorService) {
  }

  ngOnInit(): void {
    const myModal: any = document.querySelector('#cobrar-modal');
    myModal.addEventListener('hide.bs.modal', () => {
      if (!this.formReg) {
        this.responseData.emit(this.formReg);
        this.cobrarForm.reset({paymentMethod: 0, docType: 'BOLETA', montoTotal: null, remark: ''});
      }
    });
    myModal.addEventListener('hidden.bs.modal', () => {
      if (!this.formReg) {
        this.formReg = true;
      }
    });
  }

  // Verificar campo invalido.
  public inputIsInvalid(field: string) {
    return this.cobrarForm.controls[field].errors && this.cobrarForm.controls[field].touched;
  }

  public cobrarVenta(): void {
    if (this.cobrarForm.invalid) {
      this.cobrarForm.markAllAsTouched();
      return;
    }
    // Guardar datos, sólo si es válido el formulario.
    confirmTask().then(result => {
      if (result.isConfirmed) {
        this.statusProgress = true;
        this.sale = {...this.sale, ...this.cobrarForm.value};
        // guardar comprobante de pago.
        this.invoiceService.createQuickSale(this.cajaDiariaId, this.sale)
          .subscribe(result => {
            if (result.ok) {
              if (result.data) {
                const {data} = result;
                this.sale = data;
                this.urlPdf = `${this.appURL}Facturador/GetPdf?invoice=${data?.invoiceId}`;
                console.info(result.msg);
                if (data?.docType === 'NOTA') {
                  this.statusProgress = false;
                  this.formReg = false;
                } else {
                  // cargar la Lista de archivos JSON.
                  this.facturadorService.ActualizarPantalla()
                    .subscribe(result => {
                      if (result.listaBandejaFacturador.length > 0) {
                        // generar fichero XML del comprobante.
                        this.facturadorService.GenerarComprobante(data.invoiceId)
                          .subscribe(result => {
                            if (result.listaBandejaFacturador.length > 0) {
                              // generar fichero PDF del comprobante.
                              this.facturadorService.GenerarPdf(data.invoiceId)
                                .subscribe(result => {
                                  this.statusProgress = false;
                                  this.formReg = false;
                                  console.info(result);
                                });
                            }
                          });
                      }
                    });
                }
              }
            }
          });
      }
    });
  }

}
