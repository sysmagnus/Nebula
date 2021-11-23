import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {TransferListComponent} from './pages/transfer-list/transfer-list.component';
import {HistoryListComponent} from './pages/history-list/history-list.component';
import {NoteFormComponent} from './pages/note-form/note-form.component';
import {TransferFormComponent} from './pages/transfer-form/transfer-form.component';
import {InputNoteListComponent} from './pages/input-note-list/input-note-list.component';
import {OutputNoteListComponent} from './pages/output-note-list/output-note-list.component';

const routes: Routes = [{
  path: '',
  children: [
    {path: '', component: HistoryListComponent},
    {path: 'input-note', component: InputNoteListComponent},
    {path: 'output-note', component: OutputNoteListComponent},
    {path: 'note-form/:type', component: NoteFormComponent},
    {path: 'transfer', component: TransferListComponent},
    {path: 'transfer/form', component: TransferFormComponent},
    {path: '**', redirectTo: ''}
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventoryRoutingModule {
}
