import { Component } from '@angular/core';
import { WalletService } from '../../api';
import { firstValueFrom } from 'rxjs';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayerContextService } from '../../core/state/player-context.service';
import { ToastService } from '../../core/ui/toast.service';

@Component({
  selector: 'app-wallet',
  standalone: true,
  templateUrl: './wallet.component.html',
  styleUrls: ['./wallet.component.scss'],
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe]
})
export class WalletComponent {
  amount = 20;
  loading = false;
  err?: string;
  data?: any;
  messages: string[] = [];

  constructor(private api: WalletService, private ctx: PlayerContextService, private toast: ToastService) {
    this.ctx.current$.subscribe(_ => this.load()); 
  }

  trackByRef = (_: number, t: any) => t.referenceId ?? t.occurredAtUtc;

  async load() {
    const current = this.ctx.snapshot;
    if (!current) { this.data = undefined; return; }

    this.loading = true; this.err = undefined;
    try {
      this.data = await firstValueFrom(this.api.apiWalletPlayerIdGet(current.id));
    } catch (e: any) {
      this.err = e?.messages?.join(', ') ?? 'Error';
    } finally {
      this.loading = false;
    }
  }

  async deposit() {
    const current = this.ctx.snapshot;
    if (!current) {
      this.toast.warning('Select a player first (Players tab).');
      return;
    }
    if (!this.amount || this.amount <= 0) {
      this.toast.warning('Amount must be greater than 0.');
      return;
    }
  
    try {
      await firstValueFrom(
        this.api.apiWalletDepositPost({ playerId: current.id, amount: this.amount })
      );
      this.toast.success('Deposit successful.');
      this.load();
    } catch (e: any) {
      (e?.messages ?? ['Deposit failed.']).forEach((m: string) => this.toast.error(m));
    }
  }
  
}
