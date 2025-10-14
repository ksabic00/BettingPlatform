import { Component } from '@angular/core';
import { WalletService } from '../../api';
import { firstValueFrom } from 'rxjs';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

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

  constructor(private api: WalletService) { this.load(); }

  trackByRef = (_: number, t: any) => t.referenceId ?? t.occurredAtUtc;

  async load() {
    this.loading = true; this.err = undefined;
    try {
      const playerId = localStorage.getItem('playerId')!;
      this.data = await firstValueFrom(this.api.apiWalletPlayerIdGet(playerId));
    } catch (e: any) {
      this.err = e?.messages?.join(', ') ?? 'Error';
    } finally {
      this.loading = false;
    }
  }

  async deposit() {
    this.messages = [];
    try {
      const playerId = localStorage.getItem('playerId')!;
      await firstValueFrom(this.api.apiWalletDepositPost({ playerId, amount: this.amount }));
      this.messages = ['Deposit successful.'];
      this.load();
    } catch (e: any) {
      this.messages = e?.messages ?? ['Error'];
    }
  }
}
