import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api, errMsg } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-cows',
  imports: [FormsModule, DatePipe],
  templateUrl: './cows.html'
   
})
export class Cows implements OnInit {
  private api = inject(Api);
  auth = inject(AuthService);
  cows = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);
  filter = '';
  editId = 0;
  form: any = this.blank();

  blank() {
    return { tagNumber: '', name: '', breed: '', cowType: 'New', dateOfBirth: '', purchaseDate: '', purchasePrice: null, status: 'Active' };
  }

  ngOnInit() { this.load(); }

  load() { this.api.get('cows', { type: this.filter }).subscribe(r => this.cows.set(r)); }

  save() {
    const body = {
      ...this.form,
      name: this.form.name || null,
      breed: this.form.breed || null,
      dateOfBirth: this.form.dateOfBirth || null,
      purchaseDate: this.form.purchaseDate || null,
      purchasePrice: this.form.purchasePrice || null
    };
    const req = this.editId ? this.api.put(`cows/${this.editId}`, body) : this.api.post('cows', body);
    req.subscribe({
      next: () => { this.show('Saved', true); this.reset(); this.load(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  edit(c: any) {
    this.editId = c.cowId;
    this.form = {
      ...c,
      dateOfBirth: c.dateOfBirth?.slice(0, 10) ?? '',
      purchaseDate: c.purchaseDate?.slice(0, 10) ?? ''
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  remove(c: any) {
    if (!confirm(`Delete cow ${c.tagNumber}?`)) return;
    this.api.delete(`cows/${c.cowId}`).subscribe({
      next: (r: any) => { this.show(r?.message ?? 'Deleted', true); this.load(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  reset() { this.editId = 0; this.form = this.blank(); }
  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}

