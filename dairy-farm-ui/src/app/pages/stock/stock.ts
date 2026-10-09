import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api, errMsg } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-stock',
  imports: [FormsModule, DecimalPipe],
  templateUrl:'./stock.html'
})
export class Stock implements OnInit {
  private api = inject(Api);
  auth = inject(AuthService);
  items = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);
  form: any = this.blank();

  blank() { return { itemName: '', unit: 'kg', currentQty: 0, reorderLevel: 0 }; }

  ngOnInit() { this.load(); }
  load() { this.api.get('stock').subscribe(r => this.items.set(r)); }

  save() {
    const body = { ...this.form, currentQty: Number(this.form.currentQty) || 0, reorderLevel: Number(this.form.reorderLevel) || 0 };
    this.api.post('stock', body).subscribe({
      next: () => { this.show('Item added', true); this.form = this.blank(); this.load(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  remove(s: any) {
    if (!confirm(`Delete ${s.itemName}?`)) return;
    this.api.delete(`stock/${s.itemId}`).subscribe({ next: () => this.load(), error: e => this.show(errMsg(e), false) });
  }

  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}
