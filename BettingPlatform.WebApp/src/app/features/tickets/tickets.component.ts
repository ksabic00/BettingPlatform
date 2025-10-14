import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketsService, TicketSummaryDto } from '../../api';

@Component({
  standalone: true,
  selector: 'app-tickets',
  imports: [CommonModule],
  templateUrl: './tickets.component.html',
  styleUrls: ['./tickets.component.scss']
})
export class TicketsComponent implements OnInit {
  tickets: TicketSummaryDto[] = [];
  loading = false;
  error: string | null = null;

  constructor(private api: TicketsService) {}

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    const playerId = localStorage.getItem('playerId') ?? '';

    if (!playerId) {
      this.tickets = [];
      this.error = 'Player is not set (missing playerId in localStorage).';
      return;
    }

    this.loading = true;
    this.error = null;

    this.api.apiTicketsPlayerPlayerIdGet(playerId).subscribe({
      next: (list) => {
        this.tickets = list ?? [];
        this.loading = false;
      },
      error: (e) => {
        this.loading = false;
        console.error(e);
        this.error = e?.messages?.join(', ') ?? 'Failed to load tickets.';
      }
    });
  }

  trackById = (_: number, t: TicketSummaryDto) => t.ticketId;
}
