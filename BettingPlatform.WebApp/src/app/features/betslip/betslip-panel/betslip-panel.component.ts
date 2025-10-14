import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { TicketsService } from '../../../api';
import { BetslipService } from '../../../core/state/betslip.service';
import { PlayerContextService } from '../../../core/state/player-context.service';
import { ToastService } from '../../../core/ui/toast.service';   // <-- DODANO

@Component({
  selector: 'app-betslip-panel',
  standalone: true,
  templateUrl: './betslip-panel.component.html',
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe]
})
export class BetslipPanelComponent implements OnInit {
  stake = 10;
  placing = false;


  selections$ = this.slip.selections$;

  constructor(
    public slip: BetslipService,
    private tickets: TicketsService,
    private ctx: PlayerContextService,
    private toast: ToastService                     
  ) {}

  ngOnInit() {
    this.slip.stake$.subscribe(v => this.stake = v ?? 10);
  }

  setStake(v: any) { this.slip.setStake(Number(v)); }
  remove(offerId: string) { this.slip.removeByOffer(offerId); }
  get combinedOdds() { return this.slip.combinedOdds(); }

  async place() {
    const { stake, selections } = this.slip.getSnapshot();

    if (!selections.length) {
      this.toast.info('Add at least one selection.');
      return;
    }

    const current = this.ctx.snapshot;
    if (!current) {
      this.toast.warning('Select a player first (Players tab).');
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

    try {
      this.placing = true;
      const dto = await firstValueFrom(this.tickets.apiTicketsPost(body));
      this.toast.success(`Ticket ${dto.ticketId} placed. Payout: ${dto.potentialPayout}`);
      this.slip.clear();
    } catch (e: any) {
      const msgs: string[] = e?.messages ?? ['Failed to place ticket.'];
      msgs.forEach(m => this.toast.error(m));
    } finally {
      this.placing = false;
    }
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
