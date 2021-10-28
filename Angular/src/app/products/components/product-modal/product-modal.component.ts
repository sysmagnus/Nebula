import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {faBars, faSearch, faTrashAlt} from '@fortawesome/free-solid-svg-icons';
import {FormBuilder, FormGroup} from '@angular/forms';
import {ProductService, UndMedidaService} from '../../services';
import {Product, UndMedida} from '../../interfaces';
import {ResponseData} from '../../../global/interfaces';

@Component({
  selector: 'app-product-modal',
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})
export class ProductModalComponent implements OnInit {
  faBars = faBars;
  faSearch = faSearch;
  faTrashAlt = faTrashAlt;
  // ====================================================================================================
  undMedidas: Array<UndMedida> = new Array<UndMedida>();
  productForm: FormGroup = this.fb.group({
    id: [null],
    description: [''],
    barcode: [''],
    icbper: [''],
    price: [0],
    igvSunat: [''],
    type: [''],
    undMedidaId: [null]
  });
  fileImage: any;
  @ViewChild('inputFile')
  inputFile: ElementRef | undefined;
  // ====================================================================================================
  @Input()
  title: string = '';
  @Input()
  product: Product = new Product();
  @Output()
  responseData = new EventEmitter<ResponseData<Product>>();

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private undMedidaService: UndMedidaService) {
  }

  ngOnInit(): void {
    // cargar las unidades de medidas.
    this.undMedidaService.index().subscribe(result => this.undMedidas = result);
    // suscribir formGroup.
    this.productForm.valueChanges.subscribe(value => this.product = value);
    // cargar valores por defecto.
    if (document.querySelector('#product-modal')) {
      const myModal: any = document.querySelector('#product-modal');
      myModal.addEventListener('shown.bs.modal', () => {
        this.productForm.reset({...this.product});
        // @ts-ignore
        this.inputFile?.nativeElement.value = null;
        this.fileImage = null;
      });
    }
  }

  // seleccionar imagen.
  public selectedImage(event: any): void {
    this.fileImage = event.target.files[0];
  }

  // guardar todos los cambios.
  public saveChanges(): void {
    const formData = new FormData();
    formData.append('description', this.product.description);
    formData.append('barcode', this.product.barcode);
    formData.append('icbper', this.product.icbper);
    formData.append('price', this.product.price.toString());
    formData.append('igvSunat', this.product.igvSunat);
    formData.append('type', this.product.type);
    formData.append('undMedidaId', this.product.undMedidaId);
    if (this.fileImage) formData.append('file', this.fileImage);
    if (this.productForm.get('id')?.value == null) {
      this.productService.store(formData)
        .subscribe(result => {
          this.responseData.emit(result);
        });
    } else {
      formData.append('id', <any>this.product.id);
      this.productService.update(<any>this.product.id, formData)
        .subscribe(result => {
          this.responseData.emit(result);
        });
    }
  }

}
