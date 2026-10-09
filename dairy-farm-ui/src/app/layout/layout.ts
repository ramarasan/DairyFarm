import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
 templateUrl:'./layout.html'
})
export class Layout {
  auth = inject(AuthService);
  private router = inject(Router);
  logout() { this.auth.logout(); this.router.navigate(['/login']); }
}
