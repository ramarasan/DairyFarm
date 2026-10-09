
import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Api, errMsg, today } from '../../services/api';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-doctor-visits',
  imports: [FormsModule, DatePipe],
  templateUrl: './doctor-visits.html'
})
export class DoctorVisits implements OnInit {
  private api = inject(Api);
  auth = inject(AuthService);
  cows = signal<any[]>([]);
  visits = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);
  form: any = this.blank();

  blank() { return { cowId: null, visitDate: today(), doctorName: '', disease: '', treatment: '', cost: null, nextVisitDate: '' }; }

  ngOnInit() {
    this.api.get('cows', { status: 'Active' }).subscribe(r => this.cows.set(r));
    this.load();
  }

  load() { this.api.get('doctor').subscribe(r => this.visits.set(r)); }

  save() {
    const body = {
      ...this.form,
      disease: this.form.disease || null,
      treatment: this.form.treatment || null,
      cost: this.form.cost || null,
      nextVisitDate: this.form.nextVisitDate || null
    };
    this.api.post('doctor', body).subscribe({
      next: () => { this.show('Visit saved', true); this.form = this.blank(); this.load(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  remove(v: any) {
    if (!confirm('Delete this visit?')) return;
    this.api.delete(`doctor/${v.visitId}`).subscribe({ next: () => this.load(), error: e => this.show(errMsg(e), false) });
  }

  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}

