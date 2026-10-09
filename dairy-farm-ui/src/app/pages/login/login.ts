import { Component, ChangeDetectorRef  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  username = '';
  password = '';
  error = '';
  loading = false;

  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  onLogin():void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {console.log('Success:', res); this.router.navigate(['/dashboard'])},
      error: (err) => {
        console.log('Login Error:', err);
        this.error = err.error?.message ||
      err?.error?.error ||
      err?.message ||'Cannot connect to server';
        this.loading = false;

        // Force UI refresh
        this.cdr.detectChanges();
      },
      complete: () => {
        this.loading = false;
      }
    });
    console.log(this.error);
  }

  clearError(): void {
    this.error = '';
  }
}