import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Api, errMsg, today } from '../../services/api';

@Component({
  selector: 'app-milk-entry',
  imports: [FormsModule, DecimalPipe],
  templateUrl: './milk-entry.html'
})
export class MilkEntry implements OnInit {
  private api = inject(Api);
  date = today();
  shift = 'Morning';
  rows = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);

  ngOnInit() { this.load(); }

  load() {
    this.msg.set('');
    forkJoin({
      cows: this.api.get<any[]>('cows', { status: 'Active' }),
      recs: this.api.get<any[]>('milk', { date: this.date, shift: this.shift })
    }).subscribe(({ cows, recs }) => {
      this.rows.set(cows.map(c => {
        const r = recs.find(x => x.cowId === c.cowId);
        return { cowId: c.cowId, tagNumber: c.tagNumber, name: c.name, litres: r ? r.litres : null, fat: r ? r.fat : null };
      }));
    });
  }

  total() { return this.rows().reduce((s, r) => s + (Number(r.litres) || 0), 0); }

  save() {
    const body = this.rows()
      .filter(r => r.litres !== null && r.litres !== '')
      .map(r => ({ cowId: r.cowId, recordDate: this.date, shift: this.shift, litres: Number(r.litres), fat: r.fat === null || r.fat === '' ? null : Number(r.fat) }));
    if (!body.length) { this.show('Enter litres for at least one cow', false); return; }
    this.api.post('milk/bulk', body).subscribe({
      next: (r: any) => this.show(r.message, true),
      error: e => this.show(errMsg(e), false)
    });
  }

  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}
