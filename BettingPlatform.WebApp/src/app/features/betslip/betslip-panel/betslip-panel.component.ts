import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { TicketsService } from '../../../api';
import { BetslipService } from '../../../core/state/betslip.service';
import { PlayerContextService } from '../../../core/state/player-context.service';

@Component({
  selector: 'app-betslip-panel',
  standalone: true,
  templateUrl: './betslip-panel.component.html',
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe]
})
export class BetslipPanelComponent implements OnInit {
  stake = 10;
  placing = false;
  messages: string[] = [];
  selections$ = this.slip.selections$;

  constructor(
    public slip: BetslipService,
    private tickets: TicketsService,
    private ctx: PlayerContextService
  ) {}

  ngOnInit() {
    this.slip.stake$.subscribe(v => this.stake = v ?? 10);
  }

  setStake(v: any) { this.slip.setStake(Number(v)); }
  remove(offerId: string) { this.slip.removeByOffer(offerId); }
  get combinedOdds() { return this.slip.combinedOdds(); }

  async place() {
    const { stake, selections } = this.slip.getSnapshot();
    if (!selections.length) return;

    const current = this.ctx.snapshot;
    if (!current) {
      this.messages = ['Select a player first (Players tab).'];
      return;
    }

    const body = {
      playerId: current.id,
      stake,
      selections: selections.map(s => ({
        offerId: s.offerId,
        outcomeTemplateId: s.outcomeTemplateId
      }))
    };

    this.messages = [];
    try {
      this.placing = true;
      const dto = await firstValueFrom(this.tickets.apiTicketsPost(body));
      this.messages = [`Ticket ${dto.ticketId} placed. Payout: ${dto.potentialPayout}`];
      this.slip.clear();
    } catch (e: any) {
      this.messages = e?.messages ?? ['Error'];
    } finally {
      this.placing = false;
    }
  }
}
