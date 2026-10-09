import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard'; 
import { Login } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { adminGuard } from './guards/admin-guard';
import { Layout } from './layout/layout'; 
import { Cows } from './pages/cows/cows';
import { MilkEntry } from './pages/milk-entry/milk-entry';
import { DoctorVisits } from './pages/doctor-visits/doctor-visits';
import { LoadUnload } from './pages/load-unload/load-unload';
import { Stock } from './pages/stock/stock';
import { Users } from './pages/users/users';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },   // default page = login
  { path: 'login', component: Login },
  //{ path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
    {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'cows', component: Cows },
      { path: 'milk-entry', component: MilkEntry },
      { path: 'doctor-visits', component: DoctorVisits },
      { path: 'load-unload', component: LoadUnload },
      { path: 'stock', component: Stock },
      { path: 'users', component: Users, canActivate: [adminGuard] }
    ]
  },
  { path: '**', redirectTo: 'login' }                     // unknown URL -> login
];