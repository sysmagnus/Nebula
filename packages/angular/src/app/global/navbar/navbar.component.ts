import {Component, OnInit} from '@angular/core';
import {faServer, faSignOutAlt, faThLarge, faUserCircle} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  faServer = faServer;
  faUserCircle = faUserCircle;
  faSignOutAlt = faSignOutAlt;
  faThLarge = faThLarge;

  constructor() {
  }

  ngOnInit(): void {
  }

}