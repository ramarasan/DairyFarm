import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common'; 
import { Api} from '../../services/api'

@Component({
  selector: 'app-dashboard',
  imports: [DecimalPipe, DatePipe],
  templateUrl: './dashboard.html'
})
export class Dashboard implements OnInit {
  private api = inject(Api);
  data = signal<any>(null);
  days = signal<{ date: string; litres: number }[]>([]);
  max = computed(() => Math.max(1, ...this.days().map(d => d.litres)));

  ngOnInit() {
    this.api.get('dashboard').subscribe(d => this.data.set(d));
    this.api.get('milk/summary', { days: 7 }).subscribe(r => this.days.set(r));
  }
}
