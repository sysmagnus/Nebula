import {Component, OnInit} from '@angular/core';
import {faBox, faCashRegister, faCogs, faUsers} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  faBox = faBox;
  faCashRegister = faCashRegister;
  faCogs = faCogs;
  faUsers = faUsers;

  constructor() {
  }

  ngOnInit(): void {
  }

}
