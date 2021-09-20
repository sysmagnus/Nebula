import {Component, OnInit} from '@angular/core';
import {
  faBars,
  faBox, faCashRegister,
  faCog, faSignOutAlt, faTh,
  faUserCircle
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  faUserCircle = faUserCircle;
  faSignOutAlt = faSignOutAlt;
  faCog = faCog;
  faCashRegister = faCashRegister;
  faBars = faBars;
  faBox = faBox;
  faTh = faTh;

  constructor() {
  }

  ngOnInit(): void {
  }

  // Toggle menu principal.
  mainMenuToggle(e: any) {
    e.preventDefault();
    const mainMenu: any = document.getElementById('MainMenu');
    if (mainMenu) {
      mainMenu.classList.toggle('hiddenNavigation');
      localStorage.setItem('classMainMenu', mainMenu.classList.value);
    }
  }

}
