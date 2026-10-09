
import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api, errMsg } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-load-unload',
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './load-unload.html'
})
export class LoadUnload implements OnInit {
  private api = inject(Api);
  auth = inject(AuthService);
  items = signal<any[]>([]);
  list = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);
  filter = '';
  form: any = this.blank();

  blank() { return { direction: 'Unload', itemId: null, quantity: null, vehicleNo: '', partyName: '', remarks: '' }; }

  ngOnInit() { this.loadItems(); this.load(); }
  loadItems() { this.api.get('stock').subscribe(r => this.items.set(r)); }
  load() { this.api.get('loadunload', { direction: this.filter }).subscribe(r => this.list.set(r)); }

  save() {
    const body = {
      ...this.form,
      quantity: Number(this.form.quantity),
      vehicleNo: this.form.vehicleNo || null,
      partyName: this.form.partyName || null,
      remarks: this.form.remarks || null
    };
    this.api.post('loadunload', body).subscribe({
      next: () => { this.show('Saved. Stock updated.', true); this.form = this.blank(); this.load(); this.loadItems(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  remove(t: any) {
    if (!confirm('Delete this entry? Stock quantity will NOT change.')) return;
    this.api.delete(`loadunload/${t.txnId}`).subscribe({ next: () => this.load(), error: e => this.show(errMsg(e), false) });
  }

  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}

