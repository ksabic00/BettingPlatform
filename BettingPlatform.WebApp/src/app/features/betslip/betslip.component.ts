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
    private tickets: TicketsService
  ) {}

  trackSel = (_: number, s: SlipSelection) => s.offerId;

  remove(offerId: string) {
    this.slip.remove(offerId);
  }

  setStake(v: number) {
    this.slip.setStake(Number(v) || 0);
  }

  combinedOdds() {
    return this.slip.combinedOdds();
  }

  async place() {
    this.messages = [];

    const playerId = localStorage.getItem('playerId') ?? '';
    if (!playerId) {
      this.messages = ['Player is not set. (Create or set playerId in localStorage)'];
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
      playerId,
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
        this.messages = [
          `Ticket ${r.ticketId} placed. Payout: ${r.potentialPayout}`
        ];
      },
      error: (e) => {
        this.placing = false;
        this.messages = e?.messages ?? ['Failed to place ticket.'];
      }
    });
  }
}
