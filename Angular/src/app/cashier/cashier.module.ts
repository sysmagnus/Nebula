import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';

import {CashierRoutingModule} from './cashier-routing.module';
import {GlobalModule} from '../global/global.module';
import {CashDetailComponent} from './pages/cash-detail/cash-detail.component';
import {CajaDiariaComponent} from './pages/caja-diaria/caja-diaria.component';
import {CashInOutModalComponent} from './components/cash-in-out-modal/cash-in-out-modal.component';
import {TerminalComponent} from './pages/terminal/terminal.component';
import {CobrarModalComponent} from './components/cobrar-modal/cobrar-modal.component';
import {OffcanvasComponent} from './components/offcanvas/offcanvas.component';

@NgModule({
  declarations: [
    CajaDiariaComponent,
    CashDetailComponent,
    CashInOutModalComponent,
    TerminalComponent,
    CobrarModalComponent,
    OffcanvasComponent,
  ],
  imports: [
    CommonModule,
    CashierRoutingModule,
    GlobalModule,
    FontAwesomeModule
  ]
})
export class CashierModule {
}