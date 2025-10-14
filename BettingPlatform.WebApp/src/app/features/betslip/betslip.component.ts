import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import {
  TicketsService,
  PlaceTicketCommand,
  PlaceTicketSelectionDto
} from '../../api';
import { BetslipService, SlipSelection } from '../../core/state/betslip.service';
import { PlayerContextService } from '../../core/state/player-context.service';
import { ToastService } from '../../core/ui/toast.service';

@Component({
  standalone: true,
  selector: 'app-betslip-panel',
  imports: [CommonModule, FormsModule],
  templateUrl: './betslip.component.html',
  styleUrls: ['./betslip.component.scss']
})
export class BetslipComponent {
  selections$ = this.slip.selections$;
  stake$ = this.slip.stake$;

  placing = false;
  messages: string[] = [];

  constructor(
    private slip: BetslipService,
    private tickets: TicketsService,
    private ctx: PlayerContextService,
    private toast: ToastService
  ) {}

  trackSel = (_: number, s: SlipSelection) => s.offerId;

  remove(offerId: string) { this.slip.removeByOffer(offerId); }
  setStake(v: number) { this.slip.setStake(Number(v) || 0); }
  combinedOdds() { return this.slip.combinedOdds(); }

  async place() {
    this.messages = [];

    const current = this.ctx.snapshot;
    if (!current) {
      this.messages = ['Select a player first (Players tab).'];
      return;
    }

    const selections = await firstValueFrom(this.selections$);
    if (!selections?.length) {
      this.messages = ['Add at least one selection.'];
      return;
    }

    const stake = await firstValueFrom(this.stake$);
    if (!stake || stake <= 0) {
      this.messages = ['Stake must be greater than 0.'];
      return;
    }

    const body: PlaceTicketCommand = {
      playerId: current.id,
      stake,
      selections: selections.map<PlaceTicketSelectionDto>(s => ({
        offerId: s.offerId,
        outcomeTemplateId: s.outcomeTemplateId
      }))
    };

    this.placing = true;
    this.tickets.apiTicketsPost(body).subscribe({
      next: r => {
        this.placing = false;
        this.toast.success(`Ticket ${r.ticketId} placed. Payout: ${r.potentialPayout}`);
        this.slip.clear();
      },
      error: (e) => {
        this.placing = false;
        const msgs: string[] = e?.messages ?? ['Failed to place ticket.'];
        msgs.forEach(m => this.toast.error(m));
      }
    });
  }

  private readonly MARKET_LABELS: Record<string, string> = {
    '1X2': 'Basic',
    'DoubleChance': 'Double Chance'
  };
  
  marketLabel(code: string | null | undefined): string {
    const c = (code ?? '').trim();   
    if (!c) return '';
    return this.MARKET_LABELS[c] ?? c;
  }
}
