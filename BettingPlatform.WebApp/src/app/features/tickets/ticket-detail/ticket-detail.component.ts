import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketDetailDto, TicketsService } from '../../../api';

@Component({
  standalone: true,
  selector: 'app-ticket-detail',
  imports: [CommonModule, DatePipe, DecimalPipe],
  templateUrl: './ticket-detail.component.html',
  styleUrls: ['./ticket-detail.component.scss']
})
export class TicketDetailComponent implements OnInit {
  id = '';
  loading = false;
  error: string | null = null;
  data?: TicketDetailDto;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: TicketsService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') || '';
    if (!this.id) {
      this.error = 'Invalid ticket id.';
      return;
    }
    this.load();
  }

  load() {
    this.loading = true; this.error = null;
    this.api.apiTicketsTicketIdGet(this.id).subscribe({
      next: dto => { this.data = dto; this.loading = false; },
      error: e => { this.loading = false; this.error = e?.messages?.join(', ') ?? 'Failed to load ticket.'; }
    });
  }

  back() { this.router.navigate(['/tickets']); }

  fee() {
    if (!this.data) return 0;
    return (this.data.stakeGross ?? 0) - (this.data.stakeNet ?? 0);
  }
}
