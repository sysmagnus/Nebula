import {Component, OnInit} from '@angular/core';
import {faEdit, faSearch, faTrashAlt} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-invoice-note-list',
  templateUrl: './invoice-note-list.component.html',
  styleUrls: ['./invoice-note-list.component.scss']
})
export class InvoiceNoteListComponent implements OnInit {
  faSearch = faSearch;
  faEdit = faEdit;
  faTrashAlt = faTrashAlt;

  constructor() {
  }

  ngOnInit(): void {
  }

}