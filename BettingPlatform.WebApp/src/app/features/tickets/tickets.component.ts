import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketsService, TicketSummaryDto } from '../../api';
import { PlayerContextService } from '../../core/state/player-context.service';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-tickets',
  imports: [CommonModule, RouterModule],
  templateUrl: './tickets.component.html',
  styleUrls: ['./tickets.component.scss']
})
export class TicketsComponent implements OnInit {
  tickets: TicketSummaryDto[] = [];
  loading = false;
  error: string | null = null;

  constructor(private api: TicketsService, private ctx: PlayerContextService) {}

  ngOnInit(): void {
    this.ctx.current$.subscribe(p => {
      if (!p) {
        this.tickets = [];
        this.error = 'Select a player (Players tab).';
        return;
      }
      this.loadFor(p.id);
    });
  }

  private loadFor(playerId: string) {
    this.loading = true;
    this.error = null;

    this.api.apiTicketsPlayerPlayerIdGet(playerId).subscribe({
      next: (list) => { this.tickets = list ?? []; this.loading = false; },
      error: (e) => {
        this.loading = false;
        console.error(e);
        this.error = e?.messages?.join(', ') ?? 'Failed to load tickets.';
      }
    });
  }

  trackById = (_: number, t: TicketSummaryDto) => t.ticketId;
}
