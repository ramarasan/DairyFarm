import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api, errMsg } from '../../services/api';

@Component({
  selector: 'app-users',
  imports: [FormsModule], 
  templateUrl: './users.html'
})
export class Users implements OnInit {
  private api = inject(Api);
  users = signal<any[]>([]);
  msg = signal('');
  ok = signal(false);
  form: any = this.blank();

  blank() { return { fullName: '', username: '', password: '', role: 'User' }; }

  ngOnInit() { this.load(); }
  load() { this.api.get('auth/users').subscribe(r => this.users.set(r)); }

  save() {
    this.api.post('auth/users', this.form).subscribe({
      next: () => { this.show('User created', true); this.form = this.blank(); this.load(); },
      error: e => this.show(errMsg(e), false)
    });
  }

  toggle(u: any) {
    this.api.put(`auth/users/${u.userId}/active?active=${!u.isActive}`).subscribe({
      next: () => this.load(), error: e => this.show(errMsg(e), false)
    });
  }

  private show(m: string, ok: boolean) { this.msg.set(m); this.ok.set(ok); }
}
