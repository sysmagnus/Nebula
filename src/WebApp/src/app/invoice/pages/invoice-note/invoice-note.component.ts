import {Component, OnInit} from '@angular/core';
import {faArrowLeft, faEdit, faIdCardAlt, faPlus, faSave, faTrashAlt} from '@fortawesome/free-solid-svg-icons';
import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import * as moment from 'moment';
import {confirmTask} from 'src/app/global/interfaces';
import {CpeDetail, CpeGeneric, NotaComprobante} from '../../interfaces';
import {InvoiceNoteService, InvoiceSaleService} from '../../services';
import Swal from 'sweetalert2';

declare var bootstrap: any;

@Component({
  selector: 'app-invoice-note',
  templateUrl: './invoice-note.component.html',
  styleUrls: ['./invoice-note.component.scss']
})
export class InvoiceNoteComponent implements OnInit {
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faSave = faSave;
  faEdit = faEdit;
  faTrashAlt = faTrashAlt;
  faIdCardAlt = faIdCardAlt;
  noteType: string = '';
  // TODO: debug -> $noteType
  notaComprobante: NotaComprobante = new NotaComprobante();
  // TODO: debug -> $invoiceNoteForm
  invoiceNoteForm: FormGroup = this.fb.group({
    startDate: [moment().format('YYYY-MM-DD'), [Validators.required]],
    docType: ['NC', [Validators.required]],
    codMotivo: ['01', [Validators.required]],
    serie: ['', [Validators.required]],
    number: ['', [Validators.required]],
    desMotivo: ['', [Validators.required]],
  });
  // TODO: debug -> $invoiceNoteId
  invoiceNoteId: number = 0;
  productId: string = '';
  itemComprobanteModal: any;
  title: string = '';

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private invoiceService: InvoiceSaleService,
    private invoiceNoteService: InvoiceNoteService) {
  }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      this.noteType = params.get('type') || '';
      this.invoiceNoteId = Number(params.get('id'));
      this.invoiceService.show(<any>params.get('invoiceId'))
        .subscribe(result => {
          this.title = `${result.docType}: ${result.serie}-${result.number}`;
          this.notaComprobante.invoiceId = params.get('invoiceId');
          // if (result.invoiceType === 'VENTA') {
          //   this.invoiceNoteForm.controls['startDate'].disable();
          //   this.invoiceNoteForm.controls['serie'].disable();
          //   this.invoiceNoteForm.controls['number'].disable();
          // }
          // cargar cabecera de nota crédito/débito.
          if (this.invoiceNoteId > 0) {
            // if (result.invoiceType === 'COMPRA') {
            //   const factura = result;
            //   this.invoiceNoteService.show(<any>params.get('id'))
            //     .subscribe(result => {
            //       this.invoiceNoteForm.controls['startDate'].setValue(result.fecEmision);
            //       this.invoiceNoteForm.controls['docType'].setValue(result.docType);
            //       this.invoiceNoteForm.controls['codMotivo'].setValue(result.codMotivo);
            //       this.invoiceNoteForm.controls['serie'].setValue(result.serie);
            //       this.invoiceNoteForm.controls['number'].setValue(result.number);
            //       this.invoiceNoteForm.controls['desMotivo'].setValue(result.desMotivo);
            //       if (result.invoiceNoteDetails.length > 0) {
            //         // cargar detalle desde la nota de crédito/débito.
            //         result.invoiceNoteDetails.forEach(item => {
            //           const itemDetail = CpeGeneric.getItemDetail(item);
            //           itemDetail.calcularItem();
            //           this.notaComprobante.addItemWithData(itemDetail);
            //         });
            //       } else {
            //         // cargar detalle desde la factura/boleta.
            //         factura.invoiceDetails.forEach(item => {
            //           const itemDetail = CpeGeneric.getItemDetail(item);
            //           itemDetail.calcularItem();
            //           this.notaComprobante.addItemWithData(itemDetail);
            //         });
            //       }
            //     });
            // }
          } else {
            // detalle de nota crédito/débito.
            // cargar detalle de factura/boleta.
            // result.invoiceDetails.forEach(item => {
            //   const itemDetail = CpeGeneric.getItemDetail(item);
            //   itemDetail.calcularItem();
            //   this.notaComprobante.addItemWithData(itemDetail);
            // });
          }
        });
    });
    // item comprobante modal.
    this.itemComprobanteModal = new bootstrap.Modal(document.querySelector('#item-comprobante'));
  }

  // borrar item comprobante.
  public deleteItem(prodId: number | any): void {
    this.notaComprobante.deleteItem(prodId);
  }

  // volver una página atrás.
  public historyBack(): void {
    window.history.back();
  }

  // abrir item comprobante modal.
  public showItemComprobanteModal(): void {
    this.productId = '';
    this.itemComprobanteModal.show();
  }

  // editar modal item-comprobante.
  public editItemComprobanteModal(id: string): void {
    this.productId = id;
    this.itemComprobanteModal.show();
  }

  // ocultar item comprobante modal.
  public hideItemComprobanteModal(data: CpeDetail): void {
    this.notaComprobante.addItemWithData(data);
    this.itemComprobanteModal.hide();
  }

  // Verificar campo invalido.
  public inputIsInvalid(field: string) {
    return this.invoiceNoteForm.controls[field].errors && this.invoiceNoteForm.controls[field].touched;
  }

  // registrar nota crédito/débito.
  public async registerNote() {
    if (this.invoiceNoteForm.invalid) {
      this.invoiceNoteForm.markAllAsTouched();
      return;
    }
    // verificar detalle de la nota.
    if (this.notaComprobante.details.length <= 0) {
      await Swal.fire(
        'Información',
        'Debe existir al menos un Item para registrar!',
        'info'
      );
      return;
    }
    // Guardar datos, sólo si es válido el formulario.
    confirmTask().then(result => {
      if (result.isConfirmed) {
        this.notaComprobante = {...this.notaComprobante, ...this.invoiceNoteForm.value};
        if (this.invoiceNoteId > 0) {
          // actualizar nota crédito/débito.
          this.invoiceNoteService.update(this.invoiceNoteId, this.notaComprobante)
            .subscribe(async (result) => {
              if (result.ok) await this.router.navigate(['/invoice/detail', this.noteType, result.data?.invoiceId]);
            });
        } else {
          // registrar nota crédito/débito.
          this.invoiceNoteService.create(this.notaComprobante)
            .subscribe(async (result) => {
              if (result.ok) await this.router.navigate(['/invoice/detail', this.noteType, result.data?.invoiceId]);
            });
        }
      }
    });
  }

}