import {Component, OnInit} from '@angular/core';
import {faIdCardAlt, faPlus, faSearch, faSignOutAlt, faTrashAlt} from '@fortawesome/free-solid-svg-icons';
import {FormBuilder, FormGroup} from '@angular/forms';
import * as moment from 'moment';
import {InventoryNoteService, WarehouseService} from '../../services';
import {InventoryNote, Warehouse} from '../../interfaces';

@Component({
  selector: 'app-input-note-list',
  templateUrl: './input-note-list.component.html',
  styleUrls: ['./input-note-list.component.scss']
})
export class InputNoteListComponent implements OnInit {
  faSearch = faSearch;
  faPlus = faPlus;
  faSignOutAlt = faSignOutAlt;
  faTrashAlt = faTrashAlt;
  faIdCardAlt = faIdCardAlt;
  inventoryNotes: Array<InventoryNote> = new Array<InventoryNote>();
  warehouses: Array<Warehouse> = new Array<Warehouse>();
  filterForm: FormGroup = this.fb.group({
    warehouseId: [''],
    year: [moment().format('YYYY')],
    month: [moment().format('MM')],
  });

  constructor(
    private fb: FormBuilder,
    private warehouseService: WarehouseService,
    private inventoryNoteService: InventoryNoteService) {
  }

  ngOnInit(): void {
    // cargar lista de almacenes.
    this.warehouseService.index().subscribe(result => this.warehouses = result);
    // cargar lista de notas.
    this.getInventoryNotes();
  }

  // cargar lista de notas.
  public getInventoryNotes(): void {
    this.inventoryNoteService.index({...this.filterForm.value, noteType: 'INPUT'})
      .subscribe(result => this.inventoryNotes = result);
  }

}
